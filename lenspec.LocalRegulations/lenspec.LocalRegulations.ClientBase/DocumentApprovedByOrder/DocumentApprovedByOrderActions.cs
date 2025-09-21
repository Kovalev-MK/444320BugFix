using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.LocalRegulations.DocumentApprovedByOrder;

namespace lenspec.LocalRegulations.Client
{
  partial class DocumentApprovedByOrderActions
  {

    
    //Добавлено Avis Expert
    public override void SaveAndClose(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.HasVersions)
      {
        e.AddError(lenspec.LocalRegulations.DocumentApprovedByOrders.Resources.NeedCreateDocumentVersion);
        return;
      }
      base.SaveAndClose(e);
    }

    public override bool CanSaveAndClose(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSaveAndClose(e);
    }

    public override void Save(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.HasVersions)
      {
        e.AddError(lenspec.LocalRegulations.DocumentApprovedByOrders.Resources.NeedCreateDocumentVersion);
        return;
      }
      base.Save(e);
    }

    public override bool CanSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSave(e);
    }
    
    public override void UpdateTemplatelenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.UpdateTemplatelenspec(e);
    }

    public override bool CanUpdateTemplatelenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
    }
    //конец Добавлено Avis Expert

  }

}