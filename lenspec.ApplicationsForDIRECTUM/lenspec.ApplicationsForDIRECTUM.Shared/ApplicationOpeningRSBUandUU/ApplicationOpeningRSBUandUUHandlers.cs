using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForDIRECTUM.ApplicationOpeningRSBUandUU;

namespace lenspec.ApplicationsForDIRECTUM
{
  partial class ApplicationOpeningRSBUandUUSharedHandlers
  {

    public virtual void EndPeriodChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {    
      FillName();
    }

    public virtual void BeginPeriodChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      FillName();
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      FillName();
    }

  }
}