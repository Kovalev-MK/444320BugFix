using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.RoleKind;

namespace lenspec.EtalonDatabooks
{
  partial class RoleKindServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsComputeFromObject = false;
    }
  }

}