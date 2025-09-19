using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PowerOfAttorneyModule.Server
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Найти доверенности (и электронные доверенности) по диалогу
    /// </summary>
    /// <returns>Список доверенностей</returns>
    [Remote]
    public List<Sungero.Docflow.IPowerOfAttorneyBase> SearchPoawerOfAttorneyFromDialog(Sungero.Docflow.IDocumentKind kind, Sungero.Company.IBusinessUnit company, DateTime validFrom, Sungero.Company.IEmployee employee, Sungero.Parties.ICounterparty attorney)
    {
      var powerOfAttorneys = Sungero.Docflow.PowerOfAttorneyBases.GetAll(x => company.Equals(x.BusinessUnit) && x.ValidFrom >= validFrom);
      if (kind != null)
        powerOfAttorneys = powerOfAttorneys.Where(x => kind.Equals(x.DocumentKind));
      if (employee != null)
        powerOfAttorneys = powerOfAttorneys.Where(x => employee.Equals(x.PreparedBy));
      if (attorney != null)
        powerOfAttorneys = powerOfAttorneys.Where(x => attorney.Equals(x.IssuedToParty) || (Sungero.Parties.People.Is(attorney) && attorney.Equals(x.IssuedTo.Person)));
       
      return powerOfAttorneys.ToList();
    }



  }
}