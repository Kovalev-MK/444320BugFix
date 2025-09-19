using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractCategory;

namespace lenspec.Etalon.Client
{
  partial class ContractCategoryFunctions
  {
    [Public]
    public void CheckSumRequired()
    {
      if (_obj.CheckSumavis != null && _obj.CheckSumavis == lenspec.Etalon.ContractCategory.CheckSumavis.Yes)
      {
        // Если да. То делаем свойство доступным.
        _obj.State.Properties.Sumavis.IsRequired = true;
        _obj.State.Properties.Sumavis.IsEnabled = true;
      }
      else
      {
        _obj.State.Properties.Sumavis.IsRequired = false;
        _obj.State.Properties.Sumavis.IsEnabled = false;
        
        if (_obj.Sumavis != null)
          _obj.Sumavis = null;
      }
    }
  }
}