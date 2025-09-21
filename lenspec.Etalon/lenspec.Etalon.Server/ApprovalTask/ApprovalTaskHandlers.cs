using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalTask;
using Sungero.Domain.Shared;

namespace lenspec.Etalon
{
  partial class ApprovalTaskServerHandlers
  {
    //Добавлено Avis Expert
    public override void BeforeAbort(Sungero.Workflow.Server.BeforeAbortEventArgs e)
    {
      if (Sungero.Company.Employees.Is(_obj.Author) && Sungero.Company.Employees.As(_obj.Author).Status == Sungero.Company.Employee.Status.Closed)
      {
        ((Sungero.Domain.Shared.IExtendedEntity)_obj).Params[Sungero.Docflow.Constants.ApprovalTask.NeedSetDocumentObsolete] = false;
      }
      
      base.BeforeAbort(e);
    }

    public override void BeforeRestart(Sungero.Workflow.Server.BeforeRestartEventArgs e)
    {
      var errorMessage = Functions.ApprovalTask.ExecuteByMainDocumentType(_obj);
      if (!string.IsNullOrEmpty(errorMessage))
      {
        e.AddError(errorMessage);
        return;
      }
      base.BeforeRestart(e);
    }

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      var errorMessage = Functions.ApprovalTask.ExecuteByMainDocumentType(_obj);
      if (!string.IsNullOrEmpty(errorMessage))
      {
        e.AddError(errorMessage);
        return;
      }
      base.BeforeStart(e);
    }

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      base.Saving(e);
      
      foreach(var recipient in _obj.ComputedReqApproverslenspec)
      {
        if (recipient.Approver != null && !_obj.ReqApprovers.Any(x => recipient.Approver.Equals(x.Approver)))
        {
          // Участников коллекции "Обязательные согласующие по вычисляемой роли" добавить в поле Обязательные,
          // т.к. после сохранения карточки они не учтены в базовом обработчике.
          var newLine = _obj.ReqApprovers.AddNew();
          newLine.Approver = recipient.Approver;
        }
      }
      
    }
    //конец Добавлено Avis Expert
  }
}