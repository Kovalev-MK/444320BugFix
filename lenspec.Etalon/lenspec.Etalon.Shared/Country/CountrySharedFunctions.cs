using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Country;

namespace lenspec.Etalon.Shared
{
  partial class CountryFunctions
  {
    /// <summary>
    /// Проверить дубли стран.
    /// </summary>
    /// <returns>True, если дубликаты имеются, иначе - false.</returns>
    public new bool HaveDuplicates()
    {
      if (string.IsNullOrWhiteSpace(_obj.Code) ||
          Equals(_obj.Code, "000") ||
          _obj.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed)
        return false;
      
      return Functions.Country.Remote.GetDuplicates(_obj).Any();
    }
  }
}