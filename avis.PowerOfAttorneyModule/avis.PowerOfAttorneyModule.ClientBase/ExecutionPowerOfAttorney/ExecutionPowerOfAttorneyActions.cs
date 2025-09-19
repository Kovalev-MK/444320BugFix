using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ExecutionPowerOfAttorney;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class ExecutionPowerOfAttorneyActions
  {


    public override void Abort(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Abort(e);
    }
    
    public override bool CanAbort(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Sungero.Company.Employees.Current.IncludedIn(Roles.Administrators) || Sungero.Company.Employees.Current == _obj.Author;
    }

  }


}