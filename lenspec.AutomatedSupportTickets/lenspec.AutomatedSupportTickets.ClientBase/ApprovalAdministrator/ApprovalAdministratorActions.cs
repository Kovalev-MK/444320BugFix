using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.ApprovalAdministrator;

namespace lenspec.AutomatedSupportTickets.Client
{
  // Добавлено avis.
  partial class ApprovalAdministratorActions
  {
    /// <summary>
    /// Кнопка "Отправить на согласование".
    /// </summary>
    /// <param name="e"></param>
    public virtual void SendForApproval(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (!_obj.AddApprovers.Any())
      {
        e.AddError(lenspec.AutomatedSupportTickets.ApprovalAdministrators.Resources.NeedFillAddApprovers);
        return;
      }
      
      // Прокинуть нового исполнителя в задачу.
      var task = EditComponentRXRequestTasks.As(_obj.Task);
      //task.Forwarding = _obj.Coordinating;
      foreach(var item in _obj.AddApprovers)
      {
        var approver = task.AddApprovers.AddNew();
        approver.Approver = item.Approver;
      }
      task.Save();
    }

    public virtual bool CanSendForApproval(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    /// <summary>
    /// Кнопка "Отклонить".
    /// </summary>
    /// <param name="e"></param>
    public virtual void Reject(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      // Валидация заполненности активного текста.
      if (!Functions.EditComponentRXRequestTask.ValidateBeforeReject(_obj, lenspec.AutomatedSupportTickets.ApprovalAdministrators.Resources.NeedFillActiveTextBeforeReject, e))
      {
        e.Cancel();
      }
    }

    public virtual bool CanReject(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
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
  // Конец добавлено avis.
}