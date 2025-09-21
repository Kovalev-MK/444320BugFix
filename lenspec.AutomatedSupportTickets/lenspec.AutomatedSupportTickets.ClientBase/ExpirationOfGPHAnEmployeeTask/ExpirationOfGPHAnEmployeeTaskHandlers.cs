using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTask;

namespace lenspec.AutomatedSupportTickets
{
  partial class ExpirationOfGPHAnEmployeeTaskClientHandlers
  {
    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      // Обновляем информацию о сотруднике.
      if (_obj.Employee == null)
      {
        _obj.State.Controls.ControlInfoEmployee.IsVisible = false;
        return;
      }

      _obj.State.Controls.ControlInfoEmployee.IsVisible = true;
      _obj.State.Controls.ControlInfoEmployee.Refresh();
    }
  }
}