using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.CollegialBody;

namespace lenspec.ProtocolsCollegialBodies
{
  partial class CollegialBodyBasisDocumentPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> BasisDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(
        d =>
        Equals(d.LifeCycleState, Sungero.RecordManagement.Order.LifeCycleState.Active) &&
        d.InternalApprovalState != null &&
        Equals(d.InternalApprovalState.Value, Sungero.Docflow.OfficialDocument.InternalApprovalState.Signed));
    }
  }


}