using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OfficialDocument;

namespace lenspec.Etalon.Client
{
  internal static class OfficialDocumentStaticActions
  {

    public static bool CanCreateDocumentFromSpecialFolderlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public static void CreateDocumentFromSpecialFolderlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Доступные типы документов...
      var availableDocumentTypeGuids = new List<string>(){
        lenspec.Tenders.PublicConstants.Module.TenderDocumentTypeGuid,
        lenspec.Tenders.PublicConstants.Module.TenderCommitteeProtocolTypeGuid,
        lenspec.Tenders.PublicConstants.Module.TenderQualificationFormTypeGuid,
        lenspec.Etalon.PublicConstants.Docflow.CounterpartyDocument.CouterpartyDocumentTypeGuid
      };
      var availableDocumentKinds = Sungero.Docflow.DocumentKinds.GetAll(x => availableDocumentTypeGuids.Contains(x.DocumentType.DocumentTypeGuid));
      // ... за исключением следующих видов:
      var decisionOnInclusionOfCounterpartyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(lenspec.Tenders.PublicConstants.Module.DecisionOnInclusionOfCounterpartyKind);
      var decisionOnExclusionOfCounterpartyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(lenspec.Tenders.PublicConstants.Module.DecisionOnExclusionOfCounterpartyKind);
      
      availableDocumentKinds = availableDocumentKinds.Where(x =>
                                                            !Equals(x, decisionOnInclusionOfCounterpartyKind) &&
                                                            !Equals(x, decisionOnExclusionOfCounterpartyKind));
      
      var dialog = Dialogs.CreateInputDialog(lenspec.Etalon.OfficialDocuments.Resources.CreateDocument);
      var selectedDocumentKind = dialog.AddSelect(Sungero.Docflow.DocumentKinds.Info.LocalizedName, true, Sungero.Docflow.DocumentKinds.Null).From(availableDocumentKinds);
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        var document = Functions.OfficialDocument.Remote.CreateQualificationDocument(selectedDocumentKind.Value);
        document.ShowModal();
      }
    }
  }


  partial class OfficialDocumentVersionsActions
  {
    public virtual bool CanConvertVersionToPdfavis(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      if (Sungero.Company.Employees.Current.IncludedIn(Roles.Administrators))
        return true;
      
      return false;
    }

    public virtual void ConvertVersionToPdfavis(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var document = OfficialDocuments.As(_obj.RootEntity);
      // Преобразовываем версию в ПДФ.
      var error = lenspec.Etalon.Functions.OfficialDocument.Remote.ConvertToPdfWithSignatureMarkAvis(document, _obj.Id);
      
      if (!string.IsNullOrEmpty(error))
        Dialogs.CreateTaskDialog("Ошибка преобразования", error).Show();
    }
  }

  partial class OfficialDocumentActions
  {

    public virtual void ShowContactavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Открываем карточку на вкладке "Связи".
      _obj.Show();
      _obj.State.Pages.Relations.Activate();
    }

    public virtual bool CanShowContactavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }


    public virtual void UpdateTemplatelenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var currentUser = Users.Current;
      if (_obj.AccessRights.IsGranted(DefaultAccessRightsTypes.FullAccess, currentUser) || _obj.AccessRights.IsGranted(DefaultAccessRightsTypes.Change, currentUser))
      {
        _obj.UpdateTemplateParameters();
        _obj.Save();
      }
    }

    public virtual bool CanUpdateTemplatelenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && !_obj.State.IsChanged && _obj.HasVersions;
    }
    
    public override void BarcodeReport(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var printers = avis.PrinterSettings.PublicFunctions.Module.Remote.GetActivePrinters();
      switch (printers.Count)
      {
          // Если нет настройки для принтера.
        case 0:
          base.BarcodeReport(e);
          break;
          // Если есть 1 настройка для принтера.
        case 1:
          Functions.OfficialDocument.SaveBarcode(_obj, printers.FirstOrDefault().Printer);
          break;
          // Если много настроек для принтера.
        default:
          var dialog = Dialogs.CreateInputDialog("Выберите принтер");
          var printer = dialog.AddSelect("Принтер", true).From(printers.Select(p => p.Printer).ToArray());
          if (dialog.Show() == DialogButtons.Ok)
          {
            Functions.OfficialDocument.SaveBarcode(_obj, printer.Value);
          }
          break;
      }
    }

    public override bool CanBarcodeReport(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanBarcodeReport(e);
    }

    public virtual void MultipleStampsavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // При асинхронном преобразовании не отправлять на обработку повторно.
      int convertingVersionIdParamValue = -1;
      bool needConversion = !e.Params.TryGetValue(Sungero.Docflow.Constants.OfficialDocument.ConvertingVersionId, out convertingVersionIdParamValue) ||
        convertingVersionIdParamValue != _obj.LastVersion.Id;
      
      // Преобразование в PDF.
      lenspec.Etalon.Structures.Docflow.OfficialDocument.СonversionToPdfResultAvis result = null;
      //Sungero.Docflow.Structures.OfficialDocument.СonversionToPdfResult result = null;
      if (needConversion)
      {
        result = lenspec.Etalon.Functions.OfficialDocument.Remote.ConvertToPdfWithSignatureMarkAvis(_obj);
        if (result.IsOnConvertion)
          e.Params.AddOrUpdate(Sungero.Docflow.Constants.OfficialDocument.ConvertingVersionId, _obj.LastVersion.Id);
        
        // Успешная интерактивная конвертация.
        if (!result.HasErrors && result.IsFastConvertion)
        {
          Dialogs.NotifyMessage(OfficialDocuments.Resources.ConvertionDone);
          return;
        }
      }
      
      // Сообщение о статусе асинхронного преобразования.
      var title = string.Empty;
      var message = string.Empty;
      if (needConversion && result.HasErrors)
      {
        // Возникла ошибка.
        title = result.ErrorTitle;
        message = result.ErrorMessage;
      }
      else
      {
        // Преобразование "В процессе".
        title = OfficialDocuments.Resources.ConvertionInProgress;
        message = OfficialDocuments.Resources.CloseDocumentAndOpenLater;
      }
      
      Dialogs.ShowMessage(title, message, MessageType.Information);
    }

    public virtual bool CanMultipleStampsavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && _obj.HasVersions && !_obj.State.IsChanged &&
        _obj.AccessRights.CanUpdate() && Locks.GetLockInfo(_obj).IsLockedByMe;
    }
    
    //конец Добавлено Avis Expert
  }
}