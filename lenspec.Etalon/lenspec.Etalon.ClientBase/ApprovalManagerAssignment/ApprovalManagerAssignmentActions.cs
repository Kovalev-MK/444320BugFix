using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalManagerAssignment;

namespace lenspec.Etalon.Client
{
  partial class ApprovalManagerAssignmentActions
  {
    public override void ExtendDeadline(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ExtendDeadline(e);
    }

    public override bool CanExtendDeadline(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      //TODO: заменить вызов remote-функции
      return base.CanExtendDeadline(e) && Functions.ApprovalManagerAssignment.Remote.CheckPossibilityExtending(_obj);
    }

  }

}