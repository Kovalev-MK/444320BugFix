using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ApplicationRelinquishmentAuthority;

namespace avis.PowerOfAttorneyModule.Shared
{
  partial class ApplicationRelinquishmentAuthorityFunctions
  {

    /// <summary>
    /// Заполнить имя
    /// </summary>
    public override void FillName()
    {
      _obj.Name = null;
      if (_obj.DocumentKind != null)
        _obj.Name = _obj.DocumentKind.Name;
      if (_obj.Created != null)
        _obj.Name += $" от {_obj.Created.Value.ToShortDateString()}";
      if (_obj.Attorney != null)
        _obj.Name += $" на {_obj.Attorney.Name}";
    }

  }
}