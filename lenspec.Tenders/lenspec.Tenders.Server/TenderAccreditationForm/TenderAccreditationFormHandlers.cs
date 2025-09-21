using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderAccreditationForm;

namespace lenspec.Tenders
{
  partial class TenderAccreditationFormServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (!_obj.HasVersions)
      {
        e.AddError(_obj.Info.Properties.HasVersions, lenspec.Tenders.TenderAccreditationForms.Resources.NeedCreateDocumentVersion);
        return;
      }
      
      base.BeforeSave(e);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsQualificationDocumentlenspec = true;
    }
  }

}