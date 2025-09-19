using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.FormalizedPowerOfAttorney;

namespace lenspec.Etalon
{

  partial class FormalizedPowerOfAttorneySharedHandlers
  {

    public override void RepresentativesChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.RepresentativesChanged(e);
      Functions.FormalizedPowerOfAttorney.FillName(_obj);
    }

    public virtual void PrincipalRepresentativeTypelenspecChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue == lenspec.Etalon.FormalizedPowerOfAttorney.PrincipalRepresentativeTypelenspec.LegalEntity)
      {
        var businessUnit = lenspec.Etalon.BusinessUnits.As(_obj.BusinessUnit);
        if (businessUnit != null && businessUnit.SoleExecutiveAuthoritylenspec != null)
          _obj.SoleExecutiveAuthoritylenspec = businessUnit.SoleExecutiveAuthoritylenspec;
      }
      else
        _obj.SoleExecutiveAuthoritylenspec = null;
    }

    public virtual void SoleExecutiveAuthoritylenspecChanged(lenspec.Etalon.Shared.FormalizedPowerOfAttorneySoleExecutiveAuthoritylenspecChangedEventArgs e)
    {
      if (e.NewValue != null)
        _obj.OurSignatory = e.NewValue.CEO;
      else
        _obj.OurSignatory = null;
    }

    public override void StructuredPowersChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.StructuredPowersChanged(e);
      var isChangeLenght = Functions.FormalizedPowerOfAttorney.FillPowersAndCheckLenght(_obj);
      e.Params.AddOrUpdate(Etalon.Constants.Docflow.FormalizedPowerOfAttorney.PowersLengthChangedParamName, isChangeLenght);
    }

    public virtual void FpoaKindlenspecChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      _obj.StructuredPowers.Clear();
      _obj.FreeFormPowerslenspec = null;
      _obj.Powers = null;
      
      if (e.NewValue == lenspec.Etalon.FormalizedPowerOfAttorney.FpoaKindlenspec.State)
        _obj.PowersType = Sungero.Docflow.FormalizedPowerOfAttorney.PowersType.Classifier;
      else
        _obj.PowersType = Sungero.Docflow.FormalizedPowerOfAttorney.PowersType.FreeForm;
    }

    public virtual void FreeFormPowerslenspecChanged(Sungero.Domain.Shared.TextPropertyChangedEventArgs e)
    {
      var isChangeLenght = Functions.FormalizedPowerOfAttorney.FillPowersAndCheckLenght(_obj);
      e.Params.AddOrUpdate(Etalon.Constants.Docflow.FormalizedPowerOfAttorney.PowersLengthChangedParamName, isChangeLenght);
    }

    public override void PowersChanged(Sungero.Domain.Shared.TextPropertyChangedEventArgs e)
    {
      base.PowersChanged(e);
    }

    public override void RegistrationDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.RegistrationDateChanged(e);
      Functions.FormalizedPowerOfAttorney.FillName(_obj);
    }

    public override void RegistrationNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.RegistrationNumberChanged(e);
      Functions.FormalizedPowerOfAttorney.FillName(_obj);
    }

    public override void UnifiedRegistrationNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.UnifiedRegistrationNumberChanged(e);
      Functions.FormalizedPowerOfAttorney.FillName(_obj);
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      // Очищение Подписанта необходимо, т.к. если у НОР есть ЕИО без CEO, то в поле подписант останется CEO принадлежащий НОР, а не ЕИО
      if (e.NewValue != e.OldValue)
        _obj.OurSignatory = null;
      
      base.BusinessUnitChanged(e);
      if (e.NewValue == null)
      {
        _obj.SoleExecutiveAuthoritylenspec = null;
        _obj.PrincipalRepresentativeTypelenspec = null;
      }
      
      Functions.FormalizedPowerOfAttorney.FillName(_obj);
      
      var businessUnitLenspec = lenspec.Etalon.BusinessUnits.As(e.NewValue);
      if (businessUnitLenspec != null)
      {
        if (businessUnitLenspec.SoleExecutiveAuthoritylenspec != null)
        {
          _obj.SoleExecutiveAuthoritylenspec = businessUnitLenspec.SoleExecutiveAuthoritylenspec;
          _obj.PrincipalRepresentativeTypelenspec = lenspec.Etalon.FormalizedPowerOfAttorney.PrincipalRepresentativeTypelenspec.LegalEntity;
        }
      }
    }

    public override void IssuedToPartyChanged(Sungero.Docflow.Shared.PowerOfAttorneyBaseIssuedToPartyChangedEventArgs e)
    {
      base.IssuedToPartyChanged(e);
      Functions.FormalizedPowerOfAttorney.FillName(_obj);
    }

    public override void IssuedToChanged(Sungero.Docflow.Shared.PowerOfAttorneyBaseIssuedToChangedEventArgs e)
    {
      base.IssuedToChanged(e);
      Functions.FormalizedPowerOfAttorney.FillName(_obj);
    }

    public virtual void AuthoritiesavisChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      
    }
  }
}