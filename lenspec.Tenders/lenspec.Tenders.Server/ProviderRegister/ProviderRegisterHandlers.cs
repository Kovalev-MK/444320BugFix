using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ProviderRegister;

namespace lenspec.Tenders
{
  partial class ProviderRegisterFilteringServerHandler<T>
  {

    public virtual IQueryable<avis.EtalonParties.IMaterialGroup> MaterialGroupFiltering(IQueryable<avis.EtalonParties.IMaterialGroup> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = query.Where(x => x.Status == avis.EtalonParties.MaterialGroup.Status.Active);
      
      return query;
    }

    public virtual IQueryable<avis.EtalonParties.IMaterial> MaterialFiltering(IQueryable<avis.EtalonParties.IMaterial> query, Sungero.Domain.FilteringEventArgs e)
    {
      if (_filter.MaterialGroup != null)
        query = query.Where(x => Equals(x.Group, _filter.MaterialGroup));
      else
        query = query.Where(x => x.Group != null && x.Group.Status == avis.EtalonParties.MaterialGroup.Status.Active);
      
      query = query.Where(x => x.Status == avis.EtalonParties.Material.Status.Active);
      
      return query;
    }

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      if (_filter == null)
        return query;
      
      query = base.Filtering(query, e);

      if (_filter.MaterialGroup != null)
        query = query.Where(r => Equals(r.MaterialGroup, _filter.MaterialGroup));

      if (_filter.Material != null)
        query = query.Where(r => Equals(r.Material, _filter.Material));
      
      return query;
    }
  }

  partial class ProviderRegisterMaterialPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MaterialFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.MaterialGroup != null)
        query = query.Where(x => Equals(x.Group, _obj.MaterialGroup));
      else
        query = query.Where(x => x.Group != null && x.Group.Status == avis.EtalonParties.MaterialGroup.Status.Active);
      
      return query;
    }
  }

  partial class ProviderRegisterCreatingFromServerHandler
  {

    public override void CreatingFrom(Sungero.Domain.CreatingFromEventArgs e)
    {
      base.CreatingFrom(e);
    }
  }

}