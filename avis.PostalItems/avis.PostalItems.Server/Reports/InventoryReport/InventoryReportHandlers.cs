using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PostalItems
{
  partial class InventoryReportServerHandlers
  {
    // Добавлено avis.

    public virtual IQueryable<avis.PostalItems.IPostalItem> GetPostalItems()
    {
      IQueryable<avis.PostalItems.IPostalItem> result;
      
      // Если это одиночный отчёт. 
      if (InventoryReport.Id > 0)
      {
        return avis.PostalItems.PostalItems.GetAll(p => p.Id == InventoryReport.Id);
      }
      
      // Фильтруем по дате создания.
      if (InventoryReport.EndDateCreate != null)
        result = avis.PostalItems.PostalItems.GetAll(p => p.DateCreated >= InventoryReport.BeginDateCreate && p.DateCreated <= InventoryReport.EndDateCreate);
      else
        result = avis.PostalItems.PostalItems.GetAll(p => p.DateCreated == InventoryReport.BeginDateCreate);
      
      // Фильтруем по способу доставки
      if (InventoryReport.MailDeliveryMethod != null)
        result = result.Where(p => p.MailDeliveryMethod == InventoryReport.MailDeliveryMethod);
      
      // Фильтруем по отправителям.
      if (InventoryReport.Senders.Count > 0)
        result = result.Where(p => InventoryReport.Senders.Contains(p.Sender));
      
      return result;
    }
    
    // Конец добавлено avis.
  }
}