using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.CompanyBase;

namespace lenspec.Etalon
{
  partial class CompanyBaseHeadBankavisPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> HeadBankavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // ������� ��� �����, ����� ���� �� �������� �������� ���������.
      query = query.Where(q => q != _obj);
      
      return query;
    }
  }

}