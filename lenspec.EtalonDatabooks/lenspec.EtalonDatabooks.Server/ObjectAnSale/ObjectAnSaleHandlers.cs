using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnSale;

namespace lenspec.EtalonDatabooks
{
  partial class ObjectAnSaleObjectAnProjectPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ObjectAnProjectFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.IsLinkToInvest == true);
      return query;
    }
  }


  // Добавлено avis.
  
  partial class ObjectAnSaleServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      if (_obj.State.IsCopied)
      {
        _obj.IdInvest = null;
      }
    }
    
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      Functions.ObjectAnSale.FillName(_obj);
    }
  }

  // Конец добавлено avis.
}