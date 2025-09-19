using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ApprovalManagerInitiatorAssignment;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class ApprovalManagerInitiatorAssignmentFunctions
  {

    /// <summary>
    /// Контрол состояния
    /// </summary>
    [Remote]
    public StateView GetApprovalManagerInitiatorAssignmentState()
    {
      var task = ExecutionPowerOfAttorneys.As(_obj.Task);
      return Functions.ExecutionPowerOfAttorney.GetExecutionPowerOfAttorneyState(task);
    }

  }
}