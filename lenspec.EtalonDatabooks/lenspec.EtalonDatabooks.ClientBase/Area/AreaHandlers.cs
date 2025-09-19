using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.Area;

namespace lenspec.EtalonDatabooks
{
  partial class AreaClientHandlers
  {

    public virtual void NameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      e.NewValue = e.NewValue?.Trim();
    }

  }
}