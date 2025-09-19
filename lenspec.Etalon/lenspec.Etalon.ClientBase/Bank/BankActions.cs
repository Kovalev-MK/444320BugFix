using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Bank;

namespace lenspec.Etalon.Client
{
  partial class BankActions
  {
    public override void Export1Cavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Export1Cavis(e);
      var check           = string.Empty;
      var checkHead       = string.Empty;
      var checkList       = new List<string>();
      var checkHeadList   = new List<string>();
      
      var user = Sungero.Company.Employees.Current;
      if (user == null)
        return;
      var dialog          = Dialogs.CreateInputDialog("Экспорт в 1С");
      //      var businesUnit     = dialog.AddSelect("Наша организация", true, Sungero.Company.BusinessUnits.Null);
      //      businesUnit.Value   = user.Department.BusinessUnit;
      var employee        = dialog.AddSelect("Инициатор", true, Sungero.Company.Employees.Null);
      employee.Value = user;
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        if(_obj.Status != Sungero.Parties.Counterparty.Status.Active)
        {
          e.AddError("Выгрузить в 1С можно только действующие записи справочника" );
          return;
        }
        if(_obj.ResultApprovalDEBavis.Value != Etalon.Counterparty.ResultApprovalDEBavis.CoopPossible   &&
           _obj.ResultApprovalDEBavis.Value != Etalon.Counterparty.ResultApprovalDEBavis.CoopWithRisks  &&
           _obj.ResultApprovalDEBavis.Value != Etalon.Counterparty.ResultApprovalDEBavis.DoesNotReqAppr)
        {
          e.AddError(lenspec.Etalon.Companies.Resources.SbError);
          return;
        }
        if(string.IsNullOrEmpty(_obj.LegalName))
        {
          checkList.Add("Юрид. наименование");
        }
        if(string.IsNullOrEmpty(_obj.TIN))
        {
          checkList.Add("ИНН");
        }
        if(string.IsNullOrEmpty(_obj.TRRC))
        {
          checkList.Add("КПП");
        }
        if(string.IsNullOrEmpty(_obj.PostalAddress))
        {
          checkList.Add("Почтовый адрес");
        }
        if (checkList.Count() > 0)
        {
          check = string.Join(", ", checkList);
          e.AddError("Для выгрузки в 1С необходимо заполнить: " + check);
          return;
        }
        //        var busineUnitCode = lenspec.Etalon.BusinessUnits.As(businesUnit.Value).ExternalCodeavis;
        var busineUnitCode = "000";
        if(_obj.HeadBankavis != null)
        {
          var headCompany = lenspec.Etalon.Banks.GetAll().Where(x => x.Id == _obj.HeadBankavis.Id).FirstOrDefault();
          if(headCompany.LegalName  == null)
          {
            checkHeadList.Add("Юрид. наименование");
          }
          if(headCompany.TIN == null)
          {
            checkHeadList.Add("ИНН");
          }
          if(headCompany.TRRC == null)
          {
            checkHeadList.Add("КПП");
          }
          if(headCompany.PostalAddress == null)
          {
            checkHeadList.Add("Почтовый адрес");
          }
          if (checkHeadList.Count() > 0)
          {
            checkHead = string.Join(", ", checkHeadList);
            e.AddError("Для выгрузки филиала, нужно выгрузить головную организацию. В головной организации необходимо заполнить: " + checkHead);
            return;
          }
          lenspec.Etalon.Module.Parties.PublicFunctions.Module.UnloadingKAFromReference(headCompany.Id, busineUnitCode);
        }
        Dialogs.NotifyMessage("Выгрузка в интеграционную базу началась");
        lenspec.Etalon.Module.Parties.PublicFunctions.Module.UnloadingKAFromReference(_obj.Id, busineUnitCode);
      }
    }

    public override bool CanExport1Cavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Роль Ответственные за выгрузку контрагентов в 1С
      var responsibleUnloadingContractors1CGuid = Etalon.Module.Parties.Constants.Module.ResponsibleUnloadingContractors1CGuid;
      var responsibleUnloadingContractorsRole = Roles.GetAll(r => Equals(r.Sid, responsibleUnloadingContractors1CGuid)).FirstOrDefault();
      
      // Отображаем кнопку если сотрудник входит в роль Ответственные за выгрузку контрагентов в 1С
      if (Employees.Current.IncludedIn(responsibleUnloadingContractorsRole))
        return true;
      return base.CanExport1Cavis(e);
    }

    public virtual void FillRequisitesFromDadatalenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (string.IsNullOrEmpty(_obj.BIC) && string.IsNullOrEmpty(_obj.TIN) && string.IsNullOrEmpty(_obj.TRRC))
      {
        e.AddInformation(lenspec.Etalon.Banks.Resources.NeedFillTinAndTRRC);
        return;
      }
      
      var error = lenspec.Etalon.Module.Integration.PublicFunctions.Module.Remote.FillBankRequisitesFromDadata(_obj);
      if (!string.IsNullOrEmpty(error))
        e.AddError(error);
    }

    public virtual bool CanFillRequisitesFromDadatalenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(Roles.Administrators) || Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid);
    }

    public override void FillFromKonturavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.FillFromKonturavis(e);
      this.FillRequisitesFromDadatalenspec(e);
    }

    public override bool CanFillFromKonturavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Employees.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid);
    }

  }

}