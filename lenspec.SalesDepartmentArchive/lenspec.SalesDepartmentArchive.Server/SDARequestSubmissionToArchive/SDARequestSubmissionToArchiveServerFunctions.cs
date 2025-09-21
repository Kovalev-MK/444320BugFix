using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDARequestSubmissionToArchive;

namespace lenspec.SalesDepartmentArchive.Server
{
  partial class SDARequestSubmissionToArchiveFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Построить сводку по документу.
    /// </summary>
    /// <returns>Сводка по документу.</returns>
    public override StateView GetDocumentSummary()
    {
      var documentSummary = StateView.Create();
      documentSummary.AddDefaultLabel(string.Empty);
      
      var block = documentSummary.AddBlock();
      const string none = "-";
      
      // Тип заявки.
      var requestType = _obj.RequestType != null ? _obj.Info.Properties.RequestType.GetLocalizedValue(_obj.RequestType) : none;
      block.AddLabel(string.Format("{0}: {1}", _obj.Info.Properties.RequestType.LocalizedName, requestType));
      
      return documentSummary;
    }
    
    /// <summary>
    /// Создать клиентский документ.
    /// </summary>
    /// <returns>ИД нового клиентского документа.</returns>
    [Remote]
    public SalesDepartmentArchive.ISDAClientDocument CreateClientDocument(Sungero.Docflow.IDocumentKind documentKind,
                                    SalesDepartmentArchive.ISDAClientContract clientContract,
                                    Sungero.CoreEntities.IUser author,
                                    Sungero.Company.IBusinessUnit businessUnit,
                                    SalesDepartmentArchive.IDocumentKindsOfRealEstateSale documentKindsOfRealEstateSale)
    {
      var clientDocument = SalesDepartmentArchive.SDAClientDocuments.Create();
      
      clientDocument.DocumentKind = documentKind;
      clientDocument.ClientContract = clientContract;
      clientDocument.Author = author;
      clientDocument.BusinessUnit = businessUnit;
      clientDocument.RealEstateDocumentKind = documentKindsOfRealEstateSale;
      if (clientDocument.ClientContract != null && clientDocument.ClientContract.CounterpartyClient.Any())
      {
        foreach(var counterparty in clientDocument.ClientContract.CounterpartyClient)
        {
          clientDocument.CounterpartyClient.AddNew().ClientItem = counterparty.ClientItem;
        }
      }
      clientDocument.Save();
      
      return clientDocument;
    }
    //конец Добавлено Avis Expert
  }
}