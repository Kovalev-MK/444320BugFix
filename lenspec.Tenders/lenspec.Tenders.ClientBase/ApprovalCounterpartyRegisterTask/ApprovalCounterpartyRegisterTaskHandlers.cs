using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ApprovalCounterpartyRegisterTask;

namespace lenspec.Tenders
{
  partial class ApprovalCounterpartyRegisterTaskClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      var needShowDecision = _obj.Status != Tenders.ApprovalCounterpartyRegisterTask.Status.Draft;
      _obj.State.Properties.QCDecision.IsVisible = needShowDecision;
      _obj.State.Properties.QCDecisionDate.IsVisible = needShowDecision;
      _obj.State.Properties.ApprovalResult.IsVisible = needShowDecision;
      _obj.State.Properties.Author.IsVisible = true;
    }

  }
}