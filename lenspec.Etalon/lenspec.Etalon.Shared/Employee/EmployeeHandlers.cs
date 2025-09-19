using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Employee;

namespace lenspec.Etalon
{
  partial class EmployeeSharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void IsMaternityLeavelenspecChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue == Employee.IsMaternityLeavelenspec.Yes)
      {
        _obj.Status = Employee.Status.Closed;
        _obj.Note = lenspec.Etalon.Employees.Resources.ClosingDueToMaternityLeave + _obj.Note;
      }
    }
    
    /// <summary>
    /// Изменение значения свойства срока дйствия гпх "По".
    /// </summary>
    /// <param name="e"></param>
    public virtual void ContractValidTilllenspecChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      _obj.IsGPHavis = false;
    }

    public virtual void CivilLawContractlenspecChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if(_obj.CivilLawContractlenspec == false)
        _obj.ContractValidFromlenspec = _obj.ContractValidTilllenspec = null;
    }

    public override void DepartmentChanged(Sungero.Company.Shared.EmployeeDepartmentChangedEventArgs e)
    {
      base.DepartmentChanged(e);
      if (e.NewValue != null)
      {
        _obj.BusinessUnitlenspec = _obj.Department.BusinessUnit;
      }
      else
      {
        _obj.BusinessUnitlenspec = null;
      }
    }
    //конец Добавлено Avis Expert
  }
}