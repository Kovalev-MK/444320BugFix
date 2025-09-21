using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.CustomerRequests.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// Перенос значений КД и Договора МКД во множественные поля.
    /// </summary>
    public virtual void FillMultipleFieldsOfIncomingDocument()
    {
      // Вх. письма, в которых заполнено поле Клиентский договор и пустое множественное поле Клиентский договор
      // или заполнено поле Договор МКД и пустое множественное поле Договор МКД.
      var incomingLetters = lenspec.Etalon.IncomingLetters.GetAll(x => x.ClientContractlenspec != null && !x.ClientContractslenspec.Any() ||
                                                                  x.ManagementContractMKDavis != null && !x.ManagementContractsMKDlenspec.Any());
      foreach (var incomingLetter in incomingLetters)
      {
        try
        {
          if (incomingLetter.ClientContractlenspec != null)
          {
            var clientContract = incomingLetter.ClientContractslenspec.AddNew();
            clientContract.ClientContract = incomingLetter.ClientContractlenspec;
          }
          
          if (incomingLetter.ManagementContractMKDavis != null)
          {
            var contractMKD = incomingLetter.ManagementContractsMKDlenspec.AddNew();
            contractMKD.ManagementContractMKD = incomingLetter.ManagementContractMKDavis;
          }
          
          incomingLetter.Save();
        }
        catch(Exception ex)
        {
          Logger.ErrorFormat("Avis - FillMultipleFieldsOfIncomingDocument - не удалось обновить Клиентский договор, Договор МКД в карточке вх. письма {0} - ", ex, incomingLetter.Id);
        }
      }
      
      // Обращения клиента, в которых заполнено поле Договор МКД и пустое множественное поле Договор МКД.
      var customerRequests = avis.CustomerRequests.CustomerRequests.GetAll(x => x.ManagementContractMKD != null && !x.ManagementContractsMKD.Any());
      foreach (var customerRequest in customerRequests)
      {
        try
        {
          var contractMKD = customerRequest.ManagementContractsMKD.AddNew();
          contractMKD.ManagementContractMKD = customerRequest.ManagementContractMKD;
          
          customerRequest.Save();
        }
        catch(Exception ex)
        {
          Logger.ErrorFormat("Avis - FillMultipleFieldsOfIncomingDocument - не удалось обновить Договор МКД в карточке обращения клиента {0} - ", ex, customerRequest.Id);
        }
      }
    }

  }
}