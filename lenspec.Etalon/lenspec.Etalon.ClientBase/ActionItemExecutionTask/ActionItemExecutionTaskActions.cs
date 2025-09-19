using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ActionItemExecutionTask;

namespace lenspec.Etalon.Client
{


  partial class ActionItemExecutionTaskActions
  {
    
    //Добавлено Avis Expert
    public override void AddPerformer(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.AddPerformer(e);
    }

    public override bool CanAddPerformer(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (!Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
        return false;
      
      return base.CanAddPerformer(e);
    }
    //конец Добавлено Avis Expert

    public override void Start(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Start(e);
      
      if (_obj.TaskAssignedlenspec != null)
      {
        _obj.TaskAssignedlenspec.Complete(Sungero.Docflow.ApprovalReviewAssignment.Result.AddActionItem);
      }
    }

    public override bool CanStart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanStart(e);
    }

  }


}