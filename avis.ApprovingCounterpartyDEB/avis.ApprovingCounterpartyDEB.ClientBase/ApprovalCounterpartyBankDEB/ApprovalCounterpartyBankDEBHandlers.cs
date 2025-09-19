using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEB;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalCounterpartyBankDEBClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      Functions.ApprovalCounterpartyBankDEB.SetControlPropery(_obj);
    }

  }
}