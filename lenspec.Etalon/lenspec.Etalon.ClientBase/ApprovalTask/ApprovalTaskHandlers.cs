using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalTask;

namespace lenspec.Etalon
{
  partial class ApprovalTaskClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {      
      base.Showing(e);
      
      _obj.State.Properties.DeliveryMethod.IsEnabled = Users.Current.IncludedIn(Roles.Administrators);
      
      if (_obj.Status == Sungero.Docflow.ApprovalTask.Status.Draft && _obj.ApprovalRule != null)
        Functions.ApprovalTask.SetHeadOfTheInitiator(_obj);
      else
        _obj.State.Properties.HeadOfTheInitiatorlenspec.IsVisible = _obj.State.Properties.HeadOfTheInitiatorlenspec.IsRequired = false;
    }
    
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
        _obj.State.Properties.Signatory.IsEnabled = false;
      }
      // Документ типа "Доверенность".
      else if (lenspec.Etalon.PowerOfAttorneys.Is(document))
        _obj.State.Properties.Signatory.IsVisible = false;

      // Документ типа "Исходящее письмо".
      else if (lenspec.Etalon.OutgoingLetters.Is(document))
        _obj.State.Properties.DeliveryMethod.IsEnabled = false;
    }
  }
}