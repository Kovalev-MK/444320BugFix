using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalSimpleAssignment;

namespace lenspec.Etalon
{
  partial class ApprovalSimpleAssignmentClientHandlers
  {

    //Добавлено Avis Expert
    public virtual void DeliveryMethodavisValueInput(lenspec.Etalon.Client.ApprovalSimpleAssignmentDeliveryMethodavisValueInputEventArgs e)
    {
      if (e.NewValue == null)
      {
        e.AddError(lenspec.Etalon.ApprovalSimpleAssignments.Resources.NeedFillDeliveryMethod);
      }
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var outgoingLetter = Sungero.RecordManagement.OutgoingLetters.As(_obj.DocumentGroup.OfficialDocuments.FirstOrDefault());
      _obj.State.Properties.DeliveryMethodavis.IsVisible = outgoingLetter != null;
      
      bool deliveryMethodIsEditable;
      if (e.Params.TryGetValue("deliveryMethodIsEditable", out deliveryMethodIsEditable))
      {
        _obj.State.Properties.DeliveryMethodavis.IsEnabled = _obj.State.Properties.DeliveryMethodavis.IsVisible && deliveryMethodIsEditable;
      }
      
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {        
        // Если согл. документ - Электронная доверенность, скрыты группы вложений приложения и дополнительно
        _obj.State.Attachments.AddendaGroup.IsVisible = !Sungero.Docflow.FormalizedPowerOfAttorneys.Is(document);
        _obj.State.Attachments.OtherGroup.IsVisible = !Sungero.Docflow.FormalizedPowerOfAttorneys.Is(document);
      }
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      // Скрывать контрол состояния со сводкой, если сводка пустая.
      _obj.State.Controls.DocumentSummary.IsVisible = Functions.ApprovalSimpleAssignment.NeedViewDocumentSummary(_obj);
      
      if (Functions.ApprovalSimpleAssignment.NeedViewInstruction(_obj))
      {
        var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
        if (CourierShipments.CourierShipmentsApplications.Is(document))
        {
          e.Instruction = CourierShipments.CourierShipmentsApplications.Resources.Instruction ?? string.Empty;
        }
      }
      
      var deliveryMethodIsEditable = Functions.ApprovalSimpleAssignment.Remote.GetDeliveryMethodIsEditable(_obj);
      _obj.State.Properties.DeliveryMethodavis.IsVisible = deliveryMethodIsEditable;
      e.Params.AddOrUpdate("deliveryMethodIsEditable", deliveryMethodIsEditable);
    }
    //конец Добавлено Avis Expert

  }
}