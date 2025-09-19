using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorney;

namespace lenspec.Etalon.Client
{
  partial class PowerOfAttorneyFunctions
  {

    /// <summary>
    /// Состояние полей
    /// </summary>
    public void CheckStateFields()
    {
      var requestCreatePOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
      var poaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Sungero.Docflow.PublicConstants.Module.Initialize.PowerOfAttorneyKind);
      var requestCreateNPOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
      var nPoaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.DocumentNotarialKindGuid);
      
      //Обязательность
      _obj.State.Properties.Termavis.IsRequired = _obj.DocumentKind != null && (_obj.DocumentKind.Equals(requestCreatePOAKind) || _obj.DocumentKind.Equals(poaKind));
      _obj.State.Properties.OurBusinessUavis.IsRequired = _obj.DocumentKind != null && (_obj.DocumentKind.Equals(requestCreatePOAKind) || _obj.DocumentKind.Equals(requestCreateNPOAKind));
      
      //Видимость
      _obj.State.Properties.BusinessUnit.IsVisible = _obj.DocumentKind != null && !_obj.DocumentKind.Equals(requestCreatePOAKind) && !_obj.DocumentKind.Equals(requestCreateNPOAKind);
      _obj.State.Properties.OurBusinessUavis.IsVisible = _obj.DocumentKind != null && (_obj.DocumentKind.Equals(requestCreatePOAKind) || _obj.DocumentKind.Equals(requestCreateNPOAKind));
      _obj.State.Properties.RegistryNumavis.IsVisible = _obj.DocumentKind != null && _obj.DocumentKind.Equals(nPoaKind);

      //Доступность
      var isIncludeContract = _obj.DocKindsavis != null && _obj.DocKindsavis.Any(i => i.Kind != null && i.Kind.DocumentType.DocumentTypeGuid.Equals("f37c7e63-b134-4446-9b5b-f8811f6c9666"));
      _obj.State.Properties.ContractKindGroupslenspec.IsEnabled = isIncludeContract;
      _obj.State.Properties.ContractKindslenspec.IsEnabled = isIncludeContract;
      _obj.State.Properties.ContractCategoryavis.IsEnabled = isIncludeContract;
      if (Equals(_obj.DocumentKind, nPoaKind))
      {
        _obj.State.Properties.InternalApprovalState.IsVisible = true;
        _obj.State.Properties.LifeCycleState.IsVisible = true;
      }
    }
    
    public override bool NeedViewDocumentSummary()
    {
      return true;
    }
    
  }
}