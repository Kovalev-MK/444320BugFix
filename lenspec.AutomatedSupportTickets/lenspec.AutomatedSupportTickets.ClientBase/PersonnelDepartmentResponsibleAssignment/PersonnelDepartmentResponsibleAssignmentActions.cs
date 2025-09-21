using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.PersonnelDepartmentResponsibleAssignment;

namespace lenspec.AutomatedSupportTickets.Client
{
  // Добавлено avis.
  
  partial class PersonnelDepartmentResponsibleAssignmentActions
  {
    public virtual void CompleteGPH(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanCompleteGPH(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      if (_obj.BlockUid == "13")
        return true;
      
      return false;
    }

    /// <summary>
    /// Результат "Выполнить".
    /// </summary>
    /// <param name="e"></param>
    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      // Проверяем руководителя только на этапе 7.
      if (_obj.BlockUid == "7" && _obj.Supervisor == null)
      {
        e.AddError(lenspec.AutomatedSupportTickets.PersonnelDepartmentResponsibleAssignments.Resources.NeedFillEmployeeManager);
        return;
      }
      
      var task = lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTasks.As(_obj.MainTask);
      task.Supervisor = _obj.Supervisor;
      task.Save();
      //_obj.Save();
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      if (_obj.BlockUid == "13")
        return false;
      
      return true;
    }
  }
  
  // Конец добавлено avis.
}