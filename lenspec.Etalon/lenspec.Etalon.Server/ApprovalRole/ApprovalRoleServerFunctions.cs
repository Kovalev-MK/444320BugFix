using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalRole;

namespace lenspec.Etalon.Server
{
  partial class ApprovalRoleFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Получить сотрудника из роли.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Сотрудник.</returns>
    [Public]
    public override Sungero.Company.IEmployee GetRolePerformer(Sungero.Docflow.IApprovalTask task)
    {      
      if (_obj.Type == Sungero.Docflow.ApprovalRoleBase.Type.InitManager)
        return EtalonDatabooks.PublicFunctions.ComputedRole.GetManagerTaskCardRolePerformer(task);
      
      return base.GetRolePerformer(task);
    }
    //конец Добавлено Avis Expert
  }
}