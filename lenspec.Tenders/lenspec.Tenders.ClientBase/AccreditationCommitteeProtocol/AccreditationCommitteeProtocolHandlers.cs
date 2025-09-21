using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.AccreditationCommitteeProtocol;

namespace lenspec.Tenders
{
  partial class AccreditationCommitteeProtocolClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.Department.IsEnabled = false;
      
      var isFillingByDepartmentEmployees = _obj.IsFillingByDepartmentEmployees == true;
      _obj.State.Properties.DepartmentsForAddressees.IsEnabled = isFillingByDepartmentEmployees;
      _obj.State.Properties.DepartmentsForAddressees.IsRequired = isFillingByDepartmentEmployees;
      _obj.State.Properties.AddresseesFilingOption.IsEnabled = isFillingByDepartmentEmployees;
      _obj.State.Properties.AddresseesFilingOption.IsRequired = isFillingByDepartmentEmployees;
      
      _obj.State.Properties.MaterialKinds.IsVisible = _obj.IsProvider == true;
      _obj.State.Properties.WorkKinds.IsVisible = _obj.IsContractor == true;
    }

  }
}