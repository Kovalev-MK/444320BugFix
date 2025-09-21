using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalerDEBApprovalAssignmentClientHandlers
  {

    public virtual void ForwardValueInput(avis.ApprovingCounterpartyDEB.Client.ApprovalerDEBApprovalAssignmentForwardValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignments.Resources.ErrorMessageNotAutomatedEmployee);
        return;
      }
    }

    public virtual void CounterpartyLimitValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      if (e.NewValue != null && e.NewValue < 0)
        e.AddError(ApprovalerDEBApprovalAssignments.Resources.PositiveNumberErrorMessage);
    }

    public virtual void CompletionDateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue != null && e.NewValue != e.OldValue && e.NewValue < Calendar.Today)
      {
        e.AddError(ApprovalerDEBApprovalAssignments.Resources.DateApprovalErrorMessage);
        return;
      }
    }
  }
}