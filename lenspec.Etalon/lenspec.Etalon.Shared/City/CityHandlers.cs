using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.City;

namespace lenspec.Etalon
{
  partial class CitySharedHandlers
  {

    public virtual void ArealenspecChanged(lenspec.Etalon.Shared.CityArealenspecChangedEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.Region = null;
        return;
      }
      
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.Region = e.NewValue.Region;
    }

    public override void CountryChanged(Sungero.Commons.Shared.CityCountryChangedEventArgs e)
    {
      // Убираем базовый обработчик (страна заполнится автоматически, доп. проверки не нужны).
      //base.CountryChanged(e);
    }

    public override void RegionChanged(Sungero.Commons.Shared.CityRegionChangedEventArgs e)
    {
      _obj.Districtlenspec = lenspec.Etalon.Regions.As(e.NewValue)?.Districtlenspec;
      _obj.Country = e.NewValue?.Country;
    }
  }
}