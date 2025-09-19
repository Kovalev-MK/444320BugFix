using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Department;

namespace lenspec.Etalon
{
  partial class DepartmentClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      var roleRightChangeCheckBoxBudget = Roles.GetAll(x => x.Sid == avis.EtalonContracts.PublicConstants.Module.FillingBudgetFormed).FirstOrDefault();
      _obj.State.Properties.IsBudgetFormedlenspec.IsEnabled = Sungero.CoreEntities.Users.Current.IncludedIn(roleRightChangeCheckBoxBudget);
    }

  }
}