using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApplicationBUEditingDocument;

namespace lenspec.EtalonDatabooks
{
  partial class ApplicationBUEditingDocumentSharedHandlers
  {

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      if(e.NewValue != null)
      {
        _obj.TIN = e.NewValue.TIN;
        _obj.CompanyProfile = lenspec.Etalon.BusinessUnits.Get(e.NewValue.Id).CompanyProfileavis;
      }
    }

  }
}