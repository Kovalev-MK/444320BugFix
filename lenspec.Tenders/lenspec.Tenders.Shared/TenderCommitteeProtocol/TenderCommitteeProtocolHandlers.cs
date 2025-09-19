using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderCommitteeProtocol;

namespace lenspec.Tenders
{
  partial class TenderCommitteeProtocolAddresseesSharedHandlers
  {

    public virtual void AddresseesAddresseeChanged(lenspec.Tenders.Shared.TenderCommitteeProtocolAddresseesAddresseeChangedEventArgs e)
    {
      if (e.NewValue != null && e.NewValue.Department != null)
        _obj.Department = e.NewValue.Department;
    }
  }

  partial class TenderCommitteeProtocolAddresseesSharedCollectionHandlers
  {

    public virtual void AddresseesDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextItems = _obj.Addressees.Where(x => x.Id > _deleted.Id);
      foreach(var item in nextItems)
      {
        item.Number = Convert.ToInt32(item.Number) - 1;
      }
    }

    public virtual void AddresseesAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = (_obj.Addressees.Max(a => a.Number) ?? 0) + 1;
    }
  }

  partial class TenderCommitteeProtocolSharedHandlers
  {

    public override void RegistrationDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.RegistrationDateChanged(e);
    }

    public virtual void TenderWinnerChanged(lenspec.Tenders.Shared.TenderCommitteeProtocolTenderWinnerChangedEventArgs e)
    {
      _obj.State.Controls.Control.Refresh();
    }

    public virtual void AddresseesChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      
    }

    public override void OurCFChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.OurCFChanged(e);
      
      var names = _obj.OurCF.Where(x => x.OurCF != null).Select(x => x.OurCF.CommercialName).ToList();
      _obj.OurCFNames = string.Join("; ", names);
    }

    public override void CounterpartiesChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.CounterpartiesChanged(e);
      
      var counterparties = _obj.Counterparties.Where(x => x.Counterparty != null);
      
      var names = counterparties.Select(x => x.Counterparty.Name).ToList();
      _obj.CounterpartyNames = string.Join("; ", names);
      
      var nameTins = counterparties.Select(x => x.Counterparty.Name + " " + x.Counterparty.TIN).ToList();
      _obj.CounterpartiesString = string.Join("; ", nameTins);
      
      _obj.TenderWinner = null;
      _obj.TenderWinnerReserve = null;
    }

    public override void ObjectAnProjectsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.ObjectAnProjectsChanged(e);
      Functions.TenderCommitteeProtocol.FillName(_obj);
      
      var names = _obj.ObjectAnProjects.Where(x => x.ObjectAnProject != null).Select(x => x.ObjectAnProject.Name).ToList();
      _obj.ObjectAnProjectNames = string.Join("; ", names);
    }

    public virtual void BasisDocumentChanged(lenspec.Tenders.Shared.TenderCommitteeProtocolBasisDocumentChangedEventArgs e)
    {
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.Constants.Module.BasisRelationName, e.OldValue, e.NewValue);
      if (Equals(e.NewValue, e.OldValue))
        return;

      _obj.BusinessUnit = null;
      _obj.Counterparties.Clear();
      _obj.TenderSelectionSubject = null;
      _obj.OurCF.Clear();
      _obj.ObjectAnProjects.Clear();
      
      e.Params.AddOrUpdate(lenspec.Tenders.TenderCommitteeProtocols.Resources.BasisDocumentIsChanged, true);
      
      if (e.NewValue != null)
      {
        _obj.BusinessUnit = e.NewValue.BusinessUnit;
        foreach (var сounterparties in e.NewValue.Counterparties)
          _obj.Counterparties.AddNew().Counterparty = сounterparties.Counterparty;
        _obj.TenderSelectionSubject = e.NewValue.TenderSelectionSubject;
        foreach (var ourCF in e.NewValue.OurCF)
          _obj.OurCF.AddNew().OurCF = ourCF.OurCF;
        foreach (var objectAnProjects in e.NewValue.ObjectAnProjects)
          _obj.ObjectAnProjects.AddNew().ObjectAnProject = objectAnProjects.ObjectAnProject;
      }
    }

    public virtual void FillEmpDepartmentChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == true)
      {
        _obj.State.Properties.AddressessDepartments.IsEnabled = true;
        _obj.State.Properties.AddressessDepartments.IsRequired = true;
        _obj.State.Properties.FillOption.IsEnabled = true;
        _obj.State.Properties.FillOption.IsRequired = true;
      }
      else
      {
        _obj.State.Properties.AddressessDepartments.IsEnabled = false;
        _obj.State.Properties.AddressessDepartments.IsRequired = false;
        _obj.State.Properties.FillOption.IsEnabled = false;
        _obj.State.Properties.FillOption.IsRequired = false;
        _obj.FillOption = null;
        if (_obj.AddressessDepartments.Count() != 0)
          _obj.AddressessDepartments.Clear();
      }
    }

  }
}