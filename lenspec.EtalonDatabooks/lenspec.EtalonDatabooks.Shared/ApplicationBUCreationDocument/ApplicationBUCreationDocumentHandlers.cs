using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApplicationBUCreationDocument;

namespace lenspec.EtalonDatabooks
{
  partial class ApplicationBUCreationDocumentSharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void UploadBusinessUnitChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      _obj.State.Properties.ApprovalRuleDescription.IsRequired =
        _obj.State.Properties.CEO.IsRequired =
        _obj.State.Properties.ResponsibleAccountant.IsRequired =
        _obj.State.Properties.ResponsibleForRecordManagment.IsRequired = e.NewValue == true;
    }
    //конец Добавлено Avis Expert

  }
}