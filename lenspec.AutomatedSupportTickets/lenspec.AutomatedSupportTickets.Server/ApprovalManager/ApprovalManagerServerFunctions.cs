using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.ApprovalManager;

namespace lenspec.AutomatedSupportTickets.Server
{
  partial class ApprovalManagerFunctions
  {

    /// <summary>
    /// Описание заявки.
    /// </summary>       
    [Remote]
    public StateView GetApprovalManagerState()
    {
      // Формируем текст для вывода в форму.
      var stateView = StateView.Create();
      var block = stateView.AddBlock();
      block.DockType = DockType.Bottom;
      
      // Формируем начало информации.
      block.AddLabel("ИНФОРМАЦИЯ О СОТРУДНИКЕ");
      block.AddLineBreak();
      
      // Получаем задачу.
      var task = lenspec.AutomatedSupportTickets.EditComponentRXRequestTasks.As(_obj.Task);
      // Заполняем подробную информацию
      if (task.TypeRequest == AutomatedSupportTickets.EditComponentRXRequestTask.TypeRequest.EditDoc)
        lenspec.AutomatedSupportTickets.PublicFunctions.EditComponentRXRequestTask.GetEditDocumentDiscription(task, block);
      if (task.TypeRequest == AutomatedSupportTickets.EditComponentRXRequestTask.TypeRequest.EditProcess)
        lenspec.AutomatedSupportTickets.PublicFunctions.EditComponentRXRequestTask.GetEditProcessDiscription(task, block);
      if (task.TypeRequest == AutomatedSupportTickets.EditComponentRXRequestTask.TypeRequest.Other)
        lenspec.AutomatedSupportTickets.PublicFunctions.EditComponentRXRequestTask.GetOtherDiscription(task, block);

      // Отрисовываем на форму.
      return stateView;
    }
  }
}