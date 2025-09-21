using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.ActAcceptanceOfApartment;

namespace lenspec.SalesDepartmentArchive
{
  partial class ActAcceptanceOfApartmentClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      // base.Refresh(e);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      // base.Showing(e);
      _obj.State.Properties.Subject.IsRequired = false;
    }
  }

}