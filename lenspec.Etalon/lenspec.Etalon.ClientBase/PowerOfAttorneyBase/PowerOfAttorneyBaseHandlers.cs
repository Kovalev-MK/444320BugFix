using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorneyBase;

namespace lenspec.Etalon
{
  partial class PowerOfAttorneyBaseRepresentativesClientHandlers
  {

    public virtual void RepresentativesEmployeelenspecValueInput(lenspec.Etalon.Client.PowerOfAttorneyBaseRepresentativesEmployeelenspecValueInputEventArgs e)
    {
      if (_obj.AgentType != Sungero.Docflow.PowerOfAttorneyBaseRepresentatives.AgentType.Person)
      {
        e.AddError("Сотрудник заполняется только если тип представителя - физическое лицо.");
        return;
      }
    }
  }


  partial class PowerOfAttorneyBaseClientHandlers
  {

  }
}