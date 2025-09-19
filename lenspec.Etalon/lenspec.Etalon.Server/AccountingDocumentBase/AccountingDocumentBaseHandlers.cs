using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.AccountingDocumentBase;

namespace lenspec.Etalon
{
  partial class AccountingDocumentBaseLeadingDocumentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> LeadingDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.LeadingDocumentFiltering(query, e);
      query = query.Where(q => Sungero.Contracts.Contracts.Is(q));
      
      return query;
    }
  }

}