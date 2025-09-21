using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.Authority;

namespace avis.PowerOfAttorneyModule
{
  partial class AuthorityClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      _obj.State.Properties.Code.IsVisible = _obj.IsFormalizedPowerOfAttorneys == true;
      _obj.State.Properties.Code.IsRequired = _obj.IsFormalizedPowerOfAttorneys == true;
      _obj.State.Properties.Mnemonics.IsVisible = _obj.IsFormalizedPowerOfAttorneys == true;
      _obj.State.Properties.Mnemonics.IsRequired = _obj.IsFormalizedPowerOfAttorneys == true;
      _obj.State.Properties.ValidFrom.IsVisible = _obj.IsFormalizedPowerOfAttorneys == true;
      _obj.State.Properties.ValidFrom.IsRequired = _obj.IsFormalizedPowerOfAttorneys == true;
      _obj.State.Properties.ValidTill.IsVisible = _obj.IsFormalizedPowerOfAttorneys == true;
      _obj.State.Properties.ValidTill.IsRequired = _obj.IsFormalizedPowerOfAttorneys == true;
    }

    public virtual void IsFormalizedPowerOfAttorneysValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      _obj.State.Properties.Code.IsVisible = e.NewValue == true;
      _obj.State.Properties.Code.IsRequired = e.NewValue == true;
      _obj.State.Properties.Mnemonics.IsVisible = e.NewValue == true;
      _obj.State.Properties.Mnemonics.IsRequired = e.NewValue == true;
      _obj.State.Properties.ValidFrom.IsVisible = e.NewValue == true;
      _obj.State.Properties.ValidFrom.IsRequired = e.NewValue == true;
      _obj.State.Properties.ValidTill.IsVisible = e.NewValue == true;
      _obj.State.Properties.ValidTill.IsRequired = e.NewValue == true;
    }

  }
}