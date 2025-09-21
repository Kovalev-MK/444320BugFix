using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEB;

namespace avis.ApprovingCounterpartyDEB.Client
{
  partial class ApprovalCounterpartyBankDEBActions
  {
    public virtual void ReportFromExternalResource(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var counterparty = _obj.Counterparty;
      if (counterparty == null)
      {
        e.AddWarning("Для получения ссылки на отчет необходимо заполнить поле Контрагент.");
        return;
      }
      if (string.IsNullOrEmpty(counterparty.TIN))
      {
        e.AddWarning("Для получения ссылки на отчет необходимо заполнить ИНН контрагента.");
        return;
      }
      
      avis.ApprovingCounterpartyDEB.PublicFunctions.Module.GetExpressReportFromKF(counterparty.TIN);
    }

    public virtual bool CanReportFromExternalResource(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(Roles.Administrators) ||
        Users.Current.IncludedIn(avis.ApprovingCounterpartyDEB.PublicConstants.Module.ExpressReportKFRoleGuid);
    }

    public virtual void ExpressReport(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var report = avis.EtalonParties.Reports.GetDebReport();
      report.Inn = _obj.Counterparty?.TIN;
      report.InitiatorEmployeer = Sungero.Company.Employees.Current?.Name;
      report.Department = Sungero.Company.Employees.Current?.Department?.Name;
      report.BusinessUnit = Sungero.Company.Employees.Current?.Department?.BusinessUnit?.Name;
      report.PurposeOfInspection = _obj.Comments;
      report.Essential = _obj.EstimatedAmountTransaction.HasValue
        ? _obj.EstimatedAmountTransaction.Value.ToString()
        : string.Empty;
      report.Open();
    }

    public virtual bool CanExpressReport(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(Roles.Administrators) ||
        Users.Current.IncludedIn(avis.ApprovingCounterpartyDEB.PublicConstants.Module.DebReportRoleGuid);
    }

  }


}