using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.CourierShipments.CourierShipmentsApplication;

namespace lenspec.CourierShipments
{
  partial class CourierShipmentsApplicationClientHandlers
  {

    //Добавлено Avis Expert
    public virtual void SenderValueInput(lenspec.CourierShipments.Client.CourierShipmentsApplicationSenderValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
    }

    public virtual void RecipientPhoneNumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.NewValue))
      {
        var pattern = @"\D";
        var target = string.Empty;
        var regex = new System.Text.RegularExpressions.Regex(pattern);
        var result = regex.Replace(e.NewValue, target);
        
        if (result.Length < 11)
        {
          e.AddError(lenspec.CourierShipments.CourierShipmentsApplications.Resources.PhoneNumberNotValid, _obj.Info.Properties.SenderPhoneNumber);
        }
      }
    }

    public virtual void SenderPhoneNumberValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.NewValue))
      {
        var pattern = @"\D";
        var target = string.Empty;
        var regex = new System.Text.RegularExpressions.Regex(pattern);
        var result = regex.Replace(e.NewValue, target);
        
        if (result.Length < 11)
        {
          e.AddError(lenspec.CourierShipments.CourierShipmentsApplications.Resources.PhoneNumberNotValid, _obj.Info.Properties.SenderPhoneNumber);
        }
      }
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var needContact = _obj.Recipient != null && Sungero.Parties.Companies.Is(_obj.Recipient);
      _obj.State.Properties.Contact.IsVisible = _obj.State.Properties.Contact.IsRequired = needContact;
    }
    //конец Добавлено Avis Expert

  }
}