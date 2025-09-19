using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalPersonDEB;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalPersonDEBCounterpartyPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> CounterpartyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => lenspec.Etalon.People.Is(x));
      return query;
    }
  }

}