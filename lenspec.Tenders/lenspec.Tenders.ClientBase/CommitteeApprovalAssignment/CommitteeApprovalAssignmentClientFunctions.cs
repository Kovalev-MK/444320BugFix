using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.CommitteeApprovalAssignment;

namespace lenspec.Tenders.Client
{
  partial class CommitteeApprovalAssignmentFunctions
  {

    /// <summary>
    /// Показать диалог переадресации.
    /// </summary>
    /// <returns>True, если запрос был подтвержден.</returns>
    public virtual bool ShowForwardingDialog()
    {
      var notAvailablePerformers = Functions.CommitteeApprovalAssignment.Remote.GetActiveAndFutureAssignmentsPerformers(_obj).ToList();

      var dialogResult = Sungero.Docflow.PublicFunctions.Module.ShowForwardDialog(notAvailablePerformers, _obj.Deadline, TimeSpan.Zero);
      if (dialogResult.ForwardButtonIsPressed)
      {
        _obj.ForwardTo = dialogResult.ForwardTo;
        _obj.ForwardDeadline = dialogResult.Deadline;
        return true;
      }
      
      return false;
    }
  }
}