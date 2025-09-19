using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.SubstitutionRequestTask;

namespace lenspec.AutomatedSupportTickets
{
  partial class SubstitutionRequestTaskSharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void ManagerOfSubstitutedUserChanged(lenspec.AutomatedSupportTickets.Shared.SubstitutionRequestTaskManagerOfSubstitutedUserChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (!lenspec.AutomatedSupportTickets.PublicFunctions.Module.Remote.CheckAutomatedUser(e.NewValue))
        {
          _obj.ManagerOfSubstitutedUser = null;
        }
      }
    }

    public virtual void ProlongationChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      _obj.State.Properties.EndDate.IsRequired = e.NewValue == false;
      
      this.FillSubject();
    }

    public virtual void SubstitutedUserChanged(lenspec.AutomatedSupportTickets.Shared.SubstitutionRequestTaskSubstitutedUserChangedEventArgs e)
    {
      this.FillSubject();
      
      if (e.NewValue != null)
      {
        _obj.ManagerOfSubstitutedUser = e.NewValue.Department != null ? e.NewValue.Department.Manager : null;
      }
    }

    public virtual void SubstituteChanged(lenspec.AutomatedSupportTickets.Shared.SubstitutionRequestTaskSubstituteChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (!lenspec.AutomatedSupportTickets.PublicFunctions.Module.Remote.CheckAutomatedUser(e.NewValue))
        {
          _obj.Substitute = null;
        }
      }
      
      this.FillSubject();
    }
    
    protected void FillSubject()
    {
      Functions.SubstitutionRequestTask.FillSubject(_obj);
    }
    //конец Добавлено Avis Expert

  }
}