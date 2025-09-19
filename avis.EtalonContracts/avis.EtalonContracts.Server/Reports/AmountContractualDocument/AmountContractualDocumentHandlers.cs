using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonContracts
{
  partial class AmountContractualDocumentServerHandlers
  {

    public virtual IQueryable<Sungero.Contracts.IContractualDocument> GetContractualDocuments()
    {
      var contractualDocument = Sungero.Contracts.ContractualDocuments.Get(AmountContractualDocument.DocumentId.Value);
      long contractId = contractualDocument.Id;
      
      if (Sungero.Contracts.SupAgreements.Is(contractualDocument))
        contractId = contractualDocument.LeadingDocument.Id;
      IQueryable<Sungero.Contracts.IContractualDocument> contractualDocuments = null;
      AccessRights.AllowRead(() =>
                             {
                               contractualDocuments = Sungero.Contracts.ContractualDocuments.GetAll().Where(x => x.Id == contractId || x.LeadingDocument.Id == contractId);
                             });
      // Сначала надо выводить договоры потом допники
      return contractualDocuments.OrderBy(x => x.Id);
    }

  }
}