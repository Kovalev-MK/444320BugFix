using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalRoleBase;

namespace lenspec.Etalon.Server
{
  partial class ApprovalRoleBaseFunctions
  {

    /// <summary>
    /// Получить сотрудника из роли.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Сотрудник.</returns>
    [Public]
    public override Sungero.Company.IEmployee GetRolePerformer(Sungero.Docflow.IApprovalTask task)
    {
      return base.GetRolePerformer(task);
    }
  }
}