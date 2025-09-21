using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ContractualWork.BudgetItems;

namespace lenspec.ContractualWork
{
  partial class BudgetItemsClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      _obj.State.Properties.Code1C.IsEnabled = false;
      _obj.State.Properties.Codifier.IsEnabled = false;
      _obj.State.Properties.Status.IsEnabled = false;
      _obj.State.Properties.Status.IsEnabled = Users.Current.IncludedIn(Roles.Administrators);
    }

  }
}