using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonContracts.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Получение договорных документов (без учета прав доступа).
    /// </summary>
    /// <param name="dateFrom">Дата создания с.</param>
    /// <param name="dateTo">Дата создания по.</param>
    /// <returns>Договорные документы.</returns>
    [Public, Remote(IsPure = true)]
    public static IQueryable<lenspec.Etalon.IContractualDocument> GetContractualDocs(DateTime dateFrom, DateTime dateTo)
    {      
      dateTo = Calendar.EndOfDay(dateTo);

      var documents = Enumerable.Empty<lenspec.Etalon.IContractualDocument>().AsQueryable();

      AccessRights.AllowRead(
        () =>
        {
          documents = lenspec.Etalon.ContractualDocuments
            .GetAll()
            .Where(x => x.Created.Value >= dateFrom && x.Created.Value <= dateTo);
        });
      
      return documents;
    }
    
    /// <summary>
    /// Проверить доступ к документу от лица пользователя
    /// </summary>
    /// <param name="docId"></param>
    /// <returns>true - права на просмотр есть, иначе нет</returns>
    [Public]
    public static bool CheckAccessRightsAllowRead(long docId)
    {
      var contract = Sungero.Contracts.ContractualDocuments.Null;
      AccessRights.AllowRead(() =>
                             {
                               contract = Sungero.Contracts.ContractualDocuments.GetAll(x => x.Id == docId).SingleOrDefault();
                             });
      return contract != null && contract.AccessRights.CanRead();
    }

    /// <summary>
    /// Получить гиперссылку на документ для отчета
    /// </summary>
    /// <param name="documentId">ИД документа</param>
    /// <returns>Гиперссылка</returns>
    [Public]
    public static string GetLinkToContractualDocument(long documentId)
    {
      var link = string.Empty;
      AccessRights.AllowRead(() =>
                             {
                               var document = lenspec.Etalon.ContractualDocuments.Get(documentId);
                               link = Hyperlinks.Get(document);
                             });
      return link;
    }
  }
}