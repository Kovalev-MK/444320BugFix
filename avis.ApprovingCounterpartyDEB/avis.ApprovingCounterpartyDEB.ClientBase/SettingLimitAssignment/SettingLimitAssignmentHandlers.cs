using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.SettingLimitAssignment;

namespace avis.ApprovingCounterpartyDEB
{
  partial class SettingLimitAssignmentClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      var task = ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.As(_obj.Task);
      _obj.State.Properties.Limit.IsVisible = lenspec.Etalon.CompanyBases.Is(task.Counterparty);
    }

  }
}