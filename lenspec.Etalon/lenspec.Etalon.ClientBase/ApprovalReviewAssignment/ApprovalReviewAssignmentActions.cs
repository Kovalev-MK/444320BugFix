using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalReviewAssignment;

namespace lenspec.Etalon.Client
{
  partial class ApprovalReviewAssignmentActions
  {
    public override void CreateActionItem(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!lenspec.Etalon.Functions.ApprovalTask.HasDocumentAndCanRead(ApprovalTasks.As(_obj.Task)))
      {
        e.AddError(ApprovalTasks.Resources.NoRightsToDocument);
        return;
      }
      
      _obj.Save();
      var parentAssignmentId = _obj.Id;
      var document = _obj.DocumentGroup.OfficialDocuments.First();
      var assignedBy = Employees.Current;
      var hackTask = Sungero.RecordManagement.PublicFunctions.Module.Remote.CreateActionItemExecutionWithResolution(document, parentAssignmentId, _obj.ActiveText, assignedBy);
      if (hackTask != null)
      {
        lenspec.Etalon.ActionItemExecutionTasks.As(hackTask).TaskAssignedlenspec = _obj;
        hackTask.ShowModal();
        // Если задание на рассмотрение завершено с результатом "Отправлено на исполнение", то сохранить и закрыть карточку задания.
        if (_obj.Status == ApprovalReviewAssignment.Status.Completed &&
            _obj.Result != null && _obj.Result == ApprovalReviewAssignment.Result.AddActionItem)
        {
          this.SaveAndClose(e);
        }
      }
    }

    public override bool CanCreateActionItem(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCreateActionItem(e);
    }

  }

}