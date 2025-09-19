using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.CourierShipments.CourierShipmentsApplication;

namespace lenspec.CourierShipments.Shared
{
  partial class CourierShipmentsApplicationFunctions
  {
    //Добавлено Avis Expert
    
    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      _obj.State.Properties.Subject.IsRequired = false;
    }
    
    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var documentKind = _obj.DocumentKind;
      
      if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      
      if (documentKind == null || !documentKind.GenerateDocumentName.Value)
        return;
      
      var name = string.Empty;
      
      /* Имя в формате:
        <Вид документа> №<Рег. номер> от <Отправитель> на <ФИО получателя>.
       */
      using (TenantInfo.Culture.SwitchTo())
      {
        if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
        
        if (_obj.Sender != null)
        {
          name += lenspec.CourierShipments.CourierShipmentsApplications.Resources.CourierShipmentFrom + _obj.Sender.Name;
        }
        
        if (_obj.Recipient != null)
        {
          if (Sungero.Parties.Companies.Is(_obj.Recipient) && _obj.Contact != null)
          {
            name += lenspec.CourierShipments.CourierShipmentsApplications.Resources.CourierShipmentTo + _obj.Contact.Name;
          }
          else
          {
            name += lenspec.CourierShipments.CourierShipmentsApplications.Resources.CourierShipmentTo + _obj.Recipient.Name;
          }
        }
      }
      
      if (string.IsNullOrWhiteSpace(name))
      {
        if (_obj.VerificationState == null)
          name = Sungero.Docflow.Resources.DocumentNameAutotext;
        else
          name = _obj.DocumentKind.ShortName;
      }
      else if (documentKind != null)
      {
        name = documentKind.ShortName + name;
      }
      
      _obj.Name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
    }
    //конец Добавлено Avis Expert
  }
}