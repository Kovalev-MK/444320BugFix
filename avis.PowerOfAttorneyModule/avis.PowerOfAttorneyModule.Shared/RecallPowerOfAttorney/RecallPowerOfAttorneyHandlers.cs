using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.RecallPowerOfAttorney;

namespace avis.PowerOfAttorneyModule
{
  partial class RecallPowerOfAttorneySharedHandlers
  {

    public override void SubjectChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      // Обрезать заголовок.
      if (e.NewValue != null && e.NewValue.Length > RecallPowerOfAttorneys.Info.Properties.Subject.Length)
        _obj.Subject = e.NewValue.Substring(0, RecallPowerOfAttorneys.Info.Properties.Subject.Length);
    }

    public virtual void PowerOfAttorneyAdded(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      Functions.RecallPowerOfAttorney.FillName(_obj);
    }
    
  }
}