using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonContracts.PresenceRegion;

namespace avis.EtalonContracts
{
  partial class PresenceRegionServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsUseToAFP = false;
      _obj.IsUsedForQualification = false;
    }
  }

}