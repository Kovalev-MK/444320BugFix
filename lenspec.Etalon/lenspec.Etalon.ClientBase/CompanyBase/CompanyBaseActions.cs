using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.CompanyBase;

namespace lenspec.Etalon.Client
{
  partial class CompanyBaseActions
  {
    public virtual void ReportFromExternalResourcelenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (string.IsNullOrEmpty(_obj.TIN))
      {
        e.AddWarning("Для получения ссылки на отчет необходимо заполнить ИНН.");
        return;
      }
      
      avis.ApprovingCounterpartyDEB.PublicFunctions.Module.GetExpressReportFromKF(_obj.TIN);
    }

    public virtual bool CanReportFromExternalResourcelenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(Roles.Administrators) ||
        Users.Current.IncludedIn(avis.ApprovingCounterpartyDEB.PublicConstants.Module.ExpressReportKFRoleGuid);
    }

    public virtual void ShowBankDetailslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var bankDetails = avis.EtalonParties.PublicFunctions.BankDetail.Remote.GetBankDetails(_obj);
      bankDetails.Show();
    }

    public virtual bool CanShowBankDetailslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Sungero.Parties.Banks.Is(_obj) || Sungero.Parties.Companies.Is(_obj);
    }

    public virtual void ExpressReportavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var report = avis.EtalonParties.Reports.GetDebReport();
      report.Inn = _obj.TIN;
      report.InitiatorEmployeer = Employees.Current?.Name;
      report.Department = Employees.Current?.Department?.Name;
      report.BusinessUnit = Employees.Current?.Department?.BusinessUnit?.Name;
      report.Open();
    }

    public virtual bool CanExpressReportavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(Roles.Administrators) ||
        Users.Current.IncludedIn(avis.ApprovingCounterpartyDEB.PublicConstants.Module.DebReportRoleGuid);
    }

    public virtual void FillFromKonturavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (string.IsNullOrEmpty(_obj.TIN))
      {
        e.AddError(lenspec.Etalon.Banks.Resources.TINEmptyErrorMessage);
        return;
      }
      
      var errorMessage = Sungero.Parties.PublicFunctions.Counterparty.CheckTin(_obj.TIN, true);
      if (!string.IsNullOrEmpty(errorMessage))
      {
        e.AddError(errorMessage);
        return;
      }
      
      lenspec.Etalon.Module.Parties.PublicFunctions.Module.FillCompanyFromKontur(_obj);
    }

    public virtual bool CanFillFromKonturavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    /// <summary>
    /// Кнопка "Получить адрес из Dadata ".
    /// </summary>
    /// <param name="e"></param>
    public virtual void FillDadataavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Заполняем компанию или банк из дадаты.
      var errorMessage = lenspec.Etalon.Module.Integration.PublicFunctions.Module.Remote.FillCompanyFromDadata(_obj);
      
      // Если вернулся текст ошибки то выводим его.
      if (string.IsNullOrEmpty(errorMessage) == false)
        e.AddError(errorMessage);
    }

    /// <summary>
    /// Доступность кнопки "Получить адрес из Dadata".
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual bool CanFillDadataavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Права на создание всех видов контрагентов
      var createCounterpartyRoleGuid = avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid;
      var createCounterpartyRole = Roles.GetAll(r => Equals(r.Sid, createCounterpartyRoleGuid)).FirstOrDefault();
      // Права на работу с кнопкой Получить адрес из Dadata
      var fillDadataRoleGuid = avis.EtalonParties.PublicConstants.Module.RoleFillDadataGuid;
      var fillDadataRole = Roles.GetAll(r => Equals(r.Sid, fillDadataRoleGuid)).FirstOrDefault();
      
      // Отображаем кнопку если сотрудник входит в права на создание всех видов вонтрагентов.
      if (Employees.Current.IncludedIn(createCounterpartyRole) || Employees.Current.IncludedIn(fillDadataRole))
        return true;
      
      return false;
    }
  }

}