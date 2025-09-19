using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ScanningAssignment;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class ScanningAssignmentFunctions
  {

    /// <summary>
    /// Контрол состояния
    /// </summary>
    [Remote]
    public StateView GetScanningAssignmentState()
    {
      var task = PowerOfAttorneyModule.ExecutionPowerOfAttorneys.As(_obj.Task);
      return Functions.ExecutionPowerOfAttorney.GetExecutionPowerOfAttorneyState(task);
    }

  }
}