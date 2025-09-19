using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.OutgoingLetters.UnsentMassEmailsNotification;

namespace lenspec.OutgoingLetters.Client
{
  partial class UnsentMassEmailsNotificationActions
  {
    
    //Добавлено Avis Expert
    public virtual void ShowUnsentMassEmails(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var report = Reports.GetUnsentMassEmailsReport();
      report.MassMailingNotification = lenspec.OutgoingLetters.MassMailingNoticeTasks.As(_obj.Task);
      report.Open();
    }

    public virtual bool CanShowUnsentMassEmails(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return lenspec.OutgoingLetters.MassMailingNoticeTasks.As(_obj.Task).UnsentEmails.Any();
    }
    //конец Добавлено Avis Expert

  }

}