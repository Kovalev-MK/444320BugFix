using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAActAcceptanceOfApartment;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAActAcceptanceOfApartmentClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.ClientNew.IsRequired = true;
      
      if (_obj.Archiveavis == true)
        _obj.State.Properties.Client.IsVisible = true;
      else
        _obj.State.Properties.Client.IsVisible = false;
    }

  }
}