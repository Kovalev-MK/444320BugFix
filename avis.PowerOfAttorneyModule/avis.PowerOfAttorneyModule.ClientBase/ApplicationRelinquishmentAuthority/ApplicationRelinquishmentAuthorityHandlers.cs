using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ApplicationRelinquishmentAuthority;

namespace avis.PowerOfAttorneyModule
{
  partial class ApplicationRelinquishmentAuthorityClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Properties.Subject.IsRequired = false;
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      if (_obj.State.IsInserted)
        e.AddWarning(avis.PowerOfAttorneyModule.ApplicationRelinquishmentAuthorities.Resources.WarningMessageIsInserted);
    }

  }
}