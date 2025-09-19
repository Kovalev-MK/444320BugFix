using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB;

namespace avis.ApprovingCounterpartyDEB.Server
{
  partial class ApprovalCounterpartyPersonDEBFunctions
  {
    /// <summary>
    /// Актуализировать выписки из ЕГРЮЛ
    /// </summary>
    /// <param name="documentList">Список документов по КА</param>
    [Public, Remote(IsPure = false)]
    public static void ActualizeCounterpatyDocuments(List<Sungero.Docflow.ICounterpartyDocument> documentList)
    {
      var extractFromEGRULKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(lenspec.Etalon.Module.Parties.PublicConstants.Module.ExtractFromEGRULKind);
      var counterpartyDocuments = documentList.Where(x => Equals(extractFromEGRULKind, x.DocumentKind) && lenspec.Etalon.CounterpartyDocuments.Is(x));
      foreach (var counterpartyDoc in counterpartyDocuments)
      {
        try
        {
          var counterpartyCastedDoc = lenspec.Etalon.CounterpartyDocuments.As(counterpartyDoc);
          var isNeedExcerpt = counterpartyCastedDoc.State.IsInserted ||
            !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.CheckForCounterpartyChangesEGRUL(counterpartyCastedDoc.Counterparty.TIN, counterpartyCastedDoc.CustomDocumentDatelenspec);
          if (isNeedExcerpt)
          {
            var error = lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.GetExcerptEGRUL(counterpartyCastedDoc.Counterparty.TIN, counterpartyCastedDoc);
            if (!string.IsNullOrEmpty(error))
              throw AppliedCodeException.Create(error);
            
            counterpartyCastedDoc.CustomDocumentDatelenspec = Calendar.Today;
            counterpartyCastedDoc.Save();
          }
        }
        catch(Exception ex)
        {
          Logger.ErrorFormat("Avis - ActualizeAndPullUpCounterpatyDocuments - не удалось актуализировать документ с ИД {0}. ", ex, counterpartyDoc.Id);
        }
      }
    }
    
    /// <summary>
    /// Проверить есть ли активная задача по контрагенту
    /// </summary>
    /// <param name="counterparty">Контрагент</param>
    /// <returns>True - есть, иначе - false</returns>
    [Remote(IsPure = true)]
    public static avis.ApprovingCounterpartyDEB.IApprovalCounterpartyPersonDEB GetActiveTaskByCounterparty(Sungero.Parties.ICounterparty counterparty)
    {
      var task = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Null;
      AccessRights.AllowRead(() =>
                             {
                               var activeTasks = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.GetAll(x => (x.Status == Sungero.Workflow.Task.Status.InProcess ||
                                                                                                                           x.Status == Sungero.Workflow.Task.Status.UnderReview) &&
                                                                                                                     x.AttachmentDetails.Any(d => d.GroupId == Guid.Parse("ab36176b-a7ed-4359-b301-9e84f4896e5e") &&
                                                                                                                                             d.AttachmentId == counterparty.Id));
                               task = activeTasks.FirstOrDefault();
                             });
      return task;
    }
    
    /// <summary>
    /// Найти задачи на согласование КА в которых главный документ является главным документом в текущей задаче
    /// </summary>
    /// <returns>Список задач-дубликатов (прекращенные задачи не попадают в список)</returns>
    [Remote(IsPure = true)]
    public static List<avis.ApprovingCounterpartyDEB.IApprovalCounterpartyPersonDEB> GetTaskDuplicates(avis.ApprovingCounterpartyDEB.IApprovalCounterpartyBase document)
    {
      var tasks = new List<avis.ApprovingCounterpartyDEB.IApprovalCounterpartyPersonDEB>();
      AccessRights.AllowRead(() =>
                             {
                               var duplicates = ApprovalCounterpartyPersonDEBs.GetAll(x => x.Status != Sungero.Workflow.Task.Status.Aborted &&
                                                                                      x.AttachmentDetails.Any(d => d.GroupId == new Guid("a667ecb8-f154-4210-86c1-eabbd0fa27b7") && d.AttachmentId == document.Id));
                               tasks.AddRange(duplicates);
                             });
      return tasks;
    }
    
    /// <summary>
    /// Прекратить параллельные задания
    /// </summary>
    /// <param name="assignment">Текущее задание</param>
    [Public]
    public static void AbortParallelAssignments(Sungero.Workflow.IAssignment assignment)
    {
      var parallelAssignments = ApprovingCounterpartyDEB.PublicFunctions.Module.GetParallelAssignments(assignment);
      var activeParallelAssignments = parallelAssignments
        .Where(a => a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
        .Where(a => !Equals(a, assignment));
      foreach (var parallelAssignment in activeParallelAssignments)
      {
        if (!string.IsNullOrEmpty(parallelAssignment.ActiveText))
          parallelAssignment.ActiveText += Environment.NewLine;
        
        if (assignment.CompletedBy != null && !assignment.CompletedBy.Equals(assignment.Performer))
          parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserWithOnBehalfOfFormat(assignment.CompletedBy.Name,
                                                                                                                          assignment.Performer.Name);
        else
          parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserFormat(assignment.Performer.Name);

        parallelAssignment.Abort();
      }
    }
    
    /// <summary>
    /// Получить руководителя сотрудника с учетом состояния записи сотрудника, состояния УЗ и совпадения с самим сотрудником.
    /// </summary>
    /// <param name="employee">Сотрудник</param>
    /// <returns>Руководитель</returns>
    /// <remarks>Если руководитель не найден, возвращается null</remarks>
    [Public]
    public static lenspec.Etalon.IEmployee GetOnlyMangerForApprovalDEB(avis.ApprovingCounterpartyDEB.IApprovalCounterpartyBase document)
    {
      var employee = document.PreparedBy;
      var department = employee.Department;
      var businessUnit = lenspec.Etalon.BusinessUnits.As(employee.Department.BusinessUnit);
      var manager = lenspec.Etalon.Employees.Null;
      while (department != null)
      {
        // Если руководитель автоматизирован, не является CEO и не является сотрудниковм из поля Подготовил.
        if (department.Manager != null &&
            lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(department.Manager) == true &&
            !Equals(department.Manager, employee) &&
            department.Manager.Status == Sungero.Company.Employee.Status.Active)
        {
          if (businessUnit != null && Equals(department.Manager, businessUnit.CEO))
          {
            var performerByBU = GetApprovalerDBInsteadOfCEO(businessUnit);
            if (performerByBU != null)
              manager = lenspec.Etalon.Employees.As(performerByBU);
            else
              manager = lenspec.Etalon.Employees.As(department.Manager);
            
            break;
          }
          else
          {
            manager = lenspec.Etalon.Employees.As(department.Manager);
            break;
          }
        }
        
        // Переходим на следующее подразделение.
        department = department.HeadOffice;
      }
      
      if (manager == null && businessUnit != null)
      {
        var performerByBU = GetApprovalerDBInsteadOfCEO(businessUnit);
        if (performerByBU != null)
          manager = lenspec.Etalon.Employees.As(performerByBU);
        else if (businessUnit.CEO != null && lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(businessUnit.CEO))
          manager = lenspec.Etalon.Employees.As(businessUnit.CEO);
      }
      
      return manager;
    }
    
    private static lenspec.Etalon.IEmployee GetApprovalerDBInsteadOfCEO(lenspec.Etalon.IBusinessUnit businessUnit)
    {
      var approvaler = lenspec.Etalon.Employees.Null;
      var performerByBU = lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovalRoleKindPerformerByBusinessUnit(businessUnit,
                                                                                                                          null,
                                                                                                                          lenspec.EtalonDatabooks.ComputedRoles.Resources.ApprovalerDBInsteadOfCEO,
                                                                                                                          null);
      if (performerByBU != null && lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(performerByBU))
        approvaler = lenspec.Etalon.Employees.As(performerByBU);
      
      return approvaler;
    }
    
    #region Вкладка регламент
    
    /// <summary>
    /// Вкладка регламент в задаче и заданиях согласования КА ДБ
    /// </summary>
    [Remote]
    public StateView GetApprovalCounterpartyPersonDEBState()
    {
      var stateView = StateView.Create();
      if (_obj.Status != ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.Status.InProcess && _obj.Status != ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.Status.Completed)
      {
        stateView.AddDefaultLabel("Задача приостановлена или прекращена, регламент не вычислен.");
        return stateView;
      }
      CreateBlock1(stateView);
      CreateBlock2(stateView);
      CreateBlock3(stateView);
      CreateBlock5(stateView);
      CreateBlock4(stateView);
      return stateView;
    }
    
    private void CreateBlock1(Sungero.Core.StateView stateView)
    {
      var block1 = stateView.AddBlock();
      block1.AssignIcon(ApprovalCounterpartyPersonDEBs.Resources.AssignmentIcon, StateBlockIconSize.Large);
      var style = StateBlockLabelStyle.Create();
      style.FontWeight = FontWeight.Bold;
      block1.AddLabel("Согласование с руководителем", style);
      block1.AddLineBreak();
      var performerLableBlock1 = _obj.ApprovalDocument.ApprovalCounterpartyBases.Any() ? _obj.ManagerInitiator.Name : avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.PerformerNotCalculated;
      block1.AddLabel(performerLableBlock1);
      var rightContentBlock1 = block1.AddContent();
      var assignmentResult = string.Empty;
      var statusBlock1 = GetStatusBlock1(out assignmentResult);
      rightContentBlock1.AddLabel(statusBlock1);
      rightContentBlock1.AddLineBreak();
      rightContentBlock1.AddLabel(assignmentResult);
      if (string.Equals(statusBlock1, avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.InProcessBlockLableSate))
        block1.Background = Colors.Common.LightYellow;
    }
    
    private void CreateBlock2(Sungero.Core.StateView stateView)
    {
      var block2 = stateView.AddBlock();
      block2.AssignIcon(ApprovalCounterpartyPersonDEBs.Resources.AssignmentIcon, StateBlockIconSize.Large);
      var style = StateBlockLabelStyle.Create();
      style.FontWeight = FontWeight.Bold;
      block2.AddLabel("Согласование с ответственным ДБ", style);
      block2.AddLineBreak();
      var performerLableBlock2 = GetPerformerBlock2();
      block2.AddLabel(performerLableBlock2.Name);
      var rightContentBlock2 = block2.AddContent();
      var assignmentResult = string.Empty;
      var statusBlock2 = GetStatusBlock2(out assignmentResult);
      rightContentBlock2.AddLabel(statusBlock2);
      rightContentBlock2.AddLineBreak();
      rightContentBlock2.AddLabel(assignmentResult);
      if (string.Equals(statusBlock2, avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.InProcessBlockLableSate))
        block2.Background = Colors.Common.LightYellow;
    }
    
    private void CreateBlock3(Sungero.Core.StateView stateView)
    {
      var block3 = stateView.AddBlock();
      block3.AssignIcon(ApprovalCounterpartyPersonDEBs.Resources.AssignmentIcon, StateBlockIconSize.Large);
      var style = StateBlockLabelStyle.Create();
      style.FontWeight = FontWeight.Bold;
      block3.AddLabel("Согласование с руководством ДБ", style);
      block3.AddLineBreak();
      var performerLableBlock3 = GetPerformerBlock3();
      block3.AddLabel(performerLableBlock3.Name);
      var rightContentBlock3 = block3.AddContent();
      var assignmentResult = string.Empty;
      var statusBlock3 = GetStatusBlock3(out assignmentResult);
      rightContentBlock3.AddLabel(statusBlock3);
      rightContentBlock3.AddLineBreak();
      rightContentBlock3.AddLabel(assignmentResult);
      if (string.Equals(statusBlock3, avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.InProcessBlockLableSate))
        block3.Background = Colors.Common.LightYellow;
    }
    
    private void CreateBlock4(Sungero.Core.StateView stateView)
    {
      var attachment = _obj.ApprovalDatabook.Counterparties.SingleOrDefault();
      if (!lenspec.Etalon.Banks.Is(attachment) && !lenspec.Etalon.People.Is(attachment))
      {
        var company = lenspec.Etalon.Companies.As(attachment);
        if (company == null || (company != null && company.RegistryStatusavis != lenspec.Etalon.Company.RegistryStatusavis.Included))
        {
          var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
          var approvalCounterpartyBankDEB = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEBs.As(document);
          if (approvalCounterpartyBankDEB != null && approvalCounterpartyBankDEB.IsAmountBigestYearAmount == avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEB.IsAmountBigestYearAmount.Yes)
          {
            var block4 = stateView.AddBlock();
            block4.AssignIcon(ApprovalCounterpartyPersonDEBs.Resources.AssignmentIcon, StateBlockIconSize.Large);
            var style = StateBlockLabelStyle.Create();
            style.FontWeight = FontWeight.Bold;
            block4.AddLabel("Включение контрагента в реестр", style);
            block4.AddLineBreak();
            var performerLableBlock4 = GetPerformerBlock4();
            block4.AddLabel(performerLableBlock4.Name);
            var rightContentBlock4 = block4.AddContent();
            var resultAssignment = string.Empty;
            var statusBlock4 = GetStatusBlock4(out resultAssignment);
            rightContentBlock4.AddLabel(statusBlock4);
            rightContentBlock4.AddLineBreak();
            rightContentBlock4.AddLabel(resultAssignment);
            if (string.Equals(statusBlock4, avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.InProcessBlockLableSate))
              block4.Background = Colors.Common.LightYellow;
          }
        }
      }
    }
    
    private void CreateBlock5(Sungero.Core.StateView stateView)
    {
      var attachment = _obj.ApprovalDatabook.Counterparties.SingleOrDefault();
      if (lenspec.Etalon.CompanyBases.Is(attachment))
      {
        var block5 = stateView.AddBlock();
        block5.AssignIcon(ApprovalCounterpartyPersonDEBs.Resources.AssignmentIcon, StateBlockIconSize.Large);
        var style = StateBlockLabelStyle.Create();
        style.FontWeight = FontWeight.Bold;
        block5.AddLabel("Установка лимита по контрагенту", style);
        block5.AddLineBreak();
        var performerLableBlock5 = GetPerformerBlock5();
        block5.AddLabel(performerLableBlock5.Name);
        var rightContentBlock5 = block5.AddContent();
        var resultAssignment = string.Empty;
        var statusBlock5 = GetStatusBlock5(out resultAssignment);
        rightContentBlock5.AddLabel(statusBlock5);
        rightContentBlock5.AddLineBreak();
        rightContentBlock5.AddLabel(resultAssignment);
        if (string.Equals(statusBlock5, avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.InProcessBlockLableSate))
          block5.Background = Colors.Common.LightYellow;
      }
    }
    
    private string GetStatusBlock1(out string assignmentResult)
    {
      assignmentResult = string.Empty;
      var assignments = ManagerApprovalAssignments.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId);
      var assignment = assignments.Where(x => x.Created == assignments.Max(a => a.Created)).SingleOrDefault();
      if (assignment == null || (assignment != null && assignment.Status != avis.ApprovingCounterpartyDEB.ManagerApprovalAssignment.Status.InProcess &&
                                 assignment.Status != avis.ApprovingCounterpartyDEB.ManagerApprovalAssignment.Status.Completed))
      {
        return string.Empty;
      }
      // Если задание в работе, то статус = в работе а в результат записывается срок выполнения
      var state = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.InProcessBlockLableSate;
      assignmentResult = assignment.Deadline.HasValue ? string.Format("Срок: {0}", assignment.Deadline.Value.ToString("dd.MM.yy HH:mm")) : string.Empty;
      //иначе статус = выполнено, результат = согласовано
      if (assignment.Status == avis.ApprovingCounterpartyDEB.ManagerApprovalAssignment.Status.Completed)
      {
        state = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.CompletedBlockLableState;
        if (assignment.Result == ApprovingCounterpartyDEB.ManagerApprovalAssignment.Result.Complete)
          assignmentResult = "Согласовано";
        else
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.ReworkResultLable;
      }
      
      return state;
    }
    
    private Sungero.CoreEntities.IRecipient GetPerformerBlock2()
    {
      var assignments = ApprovalerDEBApprovalAssignments.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId);
      var assignment = assignments.Where(x => x.IterationId == assignments.Max(a => a.IterationId) && x.Status == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Status.Completed).SingleOrDefault();
      if (assignment != null)
        return assignment.CompletedBy;
      else
        return Roles.GetAll(x => x.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.DEBCoordinationgGuid).SingleOrDefault();
    }
    
    private string GetStatusBlock2(out string assignmentResult)
    {
      var assignments = ApprovalerDEBApprovalAssignments.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId);
      var assignmentsLast = assignments.Where(x => x.IterationId == assignments.Max(a => a.IterationId));
      var state = string.Empty;
      assignmentResult = string.Empty;
      var assignment = assignmentsLast.Where(x => x.Status == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Status.Completed).SingleOrDefault();
      if (assignment != null)
      {
        state = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.CompletedBlockLableState;
        if (assignment.Result == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopNotRecomend)
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.NotRecomendedResultLable;
        else if (assignment.Result == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopPossible)
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.PossibleResultLable;
        else if (assignment.Result == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopWithRisks)
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.WithRisksResultLable;
        else if (assignment.Result == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.DoesNotReqAppr)
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.NotRquiredResultLable;
        else if (assignment.Result == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.Rework)
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.ReworkResultLable;
      }
      else
      {
        var inProcessAssignmentLast = assignmentsLast.FirstOrDefault(x => x.Status == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Status.InProcess);
        if (inProcessAssignmentLast != null)
        {
          state = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.InProcessBlockLableSate;
          assignmentResult = inProcessAssignmentLast.Deadline.HasValue ? string.Format("Срок: {0}", inProcessAssignmentLast.Deadline.Value.ToString("dd.MM.yy HH:mm")) : string.Empty;
        }
      }
      return state;
    }
    
    private Sungero.CoreEntities.IRecipient GetPerformerBlock3()
    {
      var assignments = ApprovalManagerDEBAssignments.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId);
      var assignment = assignments.Where(x => x.IterationId == assignments.Max(a => a.IterationId) && x.Status == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Status.Completed).SingleOrDefault();
      if (assignment != null)
        return assignment.Performer;
      else
        return Roles.GetAll(x => x.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.DEBManagementGuid).SingleOrDefault();
    }
    
    private string GetStatusBlock3(out string assignmentResult)
    {
      var assignments = ApprovalManagerDEBAssignments.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId);
      var assignmentsLast = assignments.Where(x => x.IterationId == assignments.Max(a => a.IterationId));
      var state = string.Empty;
      assignmentResult = string.Empty;
      var assignment = assignmentsLast.Where(x => x.Status == avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Status.Completed).SingleOrDefault();
      if (assignment != null)
      {
        state = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.CompletedBlockLableState;
        if (assignment.Result == avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopNotRecomend)
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.NotRecomendedResultLable;
        else if (assignment.Result == avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopPossible)
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.PossibleResultLable;
        else if (assignment.Result == avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopWithRisks)
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.WithRisksResultLable;
        else if (assignment.Result == avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.DoesNotReqAppr)
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.NotRquiredResultLable;
        else if (assignment.Result == avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.Rework)
          assignmentResult = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.ReworkResultLable;
      }
      else
      {
        var inProcessAssignmentLast = assignmentsLast.FirstOrDefault(x => x.Status == avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Status.InProcess);
        if (inProcessAssignmentLast != null)
        {
          state = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.InProcessBlockLableSate;
          assignmentResult = inProcessAssignmentLast.Deadline.HasValue ? string.Format("Срок: {0}", inProcessAssignmentLast.Deadline.Value.ToString("dd.MM.yy HH:mm")) : string.Empty;
        }
      }
      return state;
    }
    
    private Sungero.CoreEntities.IRecipient GetPerformerBlock4()
    {
      var assignments = IncludeCounterpartyRegistries.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId);
      var assignment = assignments.Where(x => x.IterationId == assignments.Max(a => a.IterationId) && x.Status == ApprovingCounterpartyDEB.IncludeCounterpartyRegistry.Status.Completed).SingleOrDefault();
      if (assignment != null)
        return assignment.Performer;
      else
        return Roles.GetAll(x => x.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.TenderCoordinatorGuid).SingleOrDefault();
    }
    
    private string GetStatusBlock4(out string resultAssignment)
    {
      resultAssignment = string.Empty;
      var assignments = IncludeCounterpartyRegistries.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId);
      var assignmentsLast = assignments.Where(x => x.IterationId == assignments.Max(a => a.IterationId));
      var state = string.Empty;
      if (assignmentsLast.Any(x => x.Status == avis.ApprovingCounterpartyDEB.IncludeCounterpartyRegistry.Status.Completed))
        state = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.CompletedBlockLableState;
      else
      {
        var inProcessAssignmentLast = assignmentsLast.FirstOrDefault(x => x.Status == avis.ApprovingCounterpartyDEB.IncludeCounterpartyRegistry.Status.InProcess);
        if (inProcessAssignmentLast != null)
        {
          state = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.InProcessBlockLableSate;
          resultAssignment = inProcessAssignmentLast.Deadline.HasValue ? string.Format("Срок: {0}", inProcessAssignmentLast.Deadline.Value.ToString("dd.MM.yy HH:mm")) : string.Empty;
        }
      }
      return state;
    }
    
    private Sungero.CoreEntities.IRecipient GetPerformerBlock5()
    {
      var assignments = SettingLimitAssignments.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId);
      var assignment = assignments.Where(x => x.IterationId == assignments.Max(a => a.IterationId) && x.Status == ApprovingCounterpartyDEB.SettingLimitAssignment.Status.Completed).SingleOrDefault();
      if (assignment != null)
        return assignment.Performer;
      else
        return Roles.GetAll(x => x.Sid == avis.ApprovingCounterpartyDEB.PublicConstants.Module.ResponsibleLimitEconomist).SingleOrDefault();
    }
    
    private string GetStatusBlock5(out string resultAssignment)
    {
      resultAssignment = string.Empty;
      var assignments = SettingLimitAssignments.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId);
      var assignmentsLast = assignments.Where(x => x.IterationId == assignments.Max(a => a.IterationId));
      var state = string.Empty;
      if (assignmentsLast.Any(x => x.Status == avis.ApprovingCounterpartyDEB.SettingLimitAssignment.Status.Completed))
        state = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.CompletedBlockLableState;
      else
      {
        var inProcessAssignmentLast = assignmentsLast.FirstOrDefault(x => x.Status == avis.ApprovingCounterpartyDEB.SettingLimitAssignment.Status.InProcess);
        if (inProcessAssignmentLast != null)
        {
          state = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.InProcessBlockLableSate;
          resultAssignment = inProcessAssignmentLast.Deadline.HasValue ? string.Format("Срок: {0}", inProcessAssignmentLast.Deadline.Value.ToString("dd.MM.yy HH:mm")) : string.Empty;
        }
      }
      return state;
    }
    
    /// <summary>
    /// Вкладка Задачи в документах
    /// </summary>
    [Remote(IsPure = true)]
    public Sungero.Core.StateView GetStateView(Sungero.Docflow.IOfficialDocument document)
    {
      var stateView = StateView.Create();
      var mainBlock = stateView.AddBlock();
      mainBlock.AssignIcon(StateBlockIconType.User, StateBlockIconSize.Small);
      mainBlock.ShowBorder = false;
      mainBlock.AddLabel(string.Format("{0} Задача отправлена. {1}",_obj.Author, _obj.Started));
      var taskBlock = stateView.AddBlock();
      taskBlock.AssignIcon(StateBlockIconType.OfEntity, StateBlockIconSize.Large);
      taskBlock.Entity = _obj;
      var headerStyle = StateBlockLabelStyle.Create();
      headerStyle.Color = Colors.Common.Black;
      headerStyle.FontWeight = FontWeight.Bold;
      taskBlock.AddLabel(_obj.Subject, headerStyle);
      return stateView;
    }
    
    #endregion
  }
}