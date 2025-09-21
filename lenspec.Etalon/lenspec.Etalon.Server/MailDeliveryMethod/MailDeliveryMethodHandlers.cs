using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.MailDeliveryMethod;

namespace lenspec.Etalon
{
  partial class MailDeliveryMethodServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.SendingByMailavis = false;
    }
    //конец Добавлено Avis Expert
  }

}