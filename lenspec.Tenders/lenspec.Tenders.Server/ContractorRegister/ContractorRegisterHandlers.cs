using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ContractorRegister;

namespace lenspec.Tenders
{
  partial class ContractorRegisterFilteringServerHandler<T>
  {

    public virtual IQueryable<avis.EtalonParties.IWorkGroup> WorkGroupFiltering(IQueryable<avis.EtalonParties.IWorkGroup> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = query.Where(x => x.Status == avis.EtalonParties.WorkGroup.Status.Active);
      
      return query;
    }

    public virtual IQueryable<avis.EtalonParties.IWorkKind> WorkKindFiltering(IQueryable<avis.EtalonParties.IWorkKind> query, Sungero.Domain.FilteringEventArgs e)
    {
      if (_filter.WorkGroup != null)
        query = query.Where(x => Equals(x.Group, _filter.WorkGroup));
      else
        query = query.Where(x => x.Group != null && x.Group.Status == avis.EtalonParties.WorkGroup.Status.Active);
      
      query = query.Where(x => x.Status == avis.EtalonParties.WorkKind.Status.Active);
      
      return query;
    }

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      if (_filter == null)
        return query;
      
      query = base.Filtering(query, e);

      if (_filter.WorkGroup != null)
        query = query.Where(r => Equals(r.WorkGroup, _filter.WorkGroup));

      if (_filter.WorkKind != null)
        query = query.Where(r => Equals(r.WorkKind, _filter.WorkKind));
      
      return query;
    }
  }

  partial class ContractorRegisterWorkKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WorkKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.WorkGroup != null)
        query = query.Where(x => Equals(x.Group, _obj.WorkGroup));
      else
        query = query.Where(x => x.Group != null && x.Group.Status == avis.EtalonParties.WorkGroup.Status.Active);
      
      return query;
    }
  }

  partial class ContractorRegisterCreatingFromServerHandler
  {

    public override void CreatingFrom(Sungero.Domain.CreatingFromEventArgs e)
    {
      base.CreatingFrom(e);
    }
  }

}