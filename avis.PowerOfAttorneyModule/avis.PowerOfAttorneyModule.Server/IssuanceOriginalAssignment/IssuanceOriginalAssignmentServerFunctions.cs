using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.IssuanceOriginalAssignment;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class IssuanceOriginalAssignmentFunctions
  {

    /// <summary>
    /// Контрол состояния
    /// </summary>       
    [Remote]
    public StateView GetIssuanceOriginalAssignmentState()
    {
      var task = PowerOfAttorneyModule.ExecutionPowerOfAttorneys.As(_obj.Task);
      return Functions.ExecutionPowerOfAttorney.GetExecutionPowerOfAttorneyState(task);
    }

  }
}