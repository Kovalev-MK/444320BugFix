using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorney;

namespace lenspec.Etalon
{
  partial class PowerOfAttorneySharedHandlers
  {

    public override void RepresentativesChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.RepresentativesChanged(e);
      Functions.PowerOfAttorney.FillName(_obj);
    }

    public virtual void ContractKindslenspecChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if (!_obj.ContractKindslenspec.Any())
        _obj.ContractCategoryavis.Clear();
    }

    public virtual void ContractKindGroupslenspecChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if (!_obj.ContractKindGroupslenspec.Any())
        _obj.ContractKindslenspec.Clear();
    }

    public virtual void PowerListlenspecChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      Functions.PowerOfAttorney.FillPowers(_obj);
    }

    public virtual void FreePowerslenspecChanged(Sungero.Domain.Shared.TextPropertyChangedEventArgs e)
    {
      Functions.PowerOfAttorney.FillPowers(_obj);
    }

    public virtual void DateAbortPOAavisChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
        _obj.ValidTill = e.NewValue;
	}
	
    public override void ValidFromChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.ValidFromChanged(e);
      Functions.PowerOfAttorney.FillName(_obj);
      
      if (e.NewValue != null && e.NewValue != e.OldValue)
        _obj.TXTDatePOAavis = lenspec.EtalonDatabooks.PublicFunctions.Module.DateToWords(e.NewValue.Value);
      else
        _obj.TXTDatePOAavis = null;
    }

    public override void IssuedToChanged(Sungero.Docflow.Shared.PowerOfAttorneyBaseIssuedToChangedEventArgs e)
    {
      base.IssuedToChanged(e);
      Functions.PowerOfAttorney.FillName(_obj);
    }

    public override void IssuedToPartyChanged(Sungero.Docflow.Shared.PowerOfAttorneyBaseIssuedToPartyChangedEventArgs e)
    {
      base.IssuedToPartyChanged(e);
      Functions.PowerOfAttorney.FillName(_obj);
    }

    public virtual void OurBusinessUavisChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      var bu = _obj.OurBusinessUavis.FirstOrDefault();
      if (bu != null && bu.Company != null)
      {
        _obj.OurSignatory = bu.Company.CEO != null ? lenspec.Etalon.Employees.As(bu.Company.CEO) : null;
      }
      else
        _obj.OurSignatory = null;
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      // Запомнить значение НОР, т.к. при обновлении и сохранении в коробке очищается скрытая НОР ненумеруемых видов
      if (e.NewValue != null)
        e.Params.AddOrUpdate("BusinessUnitId", e.NewValue.Id);
      
      base.BusinessUnitChanged(e);
      Functions.PowerOfAttorney.FillName(_obj);
    }

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      Functions.PowerOfAttorney.FillName(_obj);
      Functions.PowerOfAttorney.FillIsProject(_obj);
      _obj.LifeCycleState = lenspec.Etalon.PowerOfAttorney.LifeCycleState.Active;
    }

    public virtual void RegistryNumavisChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      Functions.PowerOfAttorney.FillName(_obj);
    }

    public virtual void DocKindsavisChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if(_obj.DocKindsavis != null || !_obj.DocKindsavis.Any(i => i.Kind != null && i.Kind.DocumentType.DocumentTypeGuid.Equals("f37c7e63-b134-4446-9b5b-f8811f6c9666")))
        _obj.ContractCategoryavis.Clear();
    }

    public override void PreparedByChanged(Sungero.Docflow.Shared.OfficialDocumentPreparedByChangedEventArgs e)
    {
      base.PreparedByChanged(e);
    }
  }

  //<<Avis-Expert
}