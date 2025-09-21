using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApprovalSetting;

namespace lenspec.EtalonDatabooks.Shared
{
  partial class ApprovalSettingFunctions
  {

    /// <summary>
    /// Заполнить имя документа.
    /// </summary>       
    public void FillName()
    {
      // Имя в формате:
      // <Наименование нашей организации> - <Наименование регламента>
      var name = string.Empty;
      
      if (_obj.BusinessUnit != null)
      {
        name = _obj.BusinessUnit.Name;
        if (_obj.ApprovalRule != null)
          name += " - ";
      }
      
      if (_obj.ApprovalRule != null)
        name += _obj.ApprovalRule.Name;
      
      _obj.Name = name;
    }

  }
}