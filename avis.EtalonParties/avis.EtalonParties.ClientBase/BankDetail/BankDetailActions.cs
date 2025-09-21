using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.BankDetail;

namespace avis.EtalonParties.Client
{
  partial class BankDetailActions
  {
    public virtual void Export1C(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanExport1C(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
    }

    public virtual void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.BankDetail.Remote.GetDuplicates(_obj);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(BankDetails.Resources.DuplicatesNotFound);

    }

    public virtual bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}