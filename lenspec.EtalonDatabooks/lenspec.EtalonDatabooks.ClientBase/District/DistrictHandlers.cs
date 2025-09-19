using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.District;

namespace lenspec.EtalonDatabooks
{
  partial class DistrictClientHandlers
  {

    public virtual void NameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (e.NewValue != null)
        e.NewValue = e.NewValue.Trim();
    }
  }


}