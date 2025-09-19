using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PostalItems.PostalItem;

namespace avis.PostalItems
{
  partial class PostalItemAttachmentCollectionSharedCollectionHandlers
  {

    public virtual void AttachmentCollectionDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextItems = _obj.AttachmentCollection.Where(x => x.Number > _deleted.Number);
      foreach(var item in nextItems)
      {
        item.Number = item.Number - 1;
      }
    }

    public virtual void AttachmentCollectionAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = _obj.AttachmentCollection.Count;
    }
  }

  // Добавлено avis.
  partial class PostalItemSharedHandlers
  {
    
    /// <summary>
    /// Изменение значения "Документ основание".
    /// </summary>
    /// <param name="e"></param>
    public virtual void OutgoingLetterChanged(avis.PostalItems.Shared.PostalItemOutgoingLetterChangedEventArgs e)
    {
      // Устанавливаем отправителя.
      if (_obj.OutgoingLetter != null)
      {
        _obj.Sender = _obj.OutgoingLetter.BusinessUnit;
        _obj.Employee = _obj.OutgoingLetter.PreparedBy;
        _obj.Counterparty = _obj.OutgoingLetter.Correspondent;
        _obj.To = _obj.OutgoingLetter.Addressee;
        _obj.MailDeliveryMethod = lenspec.Etalon.MailDeliveryMethods.As(_obj.OutgoingLetter.DeliveryMethod);
        _obj.Address = _obj.OutgoingLetter.AddressOfRecipientlenspec;
        
        //Добавить строку в ТЧ Информация о вложениях с инф. о выбранном документе основании
        var docdFound = _obj.AttachmentCollection.AddNew();
        docdFound.Attachment = _obj.OutgoingLetter.Name;
        docdFound.ValueLetter = 10;
        docdFound.ValuePages = 1;
        
        var requestResult = avis.PostalItems.PublicFunctions.Module.TryGetPostalCode(_obj.Address);
        if (!requestResult.IsSuccess)
          e.Params.AddOrUpdate(Constants.PostalItem.Params.PostalCodeCalculationError, requestResult.Error);
        else if (!string.IsNullOrEmpty(requestResult.Code))
          _obj.Index = requestResult.Code;
        
        return;
      }
      
      _obj.Sender = null;
      _obj.Employee = null;
      _obj.Counterparty = null;
      _obj.To = null;
      _obj.MailDeliveryMethod = null;
      _obj.Address = null;
      _obj.Index = null;
    }
    
    /// <summary>
    /// Изменение значения "Адрессат."
    /// </summary>
    /// <param name="e"></param>
    public virtual void CounterpartyChanged(avis.PostalItems.Shared.PostalItemCounterpartyChangedEventArgs e)
    {
      if (_obj.Counterparty != null)
      {
        // Заполняем поле "адрес".
        _obj.Address = _obj.Counterparty.PostalAddress;
        // Активируем поле "Кому".
        avis.PostalItems.PublicFunctions.PostalItem.EnableTo(_obj);
        
        // Очищаем поле "Кому", если оно отключено.
        if (_obj.State.Properties.To.IsEnabled == false)
          _obj.To = null;
      }
      
      if (e.NewValue == null)
        _obj.Address = "";
      
      if (e.OldValue != e.NewValue)
      {
        _obj.Index = "";
        _obj.To = null;
      }
    }
  }

  // Конец добавлено avis.
}