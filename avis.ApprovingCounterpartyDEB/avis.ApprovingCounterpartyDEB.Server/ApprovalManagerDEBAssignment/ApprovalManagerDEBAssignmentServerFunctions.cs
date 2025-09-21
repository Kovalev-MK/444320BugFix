using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment;

namespace avis.ApprovingCounterpartyDEB.Server
{
  partial class ApprovalManagerDEBAssignmentFunctions
  {

    /// <summary>
    /// Вкладка регламент
    /// </summary>
    [Remote]
    public StateView GetApprovalManagerDEBAssignmentState()
    {
      var task = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.As(_obj.Task);
      var stateView = Functions.ApprovalCounterpartyPersonDEB.GetApprovalCounterpartyPersonDEBState(task);
      return stateView;
    }

  }
}