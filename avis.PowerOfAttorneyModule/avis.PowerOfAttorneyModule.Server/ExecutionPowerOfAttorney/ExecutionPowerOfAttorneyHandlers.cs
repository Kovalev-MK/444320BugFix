using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ExecutionPowerOfAttorney;

namespace avis.PowerOfAttorneyModule
{
  partial class ExecutionPowerOfAttorneyServerHandlers
  {

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      if (!_obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault().HasVersions)
      {
        e.AddError(ExecutionPowerOfAttorneys.Resources.ErrorMessageEmptyVersion);
        return;
      }
    }
    
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Author = Sungero.Company.Employees.Current;
    }
    
    //<<Avis-Expert
  }

}