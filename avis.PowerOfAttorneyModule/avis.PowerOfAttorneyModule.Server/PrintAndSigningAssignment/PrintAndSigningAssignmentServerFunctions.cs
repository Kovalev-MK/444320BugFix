using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.PrintAndSigningAssignment;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class PrintAndSigningAssignmentFunctions
  {

    /// <summary>
    /// Контрол состояния
    /// </summary>       
    [Remote]
    public StateView GetPrintAndSigningAssignmentState()
    {
      var task = PowerOfAttorneyModule.ExecutionPowerOfAttorneys.As(_obj.Task);
      return Functions.ExecutionPowerOfAttorney.GetExecutionPowerOfAttorneyState(task);
    }

  }
}