using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractualDocument;

namespace lenspec.Etalon
{
  partial class ContractualDocumentVersionsSharedCollectionHandlers
  {

    public override void VersionsDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      base.VersionsDeleted(e);
      
      if (_obj.LastVersion == null)
      {
        if (_obj.IsStandard == true || _obj.TemplateIDlenspec != null)
        {
          _obj.IsStandard = false;
          _obj.TemplateIDlenspec = null;
        }

        return;
      }

      var historyRow = Functions.ContractualDocument.GetImportOrFromTemplateHistoryRow(_obj);
      
      if (EtalonDatabooks.PublicFunctions.Module.IsNullOrImported(historyRow))
      {
        if (_obj.IsStandard == true || _obj.TemplateIDlenspec != null)
        {
          _obj.IsStandard = false;
          _obj.TemplateIDlenspec = null;
        }

        return;
      }
      
      if (_obj.IsStandard != true || _obj.TemplateIDlenspec == null)
      {
        _obj.IsStandard = true;
        var templateName = historyRow.Comment;
        var template = Etalon.DocumentTemplates.GetAll(x => x.Name == templateName).FirstOrDefault();
        _obj.TemplateIDlenspec = template?.Id;
      }
    }

    public override void VersionsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      base.VersionsAdded(e);
      
      Functions.ContractualDocument.IsStandard(_obj, e);
    }
  }

  partial class ContractualDocumentConstructionObjectsavisSharedHandlers
  {

    public virtual void ConstructionObjectsavisMeasureSizeAmountavisChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      // Блокируем поле сумма на вкладке свойство.
      PublicFunctions.ContractualDocument.BlockTotalAmmount(_obj.ContractualDocument);
    }

    public virtual void ConstructionObjectsavisSummChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      // Блокируем поле сумма на вкладке свойство.
      PublicFunctions.ContractualDocument.BlockTotalAmmount(_obj.ContractualDocument);
    }
  }

  partial class ContractualDocumentConstructionObjectsavisSharedCollectionHandlers
  {

    public virtual void ConstructionObjectsavisDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextItems = _obj.ConstructionObjectsavis.Where(x => x.Id > _deleted.Id);
      foreach(var item in nextItems)
      {
        item.Number = Convert.ToInt32(item.Number) - 1;
      }
    }

    public virtual void ConstructionObjectsavisAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = (_obj.ConstructionObjectsavis.Max(a => a.Number) ?? 0) + 1;
      _added.MeasureSizeAmountavis = 0;
    }
  }

  partial class ContractualDocumentSharedHandlers
  {

    public override void VersionsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.VersionsChanged(e);
    }

    public override void VatRateChanged(Sungero.Docflow.Shared.ContractualDocumentBaseVatRateChangedEventArgs e)
    {
      base.VatRateChanged(e);
      // Расчёт налога и заполнение свойства.
      PublicFunctions.ContractualDocument.CalculateNalog(_obj);
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      _obj.Department = null;
    }

    public virtual void IsICPlenspecChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      _obj.GroupContractTypeavis = null;
      _obj.ContractKindavis = null;
    }

    public virtual void OurCFavisChanged(lenspec.Etalon.Shared.ContractualDocumentOurCFavisChangedEventArgs e)
    {
      if (!Equals(e.NewValue, e.OldValue) || e.NewValue != null && e.NewValue.IsComputeApprovalers != true)
      {
        _obj.Objectlenspec = null;
        foreach (var line in _obj.ConstructionObjectsavis)
        {
          line.ObjectAnProject = null;
        }
      }
    }

    public virtual void RegionlenspecChanged(lenspec.Etalon.Shared.ContractualDocumentRegionlenspecChangedEventArgs e)
    {
      if (e.NewValue != null && e.NewValue != e.OldValue)
      {
        var region = lenspec.Etalon.Regions.As(e.NewValue);
        if (region != null)
          _obj.Districtlenspec = region.Districtlenspec;
      }
      else
        _obj.Districtlenspec = null;
    }

    public virtual void ContractKindavisChanged(lenspec.Etalon.Shared.ContractualDocumentContractKindavisChangedEventArgs e)
    {
    }

    public virtual void ContractTypeavisChanged(lenspec.Etalon.Shared.ContractualDocumentContractTypeavisChangedEventArgs e)
    {
      // Блокируем поля при изменении "Тип договора".
      PublicFunctions.ContractualDocument.EditContractType(_obj);
    }

    public virtual void NDSavisChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
    }

    public override void TotalAmountChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      base.TotalAmountChanged(e);
      
      // Расчёт налога и заполнение свойства.
      PublicFunctions.ContractualDocument.CalculateNalog(_obj);
    }

    public virtual void GroupContractTypeavisChanged(lenspec.Etalon.Shared.ContractualDocumentGroupContractTypeavisChangedEventArgs e)
    {
      /// Делаем обязательным поле "ИСП" в зависимости от группы вида.
      PublicFunctions.ContractualDocument.IsRequiredPropertyOurCF(_obj);
      
      if (e.NewValue != null && _obj.ContractKindavis != null && _obj.ContractKindavis.GroupContractType != e.NewValue)
        _obj.ContractKindavis = null;
    }

    public override void CounterpartyChanged(Sungero.Docflow.Shared.ContractualDocumentBaseCounterpartyChangedEventArgs e)
    {
      base.CounterpartyChanged(e);
      
      //_obj.EmployeeSignatoryavis = null;
      //_obj.ContactEmployeeavis = null;
      
      //_obj.CounterpartySignatory = null;
      //_obj.Contact = null;
    }

    public virtual void ContractCategoryavisChanged(lenspec.Etalon.Shared.ContractualDocumentContractCategoryavisChangedEventArgs e)
    {

    }

  }
}