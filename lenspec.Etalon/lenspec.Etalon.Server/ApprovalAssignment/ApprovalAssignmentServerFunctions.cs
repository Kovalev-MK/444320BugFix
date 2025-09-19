using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalAssignment;

namespace lenspec.Etalon.Server
{
  partial class ApprovalAssignmentFunctions
  {
    //Добавлено Avis Expert
    
    /// <summary>
    /// Отправить уведомление, если вносили изменения в карточку Заявки на оплату.
    /// </summary>
    [Remote]
    public void CheckUpdateApplicationForPayment(Sungero.Docflow.IOfficialDocument document)
    {
      var historyUsers = document.History.GetAll().Where(x => x.HistoryDate > _obj.Created && x.Action == Sungero.CoreEntities.History.Action.Update).Select(x => x.User);
      if (historyUsers.Any())
      {
        var computedRole = lenspec.Etalon.ApprovalStages.As(_obj.Stage).ApprovalRoles
          .Where(x => lenspec.EtalonDatabooks.ComputedRoles.Is(x.ApprovalRole) && x.ApprovalRole.Type == lenspec.EtalonDatabooks.ComputedRole.Type.ApprovRoleKind)
          .Select(x => lenspec.EtalonDatabooks.ComputedRoles.As(x.ApprovalRole))
          .FirstOrDefault();
        var approvRoleKindPerformers = new List<IRecipient>();
        approvRoleKindPerformers.AddRange(lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovRoleKindPerformersByName(computedRole, Sungero.Docflow.ApprovalTasks.As(_obj.Task), _obj.Stage,
                                                                                                                                 lenspec.Etalon.ApprovalAssignments.Resources.BudgetControllerRoleKindName));
        approvRoleKindPerformers.AddRange(lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovRoleKindPerformersByName(computedRole, Sungero.Docflow.ApprovalTasks.As(_obj.Task), _obj.Stage,
                                                                                                                                 lenspec.Etalon.ApprovalAssignments.Resources.ResponsibleForEconomicsRoleKindName));
        approvRoleKindPerformers.AddRange(lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovRoleKindPerformersByName(computedRole, Sungero.Docflow.ApprovalTasks.As(_obj.Task), _obj.Stage,
                                                                                                                                 lenspec.Etalon.ApprovalAssignments.Resources.BudgetControllerRefund));
        approvRoleKindPerformers.AddRange(lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovRoleKindPerformersByName(computedRole, Sungero.Docflow.ApprovalTasks.As(_obj.Task), _obj.Stage,
                                                                                                                                 lenspec.Etalon.ApprovalAssignments.Resources.BudgetControllerFine));
        approvRoleKindPerformers.AddRange(lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovRoleKindPerformersByName(computedRole, Sungero.Docflow.ApprovalTasks.As(_obj.Task), _obj.Stage,
                                                                                                                                 lenspec.Etalon.ApprovalAssignments.Resources.BudgetControllerRegl));
        approvRoleKindPerformers.AddRange(lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovRoleKindPerformersByName(computedRole, Sungero.Docflow.ApprovalTasks.As(_obj.Task), _obj.Stage,
                                                                                                                                 lenspec.Etalon.ApprovalAssignments.Resources.BudgetController300));
        if (historyUsers.Any(x => approvRoleKindPerformers.Contains(x)))
        {
          var author = Sungero.Company.Employees.As(_obj.Task.Author);
          var initManager = lenspec.Etalon.PublicFunctions.Employee.GetManagerOrEmployee(author);
          var budgetResponsible = lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovRoleKindPerformersByName(computedRole, Sungero.Docflow.ApprovalTasks.As(_obj.Task),
                                                                                                                         Sungero.Docflow.ApprovalStages.Null,
                                                                                                                         lenspec.Etalon.ApprovalAssignments.Resources.BudgetResponsibleRoleKindName);
          var performers = new List<IRecipient>();
          performers.Add(author);
          performers.Add(initManager);
          performers.AddRange(budgetResponsible);
          
          var subject = lenspec.Etalon.ApprovalAssignments.Resources.UpdateApplicationForPaymentNotificationSubjectFormat(document.Name).ToString();
          var maxSubjectLength = Sungero.Workflow.SimpleTasks.Info.Properties.Subject.Length;
          if (subject.Length > maxSubjectLength)
            subject = subject.Substring(0, maxSubjectLength);
          
          var task = Sungero.Workflow.SimpleTasks.CreateWithNotices(subject, performers.ToArray());
          task.ActiveText = lenspec.Etalon.ApprovalAssignments.Resources.UpdateApplicationForPaymentNotificationActiveText;
          task.Attachments.Add(document);
          task.Start();
        }
      }
    }
    
    [Remote]
    public string UpdateDocumentBody()
    {
      var result = string.Empty;
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
        var currentStage = Functions.ApprovalTask.GetStage(task, Sungero.Docflow.ApprovalStage.StageType.Approvers);
        if (currentStage != null)
        {
          var stage = Etalon.ApprovalStages.As(currentStage.Stage);
          // Автоматическое обновление полей в теле документа.
          if (stage != null)
          {
            var updateTemplateBeforeExecute = stage.UpdateTemplateBeforeExecuteavis.HasValue && stage.UpdateTemplateBeforeExecuteavis.Value == true;
            var updateTemplateNumberAndDateBeforeExecute = stage.UpdateTemplateNumberAndDateBeforeExecuteavis.HasValue && stage.UpdateTemplateNumberAndDateBeforeExecuteavis.Value == true;
            if (updateTemplateBeforeExecute || updateTemplateNumberAndDateBeforeExecute)
            {
              _obj.Save();
              result = Etalon.PublicFunctions.ApprovalTask.UpdateDocumentBody(task, updateTemplateBeforeExecute, updateTemplateNumberAndDateBeforeExecute);
            }
          }
        }
      }
      return result;
    }
    
    /// <summary>
    /// Проверить возможность продления срока
    /// </summary>
    /// <returns>true - есть возможность продления срока</returns>
    [Remote]
    public bool CheckPossibilityExtending()
    {
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var stageLiteModel = Functions.ApprovalTask.GetStage(task, lenspec.Etalon.ApprovalStage.StageType.Approvers);
      var stage = lenspec.Etalon.ApprovalStages.As(stageLiteModel.Stage);
      return stage.IsProhibitExtensionOfTimeavis != true;
    }
    
    /// <summary>
    /// Создать доверенности из диалога.
    /// </summary>
    /// <param name="powerOfAttorney">Заявление на формирование доверенностей.</param>
    /// <returns>Сообщение для вывода пользователю, если есть ошибка.</returns>
    [Remote]
    public string CreatePowerOfAttorneysFromDialog(lenspec.Etalon.IPowerOfAttorney powerOfAttorney)
    {
      var requestCreatePOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
      var poaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Sungero.Docflow.PublicConstants.Module.Initialize.PowerOfAttorneyKind);
      var requestCreateNPOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
      var nPoaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.DocumentNotarialKindGuid);
      
      var countSuccessCreation = 0;
      foreach (var line in powerOfAttorney.OurBusinessUavis)
      {
        try
        {
          var newPowerOfAttorney = lenspec.Etalon.PowerOfAttorneys.Copy(powerOfAttorney);
          newPowerOfAttorney.Cityavis = line.Company?.City;
          if (powerOfAttorney.DocumentKind.Equals(requestCreatePOAKind))
            newPowerOfAttorney.DocumentKind = poaKind;
          else if (powerOfAttorney.DocumentKind.Equals(requestCreateNPOAKind))
            newPowerOfAttorney.DocumentKind = nPoaKind;
          newPowerOfAttorney.BusinessUnit = line.Company;
          newPowerOfAttorney.ValidTill = powerOfAttorney.ValidTill;
          newPowerOfAttorney.OurSignatory = lenspec.Etalon.Employees.As(line.Company.CEO);
          newPowerOfAttorney.Save();
          if (powerOfAttorney.HasVersions)
          {
            using (var bodyStream = new System.IO.MemoryStream())
            {
              powerOfAttorney.LastVersion.Body.Read().CopyTo(bodyStream);
              newPowerOfAttorney.CreateVersionFrom(bodyStream, powerOfAttorney.LastVersion.AssociatedApplication.Extension);
            }
          }
          _obj.AddendaGroup.OfficialDocuments.Add(newPowerOfAttorney);
          powerOfAttorney.Relations.Add(Sungero.Docflow.Constants.Module.AddendumRelationName, newPowerOfAttorney);
          powerOfAttorney.Relations.Save();
          
          newPowerOfAttorney.UpdateTemplateParameters();
          newPowerOfAttorney.Save();
          
          ++countSuccessCreation;
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Avis - CreatePowerOfAttorneysFromDialog: Ошибка создания доверенности - {0}", ex.Message);
        }
      }
      var message = string.Empty;
      var countNeedCreation = powerOfAttorney.OurBusinessUavis.Count;
      if (countSuccessCreation != countNeedCreation)
      {
        var countErrors = countNeedCreation - countSuccessCreation;
        message = string.Format("Не удалось создать {0} из {1} доверенностей. Обратитесь к администратору.", countErrors, countNeedCreation);
      }
      return message;
    }
    
    /// <summary>
    /// Обновить карточку заявки на оформление доверенности, вложенную в задачу.
    /// </summary>
    /// <param name="powerOfAttorney">Доверенность (заявка).</param>
    /// <param name="fields">Обновляемые поля.</param>
    [Remote]
    public bool UpdatePowerOfAttorney(lenspec.Etalon.IPowerOfAttorney powerOfAttorney, lenspec.Etalon.Structures.Docflow.PowerOfAttorney.IUpdatableFields fields)
    {
      var prefix = "Avis - UpdatePowerOfAttorney: ";
      try
      {
        // Обновить поля карточки заявки.
        lenspec.Etalon.PublicFunctions.PowerOfAttorney.Remote.UpdateFields(powerOfAttorney, fields);
        
        // Обновить тему задачи и задания, так как имя заявления на оформление доверенности может измениться.
        var subject = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(Sungero.Docflow.ApprovalTasks.Resources.TaskSubject, powerOfAttorney.Name);
        _obj.Task.Subject = subject;
        _obj.Subject =      subject;
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat(prefix + lenspec.Etalon.ApprovalAssignments.Resources.ErrorUpdatingRequestForCreatingPoAFields, ex.Message);
        return false;
      }
      return true;
    }
    
    [Remote(IsPure = true)]
    public static bool CheckResponsibleClerkOnDialog(List<Sungero.Company.IBusinessUnit> businessUnits)
    {
      if (businessUnits.Count <= 1)
        return true;
      
      var firstBusinessUnit = lenspec.Etalon.BusinessUnits.As(businessUnits.FirstOrDefault());
      var roleKindEmployeeResponsibleClerk = firstBusinessUnit.RoleKindEmployeelenspec
        .FirstOrDefault(x => x.RoleKind.Name.Equals(lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk));
      if (roleKindEmployeeResponsibleClerk == null)
        return false;
      
      var firstPerformer = roleKindEmployeeResponsibleClerk.Employee;
      foreach (var line in businessUnits)
      {
        var convertCompany = lenspec.Etalon.BusinessUnits.As(line);
        if (convertCompany.RoleKindEmployeelenspec.Any(x => x.RoleKind.Name.Equals(lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk) && x.Employee.Equals(firstPerformer)))
          continue;

        return false;
      }
      
      return true;
    }
    
    [Remote(IsPure = true)]
    public static IQueryable<Sungero.Company.IBusinessUnit> GetBusinessUnitsForDialog()
    {
      var businessUnits = Sungero.Company.BusinessUnits.GetAll().Where(x => x.Status == Sungero.Company.BusinessUnit.Status.Active);
      return businessUnits;
    }
    
    [Remote(IsPure = true)]
    public static IQueryable<Sungero.Company.IEmployee> GetEmployeesForDialog()
    {
      var employees = Sungero.Company.Employees.GetAll().Where(x => x.Status == Sungero.Company.Employee.Status.Active);
      return employees;
    }
    
    [Remote(IsPure = true)]
    public static IQueryable<Sungero.Parties.ICounterparty> GetCounterpartiesForDialog()
    {
      var counterparties = Sungero.Parties.Counterparties.GetAll().Where(x => x.Status == Sungero.Parties.Counterparty.Status.Active);
      return counterparties;
    }
    
    [Remote(IsPure = true)]
    public static IQueryable<Sungero.Docflow.IDocumentKind> GetDocumentKindsForDialog()
    {
      var documentKinds = Sungero.Docflow.DocumentKinds.GetAll().Where(x => x.Status == Sungero.Docflow.DocumentKind.Status.Active);
      return documentKinds;
    }
    
    [Remote(IsPure = true)]
    public static IQueryable<Sungero.Contracts.IContractCategory> GetContractCategoriesForDialog()
    {
      var statusActive = Sungero.Contracts.ContractCategory.Status.Active;
      return Sungero.Contracts.ContractCategories.GetAll().Where(x => x.Status == statusActive);
    }
    //конец Добавлено Avis Expert
  }
}