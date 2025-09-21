using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB;

namespace avis.ApprovingCounterpartyDEB
{

  partial class ApprovalCounterpartyPersonDEBClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.ApprovalCounterpartyPersonDEB.CheckPropertiesVisible(_obj);
    }

  }
}