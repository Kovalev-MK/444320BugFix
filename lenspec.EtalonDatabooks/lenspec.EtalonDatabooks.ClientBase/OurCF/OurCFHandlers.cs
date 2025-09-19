using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.OurCF;

namespace lenspec.EtalonDatabooks
{
  partial class OurCFCollectionCoordinatorsClientHandlers
  {

    public virtual void CollectionCoordinatorsEmployeeValueInput(lenspec.EtalonDatabooks.Client.OurCFCollectionCoordinatorsEmployeeValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
    }
  }


  partial class OurCFClientHandlers
  {

    public virtual void IsComputeApprovalersValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      _obj.State.Properties.CollectionCoordinators.IsEnabled = e.NewValue != true;
    }
    
    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      _obj.State.Properties.Code1C.IsEnabled = Sungero.Company.Employees.Current.IncludedIn(Roles.Administrators);
      _obj.State.Properties.CollectionCoordinators.IsEnabled = _obj.IsComputeApprovalers != true;
    }
  }
}