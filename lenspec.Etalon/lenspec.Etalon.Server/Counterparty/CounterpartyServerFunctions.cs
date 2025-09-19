using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Counterparty;

namespace lenspec.Etalon.Server
{
  partial class CounterpartyFunctions
  {  
    /// <summary>
    /// Установка статуса экспорта в 1с "Выполнено" и дату выполнения.
    /// </summary>
    [Public]
    public void UpdateCounterpartyExport1c()
    {
        // Запускаем асинхронник для обновления статуса и простановки даты.
        var asyncRightsHandler = lenspec.Etalon.Module.Parties.AsyncHandlers.AsyncCounterpartyChangeExportStatuslenspec.Create();
        asyncRightsHandler.CounterpartyId = _obj.Id;
        asyncRightsHandler.ExecuteAsync();
    }
    
    /// <summary>
    /// Получить список тендерных документов для КА.
    /// </summary>
    /// <returns>Тендерные документы.</returns>
    [Remote(IsPure =true)]
    public virtual List<Sungero.Docflow.IOfficialDocument> GetTenderDocuments()
    {
      var documents = new List<Sungero.Docflow.IOfficialDocument>();
      documents.AddRange(lenspec.Tenders.TenderDocumentBases.GetAll(x => x.Counterparties.Any(c => c.Counterparty == _obj)).ToList());
      documents.AddRange(lenspec.Tenders.TenderAccreditationForms.GetAll(x => x.Counterparty == _obj).ToList());
      
      return documents;
    }

    /// <summary>
    /// Отобразить список задач связанный с карточкой.
    /// </summary>
    [Remote]
    public StateView GetCounterpartyState()
    {
      var stateView = StateView.Create();
      stateView.AddDefaultLabel(OfficialDocuments.Resources.StateViewDefault);
      AddTasksViews(stateView, _obj);
      stateView.IsPrintable = true;
      return stateView;
    }
    
    /// <summary>
    /// Добавить информацию о задачах, в которые вложен документ.
    /// </summary>
    /// <param name="stateView">Схема представления.</param>
    /// <param name="document">Документ.</param>
    private static void AddTasksViews(StateView stateView, ICounterparty counterparty)
    {
      var tasks = Sungero.Workflow.Tasks.GetAll()
        .Where(task => task.AttachmentDetails
               .Any(a => a.AttachmentId == counterparty.Id && 
                    (a.EntityTypeGuid == Sungero.Parties.Server.Bank.ClassTypeGuid || a.EntityTypeGuid == Sungero.Parties.Server.Company.ClassTypeGuid || a.EntityTypeGuid == Sungero.Parties.Server.Person.ClassTypeGuid)))
        .OrderBy(task => task.Created)
        .ToList();
      
      foreach (var task in tasks)
      {
        if (stateView.Blocks.Any(b => b.HasEntity(task)))
          continue;
        
        AddTaskViewXml(stateView, task, counterparty);
      }
    }
    
    /// <summary>
    /// Построение модели задачи, в которую вложен документ.
    /// </summary>
    /// <param name="stateView">Схема представления.</param>
    /// <param name="task">Задача.</param>
    /// <param name="document">Документ.</param>
    private static void AddTaskViewXml(StateView stateView, Sungero.Workflow.ITask task, ICounterparty counterparty)
    {
      // Добавить предметное отображение для простых задач.
      if (Sungero.Workflow.SimpleTasks.Is(task))
      {
        AddSimpleTaskView(stateView, Sungero.Workflow.SimpleTasks.As(task));
        return;
      }
      
      // Добавить предметное отображение для прикладных задач.
      if (avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Is(task))
      {
        var approvalCounterpartyPersonDEB = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.As(task);
        var taskStateView = AddApprovalCounterpartyPersonDEBTask(approvalCounterpartyPersonDEB);
        if (taskStateView != null)
        {
          foreach (var block in taskStateView.Blocks)
          {
            stateView.AddBlock(block);
          }
        }
      }
    }
    
    /// <summary>
    /// Отображение задач на согласование контрагентов
    /// </summary>
    /// <param name="task"></param>
    private static Sungero.Core.StateView AddApprovalCounterpartyPersonDEBTask(avis.ApprovingCounterpartyDEB.IApprovalCounterpartyPersonDEB task)
    {
      var stateView = StateView.Create();
      var mainBlock = stateView.AddBlock();
      mainBlock.AssignIcon(StateBlockIconType.User, StateBlockIconSize.Small);
      mainBlock.ShowBorder = false;
      mainBlock.AddLabel(string.Format("{0} Задача отправлена. {1}",task.Author, task.Started));
      var taskBlock = stateView.AddBlock();
      taskBlock.AssignIcon(StateBlockIconType.OfEntity, StateBlockIconSize.Large);
      taskBlock.Entity = task;
      var headerStyle = StateBlockLabelStyle.Create();
      headerStyle.Color = Colors.Common.Black;
      headerStyle.FontWeight = FontWeight.Bold;
      taskBlock.AddLabel(task.Subject, headerStyle);
      return stateView;
    }

    /// <summary>
    /// Добавить предметное отображение простой задачи.
    /// </summary>
    /// <param name="stateView">Схема представления.</param>
    /// <param name="task">Задача.</param>
    private static void AddSimpleTaskView(StateView stateView, Sungero.Workflow.ISimpleTask task)
    {
      if (task == null)
        return;
      
      // Не добавлять блок, если нет заданий. Черновик - исключение.
      var assignments = new List<Sungero.Workflow.IAssignment>() { };
      assignments.AddRange(Sungero.Workflow.SimpleAssignments.GetAll().Where(a => Equals(a.Task, task)).ToList());
      assignments.AddRange(Sungero.Workflow.ReviewAssignments.GetAll().Where(a => Equals(a.Task, task) && a.Result == null).ToList());
      if (!assignments.Any() && task.Status != Sungero.Workflow.Task.Status.Draft)
        return;
      
      // Добавить блок информации о действии.
      if (task.Started.HasValue)
        Sungero.Docflow.PublicFunctions.OfficialDocument.AddUserActionBlock(stateView, task.Author, OfficialDocuments.Resources.StateViewTaskSent, task.Started.Value, task, string.Empty, task.StartedBy);
      else
        Sungero.Docflow.PublicFunctions.OfficialDocument.AddUserActionBlock(stateView, task.Author, ApprovalTasks.Resources.StateViewTaskDrawCreated, task.Created.Value, task, string.Empty, task.Author);
      
      // Добавить блок информации по задаче.
      var mainBlock = GetSimpleTaskMainBlock(task);
      stateView.AddBlock(mainBlock);
      
      // Маршрут.
      var iterations = GetIterationDates(task);
      foreach (var iteration in iterations)
      {
        var date = iteration.Date;
        var hasReworkBefore = iteration.IsRework;
        var hasRestartBefore = iteration.IsRestart;
        
        var nextIteration = iterations.Where(d => d.Date > date).FirstOrDefault();
        var nextDate = nextIteration != null ? nextIteration.Date : Calendar.Now;
        
        // Получить задания в рамках круга согласования.
        var iterationAssignments = assignments
          .Where(a => a.Created >= date && a.Created < nextDate)
          .OrderBy(a => a.Created)
          .ToList();
        
        if (!iterationAssignments.Any())
          continue;
        
        if (hasReworkBefore || hasRestartBefore)
        {
          var activeText = task.Texts
            .Where(t => t.Modified >= date)
            .OrderBy(t => t.Created)
            .FirstOrDefault();
          
          var comment = activeText != null ? activeText.Body : string.Empty;
          var started = activeText != null ? activeText.Modified : task.Started;
          
          var header = hasReworkBefore ? OfficialDocuments.Resources.StateViewTaskSentForRevision : OfficialDocuments.Resources.StateViewTaskSentAfterRestart;
          Sungero.Docflow.PublicFunctions.OfficialDocument.AddUserActionBlock(mainBlock, task.Author, header, started.Value, task, comment, task.StartedBy);
        }
        
        AddSimpleTaskIterationsBlocks(mainBlock, iterationAssignments);
      }
    }
    
    /// <summary>
    /// Добавить маршрут в предметное отображение простой задачи.
    /// </summary>
    /// <param name="mainBlock">Блок задачи.</param>
    /// <param name="assignments">Задания по задаче.</param>
    private static void AddSimpleTaskIterationsBlocks(StateBlock mainBlock, List<Sungero.Workflow.IAssignment> assignments)
    {
      var statusGroups = assignments.OrderByDescending(a => a.Status == Sungero.Workflow.AssignmentBase.Status.Completed).GroupBy(a => a.Status);
      foreach (var statusGroup in statusGroups)
      {
        var deadlineGroups = statusGroup.OrderBy(a => a.Deadline).GroupBy(a => a.Deadline);
        foreach (var deadlineGroup in deadlineGroups)
        {
          var textGroups = deadlineGroup.OrderBy(a => a.Modified).GroupBy(a => a.ActiveText);
          foreach (var textGroup in textGroups)
          {
            var assignmentBlocks = GetSimpleAssignmentsView(textGroup.ToList()).Blocks;
            if (assignmentBlocks.Any())
              foreach (var block in assignmentBlocks)
                mainBlock.AddChildBlock(block);
          }
        }
      }
    }
    
    /// <summary>
    /// Получить предметное отображение группы простых заданий.
    /// </summary>
    /// <param name="assignments">Простые задания.</param>
    /// <returns>Предметное отображение простого задания.</returns>
    private static Sungero.Core.StateView GetSimpleAssignmentsView(List<Sungero.Workflow.IAssignment> assignments)
    {
      var stateView = StateView.Create();
      if (!assignments.Any())
        return stateView;

      // Т.к. задания в пачке должны быть с одинаковым статусом, одинаковым дедлайном - вытаскиваем первый элемент для удобной работы.
      var assignment = assignments.First();
      
      var block = stateView.AddBlock();
      if (assignments.Count == 1)
        block.Entity = assignment;

      // Иконка.
      block.AssignIcon(Sungero.Docflow.ApprovalRuleBases.Resources.Assignment, StateBlockIconSize.Large);
      if (assignments.All(a => a.Status == Sungero.Workflow.AssignmentBase.Status.Completed))
        block.AssignIcon(ApprovalTasks.Resources.Completed, StateBlockIconSize.Large);
      else if (assignments.All(a => a.Status == Sungero.Workflow.AssignmentBase.Status.Aborted || a.Status == Sungero.Workflow.AssignmentBase.Status.Suspended))
        block.AssignIcon(StateBlockIconType.Abort, StateBlockIconSize.Large);
      
      // Заголовок.
      var header = Sungero.Workflow.ReviewAssignments.Is(assignment) ? OfficialDocuments.Resources.StateViewAssignmentForReview : OfficialDocuments.Resources.StateViewAssignment;
      block.AddLabel(header, Sungero.Docflow.PublicFunctions.Module.CreateHeaderStyle());
      
      // Кому.
      block.AddLineBreak();
      var performers = assignments.Where(a => Sungero.Company.Employees.Is(a.Performer)).Select(a => Sungero.Company.Employees.As(a.Performer)).ToList();
      block.AddLabel(string.Format("{0}: {1} ", OfficialDocuments.Resources.StateViewTo, Sungero.Docflow.PublicFunctions.OfficialDocument.GetPerformersInText(performers)),
                     Sungero.Docflow.PublicFunctions.Module.CreatePerformerDeadlineStyle());
      
      // Срок.
      var deadline = assignment.Deadline.HasValue ?
        Sungero.Docflow.PublicFunctions.Module.ToShortDateShortTime(assignment.Deadline.Value.ToUserTime()) :
        OfficialDocuments.Resources.StateViewWithoutTerm;
      block.AddLabel(string.Format("{0}: {1}", OfficialDocuments.Resources.StateViewDeadline, deadline), Sungero.Docflow.PublicFunctions.Module.CreatePerformerDeadlineStyle());
      
      // Результат выполнения.
      var activeText = Sungero.Docflow.PublicFunctions.Module.GetAssignmentUserComment(assignment);
      if (!string.IsNullOrWhiteSpace(activeText))
      {
        block.AddLineBreak();
        block.AddLabel(Sungero.Docflow.PublicConstants.Module.SeparatorText, Sungero.Docflow.PublicFunctions.Module.CreateSeparatorStyle());
        block.AddLineBreak();
        block.AddEmptyLine(Sungero.Docflow.PublicConstants.Module.EmptyLineMargin);
        
        block.AddLabel(activeText);
      }
      
      // Статус.
      var assignmentStatus = Sungero.Workflow.SimpleAssignments.Info.Properties.Status.GetLocalizedValue(assignment.Status);
      if (assignment.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && assignment.IsRead == false)
        assignmentStatus = Sungero.Docflow.ApprovalTasks.Resources.StateViewUnRead;
      else if (assignment.Status == Sungero.Workflow.AssignmentBase.Status.Aborted)
        assignmentStatus = Sungero.Docflow.ApprovalTasks.Resources.StateViewAborted;
      
      if (!string.IsNullOrEmpty(assignmentStatus))
        Sungero.Docflow.PublicFunctions.Module.AddInfoToRightContent(block, assignmentStatus);
      
      // Задержка.
      if (assignment.Deadline.HasValue && assignment.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
        Sungero.Docflow.PublicFunctions.OfficialDocument.AddDeadlineHeaderToRight(block, assignment.Deadline.Value, assignment.Performer);
      
      return stateView;
    }
    
    /// <summary>
    /// Получить предметное отображение простой задачи.
    /// </summary>
    /// <param name="task">Простая задача.</param>
    /// <returns>Предметное отображение простой задачи.</returns>
    private static Sungero.Core.StateBlock GetSimpleTaskMainBlock(Sungero.Workflow.ISimpleTask task)
    {
      var stateView = StateView.Create();
      var block = stateView.AddBlock();
      if (task == null)
        return block;
      
      block.Entity = task;
      var inWork = task.Status == Sungero.Workflow.Task.Status.InProcess || task.Status == Sungero.Workflow.Task.Status.UnderReview;
      block.IsExpanded = inWork;
      block.AssignIcon(StateBlockIconType.OfEntity, StateBlockIconSize.Large);
      
      // Заголовок. Тема.
      block.AddLabel(string.Format("{0}. {1}", OfficialDocuments.Resources.StateViewTask, task.Subject), Sungero.Docflow.PublicFunctions.Module.CreateHeaderStyle());
      
      // Срок.
      var hasDeadline = task.MaxDeadline.HasValue;
      var deadline = hasDeadline ? Sungero.Docflow.PublicFunctions.Module.ToShortDateShortTime(task.MaxDeadline.Value.ToUserTime()) : OfficialDocuments.Resources.StateViewWithoutTerm;
      block.AddLineBreak();
      block.AddLabel(string.Format("{0}: {1}", OfficialDocuments.Resources.StateViewFinalDeadline, deadline), Sungero.Docflow.PublicFunctions.Module.CreatePerformerDeadlineStyle());
      
      // Текст задачи.
      var taskText = Sungero.Docflow.PublicFunctions.Module.GetTaskUserComment(task, string.Empty);
      if (!string.IsNullOrWhiteSpace(taskText))
      {
        block.AddLineBreak();
        block.AddLabel(Sungero.Docflow.PublicConstants.Module.SeparatorText, Sungero.Docflow.PublicFunctions.Module.CreateSeparatorStyle());
        block.AddLineBreak();
        block.AddEmptyLine(Sungero.Docflow.PublicConstants.Module.EmptyLineMargin);
        
        // Форматирование текста задачи.
        block.AddLabel(taskText);
      }
      
      // Статус.
      var status = Sungero.Workflow.SimpleTasks.Info.Properties.Status.GetLocalizedValue(task.Status);
      if (!string.IsNullOrEmpty(status))
        Sungero.Docflow.PublicFunctions.Module.AddInfoToRightContent(block, status);
      
      // Задержка.
      if (hasDeadline && inWork)
        Sungero.Docflow.PublicFunctions.OfficialDocument.AddDeadlineHeaderToRight(block, task.MaxDeadline.Value, Users.Current);
      
      return block;
    }
    
    /// <summary>
    /// Получить даты итераций задачи.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Список дат в формате: "Дата", "Это доработка", "Это рестарт".</returns>
    private static List<avis.EtalonParties.Structures.Module.ITaskIterations> GetIterationDates(Sungero.Workflow.ITask task)
    {
      // Дата создания.
      var iterations = new List<avis.EtalonParties.Structures.Module.ITaskIterations>() { avis.EtalonParties.Structures.Module.TaskIterations.Create(task.Created.Value, false, false) };
      
      // Даты рестартов.
      var restartDates = Sungero.Workflow.WorkflowHistories.GetAll(h => h.EntityId == task.Id && h.Operation == Sungero.Workflow.WorkflowHistory.Operation.Restart)
        .Select(h => h.HistoryDate.Value)
        .ToList();
      foreach (var restartDate in restartDates)
        iterations.Add(avis.EtalonParties.Structures.Module.TaskIterations.Create(restartDate, false, true));
      
      // Доработки в согласовании официальных документов.
      var reworkDates = ApprovalReworkAssignments.GetAll()
        .Where(a => Equals(a.Task, task) && a.Result == Sungero.Docflow.ApprovalReworkAssignment.Result.ForReapproving)
        .Select(a => a.Created.Value).ToList();
      foreach (var reworkDate in reworkDates)
        iterations.Add(avis.EtalonParties.Structures.Module.TaskIterations.Create(reworkDate, true, false));
      
      // Доработки в свободном согласовании.
      var freeReworkDates = FreeApprovalReworkAssignments.GetAll()
        .Where(a => Equals(a.Task, task) && a.Result == Sungero.Docflow.FreeApprovalReworkAssignment.Result.Reworked)
        .Select(a => a.Created.Value).ToList();
      foreach (var reworkDate in freeReworkDates)
        iterations.Add(avis.EtalonParties.Structures.Module.TaskIterations.Create(reworkDate, true, false));
      
      // Доработки в простых задачах.
      var reviewDates = Sungero.Workflow.ReviewAssignments.GetAll()
        .Where(a => Equals(a.Task, task) && a.Result == Sungero.Workflow.ReviewAssignment.Result.ForRework)
        .Select(a => a.Created.Value).ToList();
      foreach (var reviewDate in reviewDates)
        iterations.Add(avis.EtalonParties.Structures.Module.TaskIterations.Create(reviewDate, true, false));
      
      return iterations.OrderBy(d => d.Date).ToList();
    }
  }
}