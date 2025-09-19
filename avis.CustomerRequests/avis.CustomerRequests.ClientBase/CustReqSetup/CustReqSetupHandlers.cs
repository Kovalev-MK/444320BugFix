using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustReqSetup;

namespace avis.CustomerRequests
{
  partial class CustReqSetupClientHandlers
  {

    public virtual void DaysForReviewValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue.Value < 0)
        e.AddError(avis.CustomerRequests.CustReqSetups.Resources.CountMustBePositive);
    }
  }
}