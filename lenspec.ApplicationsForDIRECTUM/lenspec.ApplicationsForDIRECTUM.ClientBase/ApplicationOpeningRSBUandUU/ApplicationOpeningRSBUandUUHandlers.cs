using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForDIRECTUM.ApplicationOpeningRSBUandUU;

namespace lenspec.ApplicationsForDIRECTUM
{
  partial class ApplicationOpeningRSBUandUUClientHandlers
  {

    public virtual void BeginPeriodValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue == null)
        return;
      
      if (_obj.EndPeriod != null && e.NewValue.Value > _obj.EndPeriod.Value)
      {
        e.AddError(lenspec.ApplicationsForDIRECTUM.ApplicationOpeningRSBUandUUs.Resources.IncorrectEndPeriod);
      }
    }

    public virtual void EndPeriodValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue == null)
        return;
      
      if (_obj.BeginPeriod != null && e.NewValue.Value < _obj.BeginPeriod.Value)
      {
        e.AddError(lenspec.ApplicationsForDIRECTUM.ApplicationOpeningRSBUandUUs.Resources.IncorrectEndPeriod);
      }
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      if (_obj.BeginPeriod != null)
        _obj.State.Properties.EndPeriod.IsEnabled = true;
      else
        _obj.State.Properties.EndPeriod.IsEnabled = false;
    }

  }
}