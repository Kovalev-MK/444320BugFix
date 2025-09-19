using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Contract;

namespace lenspec.Etalon
{
  partial class ContractClientHandlers
  {

    public override void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      base.TotalAmountValueInput(e);
    }

    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.IsStandard.IsEnabled = false;
      
      PublicFunctions.Contract.IsFramework(_obj);
      PublicFunctions.Contract.IsDeferredPayment(_obj);
      
      _obj.State.Properties.PresenceRegionlenspec.IsRequired = true;
    }
  }
}