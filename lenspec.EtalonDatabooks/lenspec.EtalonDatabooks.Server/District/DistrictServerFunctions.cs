using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.District;

namespace lenspec.EtalonDatabooks.Server
{
  partial class DistrictFunctions
  {
    
    /// <summary>
    /// Получить населенные пункты текущего округа
    /// </summary>
    /// <returns>Населенные пункты</returns>
    [Remote(IsPure = true)]
    public List<lenspec.Etalon.ICity> GetRelatedCities()
    {
      var cities = lenspec.Etalon.Cities.GetAll(x => _obj.Equals(x.Districtlenspec) && x.Status == lenspec.Etalon.City.Status.Active);
      return cities.ToList();
    }
    
    /// <summary>
    /// Получить районы текущего округа
    /// </summary>
    /// <returns>Районы</returns>
    [Remote(IsPure = true)]
    public List<lenspec.EtalonDatabooks.IArea> GetRelatedAreas()
    {
      var areas = lenspec.EtalonDatabooks.Areas.GetAll(x => _obj.Equals(x.District) && x.Status == lenspec.EtalonDatabooks.Area.Status.Active);
      return areas.ToList();
    }

    /// <summary>
    /// Получить регионы текущего округа
    /// </summary>
    /// <returns>Регионы</returns>
    [Remote(IsPure = true)]
    public List<lenspec.Etalon.IRegion> GetRelatedRegions()
    {
      var regions = lenspec.Etalon.Regions.GetAll(x => _obj.Equals(x.Districtlenspec) && x.Status == lenspec.Etalon.Region.Status.Active);
      return regions.ToList();
    }

    /// <summary>
    /// Получение дублей округа.
    /// </summary>
    /// <returns>Округа, дублирующие текущий.</returns>
    [Remote(IsPure = true)]
    public IQueryable<IDistrict> GetDuplicates()
    {
      return Districts.GetAll(d => d.Name == _obj.Name && !Equals(d, _obj));
    }

  }
}