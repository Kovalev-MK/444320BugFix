using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ReworkApprovalerAssignment;

namespace avis.ApprovingCounterpartyDEB.Server
{
  partial class ReworkApprovalerAssignmentFunctions
  {

    /// <summary>
    /// Вкладка Регламент
    /// </summary>
    [Remote]
    public StateView GetReworkApprovalerAssignmentState()
    {
      var task = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.As(_obj.Task);
      var stateView = Functions.ApprovalCounterpartyPersonDEB.GetApprovalCounterpartyPersonDEBState(task);
      return stateView;
    }

  }
}