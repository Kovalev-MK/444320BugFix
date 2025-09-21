using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.LocalRegulations.StartingAcquaintanceTask;

namespace lenspec.LocalRegulations
{
  partial class StartingAcquaintanceTaskServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.IsElectronicAcquaintance = false;
    }
    //конец Добавлено Avis Expert
  }

}