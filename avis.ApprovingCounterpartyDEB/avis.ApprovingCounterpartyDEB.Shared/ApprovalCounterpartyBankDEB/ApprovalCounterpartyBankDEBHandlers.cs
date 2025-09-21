using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEB;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalCounterpartyBankDEBMaterialsSharedHandlers
  {

    public virtual void MaterialsMaterialChanged(avis.ApprovingCounterpartyDEB.Shared.ApprovalCounterpartyBankDEBMaterialsMaterialChangedEventArgs e)
    {
      _obj.MaterialGroup = e.NewValue?.Group;
    }
  }

  partial class ApprovalCounterpartyBankDEBMaterialsSharedCollectionHandlers
  {

    public virtual void MaterialsDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextItems = _obj.Materials.Where(x => x.Id > _deleted.Id);
      foreach(var item in nextItems)
      {
        item.Number = Convert.ToInt32(item.Number) - 1;
      }
    }

    public virtual void MaterialsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = (_obj.Materials.Max(a => a.Number) ?? 0) + 1;
    }
  }

  partial class ApprovalCounterpartyBankDEBWorkKindsSharedHandlers
  {

    public virtual void WorkKindsWorkKindChanged(avis.ApprovingCounterpartyDEB.Shared.ApprovalCounterpartyBankDEBWorkKindsWorkKindChangedEventArgs e)
    {
      _obj.WorkGroup = _obj.WorkKind?.Group;
    }
  }

  partial class ApprovalCounterpartyBankDEBWorkKindsSharedCollectionHandlers
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

  partial class ApprovalCounterpartyBankDEBSharedHandlers
  {

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      if (e.NewValue != null)
        _obj.TenderAmount = lenspec.Etalon.BusinessUnits.As(e.NewValue).TenderAmountavis ?? 0;
      else
        _obj.TenderAmount = 0;
    }

    public override void CounterpartyChanged(avis.ApprovingCounterpartyDEB.Shared.ApprovalCounterpartyBaseCounterpartyChangedEventArgs e)
    {
      base.CounterpartyChanged(e);
      Functions.ApprovalCounterpartyBankDEB.FillName(_obj);
      Functions.ApprovalCounterpartyBankDEB.CheckCounterpartyType(_obj);
    }

    public override void CreatedChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.CreatedChanged(e);
      Functions.ApprovalCounterpartyBankDEB.FillName(_obj);
    }

  }
}