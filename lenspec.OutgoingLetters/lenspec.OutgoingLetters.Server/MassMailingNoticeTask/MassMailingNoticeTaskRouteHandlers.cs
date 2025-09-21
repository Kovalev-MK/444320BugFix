using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using lenspec.OutgoingLetters.MassMailingNoticeTask;

namespace lenspec.OutgoingLetters.Server
{
  partial class MassMailingNoticeTaskRouteHandlers
  {

    //Добавлено Avis Expert
    public virtual void StartBlock3(lenspec.OutgoingLetters.Server.UnsentMassEmailsNotificationArguments e)
    {
      e.Block.Subject = _obj.NotificationSubject;
      e.Block.Performers.Add(_obj.NotificationPerformer);
    }
    //конец Добавлено Avis Expert
  }

}