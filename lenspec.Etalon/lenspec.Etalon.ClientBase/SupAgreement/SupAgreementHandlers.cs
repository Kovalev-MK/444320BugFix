using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.SupAgreement;

namespace lenspec.Etalon
{
  partial class SupAgreementClientHandlers
  {

    public override void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      //Снять проверку на ввод только положительной суммы в ДС.
      //base.TotalAmountValueInput(e);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Properties.IsStandard.IsEnabled = false;
      _obj.State.Properties.ConstructionObjectsavis.Properties.Summ.IsEnabled = false;
      _obj.State.Properties.Currency.IsEnabled = false;
    }

  }
}