using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters
{
  partial class RusPostMailListShortServerHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      
    }

    public virtual IQueryable<avis.PostalItems.IPostalItem> GetPostalItemsList()
    {
      // Фильтруем по дате.
      var result = avis.PostalItems.PostalItems.GetAll(p => p.DateCreated != null && p.DateCreated.Value >= RusPostMailListShort.DateStart.Value && p.DateCreated.Value <= RusPostMailListShort.DateEnd.Value);
      
      // Получаем РПО ценного письма.
      var valuableRPO = avis.PostalItems.CategoryRPOs.GetAll(c => c.Sid == avis.PostalItems.PublicConstants.Module.RPOValuableGuid.ToString()).FirstOrDefault();
      // Получаем РПО заказного письма.
      var customRPO = avis.PostalItems.CategoryRPOs.GetAll(c => c.Sid == avis.PostalItems.PublicConstants.Module.RPOCustomGuid.ToString()).FirstOrDefault();
      
      if (RusPostMailListShort.Vid == "Ценные письма")
        return result.Where(x => lenspec.Etalon.MailDeliveryMethods.As(x.MailDeliveryMethod).CategoryRPOavis == valuableRPO);
      else
        return result.Where(x => lenspec.Etalon.MailDeliveryMethods.As(x.MailDeliveryMethod).CategoryRPOavis == customRPO);
    }
  }
}