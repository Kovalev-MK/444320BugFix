using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.IncludeCounterpartyRegistry;

namespace avis.ApprovingCounterpartyDEB.Client
{
  partial class IncludeCounterpartyRegistryActions
  {
    public virtual void Include(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forward != null)
      {
        e.AddError("Выполните задание с результатом Переадресовать или очистите поле Переадресовать");
        return;
      }
    }

    public virtual bool CanInclude(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Forward(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forward == null)
      {
        e.AddError("Укажите исполнителя в поле Переадресовать сотруднику.");
        return;
      }
      if (_obj.Forward == lenspec.Etalon.Employees.Current)
      {
        e.AddError("Нельзя переадресовать самому себе.");
        return;
      }
    }

    public virtual bool CanForward(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}