using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.ApproveRevision;

namespace avis.EtalonParties.Client
{
  // Добавлено avis.
  
  partial class ApproveRevisionActions
  {
    /// <summary>
    /// Реультат переадресовать.
    /// </summary>
    /// <param name="e"></param>
    public virtual void Forward(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forwarding == null)
      {
        e.AddError("Для переадресации, заполните поле 'Переадресовать сотруднику'.");
        return;
      }
      
      // Прокинуть нового исполнителя в задачу.
      var task = CreateCompanyTasks.As(_obj.Task);
      task.Forwarding = _obj.Forwarding;
      task.Save();
    }

    public virtual bool CanForward(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    /// <summary>
    /// Результат прекратить.
    /// </summary>
    /// <param name="e"></param>
    public virtual void Stop(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
    }

    public virtual bool CanStop(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    /// <summary>
    /// Результат исполнено.
    /// </summary>
    /// <param name="e"></param>
    public virtual void Corrected(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
    }

    public virtual bool CanCorrected(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }
  }
  // Конец добавлено avis.
}