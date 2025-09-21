using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OrderBase;

namespace lenspec.Etalon
{
  partial class OrderBaseOurCFlenspecPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> OurCFlenspecFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      //query = query.Where(x => x.IsSale == true);
      return query;
    }
  }



  partial class OrderBaseServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.FillEmpDeplenspec = false;
    }
  }

  partial class OrderBaseAddresseeslenspecDepartmentPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AddresseeslenspecDepartmentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Addressee == null)
        return query;
      
      return query.Where(x => x.RecipientLinks.Any(r => Equals(r.Member, _obj.Addressee)));
    }
  }

}