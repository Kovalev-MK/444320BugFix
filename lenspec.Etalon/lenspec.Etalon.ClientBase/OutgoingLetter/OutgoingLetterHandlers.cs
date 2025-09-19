using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OutgoingLetter;

namespace lenspec.Etalon
{
  partial class OutgoingLetterClientHandlers
  {

    //Добавлено Avis Expert
    public override void PreparedByValueInput(Sungero.Docflow.Client.OfficialDocumentPreparedByValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.PreparedByValueInput(e);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Properties.DeliveryMethod.IsRequired = _obj.State.Properties.DeliveryMethod.IsEnabled = true;
      
      _obj.State.Properties.Emaillenspec.IsVisible = _obj.DeliveryMethod != null &&
        (_obj.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) ||
         _obj.DeliveryMethod.Name.Trim().Equals("Электронная почта"));
      _obj.State.Properties.Emaillenspec.IsRequired = _obj.State.Properties.Emaillenspec.IsEnabled =
        _obj.State.Properties.Emaillenspec.IsVisible && _obj.IsManyAddressees != true;
      
      _obj.State.Properties.AddressOfRecipientlenspec.IsVisible = _obj.DeliveryMethod == null ||
        !_obj.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) &&
        !_obj.DeliveryMethod.Name.Trim().Equals("Электронная почта") &&
        !_obj.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.ExchangeMethod);
      _obj.State.Properties.AddressOfRecipientlenspec.IsRequired = _obj.State.Properties.AddressOfRecipientlenspec.IsEnabled =
        _obj.State.Properties.AddressOfRecipientlenspec.IsVisible && _obj.IsManyAddressees != true;

      //_obj.State.Properties.Department.IsEnabled = _obj.State.Properties.BusinessUnit.IsEnabled = false;
      
      var addresseesIsEnabled = false;
      // Делопроизводители всегда могут редактировать список Адресатов
      if (Users.Current.IncludedIn(Sungero.Docflow.Constants.Module.RoleGuid.ClerksRole))
      {
        addresseesIsEnabled = true;
      }
      else
      {
        addresseesIsEnabled = Users.Current.Equals(_obj.Author);
      }
      _obj.State.Properties.Addressees.IsEnabled = addresseesIsEnabled;
      var addresseesProperties = _obj.State.Properties.Addressees.Properties;
      foreach(var property in addresseesProperties)
      {
        property.IsEnabled = addresseesIsEnabled;
      }
      
      _obj.State.Properties.InResponseTo.IsVisible = false;
      _obj.State.Properties.InResponseToDocuments.IsVisible = false;
      var isManyResponses = _obj.IsManyResponses.GetValueOrDefault();
      _obj.State.Properties.InResponseToCustomavis.IsVisible = !isManyResponses;
      _obj.State.Properties.InResponseToOffDocumentslenspec.IsVisible = isManyResponses;
    }
    //конец Добавлено Avis Expert

  }
}