using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonContracts.ContractType;

namespace avis.EtalonContracts
{
  partial class ContractTypeServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsTenderProtocolNotRequired = false;
    }
  }

}