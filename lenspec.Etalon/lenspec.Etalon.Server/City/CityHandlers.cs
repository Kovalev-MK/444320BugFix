using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.City;

namespace lenspec.Etalon
{
  partial class CityServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      var duplicates = Functions.City.GetDuplicates(_obj);
      if (duplicates.Any())
      {
        e.AddError("Найдены дублирующие записи.", _obj.Info.Actions.ShowDuplicateslenspec);
        return;
      }
    }
  }

  partial class CityRegionPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> RegionFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Удалена коробочная фильтрация
      return query;
    }
  }

}