using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalCheckingAssignment;

namespace lenspec.Etalon
{
  partial class ApprovalCheckingAssignmentSharedHandlers
  {

    //Добавлено Avis Expert
    public override void TaskChanged(Sungero.Workflow.Shared.AssignmentBaseTaskChangedEventArgs e)
    {
      base.TaskChanged(e);
      if (e.NewValue != null)
      {
        var approvalTask = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
        var outgoingLetter = Sungero.RecordManagement.OutgoingLetters.As(approvalTask.DocumentGroup.OfficialDocuments.FirstOrDefault());
        if (outgoingLetter != null)
        {
          _obj.DeliveryMethodavis = outgoingLetter.DeliveryMethod;
        }
      }
    }
    //конец Добавлено Avis Expert

  }
}