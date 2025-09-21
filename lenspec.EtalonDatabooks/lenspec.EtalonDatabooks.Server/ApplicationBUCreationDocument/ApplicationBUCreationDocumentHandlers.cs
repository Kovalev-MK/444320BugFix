using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApplicationBUCreationDocument;

namespace lenspec.EtalonDatabooks
{
  partial class ApplicationBUCreationDocumentServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.UploadBusinessUnit = false;
      
      _obj.State.Properties.ApprovalRuleDescription.IsVisible =
        _obj.State.Properties.CEO.IsVisible = 
        _obj.State.Properties.ResponsibleAccountant.IsVisible =
        _obj.State.Properties.ResponsibleForRecordManagment.IsVisible = false;
    }
    //конец Добавлено Avis Expert
  }

}