using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBase;

namespace avis.ApprovingCounterpartyDEB.Client
{
  partial class ApprovalCounterpartyBaseActions
  {
    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
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

    public override void SaveAndClose(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.Counterparty != null && _obj.Counterparty.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.DoesNotReqAppr)
      {
        e.AddError(avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBases.Resources.DoesNotRequirApprovalMessage);
        return;
      }
      base.SaveAndClose(e);
    }

    public override bool CanSaveAndClose(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSaveAndClose(e);
    }

    public override void Save(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.Counterparty != null && _obj.Counterparty.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.DoesNotReqAppr)
      {
        e.AddError(avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBases.Resources.DoesNotRequirApprovalMessage);
        return;
      }
      base.Save(e);
    }

    public override bool CanSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSave(e);
    }

    public virtual void StartCounterpartyApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      
      if (_obj.Counterparty != null)
      {
        var activeTask = Functions.ApprovalCounterpartyPersonDEB.Remote.GetActiveTaskByCounterparty(_obj.Counterparty);
        if (activeTask != null)
        {
          var errorMessage = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.ErrorMessageHereIsActiveTaskFormat(activeTask.Started.Value.ToShortDateString(), activeTask.Author);
          e.AddError(errorMessage);
          return;
        }
      }
      
      if (Functions.ApprovalCounterpartyPersonDEB.CheckDuplicates(_obj))
        return;
      
      e.CloseFormAfterAction = true;
      var task = Functions.Module.Remote.CreateApprovalCounterpartyTask(_obj);
      task.Show();
    }

    public virtual bool CanStartCounterpartyApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void GetApprovalList(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      if (_obj.Counterparty.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.NotAssessed)
      {
        e.AddInformation("Данная запись не была направлена на проверку в ДБ.");
        return;
      }

      var lastTask = Functions.ApprovalCounterpartyBase.Remote.GetLastTask(_obj);
      if (lastTask != null)
      {
        var report = Reports.GetApprovalSheetDefault();
        var currentUser = Sungero.Company.Employees.Current;
        var currentUserString = currentUser.JobTitle != null ? string.Format("{0}. {1}", currentUser.JobTitle.Name, currentUser.Name) : currentUser.Name;
        report.TaskId = lastTask.Id;
        report.StartId = lastTask.StartId;
        report.Author = _obj.Author?.Name;
        report.Initiator = _obj.PreparedBy?.Name;
        report.TaskSubject = lastTask.Subject;
        report.CurrentUser = currentUserString;
        
        if (_obj.Counterparty != null)
        {
          report.CounterpartyTIN = _obj.Counterparty.TIN;
          if (Sungero.Parties.CompanyBases.Is(_obj.Counterparty))
          {
            var companyBase = Sungero.Parties.CompanyBases.As(_obj.Counterparty);
            report.CounterpartyName = companyBase.LegalName;
          }
          else
            report.CounterpartyName = _obj.Counterparty.Name;
        }
        
        report.DocId = _obj.Id;
        report.Open();
      }
      else
        e.AddError("Не найдено задач по данному документу.");
    }

    public virtual bool CanGetApprovalList(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

  }

}