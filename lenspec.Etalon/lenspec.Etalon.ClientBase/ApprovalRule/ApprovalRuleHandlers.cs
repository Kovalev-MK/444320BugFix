using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalRule;

namespace lenspec.Etalon
{
  partial class ApprovalRuleClientHandlers
  {

    //Добавлено Avis Expert
    public override void ReworkPerformerValueInput(Sungero.Docflow.Client.ApprovalRuleBaseReworkPerformerValueInputEventArgs e)
    {
      if (e.NewValue != null && Sungero.Company.Employees.Is(e.NewValue) && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(Sungero.Company.Employees.As(e.NewValue)))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.ReworkPerformerValueInput(e);
    }
    //конец Добавлено Avis Expert

  }
}