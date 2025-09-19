using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PostalItems
{
  partial class EnvelopeC5ReportServerHandlers
  {
    // Добавлено avis.

    public virtual IQueryable<avis.PostalItems.IPostalItem> GetPostalItems()
    {
      IQueryable<avis.PostalItems.IPostalItem> result;
      
      // Если это одиночный отчёт. 
      if (EnvelopeC5Report.Id > 0)
      {
        return avis.PostalItems.PostalItems.GetAll(p => p.Id == EnvelopeC5Report.Id);
      }
      
      // Фильтруем по дате создания.
      if (EnvelopeC5Report.EndDateCreate != null)
        result = avis.PostalItems.PostalItems.GetAll(p => p.DateCreated >= EnvelopeC5Report.BeginDateCreate && p.DateCreated <= EnvelopeC5Report.EndDateCreate);
      else
        result = avis.PostalItems.PostalItems.GetAll(p => p.DateCreated == EnvelopeC5Report.BeginDateCreate);
      
      // Фильтруем по способу доставки
      if (EnvelopeC5Report.MailDeliveryMethod != null)
        result = result.Where(p => p.MailDeliveryMethod == EnvelopeC5Report.MailDeliveryMethod);
      
      // Фильтруем по отправителям.
      if (EnvelopeC5Report.Senders.Count > 0)
        result = result.Where(p => EnvelopeC5Report.Senders.Contains(p.Sender));
      
      return result;
    }
    
    // Конец добавлено avis.
  }
}