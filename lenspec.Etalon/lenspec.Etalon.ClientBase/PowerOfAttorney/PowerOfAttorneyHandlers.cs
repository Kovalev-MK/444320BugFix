using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorney;
using Sungero.Domain.Shared;

namespace lenspec.Etalon
{

  partial class PowerOfAttorneyClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      var role = avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidRightsToAttachScans;
      var isInRoleRightsToAttachScans = Sungero.CoreEntities.Users.Current.IncludedIn(role);
      e.Params.AddOrUpdate(lenspec.Etalon.Constants.Docflow.PowerOfAttorney.Params.IsInRoleRightsToAttachScans, isInRoleRightsToAttachScans);
      
      var officeGKrole = lenspec.EtalonDatabooks.PublicConstants.Module.OfficeGK;
      var isInRoleOfficeGK = Sungero.CoreEntities.Users.Current.IncludedIn(officeGKrole) || Substitutions.ActiveSubstitutedUsers.Any(x => x.IncludedIn(officeGKrole));
      e.Params.AddOrUpdate(lenspec.Etalon.Constants.Docflow.PowerOfAttorney.Params.IsInRoleOfficeGK, isInRoleOfficeGK);
    }
    
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Properties.InternalApprovalState.IsEnabled = Users.Current.IncludedIn(Roles.Administrators);
      
      Functions.PowerOfAttorney.CheckStateFields(_obj);
      
      if (_obj.InternalApprovalState == null)
      {
        var requestCreatePOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
        var poaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Sungero.Docflow.PublicConstants.Module.Initialize.PowerOfAttorneyKind);
        if (_obj.DocumentKind != null && (_obj.DocumentKind.Equals(poaKind) || _obj.DocumentKind.Equals(requestCreatePOAKind)))
          e.AddWarning(lenspec.Etalon.PowerOfAttorneys.Resources.WarningMessageInternalApprovalValueNull);
      }
    }

    public override void ValidFromValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      base.ValidFromValueInput(e);
      
      var documentNotarialKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.DocumentNotarialKindGuid);
      var powerOfAttorneyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Sungero.Docflow.PublicConstants.Module.Initialize.PowerOfAttorneyKind);
      var isPowerOfAttorneyOrNotarialKind = Equals(_obj.DocumentKind, documentNotarialKind) || Equals(_obj.DocumentKind, powerOfAttorneyKind);

      if (!isPowerOfAttorneyOrNotarialKind && e.NewValue != null && e.NewValue.Value < Calendar.Now.Date)
      {
        e.AddError(lenspec.Etalon.PowerOfAttorneys.Resources.ErrorMessageValidFrom);
        return;
      }
    }
    
    //<<Avis-Expert
  }

}