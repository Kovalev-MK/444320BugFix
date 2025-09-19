using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderDocument;

namespace lenspec.Tenders
{
  partial class TenderDocumentDocumentKindPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> DocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.DocumentKindFiltering(query, e);
      
      var decisionOnInclusionKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DecisionOnInclusionOfCounterpartyKind);
      var decisionOnExclusionKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DecisionOnExclusionOfCounterpartyKind);
      query = query.Where(x => !Equals(x, decisionOnInclusionKind) && !Equals(x, decisionOnExclusionKind));
      
      return query;
    }
  }


}