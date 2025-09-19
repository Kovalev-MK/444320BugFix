using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalerDEBApprovalAssignmentForwardPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ForwardFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      
      return query;
      
    }
  }


}