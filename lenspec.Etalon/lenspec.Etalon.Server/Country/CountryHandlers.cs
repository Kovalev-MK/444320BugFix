using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Country;

namespace lenspec.Etalon
{
  partial class CountryServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Проверить валидность кода.
      var result = Sungero.Commons.Shared.CountryFunctions.ValidateCountryCode(_obj.Code);
      if (!string.IsNullOrEmpty(result))
        e.AddError(Countries.Resources.InvalidCountryCode);
      
      if (Functions.Country.HaveDuplicates(_obj))
        e.AddError(Sungero.Commons.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
    }
  }

}