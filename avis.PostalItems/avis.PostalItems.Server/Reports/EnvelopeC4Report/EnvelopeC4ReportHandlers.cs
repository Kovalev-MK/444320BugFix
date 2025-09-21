using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PostalItems
{
  partial class EnvelopeC4ReportServerHandlers
  {
    // Добавлено avis.

    public virtual IQueryable<avis.PostalItems.IPostalItem> GetPostalItems()
    {
      IQueryable<avis.PostalItems.IPostalItem> result;
      
      // Если это одиночный отчёт.
      if (EnvelopeC4Report.Id > 0)
      {
        return avis.PostalItems.PostalItems.GetAll(p => p.Id == EnvelopeC4Report.Id);
      }
      
      // Фильтруем по дате создания.
      if (EnvelopeC4Report.EndDateCreate != null)
        result = avis.PostalItems.PostalItems.GetAll(p => p.DateCreated >= EnvelopeC4Report.BeginDateCreate && p.DateCreated <= EnvelopeC4Report.EndDateCreate);
      else
        result = avis.PostalItems.PostalItems.GetAll(p => p.DateCreated == EnvelopeC4Report.BeginDateCreate);
      
      // Фильтруем по способу доставки
      if (EnvelopeC4Report.MailDeliveryMethod != null)
        result = result.Where(p => p.MailDeliveryMethod == EnvelopeC4Report.MailDeliveryMethod);
      
      // Фильтруем по отправителям.
      if (EnvelopeC4Report.Senders.Count > 0)
        result = result.Where(p => EnvelopeC4Report.Senders.Contains(p.Sender));
      
      return result;
    }
    
    // Конец добавлено avis.
  }
}