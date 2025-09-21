using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.FreeApprovalTask;

namespace lenspec.Etalon.Server
{
  partial class FreeApprovalTaskFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Удалить неавтоматизированных сотрудников из значения поля Согласующие.
    /// </summary>
    [Remote]
    public void RemoveNotAutomatedApprovers()
    {
      var approvers = _obj.Approvers.Where(x => Sungero.Company.Employees.Is(x.Approver)).Select(x => Sungero.Company.Employees.As(x.Approver)).ToList();
      if (approvers != null && approvers.Any())
      {
        var notAutomatedApprovers = Sungero.Company.PublicFunctions.Module.Remote.GetNotAutomatedEmployees(approvers).ToList();
        if (notAutomatedApprovers != null && notAutomatedApprovers.Any())
        {
          var approversToRemove = _obj.Approvers.Where(x => notAutomatedApprovers.Contains(x.Approver)).ToList();
          foreach(var approver in approversToRemove)
          {
            _obj.Approvers.Remove(approver);
          }
        }
      }
    }
    //конец Добавлено Avis Expert
  }
}