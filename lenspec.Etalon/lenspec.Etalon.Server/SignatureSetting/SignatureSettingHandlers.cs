using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.SignatureSetting;

namespace lenspec.Etalon
{
  partial class SignatureSettingServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      if (CallContext.CalledFrom(Sungero.Docflow.PowerOfAttorneys.Info))
      {
        var powerOfAttorneyId = CallContext.GetCallerEntityId(Sungero.Docflow.PowerOfAttorneys.Info);
        var powerOfAttorney = lenspec.Etalon.PowerOfAttorneys.Get(powerOfAttorneyId);
        foreach (var parentLine in powerOfAttorney.DocKindsavis)
        {
          var newLine = _obj.DocumentKinds.AddNew();
          newLine.DocumentKind = parentLine.Kind;
        }
      }
        
    }
  }

}