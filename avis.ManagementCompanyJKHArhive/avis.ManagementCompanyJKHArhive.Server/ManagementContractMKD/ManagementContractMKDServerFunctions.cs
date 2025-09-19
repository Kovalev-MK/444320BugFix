using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ManagementCompanyJKHArhive.ManagementContractMKD;

namespace avis.ManagementCompanyJKHArhive.Server
{
  partial class ManagementContractMKDFunctions
  {
    
    #region [Выборка собственников по договорам]
    
    /// <summary>
    /// Получить собственников по договорам.
    /// <param name="objectAnSale">Помещение.</param>
    /// <returns>Отфильтрованная по критериям выборка.</returns>
    [Public, Remote(IsPure = true)]
    public static IQueryable<Sungero.Parties.ICounterparty> GetOwners(lenspec.EtalonDatabooks.IObjectAnSale objectAnSale)
    {
      var contracts = ManagementContractMKDs.GetAll(c => Equals(c.ObjectAnSale, objectAnSale));
      return contracts.Select(c => c.Client);
    }
    
    /// <summary>
    /// Получить собственников по договорам.
    /// <param name="objectAnProject">Объект проекта.</param>
    /// <returns>Отфильтрованная по критериям выборка.</returns>
    [Public, Remote(IsPure = true)]
    public static IQueryable<Sungero.Parties.ICounterparty> GetOwners(lenspec.EtalonDatabooks.IObjectAnProject objectAnProject)
    {
      var contracts = ManagementContractMKDs.GetAll(c => Equals(c.ObjectAnSale.ObjectAnProject, objectAnProject));
      return contracts.Select(c => c.Client);
    }
    
    /// <summary>
    /// Получить собственников по договорам.
    /// <param name="ourCF">ИСП.</param>
    /// <returns>Отфильтрованная по критериям выборка.</returns>
    [Public, Remote(IsPure = true)]
    public static IQueryable<Sungero.Parties.ICounterparty> GetOwners(lenspec.EtalonDatabooks.IOurCF ourCF)
    {
      var contracts = ManagementContractMKDs.GetAll(c => Equals(c.OurCF, ourCF));
      return contracts.Select(c => c.Client);
    }
    
    #endregion [Выборка собственников по договорам]
    
  }
}