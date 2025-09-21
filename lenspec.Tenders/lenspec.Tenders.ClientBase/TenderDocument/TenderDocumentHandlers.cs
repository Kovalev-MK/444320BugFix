using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderDocument;

namespace lenspec.Tenders
{
  partial class TenderDocumentClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      // Проверка вхождения в роль "Администраторы".
      var isAdministrator = Users.Current.IncludedIn(Roles.Administrators);
      e.Params.AddOrUpdate(Constants.TenderDocument.Params.IsAdministrator, isAdministrator);
      
      // Инициализация параметра "GUID вида документа".
      var documentKindGuid = Functions.TenderDocument.GetDocumentKindGuid(_obj.DocumentKind);
      e.Params.AddOrUpdate(Constants.TenderDocument.Params.DocumentKindGuid, documentKindGuid);
      
      // Инициализация параметра "Признак доступности действия "Создать PDF с установкой штампа".
      var isCounterpartyRegistryDecision = Functions.TenderDocument.IsCounterpartyRegistryDecision(documentKindGuid);
      var canCreatePdfWithStamp = Functions.TenderDocument.CanCreatePdfWithStampAvis(isCounterpartyRegistryDecision, isAdministrator);
      e.Params.AddOrUpdate(Constants.TenderDocument.Params.CanCreatePDFWithStamp, canCreatePdfWithStamp);
      
      // Обновление свойств полей карточки.
      Functions.TenderDocument.UpdateFieldsProperties(_obj, documentKindGuid);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var documentKindGuid = Guid.Empty;
      if (!e.Params.TryGetValue(Constants.TenderDocument.Params.DocumentKindGuid, out documentKindGuid))
      {
        documentKindGuid = Functions.TenderDocument.GetDocumentKindGuid(_obj.DocumentKind);
        e.Params.AddOrUpdate(Constants.TenderDocument.Params.DocumentKindGuid, documentKindGuid);
      }
      
      // Обновление свойств полей карточки.
      Functions.TenderDocument.UpdateFieldsProperties(_obj, documentKindGuid);
    }
  }
}