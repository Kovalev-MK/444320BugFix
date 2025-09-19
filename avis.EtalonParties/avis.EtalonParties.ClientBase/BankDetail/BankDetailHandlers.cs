using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.BankDetail;

namespace avis.EtalonParties
{
  partial class BankDetailClientHandlers
  {

    public virtual void BICValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (string.IsNullOrEmpty(e.NewValue))
        return;
      
      // Проверка корректности БИК.
      var errorMessage = Sungero.Parties.PublicFunctions.Bank.CheckBicLength(e.NewValue);
      if (!string.IsNullOrEmpty(errorMessage))
      {
        e.AddError(errorMessage);
        return;
      }
      
      // Поиск банка по БИК.
      var bank = Functions.BankDetail.Remote.GetBank(e.NewValue);
      if (bank == null)
      {
        e.AddError(avis.EtalonParties.BankDetails.Resources.BankNotFoundByBIC);
        return;
      }
    }

    public virtual void BankAccountValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      e.NewValue = e.NewValue.Trim();
      
      if (!System.Text.RegularExpressions.Regex.IsMatch(e.NewValue, @"^[0-9]*$") ||
          e.NewValue.Length != Constants.BankDetail.BankAccountValueLength)
        e.AddError(avis.EtalonParties.BankDetails.Resources.IncorrectBankAccount);
    }

  }
}