using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDARequestSubmissionToArchive;

namespace lenspec.SalesDepartmentArchive.Client
{
  partial class SDARequestSubmissionToArchiveActions
  {
    public virtual void ShowCreatedDocuments(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var clientDocumentBaseIds = new List<long>();
      var clientDocumentIds = _obj.ListOfDocument.Where(x => !string.IsNullOrEmpty(x.DocumentId)).Select(x => long.Parse(x.DocumentId)).ToList();
      clientDocumentBaseIds.AddRange(clientDocumentIds);
      var clientContractIds = _obj.ListOfDocument.Where(x => x.ClientContract != null).Select(x => x.ClientContract.Id).ToList();
      clientDocumentBaseIds.AddRange(clientContractIds);
      
      var clientDocuments = SalesDepartmentArchive.SDAClientDocumentBases.GetAll(x => clientDocumentBaseIds.Contains(x.Id));
      clientDocuments.Show();
    }

    public virtual bool CanShowCreatedDocuments(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }


    public virtual void BarcodesForListOfDocument(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Получаем список принтеров у данного пользователя.
      var printers = avis.PrinterSettings.PublicFunctions.Module.Remote.GetActivePrinters();
      if (printers.Count == 0)
      {
        Dialogs.ShowMessage(lenspec.SalesDepartmentArchive.SDARequestSubmissionToArchives.Resources.PrinterSettingsNotFound, MessageType.Warning);
        return;
      }
      var printerName = string.Empty;
      if (printers.Count == 1)
        printerName = printers.FirstOrDefault().Printer;
      else
      {
        var dialog = Dialogs.CreateInputDialog("Выберите принтер");
        var printer = dialog.AddSelect("Принтер", true).From(printers.Select(p => p.Printer).ToArray());
        if (dialog.Show() == DialogButtons.Ok)
        {
          printerName = printer.Value;
        }
      }
      
      var clientContractDocumentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.SDAClientContractKind);
      var clientDocumentsToCreate = _obj.ListOfDocument.Where(x => string.IsNullOrEmpty(x.DocumentId));
      if (clientDocumentsToCreate.Any())
      {
        _obj.Save();
        foreach (var document in clientDocumentsToCreate)
        {
          if (document.RealEstateDocumentKind.DocumentKind.Equals(clientContractDocumentKind))
          {
            document.DocumentId = document.ClientContract.Id.ToString();
            if (_obj.StorageAddress != document.ClientContract.StorageAddress && _obj.StorageAddress != null)
            {
              var asyncHandler = SalesDepartmentArchive.AsyncHandlers.AsyncUpdateClientContract.Create();
              asyncHandler.ClientContractId = document.ClientContract.Id;
              asyncHandler.StorageAddressId = _obj.StorageAddress.Id;
              asyncHandler.ExecuteAsync();
            }
          }
          else
          {
            var clientDocument = Functions.SDARequestSubmissionToArchive.Remote.CreateClientDocument(_obj,
                                                                                                     document.RealEstateDocumentKind.DocumentKind,
                                                                                                     document.ClientContract,
                                                                                                     _obj.Author,
                                                                                                     _obj.BusinessUnit,
                                                                                                     document.RealEstateDocumentKind);
            document.DocumentId = clientDocument.Id.ToString();
            // Для clientDocument вызвать отчет Штрихкод документа
            if (document.Stitched != SalesDepartmentArchive.SDARequestSubmissionToArchiveListOfDocument.Stitched.Yes)
            {
              lenspec.Etalon.PublicFunctions.OfficialDocument.SaveBarcode(clientDocument, printerName);
            }
          }
        }
        _obj.State.Properties.Subject.IsRequired = false;
        _obj.Save();
      }
      // Если не нужно создавать клиентские документы, то только печать ШК для них.
      else
      {
        foreach (var document in _obj.ListOfDocument)
        {
          if (!document.RealEstateDocumentKind.DocumentKind.Equals(clientContractDocumentKind))
          {
            var clientDocument = SalesDepartmentArchive.SDAClientDocuments.Get(long.Parse(document.DocumentId));
            // Для clientDocument вызвать отчет Штрихкод документа
            if (document.Stitched != SalesDepartmentArchive.SDARequestSubmissionToArchiveListOfDocument.Stitched.Yes)
            {
              lenspec.Etalon.PublicFunctions.OfficialDocument.SaveBarcode(clientDocument, printerName);
            }
          }
        }
      }
    }

    public virtual bool CanBarcodesForListOfDocument(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}