using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.EditComponentRXRequestTask;

namespace lenspec.AutomatedSupportTickets
{
  // Добавлено avis.
  partial class EditComponentRXRequestTaskSharedHandlers
  {

    public virtual void FromChanged(lenspec.AutomatedSupportTickets.Shared.EditComponentRXRequestTaskFromChangedEventArgs e)
    {
      if (e.NewValue != null && !lenspec.AutomatedSupportTickets.PublicFunctions.Module.Remote.CheckAutomatedUser(Sungero.Company.Employees.As(e.NewValue)))
      {
        _obj.From = null;
      }
    }
    
    /// <summary>
    /// Изменение значение, тип обращения.
    /// </summary>
    /// <param name="e"></param>
    public virtual void TypeRequestChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        // Отображаем поля в зависимости, от выбранного типа обращения.
        AutomatedSupportTickets.PublicFunctions.EditComponentRXRequestTask.ShowProperties(_obj);
      }
    }
  }
  // Конец добавлено avis.
}