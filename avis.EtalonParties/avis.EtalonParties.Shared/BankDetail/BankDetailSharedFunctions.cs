using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.BankDetail;

namespace avis.EtalonParties.Shared
{
  partial class BankDetailFunctions
  {

    /// <summary>
    /// Автоматическое формирование наименования.
    /// </summary>
    public void FillName()
    {
      var name = BankDetails.Resources.NameFormat(
        _obj.Counterparty == null ?               "<Контрагент>" :      _obj.Counterparty.Name,
        _obj.Bank == null ?                       "<Банк>" :            _obj.Bank.Name,
        string.IsNullOrEmpty(_obj.BankAccount) ?  "<Расчетный счет>" :  _obj.BankAccount
       ).ToString();
      
      _obj.Name = name.Substring(0, Math.Min(Constants.BankDetail.NameValueLength, name.Length));
    }
  }
}