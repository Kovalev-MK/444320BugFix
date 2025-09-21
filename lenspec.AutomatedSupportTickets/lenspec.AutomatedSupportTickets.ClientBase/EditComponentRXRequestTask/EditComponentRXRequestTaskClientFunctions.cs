using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.EditComponentRXRequestTask;

namespace lenspec.AutomatedSupportTickets.Client
{
  // Добавлено avis.
  partial class EditComponentRXRequestTaskFunctions
  {
    /// <summary>
    /// Проверить возможность отклонения задания.
    /// </summary>
    /// <param name="assignment">Задание.</param>
    /// <param name="errorMessage">Сообщение об ошибке.</param>
    /// <param name="eventArgs">Аргумент обработчика вызова.</param>
    /// <returns>True - разрешить отклонение, иначе false.</returns>
    public static bool ValidateBeforeReject(Sungero.Workflow.IAssignment assignment, string errorMessage, Sungero.Domain.Client.ExecuteActionArgs eventArgs)
    {
      if (string.IsNullOrWhiteSpace(assignment.ActiveText))
      {
        eventArgs.AddError(errorMessage);
        return false;
      }
      
      if (!eventArgs.Validate())
        return false;
      
      return true;
    }
    
    /// <summary>
    /// Показывает свойства в зависимости от выбранного типа обращения.
    /// </summary>  
    [Public]
    public static void ShowProperties(AutomatedSupportTickets.IEditComponentRXRequestTask task)
    {
      if (task.TypeRequest == EditComponentRXRequestTask.TypeRequest.EditDoc)
        EditDoc(task);
      
      if (task.TypeRequest == EditComponentRXRequestTask.TypeRequest.EditProcess)
        EditComponent(task);
      
      if (task.TypeRequest == EditComponentRXRequestTask.TypeRequest.Other)
        Other(task);
    }
    
    /// <summary>
    /// Скрывает все дополнительные поля.
    /// </summary>
    private static void Hide(AutomatedSupportTickets.IEditComponentRXRequestTask task)
    {
      task.State.Properties.CollectionEmployees.IsVisible = false;
      task.State.Properties.CollectionDocumentTypes.IsVisible = false;
      task.State.Properties.DescriptionPermit.IsVisible = false;
      task.State.Properties.ObjectEdit.IsVisible = false;
      task.State.Properties.ProcessEdit.IsVisible = false;
      task.State.Properties.Comment.IsVisible = false;   
      task.State.Properties.Description.IsVisible = false;
      
      task.State.Properties.CollectionEmployees.IsRequired = false;
      task.State.Properties.CollectionDocumentTypes.IsRequired = false;
      task.State.Properties.DescriptionPermit.IsRequired = false;
      task.State.Properties.ObjectEdit.IsRequired = false;
      task.State.Properties.ProcessEdit.IsRequired = false;
      task.State.Properties.Comment.IsRequired = false;   
      task.State.Properties.Description.IsRequired = false;
    }
    
    /// <summary>
    /// Отображает поля для заявки на изменение документа.
    /// </summary>
    private static void EditDoc(AutomatedSupportTickets.IEditComponentRXRequestTask task)
    {
      Hide(task);
      task.State.Properties.CollectionEmployees.IsVisible = true;
      task.State.Properties.CollectionDocumentTypes.IsVisible = true;
      task.State.Properties.DescriptionPermit.IsVisible = true;
      
      task.State.Properties.CollectionEmployees.IsRequired = true;
      task.State.Properties.CollectionDocumentTypes.IsRequired = true;
      task.State.Properties.DescriptionPermit.IsRequired = true;
    }
    
    /// <summary>
    /// Отображает поля для заявки на изменение компонента.
    /// </summary>
    private static void EditComponent(AutomatedSupportTickets.IEditComponentRXRequestTask task)
    {
      Hide(task);
      task.State.Properties.ObjectEdit.IsVisible = true;
      task.State.Properties.ProcessEdit.IsVisible = true;
      task.State.Properties.Comment.IsVisible = true;   

      task.State.Properties.ObjectEdit.IsRequired = true;
      task.State.Properties.ProcessEdit.IsRequired = true;
      task.State.Properties.Comment.IsRequired = true;        
    }
    
    /// <summary>
    ///  Отображает поля для остальных заявок.
    /// </summary>
    private static void Other(AutomatedSupportTickets.IEditComponentRXRequestTask task)
    {
      Hide(task);
      task.State.Properties.Description.IsVisible = true;
      
      task.State.Properties.Description.IsRequired = true;
    }
  }
  // Конец добавлено avis.
}