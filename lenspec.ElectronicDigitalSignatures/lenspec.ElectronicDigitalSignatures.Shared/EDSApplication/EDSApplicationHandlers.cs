using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.EDSApplication;

namespace lenspec.ElectronicDigitalSignatures
{
  partial class EDSApplicationSharedHandlers
  {

    public virtual void TargetChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty(_obj.Explanation) &&
        e.NewValue != lenspec.ElectronicDigitalSignatures.EDSApplication.Target.Other)
      {
        _obj.Explanation = null;
      }
      
      _obj.State.Controls.Control.Refresh();
    }

    public virtual void ApplicationCategoryChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if ((_obj.Target != null || !string.IsNullOrEmpty(_obj.Explanation)) &&
        e.NewValue != lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Cancellation)
      {
        _obj.Target = null;
        _obj.Explanation = null;
      }
      
      _obj.State.Controls.Control.Refresh();
    }

  }
}