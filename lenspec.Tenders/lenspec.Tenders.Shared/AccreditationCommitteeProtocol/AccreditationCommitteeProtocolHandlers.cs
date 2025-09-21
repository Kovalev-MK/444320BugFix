using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.AccreditationCommitteeProtocol;

namespace lenspec.Tenders
{
  partial class AccreditationCommitteeProtocolAddresseesSharedHandlers
  {

    public virtual void AddresseesAddresseeChanged(lenspec.Tenders.Shared.AccreditationCommitteeProtocolAddresseesAddresseeChangedEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.Department = null;
      }
      else if (e.NewValue.Department != null)
      {
        _obj.Department = e.NewValue.Department;
      }
    }
  }

  partial class AccreditationCommitteeProtocolAddresseesSharedCollectionHandlers
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

  partial class AccreditationCommitteeProtocolMaterialKindsSharedHandlers
  {

    public virtual void MaterialKindsCityChanged(lenspec.Tenders.Shared.AccreditationCommitteeProtocolMaterialKindsCityChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.Region = e.NewValue.Region;
      }
    }

    public virtual void MaterialKindsRegionChanged(lenspec.Tenders.Shared.AccreditationCommitteeProtocolMaterialKindsRegionChangedEventArgs e)
    {
      if (e.NewValue == null || _obj.City != null && _obj.City.Region != e.NewValue)
      {
        _obj.City = null;
      }
    }

    public virtual void MaterialKindsMaterialChanged(lenspec.Tenders.Shared.AccreditationCommitteeProtocolMaterialKindsMaterialChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.MaterialGroup = e.NewValue.Group;
      }
    }
  }

  partial class AccreditationCommitteeProtocolMaterialKindsSharedCollectionHandlers
  {

    public virtual void MaterialKindsDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextItems = _obj.MaterialKinds.Where(x => x.Id > _deleted.Id);
      foreach(var item in nextItems)
      {
        item.Number = Convert.ToInt32(item.Number) - 1;
      }
    }

    public virtual void MaterialKindsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = (_obj.MaterialKinds.Max(a => a.Number) ?? 0) + 1;
    }
  }

  partial class AccreditationCommitteeProtocolWorkKindsSharedCollectionHandlers
  {

    public virtual void WorkKindsDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextItems = _obj.WorkKinds.Where(x => x.Id > _deleted.Id);
      foreach(var item in nextItems)
      {
        item.Number = Convert.ToInt32(item.Number) - 1;
      }
    }

    public virtual void WorkKindsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = (_obj.WorkKinds.Max(a => a.Number) ?? 0) + 1;
    }
  }

  partial class AccreditationCommitteeProtocolWorkKindsSharedHandlers
  {

    public virtual void WorkKindsWorkKindChanged(lenspec.Tenders.Shared.AccreditationCommitteeProtocolWorkKindsWorkKindChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.WorkGroup = e.NewValue.Group;
      }
    }

    public virtual void WorkKindsCityChanged(lenspec.Tenders.Shared.AccreditationCommitteeProtocolWorkKindsCityChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.Region = e.NewValue.Region;
      }
    }

    public virtual void WorkKindsRegionChanged(lenspec.Tenders.Shared.AccreditationCommitteeProtocolWorkKindsRegionChangedEventArgs e)
    {
      if (e.NewValue == null || _obj.City != null && _obj.City.Region != e.NewValue)
      {
        _obj.City = null;
      }
    }
  }

  partial class AccreditationCommitteeProtocolSharedHandlers
  {

    public virtual void IsContractorChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == true)
      {
        _obj.State.Properties.WorkKinds.IsVisible = true;
      }
      else
      {
        _obj.State.Properties.WorkKinds.IsVisible = false;
        _obj.WorkKinds.Clear();
      }
    }

    public virtual void IsProviderChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == true)
      {
        _obj.State.Properties.MaterialKinds.IsVisible = true;
      }
      else
      {
        _obj.State.Properties.MaterialKinds.IsVisible = false;
        _obj.MaterialKinds.Clear();
      }
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      FillName();
    }

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      // Изменение Вида документа сбрасывает НОР на значение по умолчанию (от поля PreparedBy).
      var businessUnit = _obj.BusinessUnit;
      base.DocumentKindChanged(e);
      _obj.BusinessUnit = businessUnit;
    }

    public override void DepartmentChanged(Sungero.Docflow.Shared.OfficialDocumentDepartmentChangedEventArgs e)
    {
      // Отменить заполнение НОР коробкой. Обновление параметра сохранено.
      //base.DepartmentChanged(e);
      
      if (e.NewValue != null && !Equals(e.NewValue, e.OriginalValue))
        e.Params.AddOrUpdate(Sungero.Docflow.PublicConstants.OfficialDocument.GrantAccessRightsToDocumentAsync, true);
    }

    public override void PreparedByChanged(Sungero.Docflow.Shared.OfficialDocumentPreparedByChangedEventArgs e)
    {
      base.PreparedByChanged(e);
      
      if (e.NewValue != null)
      {
        _obj.Department = e.NewValue.Department;
      }
      else
      {
        _obj.Department = null;
      }
    }

    public virtual void MeetingDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      FillName();
    }

    public virtual void CounterpartyChanged(lenspec.Tenders.Shared.AccreditationCommitteeProtocolCounterpartyChangedEventArgs e)
    {
      FillName();
    }

    public virtual void IsFillingByDepartmentEmployeesChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == true)
      {
        _obj.State.Properties.DepartmentsForAddressees.IsEnabled = true;
        _obj.State.Properties.DepartmentsForAddressees.IsRequired = true;
        _obj.State.Properties.AddresseesFilingOption.IsEnabled = true;
        _obj.State.Properties.AddresseesFilingOption.IsRequired = true;
      }
      else
      {
        _obj.State.Properties.DepartmentsForAddressees.IsEnabled = false;
        _obj.State.Properties.DepartmentsForAddressees.IsRequired = false;
        _obj.DepartmentsForAddressees.Clear();
        _obj.State.Properties.AddresseesFilingOption.IsEnabled = false;
        _obj.State.Properties.AddresseesFilingOption.IsRequired = false;
        _obj.AddresseesFilingOption = null;
        if (_obj.Addressees.Count() != 0)
          _obj.Addressees.Clear();
      }
    }

    public virtual void TenderAccreditationFormChanged(lenspec.Tenders.Shared.AccreditationCommitteeProtocolTenderAccreditationFormChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.Counterparty = e.NewValue.Counterparty;
      }
      else
      {
        _obj.Counterparty = null;
      }
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.Constants.Module.BasisRelationName, e.OldValue, e.NewValue);
    }

  }
}