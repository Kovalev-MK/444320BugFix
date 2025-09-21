using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment;

namespace avis.ApprovingCounterpartyDEB.Client
{
  partial class ApprovalManagerDEBAssignmentActions
  {
    public virtual void DoesNotReqAppr(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forward != null)
      {
        e.AddError("Выполните задание с результатом Переадресовать или очистите поле Переадресовать");
        return;
      }
      if (_obj.CompletionDate != null)
      {
        e.AddError("Если персона не требует одобрения, то поле Дата окончания согласования заполнять не нужно. Очистите это поле.");
        return;
      }
    }

    public virtual bool CanDoesNotReqAppr(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void CoopWithRisks(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forward != null)
      {
        e.AddError("Выполните задание с результатом Переадресовать или очистите поле Переадресовать");
        return;
      }
      if (string.IsNullOrEmpty(_obj.ActiveText))
      {
        e.AddError("Напишите комментарий в тексте задания");
        return;
      }
      
      if(_obj.CompletionDate == null)
      {
        e.AddError("Введите дату окончания согласования.");
        return;
      }
    }

    public virtual bool CanCoopWithRisks(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void CoopNotRecomend(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forward != null)
      {
        e.AddError("Выполните задание с результатом Переадресовать или очистите поле Переадресовать");
        return;
      }
      if (_obj.CompletionDate != null)
      {
        e.AddError("Если с персоной сотрудничество не рекомендовано, то поле Дата окончания согласования заполнять не нужно. Очистите это поле");
        return;
      }
      
      if (string.IsNullOrEmpty(_obj.ActiveText))
      {
        e.AddError("Напишите комментарий в тексте задания");
        return;
      }
    }

    public virtual bool CanCoopNotRecomend(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }


    public virtual void CoopPossible(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forward != null)
      {
        e.AddError("Выполните задание с результатом Переадресовать или очистите поле Переадресовать");
        return;
      }
      
      if(_obj.CompletionDate == null)
      {
        e.AddError("Введите дату окончания согласования.");
        return;
      }
    }

    public virtual bool CanCoopPossible(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Rework(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forward != null)
      {
        e.AddError("Выполните задание с результатом Переадресовать или очистите поле Переадресовать");
        return;
      }
      if (string.IsNullOrEmpty(_obj.ActiveText))
      {
        e.AddError("Напишите комментарий в тексте задания");
        return;
      }
    }

    public virtual bool CanRework(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Forward(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forward == null)
      {
        e.AddError("Укажите исполнителя в поле Переадресовать сотруднику.");
        return;
      }
      if (_obj.Forward == Sungero.Company.Employees.Current)
      {
        e.AddError("Нельзя переадресовать самому себе.");
        return;
      }
    }

    public virtual bool CanForward(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void GetApprovalList(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var report = Reports.GetApprovalSheetDefault();
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.FirstOrDefault();
      var currentUser = Sungero.Company.Employees.Current;
      var currentUserString = currentUser.JobTitle != null ? string.Format("{0}. {1}", currentUser.JobTitle.Name, currentUser.Name) : currentUser.Name;
      report.TaskId = _obj.Task.Id;
      report.StartId = _obj.TaskStartId;
      report.Author = document.Author.Name;
      report.Initiator = document.PreparedBy.Name;
      report.CurrentUser = currentUserString;
      report.DocId = document.Id;
      report.TaskSubject = _obj.Subject;
      
      if (document.Counterparty != null)
        {
          report.CounterpartyTIN = document.Counterparty.TIN;
          if (Sungero.Parties.CompanyBases.Is(document.Counterparty))
          {
            var companyBase = Sungero.Parties.CompanyBases.As(document.Counterparty);
            report.CounterpartyName = companyBase.LegalName;
          }
          else
            report.CounterpartyName = document.Counterparty.Name;
        }
      
      report.Open();
    }

    public virtual bool CanGetApprovalList(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}