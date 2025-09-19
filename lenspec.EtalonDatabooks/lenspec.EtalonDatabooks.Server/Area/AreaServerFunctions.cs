using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.Area;

namespace lenspec.EtalonDatabooks.Server
{
  partial class AreaFunctions
  {
    /// <summary>
    /// Получение дублей Района.
    /// </summary>
    /// <returns>Повторяющиеся записи.</returns>
    [Remote(IsPure = true)]
    public IQueryable<IArea> GetDuplicates()
    {
      return Areas.GetAll(d =>
                          !Equals(d, _obj) &&
                          d.Name == _obj.Name &&
                          Equals(d.District, _obj.District));
    }
  }
}