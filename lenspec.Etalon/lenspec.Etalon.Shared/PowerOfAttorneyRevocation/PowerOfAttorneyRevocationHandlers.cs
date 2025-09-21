using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorneyRevocation;

namespace lenspec.Etalon
{
  partial class PowerOfAttorneyRevocationSharedHandlers
  {

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      Functions.PowerOfAttorneyRevocation.FillName(_obj);
    }

    public override void FormalizedPowerOfAttorneyChanged(Sungero.Docflow.Shared.PowerOfAttorneyRevocationFormalizedPowerOfAttorneyChangedEventArgs e)
    {
      base.FormalizedPowerOfAttorneyChanged(e);
      Functions.PowerOfAttorneyRevocation.FillName(_obj);
    }

  }
}