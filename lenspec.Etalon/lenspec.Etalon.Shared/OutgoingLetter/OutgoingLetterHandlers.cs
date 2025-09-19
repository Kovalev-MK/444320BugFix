using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OutgoingLetter;

namespace lenspec.Etalon
{
  partial class OutgoingLetterInResponseToOffDocumentslenspecSharedHandlers
  {

    public virtual void InResponseToOffDocumentslenspecDocumentChanged(lenspec.Etalon.Shared.OutgoingLetterInResponseToOffDocumentslenspecDocumentChangedEventArgs e)
    {
      if (Equals(e.NewValue, null))
        return;
      
      var incomingDocument = Sungero.Docflow.IncomingDocumentBases.As(e.NewValue);
      var customerRequest = avis.CustomerRequests.CustomerRequests.As(e.NewValue);
      
      if (_obj.OutgoingLetter.IsManyAddressees == false &&
          _obj.OutgoingLetter.Correspondent == null)
      {
        if (incomingDocument != null && !Equals(_obj.OutgoingLetter.Correspondent, incomingDocument.Correspondent))
          _obj.OutgoingLetter.Correspondent = incomingDocument.Correspondent;
        
        if (customerRequest != null && !Equals(_obj.OutgoingLetter.Correspondent, customerRequest.Client))
          _obj.OutgoingLetter.Correspondent = customerRequest.Client;
      }
      
      if (_obj.OutgoingLetter.IsManyAddressees == true && !_obj.OutgoingLetter.Addressees.Any())
      {
        var newAddressee = _obj.OutgoingLetter.Addressees.AddNew();
        if (incomingDocument != null)
          newAddressee.Correspondent = incomingDocument.Correspondent;
        
        if (customerRequest != null)
          newAddressee.Correspondent = customerRequest.Client;
      }
      
      if (Equals(_obj.OutgoingLetter.Project, null))
        Sungero.Docflow.PublicFunctions.OfficialDocument.CopyProjects(e.NewValue, _obj.OutgoingLetter);
    }
  }

  partial class OutgoingLetterAddresseesSharedHandlers
  {

    //Добавлено Avis Expert
    public override void AddresseesAddresseeChanged(Sungero.Docflow.Shared.OutgoingDocumentBaseAddresseesAddresseeChangedEventArgs e)
    {
      base.AddresseesAddresseeChanged(e);
      
      var addresseeEmail = Functions.OutgoingLetter.Remote.GetContactEmail(e.NewValue);
      if (!string.IsNullOrEmpty(addresseeEmail))
      {
        _obj.AddresseeEmailavis = addresseeEmail;
      }
      else if (_obj.Correspondent != null)
      {
        _obj.AddresseeEmailavis = _obj.Correspondent.Email;
      }
    }

    public override void AddresseesCorrespondentChanged(Sungero.Docflow.Shared.OutgoingDocumentBaseAddresseesCorrespondentChangedEventArgs e)
    {
      //base.AddresseesCorrespondentChanged(e);
      if (!Equals(e.NewValue, e.OldValue))
      {
        if (_obj.Addressee != null && !Equals(_obj.Addressee.Company, e.NewValue))
          _obj.Addressee = null;
        
        var outgoingLetter = lenspec.Etalon.OutgoingLetters.As(_obj.OutgoingDocumentBase);
        if (outgoingLetter.InResponseToCustomavis != null && (outgoingLetter.InResponseTo != null && !outgoingLetter.Addressees.Any(x => Equals(x.Correspondent, outgoingLetter.InResponseTo.Correspondent)) ||
                                                              outgoingLetter.InResponseToCustRequestavis != null && !outgoingLetter.Addressees.Any(x => Equals(x.Correspondent, Sungero.Parties.Counterparties.As(outgoingLetter.InResponseToCustRequestavis.Client)))))
        {
          outgoingLetter.InResponseToCustomavis = null;
        }
      }
      
      var addresseeEmail = Functions.OutgoingLetter.Remote.GetContactEmail(_obj.Addressee);
      if (!string.IsNullOrEmpty(addresseeEmail))
      {
        _obj.AddresseeEmailavis = addresseeEmail;
      }
      else if (e.NewValue != null)
      {
        _obj.AddresseeEmailavis = e.NewValue.Email;
      }
      
      if (e.NewValue != null)
      {
        _obj.PostalAddressavis = e.NewValue.PostalAddress;
      }
    }
    //конец Добавлено Avis Expert
  }

  partial class OutgoingLetterAddresseesSharedCollectionHandlers
  {

    //Добавлено Avis Expert
    public override void AddresseesDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      //base.AddresseesDeleted(e);
      if (_obj.IsManyAddressees == true && _obj.InResponseToCustomavis != null &&
          (_obj.InResponseTo != null && !_obj.Addressees.Any(x => Equals(x.Correspondent, _obj.InResponseTo.Correspondent)) ||
           _obj.InResponseToCustRequestavis != null && !_obj.Addressees.Any(x => Equals(x.Correspondent, Sungero.Parties.Counterparties.As(_obj.InResponseToCustRequestavis.Client)))))
      {
        _obj.InResponseToCustomavis = null;
      }
      var nextAddressees = _obj.Addressees.Where(x => x.Id > _deleted.Id);
      foreach(var item in nextAddressees)
      {
        item.Number = Convert.ToInt32(item.Number) - 1;
      }
    }

    public override void AddresseesAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      base.AddresseesAdded(e);
      _added.DeliveryMethod = _obj.DeliveryMethod;
      _added.Number = _obj.Addressees.Count;
    }
    //конец Добавлено Avis Expert
  }

  partial class OutgoingLetterSharedHandlers
  {

    //Добавлено Avis Expert
    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      
      if (e.NewValue != null && _obj.OurSignatory != null && !e.NewValue.Equals(_obj.OurSignatory.Department.BusinessUnit))
      {
        _obj.OurSignatory = null;
      }
    }

    public override void PreparedByChanged(Sungero.Docflow.Shared.OfficialDocumentPreparedByChangedEventArgs e)
    {
      base.PreparedByChanged(e);
      
      if (e.NewValue == null)
      {
        _obj.Department = null;
        _obj.BusinessUnit = null;
      }
      else
      {
        _obj.Department = e.NewValue.Department;
        _obj.BusinessUnit = e.NewValue.Department.BusinessUnit;
      }
    }

    public virtual void ManagementContractMKDavisChanged(lenspec.Etalon.Shared.OutgoingLetterManagementContractMKDavisChangedEventArgs e)
    {
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.CorrespondenceRelationName, e.OldValue, e.NewValue);
    }

    public override void InResponseToChanged(Sungero.Docflow.Shared.OutgoingDocumentBaseInResponseToChangedEventArgs e)
    {
      base.InResponseToChanged(e);
      
      if (e.NewValue != null && (_obj.InResponseToCustomavis == null || !Sungero.Docflow.OfficialDocuments.As(e.NewValue).Equals(_obj.InResponseToCustomavis)))
      {
        _obj.InResponseToCustomavis = e.NewValue;
      }
    }

    public virtual void InResponseToCustRequestavisChanged(lenspec.Etalon.Shared.OutgoingLetterInResponseToCustRequestavisChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;

      if (e.NewValue == null)
        return;

      if (_obj.IsManyAddressees == false && !Equals(_obj.Correspondent, e.NewValue.Client))
        _obj.Correspondent = e.NewValue.Client;
      
      if (_obj.IsManyAddressees == true && !_obj.Addressees.Any())
      {
        var newAddressee = _obj.Addressees.AddNew();
        newAddressee.Correspondent = e.NewValue.Client;
      }
      
      Sungero.Docflow.PublicFunctions.OfficialDocument.CopyProjects(e.NewValue, _obj);
    }

    public virtual void InResponseToCustomavisChanged(lenspec.Etalon.Shared.OutgoingLetterInResponseToCustomavisChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (e.NewValue == null)
      {
        _obj.InResponseTo = null;
        _obj.InResponseToCustRequestavis = null;
      }
      else
      {
        if (Sungero.Docflow.IncomingDocumentBases.Is(e.NewValue))
        {
          _obj.InResponseTo = Sungero.Docflow.IncomingDocumentBases.As(e.NewValue);
          _obj.InResponseToCustRequestavis = null;
        }
        if (avis.CustomerRequests.CustomerRequests.Is(e.NewValue))
        {
          _obj.InResponseToCustRequestavis = avis.CustomerRequests.CustomerRequests.As(e.NewValue);
          _obj.InResponseTo = null;
        }
      }
    }

    public virtual void PowerOfAttorneyDatelenspecChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      // Заполняем скрытое поле "должность подписанта для шаблона".
      PublicFunctions.OutgoingLetter.FieldJobTitle(_obj);
    }

    public virtual void RepresentativeByPowerOfAttorneyNumberlenspecChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      // Заполняем скрытое поле "должность подписанта для шаблона".
      PublicFunctions.OutgoingLetter.FieldJobTitle(_obj);
    }

    public override void OurSignatoryChanged(Sungero.Docflow.Shared.OfficialDocumentOurSignatoryChangedEventArgs e)
    {
      base.OurSignatoryChanged(e);
      // Заполняем скрытое поле "должность подписанта для шаблона".
      PublicFunctions.OutgoingLetter.FieldJobTitle(_obj);
    }

    public virtual void ClientContractlenspecChanged(lenspec.Etalon.Shared.OutgoingLetterClientContractlenspecChangedEventArgs e)
    {
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.CorrespondenceRelationName, e.OldValue, e.NewValue);

      if (e.NewValue != null)
      {
        if (e.NewValue.ObjectAnProject != null && e.NewValue.ObjectAnProject.OurCF != null)
        {
          _obj.OurCFlenspec = e.NewValue.ObjectAnProject.OurCF;
        }
        if (_obj.Archiveavis.HasValue && _obj.Archiveavis.Value == true)
        {
          _obj.BusinessUnit = e.NewValue.BusinessUnit;
        }
      }
    }

    public override void AddresseeChanged(Sungero.Docflow.Shared.OutgoingDocumentBaseAddresseeChangedEventArgs e)
    {
      base.AddresseeChanged(e);
      if (_obj.DeliveryMethod != null && (_obj.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) || _obj.DeliveryMethod.Name.Trim().Equals("Электронная почта")))
      {
        var addresseeEmail = Functions.OutgoingLetter.Remote.GetContactEmail(e.NewValue);
        if (!string.IsNullOrEmpty(addresseeEmail))
        {
          _obj.Emaillenspec = addresseeEmail;
        }
        else if (_obj.Correspondent != null)
        {
          _obj.Emaillenspec = _obj.Correspondent.Email;
        }
      }
      else
      {
        _obj.Emaillenspec = null;
      }
    }

    public override void IsManyAddresseesChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      var deliveryMethod = _obj.DeliveryMethod;
      base.IsManyAddresseesChanged(e);
      if (e.NewValue == true)
      {
        _obj.DeliveryMethod = deliveryMethod;
        _obj.Emaillenspec = null;
        _obj.AddressOfRecipientlenspec = null;
      }
    }

    public override void CorrespondentChanged(Sungero.Docflow.Shared.OutgoingDocumentBaseCorrespondentChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.State.Properties.Addressee.IsEnabled = Sungero.Parties.CompanyBases.Is(e.NewValue) || e.NewValue == null;
      if (!_obj.State.Properties.Addressee.IsEnabled ||
          (_obj.Addressee != null && !Equals(_obj.Addressee.Company, e.NewValue)))
        _obj.Addressee = null;
      
      // Очистку поля и коллекции "В ответ на", при изменении контрагента, выполнять только для одноадресного письма.
      if (_obj.IsManyAddressees == false)
      {
        var isManyResponses = _obj.IsManyResponses.GetValueOrDefault();
        if (!isManyResponses && (_obj.InResponseTo != null && !Equals(_obj.InResponseTo.Correspondent, e.NewValue) ||
                                 _obj.InResponseToCustRequestavis != null && _obj.InResponseToCustRequestavis.Client != _obj.Correspondent))
          _obj.InResponseToCustomavis = null;
        
        if (isManyResponses)
        {
          var otherCounterpartyDocuments = _obj.InResponseToOffDocumentslenspec
            .Where(d => d.Document == null ||
                   Sungero.Docflow.IncomingDocumentBases.Is(d.Document) &&
                   !Equals(Sungero.Docflow.IncomingDocumentBases.As(d.Document).Correspondent, e.NewValue) ||
                   avis.CustomerRequests.CustomerRequests.Is(d.Document) &&
                   !Equals(avis.CustomerRequests.CustomerRequests.As(d.Document).Client, e.NewValue))
            .ToList();
          
          foreach (var document in otherCounterpartyDocuments)
            _obj.InResponseToOffDocumentslenspec.Remove(document);
        }
      }
      
      if (_obj.IsManyAddressees == true)
      {
        _obj.Correspondent = Sungero.Parties.PublicFunctions.Counterparty.Remote.GetDistributionListCounterparty();
        _obj.DeliveryMethod = null;
        _obj.SentDate = null;
        _obj.TrackNumber = null;
        _obj.Addressee = null;
      }
      else if (_obj.IsManyAddressees == false)
      {
        _obj.Addressees.Clear();
        if (_obj.Correspondent != null)
        {
          var newAddressee = _obj.Addressees.AddNew();
          newAddressee.Correspondent = _obj.Correspondent;
          newAddressee.Addressee = _obj.Addressee;
          newAddressee.DeliveryMethod = _obj.DeliveryMethod;
          newAddressee.SentDate = _obj.SentDate;
          newAddressee.TrackNumber = _obj.TrackNumber;
          newAddressee.Number = 1;
        }
      }
      
      
      if (_obj.DeliveryMethod != null && (_obj.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) ||
                                          _obj.DeliveryMethod.Name.Trim().Equals("Электронная почта")))
      {
        var addresseeEmail = Functions.OutgoingLetter.Remote.GetContactEmail(_obj.Addressee);
        if (!string.IsNullOrEmpty(addresseeEmail))
          _obj.Emaillenspec = addresseeEmail;
        else if (e.NewValue != null)
          _obj.Emaillenspec =  e.NewValue.Email;
      }
      else
        _obj.Emaillenspec = null;
      
      if (_obj.DeliveryMethod != null && (_obj.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) ||
                                          _obj.DeliveryMethod.Name.Trim().Equals("Электронная почта") ||
                                          _obj.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.ExchangeMethod)))
      {
        _obj.AddressOfRecipientlenspec = null;
      }
      else if (e.NewValue != null)
      {
        // Для любого способа доставки, кроме эл. почты и сервиса обмена, взять Адрес получателя из карточки Корреспондента.
        _obj.AddressOfRecipientlenspec = e.NewValue.PostalAddress;
      }

    }

    public override void DeliveryMethodChanged(Sungero.Docflow.Shared.OfficialDocumentDeliveryMethodChangedEventArgs e)
    {
      //base.DeliveryMethodChanged(e);
      
      // Способ доставки "Электронная почта" создан через веб-клиент.
      if (e.NewValue != null && (e.NewValue.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) ||
                                 e.NewValue.Name.Trim().Equals("Электронная почта")))
      {
        var addresseeEmail = Functions.OutgoingLetter.Remote.GetContactEmail(_obj.Addressee);
        if (!string.IsNullOrEmpty(addresseeEmail))
        {
          _obj.Emaillenspec = addresseeEmail;
        }
        else if (_obj.Correspondent != null)
        {
          _obj.Emaillenspec = _obj.Correspondent.Email;
        }
      }
      else
      {
        _obj.Emaillenspec = null;
      }
      
      if (e.NewValue == null || !e.NewValue.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) &&
          !e.NewValue.Name.Trim().Equals("Электронная почта") &&
          !e.NewValue.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.ExchangeMethod))
      {
        if (_obj.Correspondent != null)
          _obj.AddressOfRecipientlenspec = _obj.Correspondent.PostalAddress;
      }
      else
      {
        _obj.AddressOfRecipientlenspec = null;
      }
      
      if (!Equals(e.NewValue, e.OldValue) && _obj.IsManyAddressees == true)
      {
        foreach(var addresse in _obj.Addressees)
        {
          addresse.DeliveryMethod = e.NewValue;
        }
      }
    }
    //конец Добавлено Avis Expert

  }
}