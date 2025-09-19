using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientContract;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAClientContractPremisesPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> PremisesFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.OurCF.IsSale == true);
      return query;
    }
  }

  partial class SDAClientContractServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      if (_obj.State.IsCopied)
        _obj.InvestContractCode = null;
    }
    //конец Добавлено Avis Expert
  }

  partial class SDAClientContractObjectAnProjectPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public virtual IQueryable<T> ObjectAnProjectFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Premises != null && _obj.Premises.ObjectAnProject != null)
      {
        query = query.Where(x => x.Equals(_obj.Premises.ObjectAnProject));
      }
      query = query.Where(x => x.OurCF.IsSale == true);
      return query;
    }
    //конец Добавлено Avis Expert
  }


}