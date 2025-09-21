using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustomerRequest;

namespace avis.CustomerRequests
{
  partial class CustomerRequestManagementContractsMKDSharedCollectionHandlers
  {

    public virtual void ManagementContractsMKDDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      _obj.Relations.RemoveFrom(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, _deleted.ManagementContractMKD);
    }

    public virtual void ManagementContractsMKDAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
    }
  }

  partial class CustomerRequestManagementContractsMKDSharedHandlers
  {

    public virtual void ManagementContractsMKDManagementContractMKDChanged(avis.CustomerRequests.Shared.CustomerRequestManagementContractsMKDManagementContractMKDChangedEventArgs e)
    {
      _obj.CustomerRequest.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, e.OldValue, e.NewValue);
    }
  }

  partial class CustomerRequestSDAContractsSharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void SDAContractsContractChanged(avis.CustomerRequests.Shared.CustomerRequestSDAContractsContractChangedEventArgs e)
    {
      _obj.CustomerRequest.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, e.OldValue, e.NewValue);
    }
    //конец Добавлено Avis Expert
  }

  partial class CustomerRequestSDAContractsSharedCollectionHandlers
  {

    //Добавлено Avis Expert
    public virtual void SDAContractsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
    }

    public virtual void SDAContractsDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      _obj.Relations.RemoveFrom(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, _deleted.Contract);
    }
    //конец Добавлено Avis Expert
  }

  partial class CustomerRequestSharedHandlers
  {

    //Добавлено Avis Expert
    public override void RegistrationDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.RegistrationDateChanged(e);
      FillName();
    }

    public virtual void ManagementContractMKDChanged(avis.CustomerRequests.Shared.CustomerRequestManagementContractMKDChangedEventArgs e)
    {
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      if (e.NewValue == null || e.NewValue != e.OldValue)
      {
        _obj.ObjectOfProject = null;
        _obj.SDAContracts.Clear();
        _obj.ManagementContractsMKD.Clear();
      }
    }
    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      FillName();
    }

    public virtual void ClientChanged(avis.CustomerRequests.Shared.CustomerRequestClientChangedEventArgs e)
    {
      _obj.Telephon = e.NewValue?.Phones;
      if (e.NewValue != null || e.NewValue != e.OldValue)
      {
        var contractToRemove = _obj.SDAContracts.Where(x => x.Contract != null && !x.Contract.CounterpartyClient.Any(c => c.ClientItem.Equals(e.NewValue))).ToList();
        foreach(var contract in contractToRemove)
        {
          _obj.SDAContracts.Remove(contract);
        }
      }
      if (e.NewValue == null || e.NewValue != e.OldValue)
      {
        var managementContractToRemove = _obj.ManagementContractsMKD.Where(x => x.ManagementContractMKD != null && !x.ManagementContractMKD.Client.Equals(e.NewValue)).ToList();
        foreach(var managementContract in managementContractToRemove)
        {
          _obj.ManagementContractsMKD.Remove(managementContract);
        }
      }
      FillName();
    }

    public override void RegistrationNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.RegistrationNumberChanged(e);
      FillName();
    }

    public virtual void ReqCategoryChanged(avis.CustomerRequests.Shared.CustomerRequestReqCategoryChangedEventArgs e)
    {
      FillName();
    }
    //конец Добавлено Avis Expert
  }
}