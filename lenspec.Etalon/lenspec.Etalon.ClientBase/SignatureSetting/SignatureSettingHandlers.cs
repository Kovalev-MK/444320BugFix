using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.SignatureSetting;

namespace lenspec.Etalon
{
  partial class SignatureSettingClientHandlers
  {

    //Добавлено Avis Expert
    public override void SigningReasonValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.SigningReasonValueInput(e);
      if ((_obj.Reason == Sungero.Docflow.SignatureSetting.Reason.FormalizedPoA || _obj.Reason == Sungero.Docflow.SignatureSetting.Reason.Duties) &&
          !string.IsNullOrEmpty(e.NewValue) && e.NewValue.Length > Constants.Docflow.SignatureSetting.SigningReasonMaxLenght)
      {
        e.AddWarning(_obj.Info.Properties.SigningReason, string.Format("Значение в поле {0} должно быть не более {1} символов. Длина строки изменилась.", _obj.Info.Properties.SigningReason.LocalizedName, Constants.Docflow.SignatureSetting.SigningReasonMaxLenght));
        e.NewValue = e.NewValue.Substring(0, Constants.Docflow.SignatureSetting.SigningReasonMaxLenght);
      }
    }

    public override void RecipientValueInput(Sungero.Docflow.Client.SignatureSettingRecipientValueInputEventArgs e)
    {
      if (e.NewValue != null && Sungero.Company.Employees.Is(e.NewValue) && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(Sungero.Company.Employees.As(e.NewValue)))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.RecipientValueInput(e);
    }
    //конец Добавлено Avis Expert

  }
}