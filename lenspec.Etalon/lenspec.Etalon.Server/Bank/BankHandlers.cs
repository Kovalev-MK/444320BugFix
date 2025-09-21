using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Bank;

namespace lenspec.Etalon
{

  partial class BankHeadCompanyPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> HeadCompanyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.HeadCompanyFiltering(query, e);
      // ������� ������ �����.
      //query = query.Where(q=> Sungero.Parties.Banks.Is(q));
      return query;
    }
  }

}