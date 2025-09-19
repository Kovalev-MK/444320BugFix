using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ProviderRegister;

namespace lenspec.Tenders
{
  partial class ProviderRegisterSharedHandlers
  {

    public virtual void MaterialChanged(lenspec.Tenders.Shared.ProviderRegisterMaterialChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (e.NewValue != null && _obj.MaterialGroup == null)
      {
        _obj.MaterialGroup = e.NewValue.Group;
      }
      
      Functions.ProviderRegister.FillName(_obj);
    }

    public virtual void MaterialGroupChanged(lenspec.Tenders.Shared.ProviderRegisterMaterialGroupChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (_obj.Material == null || Equals(e.NewValue, _obj.Material.Group))
        return;
      
      _obj.Material = null;
    }

  }
}