using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.OutgoingLetters.MassMailingApplication;

namespace lenspec.OutgoingLetters
{
  partial class MassMailingApplicationClientHandlers
  {

    //Добавлено Avis Expert
    public override void PreparedByValueInput(Sungero.Docflow.Client.OfficialDocumentPreparedByValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.PreparedByValueInput(e);
    }

    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var isBankDetailRequired = _obj.MailingType != null && _obj.MailingType.BankDetailsAreRequired == true;
      _obj.State.Properties.Bank.IsEnabled = _obj.State.Properties.Bank.IsRequired = isBankDetailRequired;
      _obj.State.Properties.BIC.IsEnabled = _obj.State.Properties.BIC.IsRequired = isBankDetailRequired;
      _obj.State.Properties.CorrespondentAccount.IsEnabled = _obj.State.Properties.CorrespondentAccount.IsRequired = isBankDetailRequired;
      _obj.State.Properties.SettlementAccount.IsEnabled = _obj.State.Properties.SettlementAccount.IsRequired = isBankDetailRequired;
      
      _obj.State.Properties.DeliveryMethod.IsVisible = _obj.State.Properties.DeliveryMethod.IsEnabled = true;
      _obj.State.Properties.DocumentRegister.IsVisible = true;
    }
    //конец Добавлено Avis Expert

  }
}