using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApprovalSetting;

namespace lenspec.EtalonDatabooks
{
  partial class ApprovalSettingClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      var isContractFlow = _obj.ApprovalRule != null && _obj.ApprovalRule.DocumentFlow != null && _obj.ApprovalRule.DocumentFlow == Sungero.Docflow.ApprovalRuleBase.DocumentFlow.Contracts;
      _obj.State.Properties.GroupKind.IsVisible = isContractFlow;
      _obj.State.Properties.ContractKind.IsVisible = isContractFlow;
      _obj.State.Properties.ContractCategories.IsVisible = isContractFlow;
    }

  }
}