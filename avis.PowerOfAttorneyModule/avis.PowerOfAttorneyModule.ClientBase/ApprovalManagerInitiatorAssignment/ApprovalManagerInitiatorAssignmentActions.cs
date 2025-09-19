using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ApprovalManagerInitiatorAssignment;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class ApprovalManagerInitiatorAssignmentActions
  {
    public virtual void Reject(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if(string.IsNullOrEmpty(_obj.ActiveText))
      {
        e.AddError(avis.PowerOfAttorneyModule.ApprovalManagerInitiatorAssignments.Resources.ErrorMessageCencelAction);
        return;
      }
    }

    public virtual bool CanReject(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Rework(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if(string.IsNullOrEmpty(_obj.ActiveText))
      {
        e.AddError(avis.PowerOfAttorneyModule.ApprovalManagerInitiatorAssignments.Resources.ErrorMessageReworkAction);
        return;
      }
    }

    public virtual bool CanRework(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
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