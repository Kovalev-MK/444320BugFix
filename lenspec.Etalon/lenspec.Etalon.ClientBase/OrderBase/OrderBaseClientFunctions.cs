using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OrderBase;

namespace lenspec.Etalon.Client
{
  partial class OrderBaseFunctions
  {

    /// <summary>
    /// Если по документу уже были запущены задачи на согласование по регламенту,
    /// то с помощью диалога определить, нужно ли создавать ещё одну.
    /// </summary>
    [Public]
    new public bool NeedCreateApprovalTask()
    {
      var result = true;
      
      var createdTasks = Sungero.Docflow.PublicFunctions.Module.Remote.GetApprovalTasks(_obj);
      if (createdTasks.Any())
      {
        var dialog = Dialogs.CreateTaskDialog("Документ ранее был отправлен на согласование",
                                              MessageType.Information);
        dialog.Buttons.AddOk();
        dialog.Show();
        result = false;
      }
      
      return result;
    }
    
    /// <summary>
    /// Проверим, есть ли тело документа, если нет то выведем диалог
    /// </summary>
    [Public]
    public bool NeedSendForApproval()
    {
      var result = true;
      
      if (!_obj.HasVersions)
      {
        var dialog = Dialogs.CreateTaskDialog("Создайте Локально-нормативный акт по кнопке Создать из шаблона",
                                              MessageType.Information);
        dialog.Buttons.AddOk();
        dialog.Show();
        result = false;
      }
      
      return result;
    }
  }
}