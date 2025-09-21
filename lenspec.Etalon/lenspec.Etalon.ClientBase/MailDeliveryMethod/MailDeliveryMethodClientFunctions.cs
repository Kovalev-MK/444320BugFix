using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.MailDeliveryMethod;

namespace lenspec.Etalon.Client
{
  partial class MailDeliveryMethodFunctions
  {

    /// <summary>
    /// Изменение полей, связанных с признаком "Отправка почтой".
    /// </summary>
    /// <param name="isSendingByMail">Значение признака "Отправка почтой".</param>
    public void SendingByMailUpdateDependents(bool isSendingByMail)
    {
      _obj.State.Properties.CategoryRPOavis.IsEnabled = isSendingByMail;
      _obj.State.Properties.CategoryRPOavis.IsRequired = isSendingByMail;
      
      _obj.State.Properties.NotificationTypeavis.IsEnabled = isSendingByMail;
      _obj.State.Properties.NotificationTypeavis.IsRequired = isSendingByMail;
      
      if (!isSendingByMail)
      {
        _obj.CategoryRPOavis = null;
        _obj.NotificationTypeavis = null;
      }
    }

  }
}