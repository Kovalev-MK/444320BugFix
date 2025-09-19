using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalStage;

namespace lenspec.Etalon
{
  partial class ApprovalStageSharedHandlers
  {

    //Добавлено Avis Expert
    public override void ApprovalRoleChanged(Sungero.Docflow.Shared.ApprovalStageApprovalRoleChangedEventArgs e)
    {
      base.ApprovalRoleChanged(e);
      if (e.NewValue == null || e.NewValue.Type != EtalonDatabooks.ComputedRole.Type.ApprovRoleKind)
      {
        _obj.RoleKindlenspec = null;
      }
    }
    
    public override void SequenceChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      base.SequenceChanged(e);
      if (e.NewValue != Sungero.Docflow.ApprovalStage.Sequence.Parallel)
        _obj.WithInterruptionlenspec = false;
    }
    //конец Добавлено Avis Expert
  }

}