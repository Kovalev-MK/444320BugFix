using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ApprovalCounterpartyRegisterTask;

namespace lenspec.Tenders.Shared
{
  partial class ApprovalCounterpartyRegisterTaskFunctions
  {
    public virtual void FillName()
    {
      using (TenantInfo.Culture.SwitchTo())
        _obj.Subject = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(
          lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Resources.TaskSubject,
          new [] { 
            Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.Procedure.GetLocalizedValue(_obj.Procedure),
            Calendar.Today.ToShortDateString(),
            _obj.Counterparty?.Name 
          });
    }
    
  }
}