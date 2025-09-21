using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.BusinessUnit;

namespace lenspec.Etalon
{
  partial class BusinessUnitClientHandlers
  {
    //Добавлено Avis Expert
    public virtual void TenderAmountavisValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      if (e.NewValue != null && e.NewValue < 0)
        e.AddError(lenspec.Etalon.BusinessUnits.Resources.ValueNoNumberErrorMessage);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Properties.CompanyProfileavis.IsRequired = true;
    }
    //конец Добавлено Avis Expert
  }


}