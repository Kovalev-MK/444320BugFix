using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.SubstitutionRequestTask;

namespace lenspec.AutomatedSupportTickets
{
  partial class SubstitutionRequestTaskClientHandlers
  {

    //Добавлено Avis Expert
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      _obj.State.Properties.ManagerOfSubstitutedUser.IsVisible = _obj.State.Properties.ManagerOfSubstitutedUser.IsRequired = _obj.Prolongation != true;
    }

    public virtual void SubstitutedUserValueInput(lenspec.AutomatedSupportTickets.Client.SubstitutionRequestTaskSubstitutedUserValueInputEventArgs e)
    {
      if (e.NewValue != null && _obj.Substitute != null && e.NewValue.Equals(_obj.Substitute))
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.UserCannotReplaceHimself);
        return;
      }
    }

    public virtual void SubstituteValueInput(lenspec.AutomatedSupportTickets.Client.SubstitutionRequestTaskSubstituteValueInputEventArgs e)
    {
      if (e.NewValue != null && _obj.SubstitutedUser != null && e.NewValue.Equals(_obj.SubstitutedUser))
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.UserCannotReplaceHimself);
        return;
      }
      if (e.NewValue != null && _obj.ManagerOfSubstitutedUser != null && e.NewValue.Equals(_obj.ManagerOfSubstitutedUser))
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.NeedDifferentSubstituteAndManagerOfSubstituted, _obj.Info.Properties.Substitute);
        return;
      }
      if (e.NewValue != null)
      {
        if (!lenspec.AutomatedSupportTickets.PublicFunctions.Module.Remote.CheckAutomatedUser(e.NewValue))
        {
          e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.NeedSpecifyAutomatedEmployee);
          return;
        }
      }
    }

    public virtual void ManagerOfSubstitutedUserValueInput(lenspec.AutomatedSupportTickets.Client.SubstitutionRequestTaskManagerOfSubstitutedUserValueInputEventArgs e)
    {
      if (e.NewValue != null && e.NewValue.Equals(_obj.Substitute))
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.NeedDifferentSubstituteAndManagerOfSubstituted, _obj.Info.Properties.ManagerOfSubstitutedUser);
        return;
      }
      if (e.NewValue != null)
      {
        if (!lenspec.AutomatedSupportTickets.PublicFunctions.Module.Remote.CheckAutomatedUser(e.NewValue))
        {
          e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.NeedSpecifyAutomatedEmployee);
          return;
        }
      }
    }

    public virtual void EndDateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue == null)
        return;
      
      if (e.NewValue.Value < Calendar.Today || _obj.StartDate != null && e.NewValue.Value < _obj.StartDate.Value)
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.IncorrectEndDate);
        return;
      }
      
      if (_obj.StartDate != null && e.NewValue.Value > _obj.StartDate.Value.AddDays(90))
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.IncorrectSubstitutionPeriod);
        return;
      }
    }

    public virtual void StartDateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue != null && e.NewValue < Calendar.Today)
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.IncorrectStartDate);
        return;
      }
      
      if (e.NewValue != null && _obj.EndDate != null && e.NewValue.Value > _obj.EndDate.Value)
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.IncorrectStartDateByEndDate);
        return;
      }
    }
    //конец Добавлено Avis Expert

  }
}