using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Order;

namespace lenspec.Etalon
{
  partial class OrderCanceledOrderslenspecCanceledOrderPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CanceledOrderslenspecCanceledOrderFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Для выбора доступны только приказы с такой же НОР и со стадией ЖЦ Действующие. DIRRXMIGR-37
      query = query.Where(q => q.LifeCycleState == LifeCycleState.Active);

      var businessUnit = _root.BusinessUnit;
      if (businessUnit != null)
        query = query.Where(x => x.BusinessUnit == businessUnit);

      return query;
    }
  }

}