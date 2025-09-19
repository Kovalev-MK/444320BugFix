using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientContract;

namespace lenspec.SalesDepartmentArchive.Server
{
  partial class SDAClientContractFunctions
  {
    
    /// <summary>
    /// Выборка клиентских договоров (без учета прав доступа).
    /// </summary>
    /// <param name="objectsAnProjects">Объекты проектов.</param>
    /// <param name="ourCFs">ИСП.</param>
    /// <returns>Отфильтрованная выборка (хотя бы по одному условию).</returns>
    [Public]
    public static IQueryable<ISDAClientContract> GetContracts(
      List<lenspec.EtalonDatabooks.IObjectAnProject> objectsAnProjects,
      List<lenspec.EtalonDatabooks.IOurCF> ourCFs)
    {
      var result = new List<ISDAClientContract>().AsQueryable();
      AccessRights.AllowRead(
        () =>
        {
          result = SDAClientContracts.GetAll(c => c.ObjectAnProject != null)
            .Where(c =>
                   objectsAnProjects.Contains(c.ObjectAnProject) ||
                   c.ObjectAnProject.OurCF != null && ourCFs.Contains(c.ObjectAnProject.OurCF));
        });
      return result;
    }
    
    /// <summary>
    /// Выборка клиентских договоров (без учета прав доступа).
    /// </summary>
    /// <param name="counterparty">Контрагент.</param>
    /// <returns>Отфильтрованная выборка.</returns>
    [Public]
    public static IQueryable<ISDAClientContract> GetContracts(Sungero.Parties.ICounterparty counterparty)
    {
      var result = new List<ISDAClientContract>().AsQueryable();
      if (counterparty == null)
        return result;
      
      AccessRights.AllowRead(
        () =>
        {
          result = SDAClientContracts.GetAll(x => x.CounterpartyClient.Any(c => 
                                                                           c.ClientItem != null && 
                                                                           Equals(c.ClientItem, counterparty)));
        });
      return result;
    }
    
    #region [Выборка клиентов по договорам]
    
    /// <summary>
    /// Получить клиентов по договорам.
    /// </summary>
    /// <param name="contracts">Договоры.</param>
    /// <returns>Клиенты.</returns>
    private static IQueryable<Sungero.Parties.ICounterparty> GetClients(IQueryable<ISDAClientContract> contracts)
    {
      var clientCollections = contracts
        .Select(contract => contract.CounterpartyClient.Select(client => client.ClientItem));
      
      var clients = new HashSet<Sungero.Parties.ICounterparty>();
      var count = 0;
      
      foreach (var contract in contracts)
        foreach (var client in contract.CounterpartyClient.Select(c => c.ClientItem))
        {
          clients.Add(client);
          // Отсекаем лишние записи на этапе выборки.
          if (++count > EtalonDatabooks.PublicConstants.Module.MaxClientsOwnersSelectionSize)
            break;
        }
      
      return clients.AsQueryable();
    }
    
    /// <summary>
    ///  Получить клиентов по договорам.
    /// </summary>
    /// <param name="objectAnSale">Помещение.</param>
    /// <param name="transferOfOwnershipDateFilled">Признак заполнения поля "Дата передачи по АПП".</param>
    /// <returns>Отфильтрованная по критериям выборка.</returns>
    [Public, Remote(IsPure = true)]
    public static IQueryable<Sungero.Parties.ICounterparty> GetClients(
      lenspec.EtalonDatabooks.IObjectAnSale objectAnSale,
      bool transferOfOwnershipDateFilled)
    {
      var contracts = SDAClientContracts
        .GetAll(c => c.TransferOfOwnershipDate.HasValue == transferOfOwnershipDateFilled)
        .Where(c => Equals(c.Premises, objectAnSale));
      return GetClients(contracts);
    }
    
    /// <summary>
    ///  Получить клиентов по договорам.
    /// </summary>
    /// <param name="objectAnProject">Объект проекта.</param>
    /// <param name="transferOfOwnershipDateFilled">Признак заполнения поля "Дата передачи по АПП".</param>
    /// <returns>Отфильтрованная по критериям выборка.</returns>
    [Public, Remote(IsPure = true)]
    public static IQueryable<Sungero.Parties.ICounterparty> GetClients(
      lenspec.EtalonDatabooks.IObjectAnProject objectAnProject,
      bool transferOfOwnershipDateFilled)
    {
      var contracts = SDAClientContracts
        .GetAll(c => c.TransferOfOwnershipDate.HasValue == transferOfOwnershipDateFilled)
        .Where(c => Equals(c.ObjectAnProject, objectAnProject));
      return GetClients(contracts);
    }
    
    /// <summary>
    ///  Получить клиентов по договорам.
    /// </summary>
    /// <param name="ourCF">ИСП.</param>
    /// <param name="transferOfOwnershipDateFilled">Признак заполнения поля "Дата передачи по АПП".</param>
    /// <returns>Отфильтрованная по критериям выборка.</returns>
    [Public, Remote(IsPure = true)]
    public static IQueryable<Sungero.Parties.ICounterparty> GetClients(
      lenspec.EtalonDatabooks.IOurCF ourCF,
      bool transferOfOwnershipDateFilled)
    {
      var contracts = SDAClientContracts
        .GetAll(c => c.TransferOfOwnershipDate.HasValue == transferOfOwnershipDateFilled)
        .Where(c => c.ObjectAnProject != null && Equals(c.ObjectAnProject.OurCF, ourCF));
      return GetClients(contracts);
    }
    
    #endregion [Выборка клиентов по договорам]
    
  }
}