using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.MailDeliveryMethod;

namespace lenspec.Etalon
{
  partial class MailDeliveryMethodSharedHandlers
  {
    public virtual void SendingByMailavisChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      var isSendingByMail = _obj.SendingByMailavis.HasValue && _obj.SendingByMailavis.Value == true;
      _obj.State.Properties.CategoryRPOavis.IsEnabled = _obj.State.Properties.CategoryRPOavis.IsRequired = isSendingByMail;
      _obj.State.Properties.NotificationTypeavis.IsEnabled = _obj.State.Properties.NotificationTypeavis.IsRequired = isSendingByMail;
      
      if (isSendingByMail == false)
      {
        _obj.CategoryRPOavis = null;
        _obj.NotificationTypeavis = null;
      }
    }
  }
}