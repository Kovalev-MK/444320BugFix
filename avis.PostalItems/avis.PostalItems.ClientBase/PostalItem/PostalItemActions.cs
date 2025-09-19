using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PostalItems.PostalItem;

namespace avis.PostalItems.Client
{
  // Добавлено avis.
  
  partial class PostalItemActions
  {
    public virtual void SearchDocuments(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Список документов для отображения.
      //var documents = new List<Sungero.Docflow.IOfficialDocument>();
      
      // Проверяем что карточка почтового отправления сохранена.
      if (_obj.State.IsChanged == true)
      {
        e.AddError("Сохраните карточку");
        return;
      }
      
      var documents = avis.PostalItems.LetterComponentDocuments.GetAll(l => l.PostalItem.Equals(_obj))
        .Select(l => Sungero.Docflow.OfficialDocuments.As(l))
        .ToList();
      
      if (_obj.OutgoingLetter?.ClientContractlenspec != null)
      {
        var clientContract = _obj.OutgoingLetter.ClientContractlenspec;
        documents.Add(Sungero.Docflow.OfficialDocuments.As(clientContract));
        var clientDocuments = lenspec.SalesDepartmentArchive.SDAClientDocuments.GetAll(x => x.ClientContract.Equals(clientContract))
          .Select(l => Sungero.Docflow.OfficialDocuments.As(l))
          .ToList();
        documents.AddRange(clientDocuments);
        
        if (clientContract.HasRelations)
        {
          var addendum = clientContract.Relations.GetRelatedDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName)
            .Select(x => Sungero.Docflow.OfficialDocuments.As(x))
            .Where(x => x != null && !documents.Contains(x))
            .ToList();
          documents.AddRange(addendum);
        }
      }
      
      documents.Show();
    }

    public virtual bool CanSearchDocuments(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void CreateLetterComponent(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверяем что карточка почтового отправления сохранена.
      if (_obj.State.IsChanged == true)
      {
        e.AddError("Для создания компонентов письма сохраните карточку");
        return;
      }
      
      // Создаём новый компонент письма.
      var documentKinds = Sungero.Docflow.DocumentKinds.GetAll(d => d.DocumentType.DocumentTypeGuid == avis.PostalItems.Constants.Module.LetterComponentDocumentGuid.ToString());
      
      var dialog = Dialogs.CreateInputDialog("Новый компонент письма");
      var documentKind = dialog.AddSelect("Вид документа", true, Sungero.Docflow.DocumentKinds.Null).From(documentKinds);

      if (dialog.Show() == DialogButtons.Ok)
      {
        var newLetterComponent = LetterComponentDocuments.Create();
        // Задаём почтовое отправление.
        newLetterComponent.PostalItem = _obj;
        // Задаём вид документа.
        newLetterComponent.DocumentKind = documentKind.Value;
        // Задаём Наша организация.
        newLetterComponent.BusinessUnit = _obj.Sender;
        // Задаём Контрагента.
        //if (Sungero.Parties.Companies.Is(_obj.Counterparty))
        newLetterComponent.Counterparty = _obj.Counterparty;
        // Задаём клиентский договор.
        if (_obj.OutgoingLetter != null)
          newLetterComponent.ClientContract = lenspec.Etalon.OutgoingLetters.As(_obj.OutgoingLetter).ClientContractlenspec;
        
        newLetterComponent.Show();
      }
    }

    public virtual bool CanCreateLetterComponent(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    /// <summary>
    /// Действие "Заполнить индекс".
    /// </summary>
    /// <param name="e"></param>
    public virtual void FillIndex(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Получаем индекс и помещаем в почтовое отправление.
      var requestResult = avis.PostalItems.PublicFunctions.Module.TryGetPostalCode(_obj.Address);
      if (!requestResult.IsSuccess)
        e.Params.AddOrUpdate(Constants.PostalItem.Params.PostalCodeCalculationError, requestResult.Error);
      else if (!string.IsNullOrEmpty(requestResult.Code))
        _obj.Index = requestResult.Code;
    }

    public virtual bool CanFillIndex(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void FromScanner(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanFromScanner(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    /// <summary>
    /// Действие "Уведомление".
    /// </summary>
    /// <param name="e"></param>
    public virtual void Notification(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверяем что карточка почтового отправления сохранена.
      if (_obj.State.IsChanged == true)
      {
        e.AddError("Для создания уведомления сохраните карточку");
        return;
      }
      
      if (_obj.MailDeliveryMethod.NotificationTypeavis == null)
      {
        e.AddError("Для данного способа доставки Уведомление не требуется");
        return;
      }
      
      // Создаём уведомление.
      var report = avis.PostalItems.Reports.GetNotificationReport();
      report.Id = _obj.Id;
      report.Open();
    }

    public virtual bool CanNotification(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    /// <summary>
    /// Кнопка "Опись".
    /// </summary>
    /// <param name="e"></param>
    public virtual void Inventory(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверяем что карточка почтового отправления сохранена.
      if (_obj.State.IsChanged == true)
      {
        e.AddError("Для создания описи сохраните карточку");
        return;
      }
      
      if (_obj.AttachmentCollection.Count == 0)
      {
        e.AddError("Для создания описи добавьте информацию о вложение");
        return;
      }
      
      // Создаём опись.
      var report = avis.PostalItems.Reports.GetInventoryReport();
      report.Id = _obj.Id;
      report.Open();
    }

    public virtual bool CanInventory(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void Envelope(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверяем что карточка почтового отправления сохранена.
      if (_obj.State.IsChanged == true)
      {
        e.AddError("Для создания конверта сохраните карточку");
        return;
      }
      
      // Создаём конверт.
      var dialog = Dialogs.CreateTaskDialog("Выберите формат.");
      
      var buttonC4 = dialog.Buttons.AddCustom("А4 [297x210]");
      var buttonC5 = dialog.Buttons.AddCustom("A5 [229x162]");
      
      // Отобразить диалог.
      var result = dialog.Show();
      
      if (result == buttonC4)
      {
        var report = avis.PostalItems.Reports.GetEnvelopeC4Report();
        report.Id = _obj.Id;
        report.Open();
      }
      
      if (result == buttonC5)
      {
        var report = avis.PostalItems.Reports.GetEnvelopeC5Report();
        report.Id = _obj.Id;
        report.Open();
      }
    }

    public virtual bool CanEnvelope(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
  }
  
  // Конец добавлено avis.
}