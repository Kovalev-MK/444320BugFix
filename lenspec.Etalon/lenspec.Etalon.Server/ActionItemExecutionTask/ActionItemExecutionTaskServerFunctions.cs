using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ActionItemExecutionTask;

namespace lenspec.Etalon.Server
{
  partial class ActionItemExecutionTaskFunctions
  {
    
    //Добавлено Avis Expert    
    /// <summary>
    /// Проверить возможность изменения пункта поручения при нажатии подтверждения в диалоге корректировки.
    /// </summary>
    /// <param name="actionItemPartExecutionTask">Задача по пункту поручения.</param>
    /// <param name="newAssignee">Новый исполнитель.</param>
    /// <param name="deadline">Новый срок.</param>
    /// <param name="dialogOpenDate">Дата открытия диалога.</param>
    /// <returns>Текст ошибки или пустую строку, если ошибок нет.</returns>
    [Remote(IsPure = true)]
    public override string CheckActionItemPartEditInDialog(Sungero.RecordManagement.IActionItemExecutionTask actionItemPartExecutionTask,
                                                           Sungero.Company.IEmployee newAssignee, DateTime? deadline, DateTime? dialogOpenDate)
    {
      return base.CheckActionItemPartEditInDialog(actionItemPartExecutionTask, newAssignee, deadline, dialogOpenDate);
    }
    //конец Добавлено Avis Expert
  }
}