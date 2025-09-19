using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonContracts.ContractKind;

namespace avis.EtalonContracts
{
  partial class ContractKindServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.CalculationOurCF = ContractKind.CalculationOurCF.No;
      _obj.IsTenderProtocolNotRequired = false;
    }
  }

}