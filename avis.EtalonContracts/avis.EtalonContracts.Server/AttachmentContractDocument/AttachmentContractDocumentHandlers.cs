using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonContracts.AttachmentContractDocument;

namespace avis.EtalonContracts
{
  partial class AttachmentContractDocumentLeadingDocumentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> LeadingDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.LeadingDocumentFiltering(query, e);
      
      // Фильтрация по договору.
      query = query.Where(q => avis.EtalonContracts.AttachmentContractDocuments.Is(q) == false);

      if (_obj.BusinessUnit != null)
        query = query.Where(q => q.BusinessUnit == _obj.BusinessUnit);
      
      if (_obj.Counterparty != null)
        query = query.Where(q => q.Counterparty == _obj.Counterparty);
          
      return query;
    }
  }

  partial class AttachmentContractDocumentServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
     
      _obj.LeadingDocumentComment = avis.EtalonContracts.AttachmentContractDocuments.Resources.LeadingDocumentCommentText;
    }
  }

}