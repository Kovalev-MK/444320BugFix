using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.ResponsibleByCounterparty;

namespace avis.EtalonParties.Server
{
  partial class ResponsibleByCounterpartyFunctions
  {
    /// <summary>
    /// Получить список ИД НОР текущего сотрудника и НОР сотрудников, которых он замещает.
    /// </summary>
    /// <returns>Список ИД НОР.</returns>
    [Remote(IsPure = true)]
    public List<long> GetSubstitutedBusinessUnitIds()
    {
      var businessUnitIds = new List<long>();
      if (Sungero.Company.Employees.Current == null)
        return businessUnitIds;
      
      var substitutedBUIds = Substitutions.SubstitutedUsers
        .Where(x => Sungero.Company.Employees.Is(x) && Sungero.Company.Employees.As(x).Department.BusinessUnit != null)
        .Select(x => Sungero.Company.Employees.As(x).Department.BusinessUnit.Id)
        .ToList();
      businessUnitIds.AddRange(substitutedBUIds);
      
      if (Sungero.Company.Employees.Current.Department.BusinessUnit != null)
        businessUnitIds.Add(Sungero.Company.Employees.Current.Department.BusinessUnit.Id);
      
      return businessUnitIds;
    }
    
    /// <summary>
    /// Получить дубли записей справочника Ответственные по контрагентам.
    /// </summary>
    /// <returns>Ответственные по контрагентам, дублирующие по полям Контрагент и Наша организация.</returns>
    [Remote(IsPure = true)]
    public IQueryable<avis.EtalonParties.IResponsibleByCounterparty> GetDuplicates()
    {
      return avis.EtalonParties.ResponsibleByCounterparties.GetAll()
        .Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active &&
               Equals(_obj.Counterparty, x.Counterparty) &&
               Equals(_obj.BusinessUnit, x.BusinessUnit));
    }
  }
}