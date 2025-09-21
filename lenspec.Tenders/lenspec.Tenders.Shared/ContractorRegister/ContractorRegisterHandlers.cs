using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ContractorRegister;

namespace lenspec.Tenders
{
  partial class ContractorRegisterSharedHandlers
  {

    public virtual void WorkKindChanged(lenspec.Tenders.Shared.ContractorRegisterWorkKindChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (e.NewValue != null && _obj.WorkGroup == null)
      {
        _obj.WorkGroup = e.NewValue.Group;
      }
      
      Functions.ContractorRegister.FillName(_obj);
    }

    public virtual void WorkGroupChanged(lenspec.Tenders.Shared.ContractorRegisterWorkGroupChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (_obj.WorkKind == null || Equals(e.NewValue, _obj.WorkKind.Group))
        return;
      
      _obj.WorkKind = null;
    }

  }
}