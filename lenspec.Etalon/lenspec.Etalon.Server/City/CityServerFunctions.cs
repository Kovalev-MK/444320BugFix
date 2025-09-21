using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.City;

namespace lenspec.Etalon.Server
{
  partial class CityFunctions
  {
    /// <summary>
    /// Получение дублей населенного пункта.
    /// </summary>
    /// <returns>Повторяющиеся записи.</returns>
    [Remote(IsPure = true)]
    public List<ICity> GetDuplicates()
    {
      var duplicates = Cities.GetAll().Where(c => !Equals(c, _obj) && Equals(_obj.Name.ToLower(), c.Name.ToLower()) && Equals(_obj.Region, c.Region));
      return duplicates.ToList();
    }
  }
}