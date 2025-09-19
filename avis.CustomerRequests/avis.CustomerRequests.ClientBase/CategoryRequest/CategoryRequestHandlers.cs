using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CategoryRequest;

namespace avis.CustomerRequests
{
  partial class CategoryRequestClientHandlers
  {

    public virtual void IsClaimValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      if (e.NewValue == false)
      {
        _obj.State.Properties.ClaimType.IsEnabled = false;
        _obj.State.Properties.ClaimType.IsRequired = false;
        _obj.ClaimType = null;
      }
      else
      {
        _obj.State.Properties.ClaimType.IsEnabled = true;
        _obj.State.Properties.ClaimType.IsRequired = true;
      }
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      if (_obj.IsClaim == false)
      {
        _obj.State.Properties.ClaimType.IsEnabled = false;
        _obj.State.Properties.ClaimType.IsRequired = false;
      }
      else
      {
        _obj.State.Properties.ClaimType.IsEnabled = true;
        _obj.State.Properties.ClaimType.IsRequired = true;
      }
    }

  }
}