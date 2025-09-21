using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.CounterpartyRegisterBase;

namespace lenspec.Tenders
{
  partial class CounterpartyRegisterBaseClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      // Карточка реестра открыта из карточки задания.
      if (CallContext.CalledFrom(Tenders.ProcessingOfApprovalResultsAssignments.Info))
      {
        var message = string.Empty;
        e.Params.TryGetValue(Constants.Module.Params.MessageToCPRegisterFromProcessingAssignment, out message);
        if (!string.IsNullOrEmpty(message))
          e.AddInformation(message);
      }
    }

    public override void Closing(Sungero.Presentation.FormClosingEventArgs e)
    {
    }

  }
}