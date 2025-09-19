using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Country;

namespace lenspec.Etalon
{
  partial class CountryClientHandlers
  {

    public override void NameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.NameValueInput(e);
      
      if (e.NewValue != null)
        e.NewValue = e.NewValue.Trim();
    }

  }
}