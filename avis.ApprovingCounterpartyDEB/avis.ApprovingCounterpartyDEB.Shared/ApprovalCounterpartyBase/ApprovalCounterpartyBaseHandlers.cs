using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBase;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalCounterpartyBaseSharedHandlers
  {

    public virtual void PresenceRegionChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      _obj.Cities.Clear();
    }

    public override void PreparedByChanged(Sungero.Docflow.Shared.OfficialDocumentPreparedByChangedEventArgs e)
    {
      base.PreparedByChanged(e);
      if (e.NewValue != null && e.NewValue != e.OldValue && e.NewValue.Department != null)
        _obj.Department = e.NewValue.Department;
      else
        _obj.Department = null;
    }
  }



}