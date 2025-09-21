using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.CustomerRequests.Server
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Получить обращения клиентов.
    /// </summary>
    [Remote(IsPure = true)]
    public static IQueryable<ICustomerRequest> GetFilteredCustumRequests(string regNumber,
                                                                         DateTime? created,
                                                                         lenspec.Etalon.IPerson client,
                                                                         lenspec.SalesDepartmentArchive.ISDAClientContract contract,
                                                                         Sungero.Company.IBusinessUnit businessUnit,
                                                                         avis.ManagementCompanyJKHArhive.IManagementContractMKD contractMKD)
    {
      var requests = CustomerRequests.GetAll();

      if (regNumber != null)
        requests = requests.Where(x => x.RegistrationNumber.Contains(regNumber));
      
      if (created != null)
        requests = requests.Where(x => created.Equals(x.RegistrationDate));
      
      if (client != null)
        requests = requests.Where(x => client.Equals(x.Client));
      
      if (contract != null)
        requests = requests.Where(x => x.SDAContracts.Any(c => contract.Equals(c.Contract)));
      
      if (businessUnit != null)
        requests = requests.Where(x => businessUnit.Equals(x.BusinessUnit));
      
      if (contractMKD != null)
        requests = requests.Where(x => x.ManagementContractsMKD.Any(c => contractMKD.Equals(c.ManagementContractMKD)));
      
      return requests;
    }

  }
}