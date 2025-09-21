using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApplicationBUDocumentBase;

namespace lenspec.EtalonDatabooks
{
  partial class ApplicationBUDocumentBaseServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);     
      _obj.LifeCycleState = ApplicationBUCreationDocument.LifeCycleState.Draft;
    }
    //конец Добавлено Avis Expert
  }


}