using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.WorkKind;

namespace avis.EtalonParties
{
  partial class WorkKindGroupPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> GroupFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q => q.Status == avis.EtalonParties.WorkGroup.Status.Active);
      
      return query;
    }
  }

}