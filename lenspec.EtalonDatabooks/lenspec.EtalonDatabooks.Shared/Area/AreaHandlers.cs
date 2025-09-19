using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.Area;

namespace lenspec.EtalonDatabooks
{
  partial class AreaSharedHandlers
  {
	public virtual void NameChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      
    }
	  
    public virtual void RegionChanged(lenspec.EtalonDatabooks.Shared.AreaRegionChangedEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.District = null;
        _obj.Country = null;
        return;
      }
      
      if (!Equals(e.NewValue, e.OldValue))
      {
        var region = lenspec.Etalon.Regions.As(e.NewValue);
        
        _obj.District = region?.Districtlenspec;
        _obj.Country = e.NewValue.Country;
      }
    }
  }
}