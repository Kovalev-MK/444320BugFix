using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingInvoice;

namespace lenspec.Etalon
{
  partial class IncomingInvoiceClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Functions.IncomingInvoice.SetRequiredPropertiesOnVisualMode(_obj);
    }

  }
}