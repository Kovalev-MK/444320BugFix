using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.City;

namespace lenspec.Etalon
{
  partial class CityClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Properties.Region.IsEnabled = _obj.Arealenspec == null;
    }
  }

}