using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.PersonnelDepartmentResponsibleAssignment;

namespace lenspec.AutomatedSupportTickets
{
  partial class PersonnelDepartmentResponsibleAssignmentClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      if (_obj.BlockUid == "10")
        _obj.State.Properties.Supervisor.IsVisible = false;

      if (_obj.BlockUid == "13")
        _obj.State.Properties.Supervisor.IsVisible = false;
      
      if (_obj.BlockUid == "14")
        _obj.State.Properties.Supervisor.IsVisible = false;
      
      if (_obj.BlockUid == "15")
        _obj.State.Properties.Supervisor.IsVisible = false;
    }
    
    /// <summary>
    /// Показ формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      if (_obj.BlockUid == "10")
        _obj.State.Properties.Supervisor.IsVisible = false;

      if (_obj.BlockUid == "13")
        _obj.State.Properties.Supervisor.IsVisible = false;
      
      if (_obj.BlockUid == "14")
        _obj.State.Properties.Supervisor.IsVisible = false;
      
      if (_obj.BlockUid == "15")
        _obj.State.Properties.Supervisor.IsVisible = false;
    }
  }
}