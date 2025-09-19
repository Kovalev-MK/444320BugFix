using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ApprovalCounterpartyRegisterTask;

namespace lenspec.Tenders
{
  partial class ApprovalCounterpartyRegisterTaskServerHandlers
  {

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      // Заполним тему задачи для корректного отображения в заданиях.
      Functions.ApprovalCounterpartyRegisterTask.FillName(_obj);
      
      var cancel = false;
      if (string.IsNullOrWhiteSpace(_obj.ActiveText))
      {
        e.AddError(lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Resources.FillActiveText);
        cancel = true;
      }
      
      var message = Functions.ApprovalCounterpartyRegisterTask.ValidateAttachments(_obj);
      if (!string.IsNullOrEmpty(message))
      {
        e.AddError(message);
        cancel = true;
      }
      
      if (cancel)
        return;
      
      base.BeforeStart(e);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      if (!_obj.State.IsCopied)
        _obj.Subject = Sungero.DocflowApproval.Resources.AutoformatTaskSubject;
      
      // Вычисление срока выполнения задачи на основе блока "Согласование с комитетом".
      var committeeApprovalBlock = Tenders.ApprovalCounterpartyRegisterTask.Blocks.CommitteeApprovalBlocks.GetAll(_obj.Scheme).FirstOrDefault();
      
      if (committeeApprovalBlock != null)
      {
        var deadline = Calendar.UserNow;
        // При наличии срока добавляем его к текущему времени.
        if (committeeApprovalBlock.Deadline.Relative.HasValue)
        {
          deadline = Calendar.AddWorkingDays(deadline, committeeApprovalBlock.Deadline.Relative.Value.Days);
          deadline = Calendar.AddWorkingHours(deadline, committeeApprovalBlock.Deadline.Relative.Value.Hours);
        }
        _obj.MaxDeadline = deadline;
      }
      else
        Logger.Error(lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Resources.CommitteeApprovalBlockNotFound);
    }
  }

}