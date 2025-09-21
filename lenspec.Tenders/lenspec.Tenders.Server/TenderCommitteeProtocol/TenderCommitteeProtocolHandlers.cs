using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderCommitteeProtocol;

namespace lenspec.Tenders
{
  partial class TenderCommitteeProtocolAddresseesAddresseePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AddresseesAddresseeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query;
    }
  }

  partial class TenderCommitteeProtocolTenderWinnerReservePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> TenderWinnerReserveFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Counterparties.Any())
      {
        var counterparties = _obj.Counterparties.Select(x => x.Counterparty);
        
        query = query.Where(x => counterparties.Contains(x) && x != _obj.TenderWinner);
      }
      
      if (_obj.TenderWinner != null)
        query = query.Where(x => x != _obj.TenderWinner);
      
      return query;
    }
  }

  partial class TenderCommitteeProtocolTenderWinnerPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> TenderWinnerFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Counterparties.Any())
      {
        var counterparties = _obj.Counterparties.Select(x => x.Counterparty);
        
        query = query.Where(x => counterparties.Contains(x));
      }
      
      return query;
    }
  }

  partial class TenderCommitteeProtocolBusinessUnitPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> BusinessUnitFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.BusinessUnitFiltering(query, e);
      
      var documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.TenderCommitteeProtocolKind);
      var registrationSettingBusinessUnints = Sungero.Docflow.RegistrationSettings.GetAll(x => x.DocumentKinds.Any(d => d.DocumentKind == documentKind))
        .SelectMany(x => x.BusinessUnits.Select(b => b.BusinessUnit))
        .ToList()
        .Distinct();
      return query.Where(x => registrationSettingBusinessUnints.Contains(x));
    }
  }

  partial class TenderCommitteeProtocolServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var notAutomatedAddresseesLines = new List<lenspec.Tenders.ITenderCommitteeProtocolAddressees>();
      
      foreach (var addresseeLine in _obj.Addressees)
      {
        // Проверка на автоматизированность сотрудника.
        if (addresseeLine != null && addresseeLine.Addressee != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(addresseeLine.Addressee))
          notAutomatedAddresseesLines.Add(addresseeLine);
      }
      
      if (notAutomatedAddresseesLines.Any())
      {
        foreach (var addresseeLine in notAutomatedAddresseesLines)
        {
          _obj.Addressees.Remove(addresseeLine);
        }
        
        var notAutomatedAddresseesNames = string.Join("; ", notAutomatedAddresseesLines.Select(x => x.Addressee.Person.Name));
        e.AddWarning(lenspec.Tenders.TenderCommitteeProtocols.Resources.NotAutomatedEmployeesAreDeletedMessageFormat(notAutomatedAddresseesLines.Count, notAutomatedAddresseesNames));
        return;
      }
      
      base.BeforeSave(e);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.FillEmpDepartment = false;
    }
  }

  partial class TenderCommitteeProtocolAddresseesDepartmentPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AddresseesDepartmentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query;
    }
  }

}