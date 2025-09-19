using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalCheckingAssignment;

namespace lenspec.Etalon
{
  partial class ApprovalCheckingAssignmentClientHandlers
  {

    //Добавлено Avis Expert
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
    }

    public virtual void DeliveryMethodavisValueInput(lenspec.Etalon.Client.ApprovalCheckingAssignmentDeliveryMethodavisValueInputEventArgs e)
    {
      if (e.NewValue == null)
      {
        e.AddError(lenspec.Etalon.ApprovalCheckingAssignments.Resources.NeedFillDelivaryMethod);
      }
    }
    
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      if (Functions.ApprovalCheckingAssignment.NeedViewInstruction(_obj))
      {
        var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
        if (CourierShipments.CourierShipmentsApplications.Is(document))
        {
          e.Instruction = CourierShipments.CourierShipmentsApplications.Resources.Instruction ?? string.Empty;
        }
      }
      
      var deliveryMethodIsEditable = Functions.ApprovalCheckingAssignment.Remote.GetDeliveryMethodIsEditable(_obj);
      _obj.State.Properties.DeliveryMethodavis.IsVisible = deliveryMethodIsEditable;
      e.Params.AddOrUpdate("deliveryMethodIsEditable", deliveryMethodIsEditable);
    }
    //конец Добавлено Avis Expert

  }
}