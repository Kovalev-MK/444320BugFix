using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ExecutionLawyerAssignment;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class ExecutionLawyerAssignmentFunctions
  {

    /// <summary>
    /// Контрол состояния
    /// </summary>       
    [Remote]
    public StateView GetExecutionLawyerAssignmentState()
    {
      var task = ExecutionPowerOfAttorneys.As(_obj.Task);
      return Functions.ExecutionPowerOfAttorney.GetExecutionPowerOfAttorneyState(task);
    }

  }
}