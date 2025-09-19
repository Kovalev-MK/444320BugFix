using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ProviderRegister;

namespace lenspec.Tenders.Shared
{
  partial class ProviderRegisterFunctions
  {

    /// <summary>
    /// Заполнить имя записи.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var name = string.Empty;
      /* Имя в формате:
        <Товарная группа> для <КА>, <Регион присутствия> от <Дата решения КК>
       */
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.Material != null)
          name += _obj.Material.Name;
        
        if (_obj.Counterparty != null)
          name += lenspec.Tenders.CounterpartyRegisterBases.Resources.ForPartOfName + _obj.Counterparty.Name;
        
        if (_obj.PresenceRegion != null)
          name += lenspec.Tenders.CounterpartyRegisterBases.Resources.DotPartOfName + _obj.PresenceRegion.Name;
        
        if (_obj.QCDecisionDate != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.QCDecisionDate.Value.ToString("d");
      }
      
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      if (name.Length > _obj.Info.Properties.Name.Length)
        _obj.Name = name.Substring(0, _obj.Info.Properties.Name.Length);
      else
        _obj.Name = name;
    }
  }
}