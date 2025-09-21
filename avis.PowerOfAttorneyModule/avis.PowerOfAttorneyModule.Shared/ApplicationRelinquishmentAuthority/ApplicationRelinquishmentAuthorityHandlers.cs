using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ApplicationRelinquishmentAuthority;

namespace avis.PowerOfAttorneyModule
{
  partial class ApplicationRelinquishmentAuthoritySharedHandlers
  {

    public virtual void AttorneyChanged(avis.PowerOfAttorneyModule.Shared.ApplicationRelinquishmentAuthorityAttorneyChangedEventArgs e)
    {
      Functions.ApplicationRelinquishmentAuthority.FillName(_obj);
    }

    public override void CreatedChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.CreatedChanged(e);
      Functions.ApplicationRelinquishmentAuthority.FillName(_obj);
    }

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      Functions.ApplicationRelinquishmentAuthority.FillName(_obj);
    }

  }
}