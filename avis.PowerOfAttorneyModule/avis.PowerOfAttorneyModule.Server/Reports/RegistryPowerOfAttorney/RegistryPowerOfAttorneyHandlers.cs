using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PowerOfAttorneyModule
{
  partial class RegistryPowerOfAttorneyServerHandlers
  {

    public virtual IQueryable<lenspec.Etalon.IPowerOfAttorney> GetPowerOfAttorneysData()
    {
      var poas = lenspec.Etalon.PowerOfAttorneys.GetAll(x => x.IsProjectPOAavis != true);
      
      if (RegistryPowerOfAttorney.Companies.Any())
        poas = poas.Where(x => x.BusinessUnit != null && RegistryPowerOfAttorney.Companies.Contains(x.BusinessUnit));
      
//      if(RegistryPowerOfAttorney.Attorney != null && RegistryPowerOfAttorney.Attorney.Any())
//        poas = poas.Where(x => x.Attorneyavis.Select(i => i.Attorn).Any(k => RegistryPowerOfAttorney.Attorney.Contains(k)));
      
      if(RegistryPowerOfAttorney.ValidFrom != null)
        poas = poas.Where(x => x.ValidFrom.Value >= RegistryPowerOfAttorney.ValidFrom.Value);
      
      if(RegistryPowerOfAttorney.ValidTo != null)
        poas = poas.Where(x => x.ValidTill.Value <= RegistryPowerOfAttorney.ValidTo.Value);
      
      if(RegistryPowerOfAttorney.Responsible != null && RegistryPowerOfAttorney.Responsible.Any())
        poas = poas.Where(x => RegistryPowerOfAttorney.Responsible.Contains(x.PreparedBy));
      
      return poas;
    }
  }

}