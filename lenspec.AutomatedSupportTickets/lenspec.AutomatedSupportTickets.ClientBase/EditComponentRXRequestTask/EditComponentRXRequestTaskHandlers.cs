using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.EditComponentRXRequestTask;

namespace lenspec.AutomatedSupportTickets
{
  // Добавлено avis.
  partial class EditComponentRXRequestTaskClientHandlers
  {

    public override void AuthorValueInput(Sungero.Workflow.Client.TaskAuthorValueInputEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (!lenspec.AutomatedSupportTickets.PublicFunctions.Module.Remote.CheckAutomatedUser(Sungero.Company.Employees.As(e.NewValue)))
        {
          e.AddError(lenspec.AutomatedSupportTickets.EditComponentRXRequestTasks.Resources.NeedSpecifyAutomatedEmployee);
        }
      }
    }

    public virtual void FromValueInput(lenspec.AutomatedSupportTickets.Client.EditComponentRXRequestTaskFromValueInputEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (!lenspec.AutomatedSupportTickets.PublicFunctions.Module.Remote.CheckAutomatedUser(Sungero.Company.Employees.As(e.NewValue)))
        {
          e.AddError(lenspec.AutomatedSupportTickets.EditComponentRXRequestTasks.Resources.NeedSpecifyAutomatedEmployee);
        }
      }
    }
    
    /// <summary>
    /// Показ формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      if (_obj.Subject == null)
      {
        _obj.Subject = "<Тема будет сформирована автоматически>";
        _obj.Number = $"{_obj.Id}-{Calendar.Today.Year}";
        _obj.DateCreated = Calendar.Today;
        _obj.From = _obj.Author;
      }
      
      // Отображаем поля в зависимости, от выбранного типа обращения.
      AutomatedSupportTickets.PublicFunctions.EditComponentRXRequestTask.ShowProperties(_obj);
    }
  }
  // Конец добавлено avis.
}