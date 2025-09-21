using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.SignatureSetting;

namespace lenspec.Etalon.Shared
{
  partial class SignatureSettingFunctions
  {

    /// <summary>
    /// Проверить, что подписывающий является представителем в доверенности или эл. доверенности.
    /// </summary>
    /// <returns>True, если подписывающий является представителем, указанным в доверенности. Иначе False.</returns>
    [Public]
    public override bool IsRecipientPowerOfAttorneyRepresentative()
    {
      if (_obj.Recipient == null)
        return false;
      
      if (_obj.Reason != Reason.FormalizedPoA && _obj.Reason != Reason.PowerOfAttorney)
        return false;
      
      var powerOfAttorney = Sungero.Docflow.PowerOfAttorneyBases.As(_obj.Document);
      if (lenspec.Etalon.PowerOfAttorneys.Is(_obj.Document))
      {
        var employee = Sungero.Company.Employees.As(_obj.Recipient);
        if (powerOfAttorney.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.Employee && Equals(powerOfAttorney.IssuedTo, _obj.Recipient))
          return true;
      }
      else
      {
        if (powerOfAttorney.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.Employee && Equals(powerOfAttorney.IssuedTo, _obj.Recipient))
          return true;
      }
      var person = Sungero.Company.Employees.As(_obj.Recipient)?.Person;
      if (powerOfAttorney.AgentType != Sungero.Docflow.PowerOfAttorneyBase.AgentType.Employee && person != null &&
          (Equals(person, powerOfAttorney.IssuedToParty) || Equals(person, powerOfAttorney.Representative)))
      {
        return true;
      }
      
      return false;
    }
  }
}