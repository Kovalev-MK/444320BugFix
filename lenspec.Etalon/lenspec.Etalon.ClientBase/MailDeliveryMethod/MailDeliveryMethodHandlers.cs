using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.MailDeliveryMethod;

namespace lenspec.Etalon
{
  partial class MailDeliveryMethodClientHandlers
  {
    //Добавлено Avis Expert
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var isSendingByMail = _obj.SendingByMailavis.HasValue && _obj.SendingByMailavis.Value == true;
      
      _obj.State.Properties.CategoryRPOavis.IsEnabled = isSendingByMail;
      _obj.State.Properties.CategoryRPOavis.IsRequired = isSendingByMail;
      
      _obj.State.Properties.NotificationTypeavis.IsEnabled = isSendingByMail;
      _obj.State.Properties.NotificationTypeavis.IsRequired = isSendingByMail;
      
      if (!isSendingByMail)
      {
        if (_obj.CategoryRPOavis != null)
          _obj.CategoryRPOavis = null;
        if (_obj.NotificationTypeavis != null)
          _obj.NotificationTypeavis = null;
      }
    }
    
    //конец Добавлено Avis Expert
  }
}