using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.ApprovalManager;

namespace lenspec.AutomatedSupportTickets.Client
{
  // Добавлено avis.
  partial class ApprovalManagerActions
  {
    /// <summary>
    /// Кнопка "Отклонить".
    /// </summary>
    /// <param name="e"></param>
    public virtual void Reject(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      // Валидация заполненности активного текста.
      if (!Functions.EditComponentRXRequestTask.ValidateBeforeReject(_obj, lenspec.AutomatedSupportTickets.ApprovalManagers.Resources.NeedFillActiveTextBeforeReject, e))
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