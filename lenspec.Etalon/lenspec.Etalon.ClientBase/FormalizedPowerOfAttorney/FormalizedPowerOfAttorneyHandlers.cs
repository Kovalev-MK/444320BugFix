using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.FormalizedPowerOfAttorney;

namespace lenspec.Etalon
{

  partial class FormalizedPowerOfAttorneyClientHandlers
  {

    public virtual void PrincipalRepresentativeTypelenspecValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      var isRepresentativePrincipalLegalEntity = e.NewValue == lenspec.Etalon.FormalizedPowerOfAttorney.PrincipalRepresentativeTypelenspec.LegalEntity;
      _obj.State.Properties.OurSignatory.IsEnabled = _obj.State.Properties.OurSignatory.IsRequired = !isRepresentativePrincipalLegalEntity;
      _obj.State.Properties.SoleExecutiveAuthoritylenspec.IsVisible = _obj.State.Properties.SoleExecutiveAuthoritylenspec.IsRequired = isRepresentativePrincipalLegalEntity;
    }

    public override IEnumerable<Enumeration> FormatVersionFiltering(IEnumerable<Enumeration> query)
    {
      query = base.FormatVersionFiltering(query);
      query = query.Where(x => !Equals(x, FormatVersion.Version002));
      
      return query;
    }

    public override void ValidFromValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      base.ValidFromValueInput(e);
      if (e.NewValue != null && e.NewValue < Calendar.Today)
      {
        e.AddError(lenspec.Etalon.FormalizedPowerOfAttorneys.Resources.DateErrorMessage);
        return;
      }
    }

    public override void ValidTillValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue < Calendar.Today)
      {
        e.AddError(lenspec.Etalon.FormalizedPowerOfAttorneys.Resources.ErrorMessageInvalidValidTill);
        _obj.ValidTill = null;
      }
      base.ValidTillValueInput(e);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.FpoaKindlenspec.IsRequired = true;
      _obj.State.Properties.FpoaKindlenspec.IsEnabled = _obj.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered;
      var isRepresentativePrincipalLegalEntity = _obj.PrincipalRepresentativeTypelenspec == lenspec.Etalon.FormalizedPowerOfAttorney.PrincipalRepresentativeTypelenspec.LegalEntity;
      _obj.State.Properties.OurSignatory.IsEnabled = _obj.State.Properties.OurSignatory.IsRequired = !isRepresentativePrincipalLegalEntity;
      _obj.State.Properties.FlagOurSignatoryavis.IsVisible = !isRepresentativePrincipalLegalEntity;
      _obj.State.Properties.SoleExecutiveAuthoritylenspec.IsVisible = isRepresentativePrincipalLegalEntity;
      
      if (_obj.InternalApprovalState == null)
        e.AddInformation(lenspec.Etalon.FormalizedPowerOfAttorneys.Resources.InformationMessageInternalApprovalEmpty);
      
      var isLenghtPowersChanged = false;
      e.Params.TryGetValue(Etalon.Constants.Docflow.FormalizedPowerOfAttorney.PowersLengthChangedParamName, out isLenghtPowersChanged);
      if (isLenghtPowersChanged)
        e.AddInformation(_obj.Info.Properties.Powers, lenspec.Etalon.FormalizedPowerOfAttorneys.Resources.PowersLengthWasChanged);
      
      _obj.State.Properties.PrincipalRepresentativeTypelenspec.IsRequired = true;
      _obj.State.Properties.DaysToFinishWorks.IsRequired = true;
    }

    public override void InternalApprovalStateValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      base.InternalApprovalStateValueInput(e);
      if (e.NewValue != e.OldValue && e.NewValue == lenspec.Etalon.FormalizedPowerOfAttorney.InternalApprovalState.Signed)
      {
        _obj.State.Properties.Authoritiesavis.IsRequired = false;
        _obj.State.Properties.Authoritiesavis.IsEnabled = false;
      }
      else
      {
        _obj.State.Properties.Authoritiesavis.IsRequired = true;
        _obj.State.Properties.Authoritiesavis.IsEnabled = true;
      }
    }
  }

}