using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ReworkInitiatorAssignment;

namespace avis.ApprovingCounterpartyDEB.Server
{
  partial class ReworkInitiatorAssignmentFunctions
  {

    /// <summary>
    /// вкладка Регламент
    /// </summary>
    [Remote]
    public StateView GetReworkInitiatorAssignmentState()
    {
      var task = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.As(_obj.Task);
      var stateView = Functions.ApprovalCounterpartyPersonDEB.GetApprovalCounterpartyPersonDEBState(task);
      return stateView;
    }

  }
}