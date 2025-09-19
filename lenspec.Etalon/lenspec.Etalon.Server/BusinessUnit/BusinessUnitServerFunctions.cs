using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.BusinessUnit;
using AvisIntegrationHelper;

namespace lenspec.Etalon.Server
{
  partial class BusinessUnitFunctions
  {
    //Добавлено Avis Expert.
    
    #region ОШС
    
    [Remote(IsPure = true)]
    public Sungero.Core.StateView GetStateViewXml()
    {
      var stateView = StateView.Create();
      stateView.IsPrintable = true;
      
      var block = stateView.AddBlock();
      block.IsExpanded = true;
      block.AssignIcon(BusinessUnits.Resources.BusinessUnitIcon, StateBlockIconSize.Large);
      block.AddLabel(_obj.Name);
      block.AddLineBreak();
      
      if (_obj.CEO != null)
      {
        block.AddLabel("Руководитель: ");
        block.AddHyperlink(_obj.CEO.Name, Hyperlinks.Get(_obj.CEO));
      }
      else
        block.AddLabel("Руководитель не указан");
      
      var status = Sungero.Company.BusinessUnits.Info.Properties.Status.GetLocalizedValue(_obj.Status);
      if (!string.IsNullOrEmpty(status))
        Sungero.Docflow.PublicFunctions.Module.AddInfoToRightContent(block, $"Состояние: {status}");
      
      var businessUnits = Sungero.Company.BusinessUnits.GetAll(x => x.Status == Sungero.Company.BusinessUnit.Status.Active).ToList();
      var departments = Sungero.Company.Departments.GetAll(x => x.Status == Sungero.Company.Department.Status.Active).ToList();
      AddSubBusinessUnit(block, Sungero.Company.BusinessUnits.As(_obj), businessUnits, departments);
      
      return stateView;
    }
    
    private void AddSubBusinessUnit(Sungero.Core.StateBlock taskBlock, Sungero.Company.IBusinessUnit businessUnit, List<Sungero.Company.IBusinessUnit> allBusinessUnits, List<Sungero.Company.IDepartment> allDepartments)
    {
      var subDeps = allDepartments.Where(x => x.BusinessUnit == businessUnit && x.HeadOffice == null).AsEnumerable();
      foreach (var dep in subDeps)
      {
        var depBlock = taskBlock.AddChildBlock();
        depBlock.AssignIcon(BusinessUnits.Resources.DepartmentIcon, StateBlockIconSize.Small);
        depBlock.AddHyperlink(dep.Name, Hyperlinks.Get(dep));
        depBlock.AddLineBreak();
        
        if (dep.Manager != null)
        {
          depBlock.AddLabel("Руководитель: ");
          depBlock.AddHyperlink(dep.Manager.Name, Hyperlinks.Get(dep.Manager));
        }
        else
          depBlock.AddLabel("Руководитель не указан");
        
        var status = Sungero.Company.BusinessUnits.Info.Properties.Status.GetLocalizedValue(dep.Status);
        if (!string.IsNullOrEmpty(status))
          Sungero.Docflow.PublicFunctions.Module.AddInfoToRightContent(depBlock, $"Состояние: {status}");
        
        AddSubDepartment(depBlock, dep, allBusinessUnits, allDepartments);
      }
      
      var subBU = allBusinessUnits.Where(x => x.HeadCompany == businessUnit).AsEnumerable();
      foreach (var bu in subBU)
      {
        var parentBlock = taskBlock.AddChildBlock();
        //parentBlock.IsExpanded = true;
        parentBlock.AssignIcon(BusinessUnits.Resources.BusinessUnitIcon, StateBlockIconSize.Large);
        parentBlock.AddHyperlink(bu.Name, Hyperlinks.Get(bu));
        parentBlock.AddLineBreak();
        if (bu.CEO != null)
        {
          parentBlock.AddLabel("Руководитель: ");
          parentBlock.AddHyperlink(bu.CEO.Name, Hyperlinks.Get(bu.CEO));
        }
        else
          parentBlock.AddLabel("Руководитель не указан");
        
        var status = Sungero.Company.BusinessUnits.Info.Properties.Status.GetLocalizedValue(bu.Status);
        if (!string.IsNullOrEmpty(status))
          Sungero.Docflow.PublicFunctions.Module.AddInfoToRightContent(parentBlock, $"Состояние: {status}");
        
        AddSubBusinessUnit(parentBlock, bu, allBusinessUnits, allDepartments);
      }
    }
    
    private void AddSubDepartment(Sungero.Core.StateBlock taskBlock, Sungero.Company.IDepartment department, List<Sungero.Company.IBusinessUnit> allBusinessUnits, List<Sungero.Company.IDepartment> allDepartments)
    {
      var subDeps = allDepartments.Where(x => x.HeadOffice == department).AsEnumerable();
      foreach (var dep in subDeps)
      {
        var parentBlock = taskBlock.AddChildBlock();
        parentBlock.AssignIcon(BusinessUnits.Resources.DepartmentIcon, StateBlockIconSize.Small);
        parentBlock.AddHyperlink(dep.Name, Hyperlinks.Get(dep));
        parentBlock.AddLineBreak();
        
        if (dep.Manager != null)
        {
          parentBlock.AddLabel("Руководитель: ");
          parentBlock.AddHyperlink(dep.Manager.Name, Hyperlinks.Get(dep.Manager));
        }
        else
          parentBlock.AddLabel("Руководитель не указан");
        
        var status = Sungero.Company.BusinessUnits.Info.Properties.Status.GetLocalizedValue(dep.Status);
        if (!string.IsNullOrEmpty(status))
          Sungero.Docflow.PublicFunctions.Module.AddInfoToRightContent(parentBlock, $"Состояние: {status}");
        
        AddSubDepartment(parentBlock, dep, allBusinessUnits, allDepartments);
      }
    }
    
    #endregion
    
    /// <summary>
    /// В табличной части Виды ролей получить первого исполнителя.
    /// </summary>
    /// <param name="businessUnit">Наша организация.</param>
    /// <param name="roleKindName">Наименование Вида роли.</param>
    /// <returns>Исполнитель по Виду роли.</returns>
    [Public]
    public static List<Sungero.Company.IEmployee> GetRoleKindPerformer(Sungero.Company.IBusinessUnit businessUnit, string roleKindName)
    {
      var performers = new List<Sungero.Company.IEmployee>();
      var roleKind = lenspec.EtalonDatabooks.RoleKinds.GetAll(x => x.Name == roleKindName).FirstOrDefault();
      if (roleKind != null)
      {
        performers.AddRange(lenspec.Etalon.BusinessUnits.As(businessUnit).RoleKindEmployeelenspec
                            .Where(x => x.RoleKind == roleKind)
                            .Select(x => x.Employee)
                            .ToList());
      }
      return performers;
    }
    
    /// <summary>
    /// Обновить все Наши организации из 1С.
    /// </summary>
    /// <returns>Сообщение о результате загрузки.</returns>
    [Remote]
    public static string UpdateAllBusinessUnits()
    {
      var activeBusinessUnits = Sungero.Company.BusinessUnits.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      var errorMessage = string.Empty;
      var recordsProcessed = 0;
      foreach(var businessUnit in activeBusinessUnits)
      {
        if (!string.IsNullOrEmpty(businessUnit.TIN))
        {
          recordsProcessed++;
          var result = GetBusinessUnitFrom1C(businessUnit.TIN);
          if (!result.Equals(Etalon.BusinessUnits.Resources.BusinessUnitIsChanged))
          {
            errorMessage += businessUnit.TIN + " - " + result + "\r\n";
          }
        }
      }
      
      if (recordsProcessed == 0)
        errorMessage = lenspec.Etalon.BusinessUnits.Resources.TINMissingError;
      
      return errorMessage;
    }
    
    /// <summary>
    /// Создать или изменить организацию по ИНН.
    /// </summary>
    /// <param name="tin">ИНН организации.</param>
    /// <returns>Сообщение о результате загрузки.</returns>
    [Public]
    public static string GetBusinessUnitFrom1C(string tin)
    {
      var settings = Integrationses.GetAll(s => s.Code == Constants.EtalonIntergation.BusinessUnit.OrgSructureImportRecordCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create(lenspec.Etalon.BusinessUnits.Resources.FailedToGetConnectionString);
      
      // инициализируем helper для получения данных из интеграционной базы
      var connectionString = Encryption.Decrypt(settings.ConnectionParams);
      if (string.IsNullOrEmpty(connectionString))
        return lenspec.Etalon.BusinessUnits.Resources.FailedToGetConnectionString;
      
      var businessUnit1C = AvisIntegrationHelper.DataBaseHelper.GetOurOrganisation(connectionString, tin);
      
      if (businessUnit1C == null)
        return BusinessUnits.Resources.OrganizationWithEnteredTinNotFound;
      else
        return DownloadBusinessUnitFrom1C(businessUnit1C);
    }

    /// <summary>
    /// Загрузить Нашу организацию из 1С
    /// Оставил загрузку полей ИНН, КПП, ОГРН, Код1С и Наименование, согласно ТЗ Переработка интеграции между 1С и Directum RX
    /// </summary>
    /// <param name="businessUnit1С">Организация из системы 1С.</param>
    /// <returns>Сообщение о результате загрузки.</returns>
    private static string DownloadBusinessUnitFrom1C(AvisIntegrationHelper.OurOrganisation businessUnit1С)
    {
      var businessUnitRX = Etalon.BusinessUnits.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active &&
                                                       x.TIN.Equals(businessUnit1С.INN)).SingleOrDefault();
      if(businessUnitRX == null)
        return CreateBusinessUnit(businessUnit1С);
      else
        return ChangeBusinessUnit(businessUnitRX, businessUnit1С);
    }
    
    private static string CreateBusinessUnit(AvisIntegrationHelper.OurOrganisation businessUnit1С)
    {
      try
      {
        var businessUnitRX = Etalon.BusinessUnits.Create();
        businessUnitRX.Name = businessUnit1С.NameAn;
        businessUnitRX.TIN = businessUnit1С.INN;
        businessUnitRX.TRRC = businessUnit1С.KPP;
        businessUnitRX.ExternalCodeavis = businessUnit1С.ExternalCode;
        businessUnitRX.PSRN = businessUnit1С.OGRN;
        businessUnitRX.Save();
        
        return Etalon.BusinessUnits.Resources.BusinessUnitIsLoaded;
      }
      catch(Exception)
      {
        return lenspec.Etalon.BusinessUnits.Resources.FailedToCreateBusinessUnit;
      }
    }
    
    private static string ChangeBusinessUnit(Etalon.IBusinessUnit businessUnitRX, AvisIntegrationHelper.OurOrganisation businessUnit1С)
    {
      try
      {
        var lockInfo = Locks.GetLockInfo(businessUnitRX);
        if (lockInfo.IsLocked)
        {
          return lockInfo.IsLockedByMe == true
            ? lenspec.Etalon.BusinessUnits.Resources.CardIsLockedByYou
            : lenspec.Etalon.BusinessUnits.Resources.CardIsLockedByOtherUserFormat(lockInfo.OwnerName);
        }
        else
        {
          businessUnitRX.Name = businessUnit1С.NameAn;
          businessUnitRX.TRRC = businessUnit1С.KPP;
          businessUnitRX.ExternalCodeavis = businessUnit1С.ExternalCode;
          businessUnitRX.PSRN = businessUnit1С.OGRN;
          businessUnitRX.Save();
          
          return Etalon.BusinessUnits.Resources.BusinessUnitIsChanged;
        }
      }
      catch(Exception)
      {
        return Etalon.BusinessUnits.Resources.FailedToChangeBusinessUnit;
      }
    }
    //конец Добавлено Avis Expert.
  }
}