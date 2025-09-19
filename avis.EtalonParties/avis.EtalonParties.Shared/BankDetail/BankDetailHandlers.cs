using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.BankDetail;

namespace avis.EtalonParties
{
  partial class BankDetailSharedHandlers
  {

    public virtual void BICChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (string.IsNullOrEmpty(e.NewValue))
      {
        _obj.Bank = null;
        _obj.CorrespondentAccount = null;
        return;
      }
      
      var bank = Functions.BankDetail.Remote.GetBank(e.NewValue);
      if (bank == null)
        return;
      
      _obj.Bank = bank;
      _obj.CorrespondentAccount = bank.CorrespondentAccount;
    }

    public virtual void BankAccountChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      Functions.BankDetail.FillName(_obj);
    }

    public virtual void CounterpartyChanged(avis.EtalonParties.Shared.BankDetailCounterpartyChangedEventArgs e)
    {
      Functions.BankDetail.FillName(_obj);
    }

    public virtual void BankChanged(avis.EtalonParties.Shared.BankDetailBankChangedEventArgs e)
    {
      Functions.BankDetail.FillName(_obj);
    }

  }
}