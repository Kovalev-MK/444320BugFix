using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ReworkApprovalerAssignment;

namespace avis.ApprovingCounterpartyDEB.Client
{
  partial class ReworkApprovalerAssignmentActions
  {
    public virtual void Corrected(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanCorrected(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}