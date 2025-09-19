using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.SettingLimitAssignment;

namespace avis.ApprovingCounterpartyDEB.Client
{
  partial class SettingLimitAssignmentActions
  {
    public virtual void Forward(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forward == null)
      {
        e.AddError("Укажите исполнителя в поле «Переадресовать сотруднику».");
        return;
      }
      if (_obj.Forward == Sungero.Company.Employees.Current)
      {
        e.AddError("Нельзя переадресовать самому себе.");
        return;
      }
    }

    public virtual bool CanForward(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Set(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (!_obj.Limit.HasValue || _obj.Limit == default(double))
      {
        e.AddError("Для выполнения задания заполните поле «Лимит на контрагента (руб.)»");
        return;
      }
    }

    public virtual bool CanSet(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }


}