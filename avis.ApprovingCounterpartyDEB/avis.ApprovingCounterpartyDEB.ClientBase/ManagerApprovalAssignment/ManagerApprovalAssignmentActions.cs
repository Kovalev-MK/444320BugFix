using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ManagerApprovalAssignment;

namespace avis.ApprovingCounterpartyDEB.Client
{
  partial class ManagerApprovalAssignmentActions
  {
    public virtual void Rework(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (string.IsNullOrEmpty(_obj.ActiveText))
      {
        e.AddError("Напишите комментарий в тексте задания");
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