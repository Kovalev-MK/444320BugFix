using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApplicationBUCreationDocument;

namespace lenspec.EtalonDatabooks
{
  partial class ApplicationBUCreationDocumentClientHandlers
  {

    //Добавлено Avis Expert
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      if(_obj.UploadBusinessUnit == null)
        _obj.UploadBusinessUnit = false;
      
      _obj.State.Properties.ApprovalRuleDescription.IsVisible =
        _obj.State.Properties.CEO.IsVisible = 
        _obj.State.Properties.ResponsibleAccountant.IsVisible =
        _obj.State.Properties.ResponsibleForRecordManagment.IsVisible = 
        _obj.UploadBusinessUnit == true;
    }
    //конец Добавлено Avis Expert

  }
}