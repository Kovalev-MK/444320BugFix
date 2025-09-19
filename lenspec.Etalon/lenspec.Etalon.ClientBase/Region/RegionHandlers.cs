using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Region;

namespace lenspec.Etalon
{
  partial class RegionClientHandlers
  {

    public override void NameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.NameValueInput(e);
      
      if (e.NewValue != null)
        e.NewValue = e.NewValue.Trim();
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
    }

  }
}