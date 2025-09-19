using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.ProtocolCollegialBody;

namespace lenspec.ProtocolsCollegialBodies.Client
{


  partial class ProtocolCollegialBodyActions
  {
    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Если у документа нет тела документа, то выведем диалог
      if (!Functions.ProtocolCollegialBody.NeedSendForApproval(_obj))
        return;
      
      // Проверка на дубликаты задач
      if (_obj.InternalApprovalState != null && _obj.InternalApprovalState != Sungero.Docflow.OfficialDocument.InternalApprovalState.Aborted)
      {
        if (lenspec.Etalon.PublicFunctions.ApprovalTask.CheckDuplicates(_obj, false))
          return;
      }
      
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }


    public override void ShowRegistrationPane(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ShowRegistrationPane(e);
    }

    public override bool CanShowRegistrationPane(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
    }

  }

}