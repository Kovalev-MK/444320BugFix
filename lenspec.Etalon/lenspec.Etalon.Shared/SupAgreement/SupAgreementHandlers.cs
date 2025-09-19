using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.SupAgreement;

namespace lenspec.Etalon
{
  partial class SupAgreementSharedHandlers
  {

    public override void ArchiveavisChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      base.ArchiveavisChanged(e);
      if (e.NewValue.HasValue && e.NewValue.Value == true)
      {
        foreach(var property in _obj.State.Properties.ConstructionObjectsavis.Properties)
        {
          property.IsRequired = false;
        }
      }
    }

    public override void TotalAmountChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      base.TotalAmountChanged(e);
    }

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);

      if (e.NewValue != null && e.NewValue != e.OldValue)
      {
        var leadingDocument = lenspec.Etalon.ContractualDocuments.As(e.NewValue);
        _obj.ContractTypeavis = leadingDocument.ContractTypeavis;
        _obj.OurCFavis = leadingDocument.OurCFavis;
        _obj.SubjectContractavis = leadingDocument.SubjectContractavis;
        _obj.Counterparty = leadingDocument.Counterparty;
        _obj.ThirdSideavis = leadingDocument.ThirdSideavis;
        _obj.OurSignatory = leadingDocument.OurSignatory;
        _obj.Currency = leadingDocument.Currency;
        _obj.IsICPlenspec = leadingDocument.IsICPlenspec;
        _obj.PresenceRegionlenspec = leadingDocument.PresenceRegionlenspec;
        _obj.GroupContractTypeavis = leadingDocument.GroupContractTypeavis;
        _obj.ContractKindavis = leadingDocument.ContractKindavis;
        _obj.ContractCategoryavis = leadingDocument.ContractCategoryavis;
        _obj.IsSMRavis = leadingDocument.IsSMRavis;
        
        if (_obj.Archiveavis != true)
        {
          _obj.ConstructionObjectsavis.Clear();
          foreach (var constructionObject in leadingDocument.ConstructionObjectsavis)
          {
            var newValue = _obj.ConstructionObjectsavis.AddNew();
            newValue.ObjectAnProject = constructionObject.ObjectAnProject;
            newValue.DetailingWorkType = constructionObject.DetailingWorkType;
            newValue.Summ = constructionObject.Summ;
            newValue.MeasureSizeAmountavis = 0;
            newValue.NumberInContract = constructionObject.Number;
          }
        }
      }
    }

    public override void ConstructionObjectsavisChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.ConstructionObjectsavisChanged(e);
      
      double summ = 0;
      
      foreach (var constructionObject in _obj.ConstructionObjectsavis)
      {
        if (constructionObject.MeasureSizeAmountavis != null)
          summ += constructionObject.MeasureSizeAmountavis.Value;
      }
      
      _obj.TotalAmount = summ;
    }

  }
}