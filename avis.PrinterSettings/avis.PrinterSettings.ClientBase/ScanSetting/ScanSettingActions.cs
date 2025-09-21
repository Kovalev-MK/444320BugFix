using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PrinterSettings.ScanSetting;

namespace avis.PrinterSettings.Client
{
  partial class ScanSettingActions
  {
    //Добавлено Avis Expert
    public virtual void SelectScanner(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanSelectScanner(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dublicate = Functions.ScanSetting.Remote.GetDublicates(_obj);
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
    //конец Добавлено Avis Expert

  }

}