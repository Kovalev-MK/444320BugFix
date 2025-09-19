using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.CommitteeApprovalAssignment;

namespace lenspec.Tenders
{
  partial class CommitteeApprovalAssignmentClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      var addingApproversIsAllowed = Functions.CommitteeApprovalAssignment.CanAddApprovers(_obj);
      if (!addingApproversIsAllowed)
        e.HideAction(_obj.Info.Actions.AddApprover);
    }

  }
}