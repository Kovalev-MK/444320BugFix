using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.EditComponentRXRequestTask;

namespace lenspec.AutomatedSupportTickets.Client
{
  // Добавлено avis.
  partial class EditComponentRXRequestTaskActions
  {
    /// <summary>
    /// Показать/скрыть поле "От".
    /// </summary>
    /// <param name="e"></param>
    public virtual void From(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.State.Properties.Author.IsVisible)
      {
        _obj.State.Properties.Author.IsVisible = false;
        return;
      }
      
      _obj.State.Properties.Author.IsVisible = true;
    }

    public virtual bool CanFrom(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
  }
  // Конец добавлено avis.
}