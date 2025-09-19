using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Company;

namespace lenspec.Etalon.Client
{
  // Добавлено avis.
  
  partial class CompanyActions
  {
    public virtual void SendForApprovalCounterpartyRegisterlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var inclusionName = Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.Procedure.GetLocalizedValue(Tenders.ApprovalCounterpartyRegisterTask.Procedure.Inclusion);
      var exclusionName = Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.Procedure.GetLocalizedValue(Tenders.ApprovalCounterpartyRegisterTask.Procedure.Exclusion);
      
      var dialog = Dialogs.CreateInputDialog(lenspec.Etalon.Companies.Resources.ApprovalCounterpartyRegisterDialogTitle);
      dialog.Buttons.AddOkCancel();
      var procedure = dialog.AddSelect(Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.Procedure.LocalizedName, true)
        .From(inclusionName, exclusionName);
      var presenceRegion = dialog.AddSelectMany(Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.PresenceRegions.LocalizedName, true, avis.EtalonContracts.PresenceRegions.Null)
        .Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && x.IsUsedForQualification.HasValue && x.IsUsedForQualification.Value == true);
      var registerKind = dialog.AddSelect(Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.RegisterKind.LocalizedName, true)
        .From(Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.RegisterKind.GetLocalizedValue(Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Contractor),
              Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.RegisterKind.GetLocalizedValue(Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Provider));
      if (dialog.Show() == DialogButtons.Ok)
      {
        // При включении в реестр – проверка согласования КА с ДБ.
        if (procedure.Value == inclusionName)
        {
          var message = Functions.Company.ValidateApprovalDEBStatus(_obj);
          if (!string.IsNullOrEmpty(message))
          {
            e.AddError(message);
            return;
          }
        }
        
        var task = Functions.Company.Remote.CreateApprovalCounterpartyRegisterTask(_obj, presenceRegion.Value.ToList(), procedure.Value, registerKind.Value);
        task.Show();
      }
    }

    public virtual bool CanSendForApprovalCounterpartyRegisterlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && Users.Current.IncludedIn(Tenders.PublicConstants.Module.ResponsibleForCounterpartyQualificationRole);
    }

    public virtual void ShowActiveContractslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var contracts = Functions.Company.Remote.GetActiveContractList(_obj);
      contracts.Show();
    }

    public virtual bool CanShowActiveContractslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

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
          e.AddError(lenspec.Etalon.Companies.Resources.CheckStatusCompany );
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
        if(string.IsNullOrEmpty(_obj.TRRC) && _obj.GroupCounterpartyavis.IdDirectum5 != 17896408 && _obj.GroupCounterpartyavis.IdDirectum5 != 17896409)
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
        if(_obj.HeadCompany != null)
        {
          var headCompany = lenspec.Etalon.Companies.GetAll().Where(x => x.Id == _obj.HeadCompany.Id).FirstOrDefault();
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

    /// <summary>
    /// Вывод дубликатов.
    /// </summary>
    /// <param name="e"></param>
    public override void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Если вызывает системный юзер, то используем базовую проверку, что бы не сломать инициализацию.
      if (Users.Current.IsSystem == true)
      {
        base.ShowDuplicates(e);
        return;
      }

      System.Linq.IQueryable<lenspec.Etalon.ICompany> duplicates = null;
      
      // Проверяем на дубли.
      if (!string.IsNullOrEmpty(_obj.TIN) && !string.IsNullOrEmpty(_obj.TRRC))
        duplicates = Companies.GetAll(c => c.TIN == _obj.TIN && c.TRRC == _obj.TRRC);
      else if (!string.IsNullOrEmpty(_obj.TIN))
        duplicates = Companies.GetAll(c => c.TIN == _obj.TIN);
      else
        duplicates = Companies.GetAll(c => c.Name == _obj.Name);
      
      // Возвращаем.
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(Sungero.Parties.Counterparties.Resources.DuplicateNotFound);
    }

    public override bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanShowDuplicates(e);
    }
  }
  
  // Конец добавлено avis.
}