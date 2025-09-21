using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ApplicationRelinquishmentAuthority;

namespace avis.PowerOfAttorneyModule
{
  partial class ApplicationRelinquishmentAuthorityServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.Name = "<Имя документа будет сформировано автоматически>";
    }
  }


}