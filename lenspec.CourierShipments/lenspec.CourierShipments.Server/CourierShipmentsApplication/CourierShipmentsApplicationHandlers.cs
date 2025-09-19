using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.CourierShipments.CourierShipmentsApplication;

namespace lenspec.CourierShipments
{

  partial class CourierShipmentsApplicationDepartmentPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public override IQueryable<T> DepartmentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.DepartmentFiltering(query, e);
      
      // Фильтровать подразделения по нашей организации.
      if (_obj.BusinessUnit != null)
        query = query.Where(d => _obj.BusinessUnit.Equals(d.BusinessUnit));
      
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class CourierShipmentsApplicationFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = base.Filtering(query, e);
      
      if (_filter == null)
        return query;
      
      // Фильтр по инициатору.
      if (_filter.Initiator != null)
        query = query.Where(c => Equals(c.Author, Users.As(_filter.Initiator)));
      
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class CourierShipmentsApplicationContactPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public virtual IQueryable<T> ContactFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Recipient != null && Sungero.Parties.Companies.Is(_obj.Recipient))
      {
        var company = Sungero.Parties.Companies.As(_obj.Recipient);
        query = query.Where(x => company.Equals(x.Company));
      }
      
      return query;
    }
    //конец Добавлено Avis Expert
  }

}