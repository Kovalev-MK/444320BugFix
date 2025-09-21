using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.Authority;

namespace avis.PowerOfAttorneyModule
{

  partial class AuthorityServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsSignContracts = false;
      _obj.IsFormalizedPowerOfAttorneys = false;
      _obj.NeedMandatoryApproval = false;
    }
  }

}