using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Region;

namespace lenspec.Etalon.Server
{
  partial class RegionFunctions
  {
    
    /// <summary>
    /// Получить населенные пункты текущего региона
    /// </summary>
    /// <returns>Населенные пункты</returns>
    [Remote(IsPure = true)]
    public List<lenspec.Etalon.ICity> GetRelatedCities()
    {
      var cities = lenspec.Etalon.Cities.GetAll(x => _obj.Equals(x.Region) && x.Status == lenspec.Etalon.City.Status.Active);
      return cities.ToList();
    }
    
    /// <summary>
    /// Получение дублей региона.
    /// </summary>
    /// <returns>Повторяющиеся записи.</returns>
    [Remote(IsPure = true)]
    public IQueryable<IRegion> GetDuplicates()
    {
      return Regions.GetAll(r =>
                            !Equals(r, _obj) &&
                            (
                              (!string.IsNullOrEmpty(_obj.Code) && Equals(_obj.Code, r.Code)) ||
                              (!string.IsNullOrEmpty(_obj.Name) && Equals(_obj.Name, r.Name))
                             ));
    }
  }
}