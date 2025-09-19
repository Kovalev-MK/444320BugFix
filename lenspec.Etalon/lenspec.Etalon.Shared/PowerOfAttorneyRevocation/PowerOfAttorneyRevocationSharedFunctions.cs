using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorneyRevocation;

namespace lenspec.Etalon.Shared
{
  partial class PowerOfAttorneyRevocationFunctions
  {

    public override void FillName()
    {
      _obj.Name = "Заявление на отзыв доверенности";
      if (_obj.FormalizedPowerOfAttorney != null)
      {
        if (!string.IsNullOrEmpty(_obj.FormalizedPowerOfAttorney.RegistrationNumber) && _obj.FormalizedPowerOfAttorney.RegistrationDate != null)
        {
          _obj.Name += string.Format(" {0} от {1}", _obj.FormalizedPowerOfAttorney.RegistrationNumber, _obj.FormalizedPowerOfAttorney.RegistrationDate);
        }
        if ((_obj.FormalizedPowerOfAttorney.IssuedTo != null || _obj.FormalizedPowerOfAttorney.IssuedToParty != null) && _obj.BusinessUnit != null)
        {
          var issuesTo = _obj.FormalizedPowerOfAttorney.IssuedTo != null ? _obj.FormalizedPowerOfAttorney.IssuedTo.Name : _obj.FormalizedPowerOfAttorney.IssuedToParty.Name;
          _obj.Name += string.Format(" для {0}, Доверитель {1}", issuesTo, _obj.BusinessUnit);
        }
      }
    }
  }
}