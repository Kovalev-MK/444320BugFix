using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientDocumentBase;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAClientDocumentBaseRealEstateDocumentKindPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public virtual IQueryable<T> RealEstateDocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.DocumentKind != null)
        query = query.Where(x => x.DocumentKind.Equals(_obj.DocumentKind));
      
      return query;
    }
    //конец Добавлено Avis Expert
  }
}