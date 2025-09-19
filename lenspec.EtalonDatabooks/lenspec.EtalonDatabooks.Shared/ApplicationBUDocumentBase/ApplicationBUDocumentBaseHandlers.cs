using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApplicationBUDocumentBase;

namespace lenspec.EtalonDatabooks
{
  partial class ApplicationBUDocumentBaseSharedHandlers
  {

    //Добавлено Avis Expert
    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
    }

    public virtual void BusinessUnitNameChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      Functions.ApplicationBUDocumentBase.FillName(_obj);
    }
    //конец Добавлено Avis Expert

  }
}