using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustomerRequest;

namespace avis.CustomerRequests
{
  partial class CustomerRequestManagementContractsMKDManagementContractMKDPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ManagementContractsMKDManagementContractMKDFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.LifeCycleState == Sungero.Docflow.OfficialDocument.LifeCycleState.Active);
      
       //Убрано по ТЗ https://tracker.yandex.ru/DIRRXMIGR-748
       //      if (_root.BusinessUnit != null)
       //        query = query.Where(x => _root.BusinessUnit == x.BusinessUnit);
      
      if (_root.Client != null)
        query = query.Where(x => _root.Client == x.Client);
      
      return query;
    }
  }

  partial class CustomerRequestManagementContractMKDPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public virtual IQueryable<T> ManagementContractMKDFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class CustomerRequestClientPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ClientFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.SDAContracts != null && _obj.SDAContracts.Any())
      {
        var contractClientsIds = _obj.SDAContracts.SelectMany(x => x.Contract.CounterpartyClient.Select(c => c.ClientItem)).Select(x => x.Id);
        query = query.Where(x => contractClientsIds.Contains(x.Id));
      }
      return query;
    }
  }

  partial class CustomerRequestObjectOfProjectPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ObjectOfProjectFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.BusinessUnit != null)
      {
        query = query.Where(x => x.SpecDeveloper != null && x.SpecDeveloper.Equals(_obj.BusinessUnit));
      }
      query = query.Where(x => x.IsLinkToInvest == true);
      return query;
    }
  }

  partial class CustomerRequestSDAContractsContractPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> SDAContractsContractFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.LifeCycleState == Sungero.Docflow.OfficialDocument.LifeCycleState.Active);
      
      if (_root.Client != null)
      {
        query = query.Where(x => x.CounterpartyClient.Any(s => _root.Client.Equals(s.ClientItem)));
      }
      //Убрано по ТЗ https://tracker.yandex.ru/DIRRXMIGR-748
      //      if (_root.BusinessUnit != null)
      //      {
      //        query = query.Where(x => _root.BusinessUnit.Equals(x.BusinessUnit));
      //      }
      return query;
    }
  }

  partial class CustomerRequestServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
    }

    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsClaim = false;
    }
  }
}