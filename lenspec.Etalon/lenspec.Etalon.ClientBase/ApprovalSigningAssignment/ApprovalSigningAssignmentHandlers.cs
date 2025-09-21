using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalSigningAssignment;

namespace lenspec.Etalon
{
  partial class ApprovalSigningAssignmentClientHandlers
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