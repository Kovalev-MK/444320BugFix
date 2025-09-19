using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.CoordinationHeadDepartmentOfInitiator;

namespace lenspec.AutomatedSupportTickets.Client
{
  partial class CoordinationHeadDepartmentOfInitiatorActions
  {
    
    // Добавлено avis.
    public virtual void Forward(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Task.GetStartedSchemeVersion() > LayerSchemeVersions.V1 && _obj.SelectedPerformer == null)
        e.AddError(lenspec.AutomatedSupportTickets.CoordinationHeadDepartmentOfInitiators.Resources.ErrorMessageByForward);
    }

    public virtual bool CanForward(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }
    
    /// <summary>
    /// Выполнение "Договор продлить".
    /// </summary>
    /// <param name="e"></param>
    public virtual void Extend(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanExtend(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void NotExtend(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanNotExtend(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }
    
    // Конец добавлено avis.
  }
}