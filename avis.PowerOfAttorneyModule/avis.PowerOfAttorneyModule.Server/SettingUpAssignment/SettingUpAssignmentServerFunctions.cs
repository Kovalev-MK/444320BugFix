using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.SettingUpAssignment;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class SettingUpAssignmentFunctions
  {

    /// <summary>
    /// ������� ���������
    /// </summary>
    [Remote]
    public StateView GetSettingUpAssignmentState()
    {
      var task = PowerOfAttorneyModule.ExecutionPowerOfAttorneys.As(_obj.Task);
      return Functions.ExecutionPowerOfAttorney.GetExecutionPowerOfAttorneyState(task);
    }

  }
}