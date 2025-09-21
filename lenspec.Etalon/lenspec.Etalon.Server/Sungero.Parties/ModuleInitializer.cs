using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.Etalon.Module.Parties.Server
{
  // Добавлено avis.
  public partial class ModuleInitializer
  {
    /// <summary>
    /// Инициализация.
    /// </summary>
    /// <param name="e"></param>
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      try
      {
        base.Initializing(e);
        
        // Создание ролей.
        CreateRoles();
        
        // Выдача прав на справочники.
        GrantRightDatabooks();
        
        //Создание видов документов.
        CreateDocumentKinds();
      }
      catch {}
    }
    
    /// <summary>
    /// Создание ролей.
    /// </summary>
    private static new void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles for Parties lenspec");
      
      // Создание роли "Полные права на Архив управляющих компаний (управление домом)".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(Resources.FullPermitArhivJKHName,
                                                                      lenspec.Etalon.Module.Parties.Resources.FullPermitArhivJKHDescription,
                                                                      Constants.Module.FullPermitArhivJKH);
      
      // Создание роли "Права на создание/изменение контрагентов, персон и контактных лиц".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(Resources.RoleCreateCounterpartyName,
                                                                      lenspec.Etalon.Module.Parties.Resources.RoleCreateCounterparteDescription,
                                                                      avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid);
      
      // Создание роли "Права на кнопку "Получить адрес из Dadata" в Контрагентах".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Etalon.Module.Parties.Resources.RoleNameAccessToDadata,
                                                                      string.Empty,
                                                                      avis.EtalonParties.PublicConstants.Module.RoleFillDadataGuid);
      
      // Создание роли "Согласующий сотрудник ДБ".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(Resources.DEBCoordinationgName,
                                                                      Resources.DEBCoordinationgDescription,
                                                                      Constants.Module.DEBCoordinationgGuid);
      
      // Создание роли "Руководство ДБ".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(Resources.DEBManagementName,
                                                                      Resources.DEBManagementDescription,
                                                                      Constants.Module.DEBManagementGuid);
      
      // Создание роли "Согласующий по тендерам".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(Resources.TenderCoordinatorName,
                                                                      Resources.TenderCoordinatorDescription,
                                                                      Constants.Module.TenderCoordinatorGuid);
      
      // Создание роли "Ответственные за тендеры".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(Resources.TenderResponsibleName,
                                                                      Resources.TenderResponsibleDescription,
                                                                      Constants.Module.TenderResponsibleGuid);

      // Создание роли "Ответственные за справочник "Ответственные по контрагентам".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Etalon.Module.Parties.Resources.ResponsibleByCounterpartiesDiadocName,
                                                                      lenspec.Etalon.Module.Parties.Resources.ResponsibleByCounterpartiesDiadocDescription,
                                                                      Constants.Module.ResponsibleByCounterpartiesDiadocGuid);
      
      // Создание роли "Полные права на Клиенты (собственники) для Канцелярий УК".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Etalon.Module.Parties.Resources.IsClientOwnerAccessName,
                                                                      string.Empty,
                                                                      Constants.Module.IsClientOwnerAccessGuid);
      
      // Создание роли "Ответственные за выгрузку контрагентов в 1С".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Etalon.Module.Parties.Resources.ResponsibleUnloadingContractors1CName,
                                                                      lenspec.Etalon.Module.Parties.Resources.ResponsibleUnloadingContractors1CDescription,
                                                                      Constants.Module.ResponsibleUnloadingContractors1CGuid);
      // Создание роли "Полные права на справочник "Банковские реквизиты".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Etalon.Module.Parties.Resources.BankDetailsRoleName,
                                                                      string.Empty,
                                                                      Constants.Module.BankDetailsRoleGuid);
      
      // Создание роли "Ответственные за квалификацию и дисквалификацию контрагентов".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Tenders.Resources.ResponsibleForCounterpartyQualificationRoleName,
                                                                      lenspec.Tenders.Resources.ResponsibleForCounterpartyQualificationRoleDescription,
                                                                      lenspec.Tenders.PublicConstants.Module.ResponsibleForCounterpartyQualificationRole);
    }
    
    /// <summary>
    /// Выдача прав на справочники.
    /// </summary>
    private static void GrantRightDatabooks()
    {
      InitializationLogger.Debug("Init: Grant rights on Databooks for Parties lenspec.");
      
      var allUsers = Sungero.CoreEntities.Roles.AllUsers;
      var roleFullPermitArhivJKH                = Roles.GetAll(r => Equals(r.Sid, Constants.Module.FullPermitArhivJKH)).FirstOrDefault();
      var roleCounterparty                      = Roles.GetAll(r => Equals(r.Sid, avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid)).FirstOrDefault();
      var roleTenderResponsible                 = Roles.GetAll(r => Equals(r.Sid, Constants.Module.TenderResponsibleGuid)).FirstOrDefault();
      var roleResponsibleByCounterpartiesDiadoc = Roles.GetAll(r => Equals(r.Sid, Constants.Module.ResponsibleByCounterpartiesDiadocGuid)).FirstOrDefault();
      var roleIsClientOwnerAccess               = Roles.GetAll(r => Equals(r.Sid, Constants.Module.IsClientOwnerAccessGuid)).FirstOrDefault();
      var roleViewPersonalData                  = Roles.GetAll(r => Equals(r.Sid, lenspec.EtalonDatabooks.PublicConstants.Module.ViewPersonalData)).FirstOrDefault();
      var roleClerks                            = Roles.GetAll(r => Equals(r.Sid, Sungero.Docflow.Constants.Module.RoleGuid.ClerksRole)).FirstOrDefault();
      var roleBankDetails                       = Roles.GetAll(r => Equals(r.Sid, lenspec.Etalon.Module.Parties.PublicConstants.Module.BankDetailsRoleGuid)).FirstOrDefault();
      
      // Выдача прав на справочник "Банковские реквизиты".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", avis.EtalonParties.BankDetails.Info.LocalizedPluralName);
      avis.EtalonParties.BankDetails.AccessRights.Grant(roleBankDetails, DefaultAccessRightsTypes.FullAccess);
      avis.EtalonParties.BankDetails.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Change);
      avis.EtalonParties.BankDetails.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      avis.EtalonParties.BankDetails.AccessRights.Save();
      
      // Выдача прав на справочник "Вид материалов для квалификации поставщиков".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", avis.EtalonParties.MaterialGroups.Info.LocalizedPluralName);
      avis.EtalonParties.MaterialGroups.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      avis.EtalonParties.MaterialGroups.AccessRights.Save();
      
      // Выдача прав на справочник "Детализация видов материалов для квалификации поставщиков".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", avis.EtalonParties.Materials.Info.LocalizedPluralName);
      avis.EtalonParties.Materials.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      avis.EtalonParties.Materials.AccessRights.Save();
      
      // Выдача прав на справочник "Виды работ для квалификации подрядчиков".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", avis.EtalonParties.WorkGroups.Info.LocalizedPluralName);
      avis.EtalonParties.WorkGroups.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      avis.EtalonParties.WorkGroups.AccessRights.Save();
      
      // Выдача прав на справочник "Детализация видов работ для квалификации подрядчиков".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", avis.EtalonParties.WorkKinds.Info.LocalizedPluralName);
      avis.EtalonParties.WorkKinds.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      avis.EtalonParties.WorkKinds.AccessRights.Save();
      
      // Выдача прав на справочник "Персоны".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", Sungero.Parties.People.Info.LocalizedPluralName);
      Sungero.Parties.People.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      Sungero.Parties.People.AccessRights.Grant(roleIsClientOwnerAccess, DefaultAccessRightsTypes.Change);
      Sungero.Parties.People.AccessRights.Grant(roleViewPersonalData, DefaultAccessRightsTypes.Change);
      
      // По требованию ТЗ: убрать права доступа для роли "Полные права на Архив управляющих компаний (управление домом)".
      if (Sungero.Parties.People.AccessRights.IsGranted(DefaultAccessRightsTypes.Create, roleFullPermitArhivJKH))
        Sungero.Parties.People.AccessRights.Revoke(roleFullPermitArhivJKH, DefaultAccessRightsTypes.Create);
      if (Sungero.Parties.People.AccessRights.IsGranted(DefaultAccessRightsTypes.Change, roleFullPermitArhivJKH))
        Sungero.Parties.People.AccessRights.Revoke(roleFullPermitArhivJKH, DefaultAccessRightsTypes.Change);
      Sungero.Parties.People.AccessRights.Save();
      // По требованию ТЗ: убрать права доступа для роли "Делопроизводители".
      if (Sungero.Parties.People.AccessRights.IsGranted(DefaultAccessRightsTypes.Create, roleClerks))
        Sungero.Parties.People.AccessRights.Revoke(roleFullPermitArhivJKH, DefaultAccessRightsTypes.Create);
      if (Sungero.Parties.People.AccessRights.IsGranted(DefaultAccessRightsTypes.Change, roleClerks))
        Sungero.Parties.People.AccessRights.Revoke(roleFullPermitArhivJKH, DefaultAccessRightsTypes.Change);
      Sungero.Parties.People.AccessRights.Save();
      
      // Выдача прав на справочник "Организации".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", Sungero.Parties.Companies.Info.LocalizedPluralName);
      Sungero.Parties.Companies.AccessRights.Grant(roleCounterparty, DefaultAccessRightsTypes.Create);
      Sungero.Parties.Companies.AccessRights.Grant(roleCounterparty, DefaultAccessRightsTypes.Change);
      Sungero.Parties.Companies.AccessRights.Grant(roleTenderResponsible, DefaultAccessRightsTypes.Change);
      Sungero.Parties.Companies.AccessRights.Save();
      
      // Выдача прав на справочник "Группы контрагентов".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", avis.EtalonParties.GroupCounterparties.Info.LocalizedPluralName);
      avis.EtalonParties.GroupCounterparties.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      avis.EtalonParties.GroupCounterparties.AccessRights.Grant(roleCounterparty, DefaultAccessRightsTypes.Change);
      avis.EtalonParties.GroupCounterparties.AccessRights.Save();
      
      // Выдача прав на справочник "Категории контрагентов".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", avis.EtalonParties.CategoryCounterparties.Info.LocalizedPluralName);
      avis.EtalonParties.CategoryCounterparties.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      avis.EtalonParties.CategoryCounterparties.AccessRights.Grant(roleCounterparty, DefaultAccessRightsTypes.Change);
      avis.EtalonParties.CategoryCounterparties.AccessRights.Save();
      
      // Выдача прав на справочник "Банки".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", Sungero.Parties.Banks.Info.LocalizedPluralName);
      Sungero.Parties.Banks.AccessRights.Grant(roleCounterparty, DefaultAccessRightsTypes.Change);
      Sungero.Parties.Banks.AccessRights.Save();
      
      // Выдача прав на справочник "Ответственные по контрагентам".
      InitializationLogger.DebugFormat("Init: Grant rights on \"{0}\".", avis.EtalonParties.ResponsibleByCounterparties.Info.LocalizedPluralName);
      avis.EtalonParties.ResponsibleByCounterparties.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      avis.EtalonParties.ResponsibleByCounterparties.AccessRights.Grant(roleResponsibleByCounterpartiesDiadoc, DefaultAccessRightsTypes.Change);
      avis.EtalonParties.ResponsibleByCounterparties.AccessRights.Save();
    }
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      InitializationLogger.Debug("Init: Create DocumentKinds for Parties lenspec.");
      
      // Создание вида документа «Учредительный документ».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(Module.Parties.Resources.ConstituentDocumentKindName,
                                                                              Module.Parties.Resources.ConstituentDocumentKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true,
                                                                              false,
                                                                              Sungero.Docflow.Server.CounterpartyDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              false,
                                                                              false,
                                                                              Constants.Module.ConstituentDocumentKind,
                                                                              false);
      // Создание вида документа «Устав и изменения».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(Module.Parties.Resources.CharterAndChangesDocumentKindName,
                                                                              Module.Parties.Resources.CharterAndChangesDocumentKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true,
                                                                              false,
                                                                              Sungero.Docflow.Server.CounterpartyDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              false,
                                                                              false,
                                                                              Constants.Module.CharterAndChangesKind,
                                                                              false);
      // Создание вида документа «Выписка из ЕГРЮЛ/ЕГРИП».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(Module.Parties.Resources.ExtractFromEGRULDocumentKindName,
                                                                              Module.Parties.Resources.ExtractFromEGRULDocumentKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true,
                                                                              false,
                                                                              Sungero.Docflow.Server.CounterpartyDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              false,
                                                                              false,
                                                                              Constants.Module.ExtractFromEGRULKind,
                                                                              false);
    }
  }
  // Конец добавлено avis.
}
