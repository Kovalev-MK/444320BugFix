using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.SalesDepartmentArchive.Server
{
  public class ModuleFunctions
  {

    //Добавлено Avis Expert
    
    #region Поиск
    
    /// <summary>
    /// Получить заявки в архив.
    /// </summary>
    /// <param name="contractNumber">№ клиент. договора.</param>
    /// <param name="counterparty">Клиент.</param>
    /// <returns>Список заявок в архив, удовлетворяющий условиям.</returns>
    [Remote(IsPure = true)]
    public static IQueryable<ISDARequestSubmissionToArchive> GetFilteredRequestToArchive(string contractNumber,
                                                                                         Sungero.Parties.ICounterparty client)
    {
      var requests = SDARequestSubmissionToArchives.GetAll();
      
      if (contractNumber != null)
        requests = requests.Where(x => x.ListOfDocument.Any(d => d.ClientContract != null && d.ClientContract.ClientDocumentNumber.Contains(contractNumber)));
      
      if (client != null)
        requests = requests.Where(x => x.ListOfDocument.Any(d => d.ClientContract != null && d.ClientContract.CounterpartyClient.Any(c => client.Equals(c.ClientItem))));
      
      return requests;
    }

    /// <summary>
    /// Получить клиентские договоры.
    /// </summary>
    /// <param name="contractNumber">№ клиент. договора.</param>
    /// <param name="contractDate">Дата договора.</param>
    /// <param name="businessUnit">Наша орг.</param>
    /// <param name="counterparty">Клиент.</param>
    /// <returns>Список клиентских договоров, удовлетворяющий условиям.</returns>
    [Remote(IsPure = true)]
    public static IQueryable<ISDAClientContract> GetFilteredClientContracts(string contractNumber,
                                                                            DateTime? contractDate,
                                                                            Sungero.Company.IBusinessUnit businessUnit,
                                                                            Sungero.Parties.ICounterparty counterparty)
    {
      var contracts = SDAClientContracts.GetAll();

      if (contractNumber != null)
        contracts = contracts.Where(x => x.ClientDocumentNumber.Contains(contractNumber));
      
      if (contractDate != null)
        contracts = contracts.Where(x => x.ClientDocumentDate.Equals(contractDate));
      
      if (businessUnit != null)
        contracts = contracts.Where(x => x.BusinessUnit.Equals(businessUnit));
      
      if (counterparty != null)
        contracts = contracts.Where(x => x.CounterpartyClient.Any(c => c.ClientItem.Equals(counterparty)));
      
      return contracts;
    }

    #endregion
    
    /// <summary>
    /// Создать заявку на сдачу в архив.
    /// </summary>
    /// <returns>Заявка на сдачу в архив.</returns>
    [Remote, Public]
    public static SalesDepartmentArchive.ISDARequestSubmissionToArchive CreateRequestSubmissionToArchive()
    {
      return SalesDepartmentArchive.SDARequestSubmissionToArchives.Create();
    }
    
    /// <summary>
    /// Создать заявку на выдачу из архива.
    /// </summary>
    /// <returns>Заявка на выдау из архива.</returns>
    [Remote, Public]
    public static SalesDepartmentArchive.ISDARequestIssuanceFromArchive CreateRequestIssuanceFromArchive()
    {
      return SalesDepartmentArchive.SDARequestIssuanceFromArchives.Create();
    }
    
    /// <summary>
    /// Создать клиентский договор.
    /// </summary>
    /// <returns>Клиентский договор.</returns>
    [Remote, Public]
    public static SalesDepartmentArchive.ISDAClientContract CreateClientContract()
    {
      return SalesDepartmentArchive.SDAClientContracts.Create();
    }
    
    /// <summary>
    /// Создать доп. соглашение к клиентскому договору.
    /// </summary>
    /// <returns>Доп. соглашение к клиентскому договору.</returns>
    [Remote, Public]
    public static SalesDepartmentArchive.ISDAClientDocument CreateAgreementToClientContract()
    {
      var clientDocument = SalesDepartmentArchive.SDAClientDocuments.Create();
      var kind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.SDAClientDocumentAgreementKind);
      if (kind != null)
      {
        clientDocument.DocumentKind = kind;
      }
      return clientDocument;
    }
    
    /// <summary>
    /// Создать приложение к клиентскому договору.
    /// </summary>
    /// <returns>Приложение к клиентскому договору.</returns>
    [Remote, Public]
    public static SalesDepartmentArchive.ISDAClientDocument CreateAddendumToClientContract()
    {
      var clientDocument = SalesDepartmentArchive.SDAClientDocuments.Create();
      var kind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.SDAClientDocumentAddendumKind);
      if (kind != null)
      {
        clientDocument.DocumentKind = kind;
      }
      return clientDocument;
    }
    
    /// <summary>
    /// Создать прочий документ к клиентскому договору.
    /// </summary>
    /// <returns>Прочий документ к клиентскому договору.</returns>
    [Remote, Public]
    public static SalesDepartmentArchive.ISDAClientDocument CreateOtherDocumentToClientContract()
    {
      var clientDocument = SalesDepartmentArchive.SDAClientDocuments.Create();
      var kind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.SDAClientDocumentOtherKind);
      if (kind != null)
      {
        clientDocument.DocumentKind = kind;
      }
      return clientDocument;
    }
    //конец Добавлено Avis Expert
  }
}