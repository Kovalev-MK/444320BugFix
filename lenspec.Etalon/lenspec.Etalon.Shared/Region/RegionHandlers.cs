using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Region;

namespace lenspec.Etalon
{
  partial class RegionSharedHandlers
  {

    public virtual void DistrictlenspecChanged(lenspec.Etalon.Shared.RegionDistrictlenspecChangedEventArgs e)
    {
      if (e.NewValue != null)
        _obj.Country = e.NewValue.Country;
      else
        _obj.Country = null;
    }
  }

}