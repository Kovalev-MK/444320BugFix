using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustReqSetup;

namespace avis.CustomerRequests.Client
{
  partial class CustReqSetupActions
  {
    
    //Добавлено Avis Expert
    public virtual void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.CustReqSetup.Remote.GetDuplicates(_obj);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(Sungero.Commons.Resources.DuplicateNotFound);
    }

    public virtual bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void SaveAndClose(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (Functions.CustReqSetup.HaveDuplicates(_obj))
      {
        e.AddError(avis.CustomerRequests.CustReqSetups.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
        return;
      }
      
      base.SaveAndClose(e);
    }

    public override bool CanSaveAndClose(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSaveAndClose(e);
    }

    public override void Save(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (Functions.CustReqSetup.HaveDuplicates(_obj))
      {
        e.AddError(avis.CustomerRequests.CustReqSetups.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
        return;
      }
      
      base.Save(e);
    }

    public override bool CanSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSave(e);
    }
    //конец Добавлено Avis Expert

  }

}