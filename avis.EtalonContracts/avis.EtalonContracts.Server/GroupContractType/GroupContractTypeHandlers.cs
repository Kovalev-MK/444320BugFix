using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonContracts.GroupContractType;

namespace avis.EtalonContracts
{
  partial class GroupContractTypeServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsTenderProtocolNotRequired = false;
    }
  }

}