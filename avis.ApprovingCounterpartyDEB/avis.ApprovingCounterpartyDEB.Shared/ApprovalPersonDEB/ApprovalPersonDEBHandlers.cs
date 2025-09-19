using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalPersonDEB;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalPersonDEBSharedHandlers
  {

    public override void CounterpartyChanged(avis.ApprovingCounterpartyDEB.Shared.ApprovalCounterpartyBaseCounterpartyChangedEventArgs e)
    {
      base.CounterpartyChanged(e);
      Functions.ApprovalPersonDEB.FillDateOfBirth(_obj);
      Functions.ApprovalPersonDEB.FillName(_obj);
    }

    public override void CreatedChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.CreatedChanged(e);
      Functions.ApprovalPersonDEB.FillName(_obj);
    }

  }
}