using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalManagerAssignment;

namespace lenspec.Etalon
{
  partial class ApprovalManagerAssignmentClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document == null)
        return;
      
      // Документ типа "Электронная доверенность".
      if (Sungero.Docflow.FormalizedPowerOfAttorneys.Is(document))
      {
        _obj.State.Attachments.AddendaGroup.IsVisible = false;
        _obj.State.Attachments.OtherGroup.IsVisible = false;
        //TODO: заменить вызов remote-функции
        _obj.State.Properties.Signatory.IsEnabled = Functions.ApprovalManagerAssignment.Remote.CheckIsPossibilityChangingSigner(_obj);
      }
      
      // Документ типа "Доверенность".
      else if (lenspec.Etalon.PowerOfAttorneys.Is(document))
        _obj.State.Properties.Signatory.IsVisible = false;
    }

    //Добавлено Avis Expert
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      e.Instruction = EtalonDatabooks.PublicFunctions.Module.GetAssignmentInstruction(_obj.Stage);
    }
    //конец Добавлено Avis Expert

  }
}