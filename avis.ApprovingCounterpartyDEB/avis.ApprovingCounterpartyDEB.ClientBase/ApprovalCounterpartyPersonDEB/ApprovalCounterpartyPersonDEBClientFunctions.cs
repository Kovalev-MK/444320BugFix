using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB;

namespace avis.ApprovingCounterpartyDEB.Client
{
  partial class ApprovalCounterpartyPersonDEBFunctions
  {

    /// <summary>
    /// Видимость полей в зависимости от статуса задачи
    /// </summary>
    public void CheckPropertiesVisible()
    {
      var isNotDraft = _obj.Status != ApprovalCounterpartyPersonDEB.Status.Draft;
      _obj.State.Properties.CompleteDate.IsVisible = isNotDraft;
      _obj.State.Properties.Date.IsVisible = isNotDraft;
      _obj.State.Properties.Note.IsVisible = isNotDraft;
      _obj.State.Properties.IsNeedQualification.IsVisible = _obj.IsNeedQualification.HasValue && _obj.IsNeedQualification.Value;
      
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.FirstOrDefault();
      var thematicDocument = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEBs.As(document);
      var isAmountSmaller = thematicDocument != null && thematicDocument.EstimatedAmountTransaction.HasValue && 
        thematicDocument.EstimatedAmountTransaction.Value < Constants.ApprovalCounterpartyBankDEB.MinAmountVerification;
      
      _obj.State.Properties.CounterpartyLimit.IsVisible = isNotDraft && isAmountSmaller;
      _obj.State.Properties.ManagerInitiator.IsVisible = isAmountSmaller;
    }
    
    /// <summary>
    /// Показать диалог с задачами-дубликатами
    /// </summary>
    /// <param name="duplicates">Список задач</param>
    public static void GetDuplicateTasksDialog(List<avis.ApprovingCounterpartyDEB.IApprovalCounterpartyPersonDEB> duplicates)
    {
      var dialog = Dialogs.CreateTaskDialog("Документ уже направлен на согласование.");
      var showTasksButton = dialog.Buttons.AddCustom("Показать задачи");
      dialog.Buttons.AddCancel();
      if (dialog.Show() == showTasksButton)
        duplicates.Show("Дублирующие задачи");
    }

    /// <summary>
    /// Проверить наличие задач-дублкатов и вывести соответствующий диалог
    /// </summary>
    /// <param name="document">Документ</param>
    /// <returns>True - есть дубликаты</returns>
    public static bool CheckDuplicates(avis.ApprovingCounterpartyDEB.IApprovalCounterpartyBase document)
    {
      var duplicates = Functions.ApprovalCounterpartyPersonDEB.Remote.GetTaskDuplicates(document);
      var isHereDuplicates = duplicates.Any();
      if (isHereDuplicates)
        GetDuplicateTasksDialog(duplicates);
      return isHereDuplicates;
    }
    
    /// <summary>
    /// Проверить наличие задач-дублкатов и вывести соответствующий диалог
    /// </summary>
    /// <param name="task">Задача</param>
    /// <returns>True - есть дубликаты</returns>
    public static bool CheckDuplicates(avis.ApprovingCounterpartyDEB.IApprovalCounterpartyPersonDEB task)
    {
      var result = false;
      var document = task.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
      var duplicates = Functions.ApprovalCounterpartyPersonDEB.Remote.GetTaskDuplicates(document).Where(x => x.Id != task.Id).ToList();
      if (duplicates.Any())
      {
        result = true;
        GetDuplicateTasksDialog(duplicates);
      }
      return result;
    }

  }
}