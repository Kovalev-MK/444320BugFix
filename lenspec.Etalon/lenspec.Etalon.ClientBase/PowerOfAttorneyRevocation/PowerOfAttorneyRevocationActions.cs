using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorneyRevocation;

namespace lenspec.Etalon.Client
{
  partial class PowerOfAttorneyRevocationActions
  {
    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверка на дубликаты задач
      if (Functions.ApprovalTask.CheckDuplicates(_obj, false))
        return;
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

  }

}