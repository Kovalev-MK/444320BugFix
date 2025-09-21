using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PowerOfAttorneyModule.Server
{
  public class ModuleJobs
  {

    public virtual void ExpirationDateMonitoring()
    {
      var powerOfAttorneyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Sungero.Docflow.PublicConstants.Module.Initialize.PowerOfAttorneyKind);
      var notarialPowerOfAttorneyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.Constants.Module.DocumentNotarialKindGuid);
      var formalizedPowerOFAttorneyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Sungero.Docflow.PublicConstants.Module.Initialize.FormalizedPowerOfAttorneyKind);
      
      var powerOfAttorneys = Sungero.Docflow.PowerOfAttorneyBases.GetAll(x => ((lenspec.Etalon.PowerOfAttorneys.Is(x) && lenspec.Etalon.PowerOfAttorneys.As(x).DateAbortPOAavis == Calendar.Today.AddDays(-1)) &&
                                                                               (Equals(x.DocumentKind, powerOfAttorneyKind) || Equals(x.DocumentKind, notarialPowerOfAttorneyKind))) ||
                                                                         ((Equals(x.DocumentKind, powerOfAttorneyKind) || Equals(x.DocumentKind, notarialPowerOfAttorneyKind) ||
                                                                           Equals(x.DocumentKind, formalizedPowerOFAttorneyKind)) && x.ValidTill == Calendar.Today.AddDays(-1)));
      var signatureSettings = Sungero.Docflow.SignatureSettings.GetAll(x => powerOfAttorneys.Contains(x.Document));
      
      var handler = avis.PowerOfAttorneyModule.AsyncHandlers.ChangingStatusExpiredPOA.Create();
      handler.PowerOfAttorneyIds = string.Join(",", powerOfAttorneys.Select(x => x.Id));
      handler.SignatureSettingIds = string.Join(",", signatureSettings.Select(x => x.Id));
      handler.ExecuteAsync();
    }

    public virtual void RecallPowerOfAttorneys()
    {
      var poa = lenspec.Etalon.PowerOfAttorneys.GetAll(x => x.IsProjectPOAavis == false && x.IsRevokedavis != true && x.ValidTill != null && x.ValidTill < Calendar.Today);
      foreach (var document in poa)
      {
        document.RevokeReasonavis = avis.PowerOfAttorneyModule.Resources.RevokeReason;
        document.IsRevokedavis = true;
        document.DateAbortPOAavis = document.ValidTill;
        document.Save();
      }
    }

  }
}