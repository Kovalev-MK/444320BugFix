using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.CourierShipments.CourierShipmentsApplication;

namespace lenspec.CourierShipments
{
  partial class CourierShipmentsApplicationAttachmentsSharedCollectionHandlers
  {

    //Добавлено Avis Expert
    public virtual void AttachmentsDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextAttachments = _obj.Attachments.Where(x => x.Id > _deleted.Id);
      foreach(var attachment in nextAttachments)
      {
        attachment.Number = attachment.Number - 1;
      }
    }

    public virtual void AttachmentsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = _obj.Attachments.Count;
    }
    //конец Добавлено Avis Expert
  }

  partial class CourierShipmentsApplicationSharedHandlers
  {

    //Добавлено Avis Expert
    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      
      if (!Equals(e.NewValue, e.OldValue))
      {
        _obj.Department = null;
      }
    }

    public override void CreatedChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.CreatedChanged(e);
      _obj.CreationDate = e.NewValue;
    }

    public virtual void ContactChanged(lenspec.CourierShipments.Shared.CourierShipmentsApplicationContactChangedEventArgs e)
    {
      FillName();
      
      if (e.NewValue != null)
      {
        _obj.RecipientPhoneNumber = e.NewValue.Phone;
        return;
      }
      
      if (e.NewValue == null && Sungero.Parties.Companies.Is(_obj.Recipient))
      {
        _obj.RecipientPhoneNumber = null;
      }
    }

    public virtual void InitiatorChanged(lenspec.CourierShipments.Shared.CourierShipmentsApplicationInitiatorChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.BusinessUnit = e.NewValue.Department.BusinessUnit;
        _obj.Department = e.NewValue.Department;
      }
      else
      {
        _obj.Department = null;
        _obj.BusinessUnit = null;
      }
    }

    public override void AuthorChanged(Sungero.Content.Shared.ElectronicDocumentAuthorChangedEventArgs e)
    {
      base.AuthorChanged(e);
      
      if (e.NewValue != null)
      {
        _obj.Initiator = Sungero.Company.Employees.As(e.NewValue);
        _obj.Sender = _obj.Initiator;
      }
    }

    public virtual void RecipientChanged(lenspec.CourierShipments.Shared.CourierShipmentsApplicationRecipientChangedEventArgs e)
    {
      FillName();
      
      if (e.NewValue != null)
      {
        var person = Sungero.Parties.People.As(e.NewValue);
        if (person != null)
        {
          _obj.RecipientPhoneNumber = person.Phones;
        }
      }
      else
      {
        _obj.RecipientPhoneNumber = null;
        _obj.Contact = null;
      }
    }

    public virtual void SenderChanged(lenspec.CourierShipments.Shared.CourierShipmentsApplicationSenderChangedEventArgs e)
    {
      FillName();
      
      if (e.NewValue != null)
      {
        var person = e.NewValue.Person;
        _obj.SenderPhoneNumber = person.Phones;
      }
      else
      {
        _obj.SenderPhoneNumber = null;
      }
    }
    //конец Добавлено Avis Expert

  }
}