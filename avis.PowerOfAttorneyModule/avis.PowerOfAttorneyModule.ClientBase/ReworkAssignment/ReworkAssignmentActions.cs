using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ReworkAssignment;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class ReworkAssignmentActions
  {
    public virtual void Abort(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var task = PowerOfAttorneyModule.ExecutionPowerOfAttorneys.As(_obj.Task);
      task.Abort();
    }

    public virtual bool CanAbort(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}