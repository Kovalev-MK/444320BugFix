using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBase;

namespace avis.ApprovingCounterpartyDEB.Server
{
  partial class ApprovalCounterpartyBaseFunctions
  {

    /// <summary>
    /// Найти последнюю задачу на согласование КА
    /// </summary>
    [Remote(IsPure = true)]
    public IApprovalCounterpartyPersonDEB GetLastTask()
    {
      var task = ApprovalCounterpartyPersonDEBs.Null;
      
      AccessRights.AllowRead(() =>
                             {
                               var tasks = ApprovalCounterpartyPersonDEBs.GetAll(x => x.Counterparty.Equals(_obj.Counterparty))
                                 .Where(x => x.AttachmentDetails.Any(a => a.AttachmentId == _obj.Id));
                               task = tasks.FirstOrDefault(x => x.Id == tasks.Max(t => t.Id));
                             });
      
      return task;
    }

  }
}