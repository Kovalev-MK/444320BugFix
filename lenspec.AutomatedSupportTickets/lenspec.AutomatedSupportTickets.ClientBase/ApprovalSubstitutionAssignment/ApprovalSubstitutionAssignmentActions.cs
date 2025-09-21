using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.ApprovalSubstitutionAssignment;

namespace lenspec.AutomatedSupportTickets.Client
{
  partial class ApprovalSubstitutionAssignmentActions
  {
    //Добавлено Avis Expert
    public virtual void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dublicate = Functions.ApprovalSubstitutionAssignment.Remote.GetDublicatesInSubstitution(_obj);
      if (dublicate.Any())
      {
        dublicate.Show();
      }
      else
      {
        Dialogs.NotifyMessage(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.DuplicateNotFound);
      }
    }

    public virtual bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void Reject(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      // Валидация заполненности активного текста.
      if (!Functions.SubstitutionRequestTask.ValidateBeforeReject(_obj, AutomatedSupportTickets.SubstitutionRequestTasks.Resources.NeedTextForReject, e))
        e.Cancel();
    }

    public virtual bool CanReject(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.EndDate == null || _obj.EndDate.Value > _obj.StartDate.Value.AddDays(90))
      {
        e.AddError(AutomatedSupportTickets.SubstitutionRequestTasks.Resources.EndDateNeeded);
        e.Cancel();
      }
      
      var dublicate = Functions.ApprovalSubstitutionAssignment.Remote.GetDublicatesInSubstitution(_obj);
      if (dublicate.Any())
      {
        e.AddError(AutomatedSupportTickets.SubstitutionRequestTasks.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
        return;
      }
      
      var task = AutomatedSupportTickets.SubstitutionRequestTasks.As(_obj.Task);
      if (task != null)
      {
        task.EndDate = _obj.EndDate.Value;
        task.Save();
      }
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }
    //конец Добавлено Avis Expert

  }

}