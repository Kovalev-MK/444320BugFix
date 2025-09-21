using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApprovalSetting;

namespace lenspec.EtalonDatabooks
{
  partial class ApprovalSettingSharedHandlers
  {

    public virtual void ContractCategoriesChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      
    }

    public virtual void GroupKindChanged(lenspec.EtalonDatabooks.Shared.ApprovalSettingGroupKindChangedEventArgs e)
    {
      if (e.NewValue != null && _obj.ContractKind != null && e.NewValue != _obj.ContractKind.GroupContractType)
        _obj.ContractKind = null;
    }

    public virtual void ContractKindChanged(lenspec.EtalonDatabooks.Shared.ApprovalSettingContractKindChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (e.NewValue != e.OldValue)
          _obj.GroupKind = e.NewValue.GroupContractType;
      }
      else
        _obj.GroupKind = null;
    }

    //Добавлено Avis Expert
    public virtual void BusinessUnitChanged(lenspec.EtalonDatabooks.Shared.ApprovalSettingBusinessUnitChangedEventArgs e)
    {
      Functions.ApprovalSetting.FillName(_obj);
    }

    public virtual void ApprovalRuleChanged(lenspec.EtalonDatabooks.Shared.ApprovalSettingApprovalRuleChangedEventArgs e)
    {
      Functions.ApprovalSetting.FillName(_obj);
      
      if (e.NewValue == null || e.NewValue.DocumentFlow == null || e.NewValue.DocumentFlow != Sungero.Docflow.ApprovalRuleBase.DocumentFlow.Contracts)
        _obj.ContractKind = null;
    }
    //конец Добавлено Avis Expert

  }
}