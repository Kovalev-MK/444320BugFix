using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ManagerApprovalAssignment;

namespace avis.ApprovingCounterpartyDEB.Server
{
  partial class ManagerApprovalAssignmentFunctions
  {

    /// <summary>
    /// Вкладка Регламент
    /// </summary>
    [Remote]
    public StateView GetManagerApprovalAssignmentState()
    {
      var task = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.As(_obj.Task);
      var stateView = Functions.ApprovalCounterpartyPersonDEB.GetApprovalCounterpartyPersonDEBState(task);
      return stateView;
    }

  }
}