using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Country;

namespace lenspec.Etalon.Server
{
  partial class CountryFunctions
  {
    
    [Remote(IsPure = true)]
    public List<lenspec.EtalonDatabooks.IDistrict> GetRelatedDistricts()
    {
      var districts = lenspec.EtalonDatabooks.Districts.GetAll(x => _obj.Equals(x.Country) && x.Status == lenspec.EtalonDatabooks.District.Status.Active);
      return districts.ToList();
    }
    
    /// <summary>
    /// Получить населенные пункты текущей страны
    /// </summary>
    /// <returns>Населенные пункты</returns>
    [Remote(IsPure = true)]
    public List<lenspec.Etalon.ICity> GetRelatedCities()
    {
      var cities = lenspec.Etalon.Cities.GetAll(x => _obj.Equals(x.Country) && x.Status == lenspec.Etalon.City.Status.Active);
      return cities.ToList();
    }
    
    /// <summary>
    /// Получить районы текущей страны
    /// </summary>
    /// <returns>Районы</returns>
    [Remote(IsPure = true)]
    public List<lenspec.EtalonDatabooks.IArea> GetRelatedAreas()
    {
      var areas = lenspec.EtalonDatabooks.Areas.GetAll(x => _obj.Equals(x.Country) && x.Status == lenspec.EtalonDatabooks.Area.Status.Active);
      return areas.ToList();
    }

    /// <summary>
    /// Получить регионы текущей страны
    /// </summary>
    /// <returns>Регионы</returns>
    [Remote(IsPure = true)]
    public List<lenspec.Etalon.IRegion> GetRelatedRegions()
    {
      var regions = lenspec.Etalon.Regions.GetAll(x => _obj.Equals(x.Country) && x.Status == lenspec.Etalon.Region.Status.Active);
      return regions.ToList();
    }
    
    /// <summary>
    /// Получение дублей страны.
    /// </summary>
    /// <returns>Повторяющиеся записи.</returns>
    [Remote(IsPure = true)]
    public new IQueryable<ICountry> GetDuplicates()
    {
      var duplicates = Countries.GetAll().Where(
        c =>
        !Equals(c, _obj) &&
        (
          (
            c.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed &&
            Equals(c.Code, _obj.Code)
           ) ||
          c.Name == _obj.Name
         )
       );
      
      return duplicates;
    }
  }
}