using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.RegistrationPowerOfAttorneyAssignment;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class RegistrationPowerOfAttorneyAssignmentFunctions
  {

    /// <summary>
    /// Контрол состояния
    /// </summary>       
    [Remote]
    public StateView GetRegistrationPowerOfAttorneyAssignmentState()
    {
      var task = PowerOfAttorneyModule.ExecutionPowerOfAttorneys.As(_obj.Task);
      return Functions.ExecutionPowerOfAttorney.GetExecutionPowerOfAttorneyState(task);
    }

  }
}