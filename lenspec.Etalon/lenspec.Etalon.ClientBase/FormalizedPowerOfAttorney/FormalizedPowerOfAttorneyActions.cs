using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.FormalizedPowerOfAttorney;

namespace lenspec.Etalon.Client
{

  partial class FormalizedPowerOfAttorneyActions
  {

    public override void ExportDocument(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ExportDocument(e);
      
    }

    public override bool CanExportDocument(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanExportDocument(e);
    }


    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      
      // Если по документу ранее были запущены задачи, то вывести соответствующий диалог.
      if (Functions.ApprovalTask.CheckDuplicates(_obj, false))
        return;
      else
      {
        var task = Functions.FormalizedPowerOfAttorney.Remote.CheckApprovalRulesAndCreateTask(_obj);

        if (task != null)
        {
          task.Show();
          e.CloseFormAfterAction = true;
        }
        else
        {
          // ���� �� ��������� ��� ����������, ������� ���������.
          e.AddError(OfficialDocuments.Resources.NoApprovalRuleWarning);
          return;
        }
      }
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
      var rightChangePowerOfAttorneysGroupRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidRightChangePowerOfAttorneysGroup).SingleOrDefault();
      var responsibleRecallPowerOfAttorneysRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidResponsibleRecallPowerOfAttorneys).SingleOrDefault();
      var currentEmployee = Sungero.Company.Employees.Current;
      return base.CanShowRegistrationPane(e) &&
        (currentEmployee.IncludedIn(rightChangePowerOfAttorneysGroupRole) || currentEmployee.IncludedIn(responsibleRecallPowerOfAttorneysRole) || currentEmployee.IncludedIn(Roles.Administrators));
    }


    public override void CancelRegistration(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CancelRegistration(e);
    }

    public override bool CanCancelRegistration(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var rightChangePowerOfAttorneysGroupRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidRightChangePowerOfAttorneysGroup).SingleOrDefault();
      var responsibleRecallPowerOfAttorneysRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidResponsibleRecallPowerOfAttorneys).SingleOrDefault();
      var currentEmployee = Sungero.Company.Employees.Current;
      return base.CanCancelRegistration(e) &&
        (currentEmployee.IncludedIn(rightChangePowerOfAttorneysGroupRole) || currentEmployee.IncludedIn(responsibleRecallPowerOfAttorneysRole) || currentEmployee.IncludedIn(Roles.Administrators));
    }
    

    public override void ChangeRequisites(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ChangeRequisites(e);
    }

    public override bool CanChangeRequisites(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var rightChangePowerOfAttorneysGroupRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidRightChangePowerOfAttorneysGroup).SingleOrDefault();
      var responsibleRecallPowerOfAttorneysRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidResponsibleRecallPowerOfAttorneys).SingleOrDefault();
      var currentEmployee = Sungero.Company.Employees.Current;
      return base.CanChangeRequisites(e) &&
        (currentEmployee.IncludedIn(rightChangePowerOfAttorneysGroupRole) || currentEmployee.IncludedIn(responsibleRecallPowerOfAttorneysRole) || currentEmployee.IncludedIn(Roles.Administrators));
    }


    public override void Register(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Register(e);
    }
    
    public override bool CanRegister(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var rightChangePowerOfAttorneysGroupRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidRightChangePowerOfAttorneysGroup).SingleOrDefault();
      var responsibleRecallPowerOfAttorneysRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidResponsibleRecallPowerOfAttorneys).SingleOrDefault();
      var currentEmployee = Sungero.Company.Employees.Current;
      return base.CanRegister(e) &&
        (currentEmployee.IncludedIn(rightChangePowerOfAttorneysGroupRole) || currentEmployee.IncludedIn(responsibleRecallPowerOfAttorneysRole) || currentEmployee.IncludedIn(Roles.Administrators));
    }
    

    public override void ImportVersionWithSignature(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ImportVersionWithSignature(e);
    }

    public override bool CanImportVersionWithSignature(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var rightChangePowerOfAttorneysGroupRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidRightChangePowerOfAttorneysGroup).SingleOrDefault();
      var responsibleRecallPowerOfAttorneysRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidResponsibleRecallPowerOfAttorneys).SingleOrDefault();
      var currentEmployee = Sungero.Company.Employees.Current;
      return base.CanImportVersionWithSignature(e) &&
        (currentEmployee.IncludedIn(rightChangePowerOfAttorneysGroupRole) || currentEmployee.IncludedIn(responsibleRecallPowerOfAttorneysRole) || currentEmployee.IncludedIn(Roles.Administrators));
    }
    

    public override void GenerateBodyWithPdf(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (string.IsNullOrEmpty(_obj.RegistrationNumber))
      {
        e.AddError(lenspec.Etalon.FormalizedPowerOfAttorneys.Resources.ErrorMessageEmptyRegNumber);
        return;
      }
      
      // Проверка заполнения обязательных реквизитов.
      var fieldsWithErrors = Functions.FormalizedPowerOfAttorney.CheckRequiredPropertiesToGenerateBody(_obj);
      if (!string.IsNullOrEmpty(fieldsWithErrors))
      {
        e.AddError(string.Format("{0}{1}", Sungero.Docflow.FormalizedPowerOfAttorneys.Resources.GenerateBodyWithPdfError, fieldsWithErrors));
        return;
      }
      
      base.GenerateBodyWithPdf(e);
    }

    public override bool CanGenerateBodyWithPdf(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var rightChangePowerOfAttorneysGroupRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidRightChangePowerOfAttorneysGroup).SingleOrDefault();
      var responsibleRecallPowerOfAttorneysRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidResponsibleRecallPowerOfAttorneys).SingleOrDefault();
      var currentEmployee = Sungero.Company.Employees.Current;
      return base.CanGenerateBodyWithPdf(e) &&
        (currentEmployee.IncludedIn(rightChangePowerOfAttorneysGroupRole) || currentEmployee.IncludedIn(responsibleRecallPowerOfAttorneysRole) || currentEmployee.IncludedIn(Roles.Administrators));
    }
    

    public override void UpdateTemplatelenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.UpdateTemplatelenspec(e);
    }

    public override bool CanUpdateTemplatelenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
    }
    

    public override void CreateRevocation(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateRevocation(e);
    }

    public override bool CanCreateRevocation(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var role = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidRightChangePowerOfAttorneysGroup).SingleOrDefault();
      return base.CanCreateRevocation(e) &&
        (Sungero.Company.Employees.Current.IncludedIn(role) || Sungero.Company.Employees.Current.IncludedIn(Roles.Administrators));
    }

  }

}