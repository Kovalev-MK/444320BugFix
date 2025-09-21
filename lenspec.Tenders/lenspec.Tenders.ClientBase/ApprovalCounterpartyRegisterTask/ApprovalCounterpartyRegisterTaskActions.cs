using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ApprovalCounterpartyRegisterTask;

namespace lenspec.Tenders.Client
{
  partial class ApprovalCounterpartyRegisterTaskActions
  {
    public override void Resume(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Resume(e);
    }

    public override bool CanResume(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanResume(e);
    }

    public override void Restart(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Restart(e);
    }

    public override bool CanRestart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanRestart(e);
    }

    public override void Start(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Start(e);
    }

    public override bool CanStart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanStart(e);
    }

  }

}