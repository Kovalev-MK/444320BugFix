using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalSendingAssignment;

namespace lenspec.Etalon.Client
{
  partial class ApprovalSendingAssignmentActions
  {
    public override void SendByMail(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && Sungero.RecordManagement.OutgoingLetters.Is(document))
      {
        Functions.ApprovalSendingAssignment.SendByMailAvis(Sungero.Docflow.ApprovalTasks.As(_obj.Task));
      }
      else
      {
        base.SendByMail(e);
      }
    }

    public override bool CanSendByMail(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendByMail(e);
    }

    //Добавлено Avis Expert
    public virtual void MassSendingOutgoingLetterslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Dialogs.NotifyMessage(lenspec.Etalon.ApprovalSendingAssignments.Resources.CreationOfOutgoingLettersStarted);
      
      var asyncRoleHandler = lenspec.OutgoingLetters.AsyncHandlers.MassSendingOutgoingLetters.Create();
      asyncRoleHandler.TaskId = _obj.Task.Id;
      asyncRoleHandler.UserId = Users.Current.Id;
      asyncRoleHandler.ExecuteAsync();
    }

    public virtual bool CanMassSendingOutgoingLetterslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      //TODO: заменить вызов remote-функции
      return Functions.ApprovalSendingAssignment.Remote.NeedShowMassSendingOutgoingLetters(_obj);
    }
    //конец Добавлено Avis Expert

  }

}