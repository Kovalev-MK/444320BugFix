using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.LocalRegulations.UpdatingFieldsInDocumentBody;

namespace lenspec.LocalRegulations
{
  partial class UpdatingFieldsInDocumentBodyServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.UpdateAllFields = false;
      _obj.UpdateRegistrationData = false;
    }
    //конец Добавлено Avis Expert
  }

}