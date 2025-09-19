using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ApplicationRelinquishmentAuthority;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class ApplicationRelinquishmentAuthorityFunctions
  {

    [Converter("GetBusinessUnitsForTemplate")]
    public static string GetBusinessUnitsForTemplate(IApplicationRelinquishmentAuthority document)
    {
      string result = null;
      
      if (document.BusinessUnits.Any())
      {
        result = string.Join(",\n", document.BusinessUnits.Where(x => x.Company != null && !string.IsNullOrWhiteSpace(x.Company.Name)).Select(x => x.Company.Name));
      }

      return result;
    }
  }
}