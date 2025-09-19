using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalSendingAssignment;

namespace lenspec.Etalon
{
  partial class ApprovalSendingAssignmentClientHandlers
  {

    //Добавлено Avis Expert
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      e.Instruction = EtalonDatabooks.PublicFunctions.Module.GetAssignmentInstruction(_obj.Stage);
    }
    //конец Добавлено Avis Expert

  }
}