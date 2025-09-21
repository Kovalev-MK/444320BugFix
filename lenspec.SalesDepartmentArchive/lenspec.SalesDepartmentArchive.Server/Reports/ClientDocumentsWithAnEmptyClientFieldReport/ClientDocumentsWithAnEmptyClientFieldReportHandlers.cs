using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.SalesDepartmentArchive
{
  partial class ClientDocumentsWithAnEmptyClientFieldReportServerHandlers
  {

    public virtual IQueryable<lenspec.SalesDepartmentArchive.ISDAClientContract> GetClientContract()
    {
      var selectedBusinessUnits = ClientDocumentsWithAnEmptyClientFieldReport.BusinessUnit.ToList();
      var clientContracts = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(cc => !cc.CounterpartyClient.Any());
      
      if (selectedBusinessUnits != null && selectedBusinessUnits.Any())
      {
        clientContracts = clientContracts.Where(cc => selectedBusinessUnits.Contains(cc.BusinessUnit));
      }
      
      return clientContracts;
    }
  }
}