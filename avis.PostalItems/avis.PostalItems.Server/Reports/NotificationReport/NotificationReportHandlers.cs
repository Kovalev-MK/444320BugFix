using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PostalItems
{
  partial class NotificationReportServerHandlers
  {
    // Добавлено avis.
    
    public virtual IQueryable<avis.PostalItems.IPostalItem> GetTest()
    {
      IQueryable<avis.PostalItems.IPostalItem> result;
      
      // Если это одиночный отчёт. 
      if (NotificationReport.Id > 0)
      {
        return avis.PostalItems.PostalItems.GetAll(p => p.Id == NotificationReport.Id);
      }
      
      // Фильтруем по дате создания.
      if (NotificationReport.EndDateCreate != null)
        result = avis.PostalItems.PostalItems.GetAll(p => p.DateCreated >= NotificationReport.BeginDateCreate && p.DateCreated <= NotificationReport.EndDateCreate);
      else
        result = avis.PostalItems.PostalItems.GetAll(p => p.DateCreated == NotificationReport.BeginDateCreate);
      
      // Фильтруем по способу доставки
      if (NotificationReport.MailDeliveryMethod != null)
        result = result.Where(p => p.MailDeliveryMethod == NotificationReport.MailDeliveryMethod);
      
      // Фильтруем по отправителям.
      if (NotificationReport.Senders.Count > 0)
        result = result.Where(p => NotificationReport.Senders.Contains(p.Sender));

      return result;
    }
    
    // Конец добавлено avis.
  }
}