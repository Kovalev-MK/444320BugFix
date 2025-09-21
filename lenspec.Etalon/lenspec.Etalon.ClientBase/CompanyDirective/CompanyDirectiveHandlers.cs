using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.CompanyDirective;

namespace lenspec.Etalon
{
  partial class CompanyDirectiveClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      if (!Users.Current.IncludedIn(Roles.Administrators))
      { _obj.State.Properties.LifeCycleState.IsEnabled = false;
        _obj.State.Properties.RegistrationState.IsEnabled = false;
        _obj.State.Properties.VerificationState.IsEnabled = false;
        _obj.State.Properties.InternalApprovalState.IsEnabled = false;
        _obj.State.Properties.ExternalApprovalState.IsEnabled = false;
        _obj.State.Properties.ExchangeState.IsEnabled = false;
        _obj.State.Properties.ExecutionState.IsEnabled = false;
        _obj.State.Properties.ControlExecutionState.IsEnabled = false;}
    }

  }
}