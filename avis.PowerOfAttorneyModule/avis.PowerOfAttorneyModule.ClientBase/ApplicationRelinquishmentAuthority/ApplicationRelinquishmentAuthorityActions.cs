using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ApplicationRelinquishmentAuthority;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class ApplicationRelinquishmentAuthorityActions
  {
    public override void UpdateTemplatelenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.State.IsChanged)
        base.UpdateTemplatelenspec(e);
      else
        return;
    }

    public override bool CanUpdateTemplatelenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanUpdateTemplatelenspec(e);
    }

  }


}