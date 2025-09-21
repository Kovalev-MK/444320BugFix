using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.StartingIntroductionTask;

namespace lenspec.ProtocolsCollegialBodies
{
  partial class StartingIntroductionTaskServerHandlers
  {
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.IsElectronic = false;
    }
  }
}