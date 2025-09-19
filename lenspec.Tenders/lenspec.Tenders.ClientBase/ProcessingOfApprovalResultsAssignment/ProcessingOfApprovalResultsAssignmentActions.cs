using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ProcessingOfApprovalResultsAssignment;

namespace lenspec.Tenders.Client
{
  partial class ProcessingOfApprovalResultsAssignmentActions
  {
    public virtual void Rework(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var task = Tenders.ApprovalCounterpartyRegisterTasks.As(_obj.Task);
      task.DateOfLastProcessingResults = Calendar.Now;
    }

    public virtual bool CanRework(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Rejected(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var task = Tenders.ApprovalCounterpartyRegisterTasks.As(_obj.Task);
      var approvalResults = Functions.ApprovalCounterpartyRegisterTask.Remote.GetApprovalResultsCount(task);
      if (approvalResults.Approved != approvalResults.Rejected &&
          !Functions.ProcessingOfApprovalResultsAssignment.ShowConfirmationDialogOnSolution(_obj, false, approvalResults.Approved, approvalResults.Rejected))
        e.Cancel();
      
      var warnings = new List<string>();
      task.QCDecisionDate = Calendar.Now;
      task.QCDecision = Tenders.ApprovalCounterpartyRegisterTask.QCDecision.Negative;
      
      // Создать решение КК и добавить во вложения задачи.
      try
      {
        var counterpartyRegistryDecision = Functions.ApprovalCounterpartyRegisterTask.Remote.AttachCounterpartyRegistryDecision(task);
        var warning = Tenders.PublicFunctions.TenderDocument.Remote.AddStamp(counterpartyRegistryDecision);
        warnings.Add(warning);
      }
      catch (Exception ex)
      {
        var message = ProcessingOfApprovalResultsAssignments.Resources.QCDecisionCreationErrorFormat(ex.Message);
        e.AddError(message);
      }
      
      // Обработка некритичных ошибок.
      if (!Functions.ProcessingOfApprovalResultsAssignment.ShowConfirmationDialogOnWarnings(warnings))
        e.Cancel();
    }

    public virtual bool CanRejected(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Approved(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var task = Tenders.ApprovalCounterpartyRegisterTasks.As(_obj.Task);
      var approvalResults = Functions.ApprovalCounterpartyRegisterTask.Remote.GetApprovalResultsCount(task);
      
      if (approvalResults.Approved != approvalResults.Rejected &&
          !Functions.ProcessingOfApprovalResultsAssignment.ShowConfirmationDialogOnSolution(_obj, true, approvalResults.Approved, approvalResults.Rejected))
        e.Cancel();
      
      // Признак внесения в реестр КА.
      task.QCDecisionDate = Calendar.Now;
      task.QCDecision = Tenders.ApprovalCounterpartyRegisterTask.QCDecision.Positive;

      // Список предупреждений.
      var warnings = new List<string>();
      var counterpartyRegistryDecision = TenderDocuments.Null;
      
      // Создать реестр КА и заполнить из анкеты квалификации,
      // сформировать решение КК.
      if (task.Procedure == ApprovalCounterpartyRegisterTask.Procedure.Inclusion)
      {
        #region [Включение в реестр]
        
        // Создать решение КК и добавить во вложения задачи.
        try
        {
          // Решение КК.
          counterpartyRegistryDecision = Functions.ApprovalCounterpartyRegisterTask.Remote.AttachCounterpartyRegistryDecision(task);
          // Штамп.
          var warning = Tenders.PublicFunctions.TenderDocument.Remote.AddStamp(counterpartyRegistryDecision);
          warnings.Add(warning);
        }
        catch (Exception ex)
        {
          var message = ProcessingOfApprovalResultsAssignments.Resources.QCDecisionCreationErrorFormat(ex.Message);
          e.AddError(message);
          return;
        }
        
        // Сформировать карточку реестра КА.
        var counterpartyRegister = CounterpartyRegisterBases.Null;
        try
        {
          // Карточка реестра КА.
          counterpartyRegister = Functions.ApprovalCounterpartyRegisterTask.Remote.CreateCounterpartyRegister(task, counterpartyRegistryDecision);
          // Загрузка данных о КА в карточку (при наличии).
          var tenderAccreditationForm = Functions.ProcessingOfApprovalResultsAssignment.Remote.GetTenderAccreditationForm(_obj);
          if (tenderAccreditationForm != null)
          {
            var error = Functions.CounterpartyRegisterBase.Remote.UploadCounterpartyData(counterpartyRegister, tenderAccreditationForm);
            // Пробросим сообщение в карточку реестра.
            if (!string.IsNullOrEmpty(error))
              ((Sungero.Domain.Shared.IExtendedEntity)counterpartyRegister).Params[Constants.Module.Params.MessageToCPRegisterFromProcessingAssignment] = error;
          }
        }
        catch (Exception ex)
        {
          var message = ProcessingOfApprovalResultsAssignments.Resources.CounterpartyRegisterCreationErrorFormat(ex.Message);
          e.AddError(message);
          return;
        }
        // Дозаполнение карточки пользователем.
        counterpartyRegister.ShowModal();
        // Некорректное завершение работы с карточкой.
        if (counterpartyRegister.State.IsInserted)
          e.Cancel();
        
        lenspec.Etalon.PublicFunctions.Company.Remote.UpdateRegisterStatus(
          lenspec.Etalon.Companies.As(task.Counterparty),
          task.RegisterKind == Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Provider,
          task.RegisterKind == Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Contractor
         );
        
        #endregion [Включение в реестр]
      }
      // Выбрать записи реестра КА для закрытия,
      // сформировать решение КК.
      else if (task.Procedure == ApprovalCounterpartyRegisterTask.Procedure.Exclusion)
      {
        #region [Исключение из реестра]
        
        // Создать решение КК и добавить во вложения задачи.
        try
        {
          counterpartyRegistryDecision = Functions.ApprovalCounterpartyRegisterTask.Remote.AttachCounterpartyRegistryDecision(task);
          var warning = Tenders.PublicFunctions.TenderDocument.Remote.AddStamp(counterpartyRegistryDecision);
          warnings.Add(warning);
        }
        catch (Exception ex)
        {
          var message = lenspec.Tenders.ProcessingOfApprovalResultsAssignments.Resources.QCDecisionCreationErrorFormat(ex.Message);
          e.AddError(message);
          return;
        }
        
        // Вывести пользователю диалоговое окно "Виды работ или товарные группы".
        var success = Functions.ProcessingOfApprovalResultsAssignment.ShowWorkKindOrMaterialGroupDialog(_obj, task.RegisterKind, task.Counterparty, counterpartyRegistryDecision);
        if (!success)
          e.Cancel();
        
        #endregion [Исключение из реестра]
      }
      else
      {
        e.AddError(Tenders.Resources.UnacceptableProcedureValueError);
        return;
      }
      
      // Обработка некритичных ошибок.
      if (!Functions.ProcessingOfApprovalResultsAssignment.ShowConfirmationDialogOnWarnings(warnings))
        e.Cancel();
      
      if (task.RegisterKind == Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Provider)
        Tenders.ProviderRegisters.GetAll().Show();
      else if (task.RegisterKind == Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Contractor)
        Tenders.ContractorRegisters.GetAll().Show();
    }

    public virtual bool CanApproved(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}