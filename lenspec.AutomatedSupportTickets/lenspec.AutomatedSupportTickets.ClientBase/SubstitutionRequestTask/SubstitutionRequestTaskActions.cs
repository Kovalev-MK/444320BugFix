using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.SubstitutionRequestTask;

namespace lenspec.AutomatedSupportTickets.Client
{
  partial class SubstitutionRequestTaskActions
  {
    
    //Добавлено Avis Expert
    public virtual void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dublicate = Functions.SubstitutionRequestTask.Remote.GetDublicatesInSubstitution(_obj);
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

    public override void Start(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dublicate = Functions.SubstitutionRequestTask.Remote.GetDublicatesInSubstitution(_obj);
      if (dublicate.Any())
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
        return;
      }
      base.Start(e);
    }

    public override bool CanStart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanStart(e);
    }
    //конец Добавлено Avis Expert

  }

}