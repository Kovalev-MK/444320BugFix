using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBase;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalCounterpartyBaseCitiesCityPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CitiesCityFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_root.PresenceRegion.Any())
      {
        var regions = _root.PresenceRegion.Select(x => x.Region);
        query = query.Where(x => regions.Contains(x.Region));
      }
        
      return query;
    }
  }

  partial class ApprovalCounterpartyBaseCreatingFromServerHandler
  {

    public override void CreatingFrom(Sungero.Domain.CreatingFromEventArgs e)
    {
      base.CreatingFrom(e);
      e.Without(_info.Properties.ApprovalStateNew);
    }
  }

  partial class ApprovalCounterpartyBasePreparedByPropertyFilteringServerHandler<T>
  {
    public override IQueryable<T> PreparedByFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => Substitutions.ActiveSubstitutedUsers.Contains(x) || x.Equals(Sungero.Company.Employees.Current));
      return query;
    }
  }

  partial class ApprovalCounterpartyBaseServerHandlers
  {
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.ResultApprovalDEB = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBase.ResultApprovalDEB.NotAssessed;
      //_obj.RegionLable = ApprovalCounterpartyBases.Resources.ActivityRegions;
    }
  }
}