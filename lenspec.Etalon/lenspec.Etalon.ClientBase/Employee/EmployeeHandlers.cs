using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Employee;

namespace lenspec.Etalon
{
  partial class EmployeeClientHandlers
  {

    //Добавлено Avis Expert
    public virtual void ContractValidFromlenspecValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue != null && _obj.ContractValidTilllenspec != null && e.NewValue.Value > _obj.ContractValidTilllenspec.Value)
      {
        e.AddError(_obj.Info.Properties.ContractValidFromlenspec, lenspec.Etalon.Employees.Resources.DateEndContractGreaterStartDateErrorMessage, 
                   new[] { _obj.Info.Properties.ContractValidFromlenspec });
      }
    }

    public virtual void ContractValidTilllenspecValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue != null && _obj.ContractValidFromlenspec != null && e.NewValue.Value < _obj.ContractValidFromlenspec.Value)
      {
        e.AddError(_obj.Info.Properties.ContractValidTilllenspec, lenspec.Etalon.Employees.Resources.DateEndContractGreaterStartDateErrorMessage, 
                   new[] { _obj.Info.Properties.ContractValidTilllenspec });
      }
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      if(_obj.CivilLawContractlenspec == null)
        _obj.CivilLawContractlenspec = false;
      
      _obj.State.Properties.ContractValidFromlenspec.IsVisible =
        _obj.State.Properties.ContractValidFromlenspec.IsEnabled =
          _obj.State.Properties.ContractValidFromlenspec.IsRequired = _obj.CivilLawContractlenspec == true;
      
      _obj.State.Properties.ContractValidTilllenspec.IsVisible =
        _obj.State.Properties.ContractValidTilllenspec.IsEnabled =
          _obj.State.Properties.ContractValidTilllenspec.IsRequired = _obj.CivilLawContractlenspec == true;
    }
    //конец Добавлено Avis Expert
  }


}