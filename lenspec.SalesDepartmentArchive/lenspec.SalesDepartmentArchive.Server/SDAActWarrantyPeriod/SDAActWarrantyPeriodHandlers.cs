using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAActWarrantyPeriod;

namespace lenspec.SalesDepartmentArchive
{

  partial class SDAActWarrantyPeriodDocumentKindPropertyFilteringServerHandler<T>
  {

    /// <summary>
    /// Фильтрация выбора из спика "Вид документа."
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override IQueryable<T> DocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var actWarrantyPeriodTypeGuid = lenspec.SalesDepartmentArchive.PublicConstants.Module.ActWarrantyPeriodTypeGuid;
      query = query.Where(q => q.DocumentType.DocumentTypeGuid == actWarrantyPeriodTypeGuid.ToString());
      
      return query;
    }
  }

}