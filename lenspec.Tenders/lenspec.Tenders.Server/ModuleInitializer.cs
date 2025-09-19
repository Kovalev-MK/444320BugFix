using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.Tenders.Server
{
  public partial class ModuleInitializer
  {

    public override bool IsModuleVisible()
    {
      return Users.Current.IncludedIn(Constants.Module.TenderCommitteeProtocolCreatingRole) ||
        Users.Current.IncludedIn(Constants.Module.ScanOfTenderCommitteeProtocolCreatingRole) ||
        Users.Current.IncludedIn(Constants.Module.TenderCommitteeProtocolReadingRole) ||
        Users.Current.IncludedIn(Constants.Module.TenderCommitteeAccessRole);
    }

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Создание ролей.
      CreateRoles();
      
      // Справочники.
      GrantRightsOnDatabooks();
      
      // Создание типов документов.
      CreateDocumentTypes();
      
      // Создание видов документов.
      CreateDocumentKinds();
      
      // Документы.
      GrantRightsOnDocuments();
      
      // Вычисляемые папки.
      GrantRightsOnFolders();
      
      // Роли согласования.
      CreateApprovalRole(lenspec.EtalonDatabooks.ComputedRole.Type.TenderChairman,
                         lenspec.EtalonDatabooks.ComputedRoles.Info.Properties.Type.GetLocalizedValue(lenspec.EtalonDatabooks.ComputedRole.Type.TenderChairman));
      CreateApprovalRole(lenspec.EtalonDatabooks.ComputedRole.Type.TenderRegistrar,
                         lenspec.EtalonDatabooks.ComputedRoles.Info.Properties.Type.GetLocalizedValue(lenspec.EtalonDatabooks.ComputedRole.Type.TenderRegistrar));
      CreateApprovalRole(lenspec.EtalonDatabooks.ComputedRole.Type.TenderMembers,
                         lenspec.EtalonDatabooks.ComputedRoles.Info.Properties.Type.GetLocalizedValue(lenspec.EtalonDatabooks.ComputedRole.Type.TenderMembers));
      CreateApprovalRole(lenspec.EtalonDatabooks.ComputedRole.Type.AccreditationChairman,
                         lenspec.EtalonDatabooks.ComputedRoles.Info.Properties.Type.GetLocalizedValue(lenspec.EtalonDatabooks.ComputedRole.Type.AccreditationChairman));
      CreateApprovalRole(lenspec.EtalonDatabooks.ComputedRole.Type.AccreditationRegistrator,
                         lenspec.EtalonDatabooks.ComputedRoles.Info.Properties.Type.GetLocalizedValue(lenspec.EtalonDatabooks.ComputedRole.Type.AccreditationRegistrator));
      CreateApprovalRole(lenspec.EtalonDatabooks.ComputedRole.Type.AccreditationMembers,
                         lenspec.EtalonDatabooks.ComputedRoles.Info.Properties.Type.GetLocalizedValue(lenspec.EtalonDatabooks.ComputedRole.Type.AccreditationMembers));
      
      // Создание временных таблиц.
      CreateTables();
      
			// Задачи.
      GrantRightsOnTasks();
    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles");
      
      // Права на просмотр всех протоколов тендерного комитета.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Tenders.Resources.TenderCommitteeProtocolReadingRoleName,
                                                                      string.Empty,
                                                                      Constants.Module.TenderCommitteeProtocolReadingRole);
      
      // Права на создание протоколов тендерного комитета.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Tenders.Resources.TenderCommitteeProtocolCreatingRoleName,
                                                                      lenspec.Tenders.Resources.TenderCommitteeProtocolCreatingRoleDescription,
                                                                      Constants.Module.TenderCommitteeProtocolCreatingRole);
      
      // Полные права на справочник Тендерные комитеты.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Tenders.Resources.TenderCommitteeAccessRoleName,
                                                                      string.Empty,
                                                                      Constants.Module.TenderCommitteeAccessRole);
      
      // Права на вложение сканов протоколов тендерного комитета.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Tenders.Resources.ScanOfTenderCommitteeProtocolCreatingRoleName,
                                                                      string.Empty,
                                                                      Constants.Module.ScanOfTenderCommitteeProtocolCreatingRole);
      
      // Права на просмотр тендерной документации.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Tenders.Resources.TenderDocumentsReadingRoleRoleName,
                                                                      lenspec.Tenders.Resources.TenderDocumentsReadingRoleRoleDescription,
                                                                      Constants.Module.TenderDocumentsReadingRoleRole);
      
      // Права на просмотр всех протоколов комитета аккредитации.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.Tenders.Resources.AccreditationCommitteeProtocolReadingRoleName,
                                                                      lenspec.Tenders.Resources.AccreditationCommitteeProtocolReadingRoleDescription,
                                                                      Constants.Module.AccreditationCommitteeProtocolReadingRole);
      
      // Ответственные за квалификацию и дисквалификацию контрагентов.
      // Инициализация роли вынесена в lenspec.Etalon.Module.Parties, где использована для выдачи прав на справочники.
    }
    
    /// <summary>
    /// Выдать права на справочники.
    /// </summary>
    public static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Grant rights on databooks");
      
      var tenderCommitteeAccessRole = Roles.GetAll(n => n.Sid == Constants.Module.TenderCommitteeAccessRole).SingleOrDefault();
      var tenderCommitteeProtocolCreatingRole = Roles.GetAll(n => n.Sid == Constants.Module.TenderCommitteeProtocolCreatingRole).SingleOrDefault();
      var scanOfTenderCommitteeProtocolCreatingRole = Roles.GetAll(n => n.Sid == Constants.Module.ScanOfTenderCommitteeProtocolCreatingRole).SingleOrDefault();
      var tenderCommitteeProtocolReadingRole = Roles.GetAll(n => n.Sid == Constants.Module.TenderCommitteeProtocolReadingRole).SingleOrDefault();
      var responsibleForCounterpartyQualificationRole = Roles.GetAll(n => n.Sid == Constants.Module.ResponsibleForCounterpartyQualificationRole).SingleOrDefault();
      
      TenderCommittees.AccessRights.Grant(tenderCommitteeAccessRole, DefaultAccessRightsTypes.FullAccess);
      TenderCommittees.AccessRights.Grant(tenderCommitteeProtocolCreatingRole, DefaultAccessRightsTypes.Read);
      TenderCommittees.AccessRights.Grant(scanOfTenderCommitteeProtocolCreatingRole, DefaultAccessRightsTypes.Read);
      TenderCommittees.AccessRights.Grant(tenderCommitteeProtocolReadingRole, DefaultAccessRightsTypes.Read);
      TenderCommittees.AccessRights.Save();
      
      AccreditationCommittees.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Create);
      AccreditationCommittees.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Change);
      AccreditationCommittees.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      AccreditationCommittees.AccessRights.Save();
      
      CounterpartyRegisterBases.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Create);
      CounterpartyRegisterBases.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Change);
      CounterpartyRegisterBases.AccessRights.Save();
      
      avis.EtalonContracts.PresenceRegions.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Create);
      avis.EtalonContracts.PresenceRegions.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Change);
      avis.EtalonContracts.PresenceRegions.AccessRights.Save();
      
      avis.EtalonParties.MaterialGroups.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Change);
      avis.EtalonParties.MaterialGroups.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Create);
      avis.EtalonParties.MaterialGroups.AccessRights.Save();
      
      avis.EtalonParties.Materials.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Change);
      avis.EtalonParties.Materials.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Create);
      avis.EtalonParties.Materials.AccessRights.Save();
      
      avis.EtalonParties.WorkGroups.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Change);
      avis.EtalonParties.WorkGroups.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Create);
      avis.EtalonParties.WorkGroups.AccessRights.Save();
      
      avis.EtalonParties.WorkKinds.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Change);
      avis.EtalonParties.WorkKinds.AccessRights.Grant(responsibleForCounterpartyQualificationRole, DefaultAccessRightsTypes.Create);
      avis.EtalonParties.WorkKinds.AccessRights.Save();
    }
    
    /// <summary>
    /// Создание типов документов в служебном справочнике "Типы документов".
    /// </summary>
    public static void CreateDocumentTypes()
    {
      InitializationLogger.Debug("Init: Create Document types");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.Tenders.Resources.TenderCommitteeProtocol,
                                                                              TenderCommitteeProtocol.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.Tenders.Resources.TenderDocument,
                                                                              TenderDocument.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.Tenders.Resources.TenderQualificationForm,
                                                                              TenderAccreditationForm.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.Tenders.Resources.AccreditationCommitteeProtocol,
                                                                              AccreditationCommitteeProtocol.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
    }
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      InitializationLogger.Debug("Init: Create Document kinds");
      
      // Создание вида документа «Протокол тендерного комитета».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Tenders.Resources.TenderCommitteeProtocol,
                                                                              lenspec.Tenders.Resources.TenderCommitteeProtocol,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              TenderCommitteeProtocol.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.TenderCommitteeProtocolKind,
                                                                              true);
      // Создание вида документа «Сводная таблица коммерческих предложений».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Tenders.Resources.CommercialOffersSummary,
                                                                              lenspec.Tenders.Resources.CommercialOffersSummary,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              TenderDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.CommercialOffersSummaryKind,
                                                                              false);
      // Создание вида документа «Протокол квалификационного отбора».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Tenders.Resources.QualificationSelectionProtocol,
                                                                              lenspec.Tenders.Resources.QualificationSelectionProtocol,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              TenderDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.QualificationSelectionProtocolKind,
                                                                              false);
      // Создание вида документа «Анкета квалификации».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Tenders.Resources.TenderQualificationForm,
                                                                              lenspec.Tenders.Resources.TenderQualificationForm,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              TenderAccreditationForm.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[]
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval,
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendActionItem
                                                                              },
                                                                              Constants.Module.TenderAccreditationFormKind,
                                                                              true);
      // Создание вида документа «Протокол комитета аккредитации».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Tenders.Resources.AccreditationCommitteeProtocol,
                                                                              lenspec.Tenders.Resources.AccreditationCommitteeProtocol,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              AccreditationCommitteeProtocol.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[]
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval
                                                                              },
                                                                              Constants.Module.AccreditationCommitteeProtocolKind,
                                                                              true);
      
      // Создание вида документа «Заявка на закупку».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Tenders.Resources.PurchaseRequisition,
                                                                              lenspec.Tenders.Resources.PurchaseRequisition,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              TenderDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[]
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval
                                                                              },
                                                                              Constants.Module.PurchaseRequisitionKind,
                                                                              false);

      // Создание вида документа «Решение комитета по квалификации включения контрагента в реестр».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Tenders.Resources.DecisionOnInclusionOfCounterparty,
                                                                              lenspec.Tenders.Resources.DecisionOnInclusionOfCounterparty,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              TenderDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[]
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForAcquaintance
                                                                              },
                                                                              Constants.Module.DecisionOnInclusionOfCounterpartyKind,
                                                                              false);
      
      // Создание вида документа «Решение комитета по квалификации исключения контрагента из реестра».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Tenders.Resources.DecisionOnExclusionOfCounterparty,
                                                                              lenspec.Tenders.Resources.DecisionOnExclusionOfCounterparty,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              TenderDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[]
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForAcquaintance
                                                                              },
                                                                              Constants.Module.DecisionOnExclusionOfCounterpartyKind,
                                                                              false);
      // Создание вида документа «Служебная записка на включение в реестр».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Tenders.Resources.MemoOnInclusionOfCounterparty,
                                                                              lenspec.Tenders.Resources.MemoOnInclusionOfCounterparty,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              TenderDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[]
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForAcquaintance
                                                                              },
                                                                              Constants.Module.MemoOnInclusionOfCounterpartyKind,
                                                                              false);
      // Создание вида документа «Служебная записка на исключение из реестра».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Tenders.Resources.MemoOnExclusionOfCounterparty,
                                                                              lenspec.Tenders.Resources.MemoOnExclusionOfCounterparty,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              TenderDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[]
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForAcquaintance
                                                                              },
                                                                              Constants.Module.MemoOnExclusionOfCounterpartyKind,
                                                                              false);
    }
    
    /// <summary>
    /// Выдать права на документы.
    /// </summary>
    public static void GrantRightsOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant rights on documents");
      
      var tenderCommitteeProtocolCreatingRole = Roles.GetAll(n => n.Sid == Constants.Module.TenderCommitteeProtocolCreatingRole).SingleOrDefault();
      var scanOfTenderCommitteeProtocolCreatingRole = Roles.GetAll(n => n.Sid == Constants.Module.ScanOfTenderCommitteeProtocolCreatingRole).SingleOrDefault();
      var tenderCommitteeProtocolReadingRole = Roles.GetAll(n => n.Sid == Constants.Module.TenderCommitteeProtocolReadingRole).SingleOrDefault();
      var tenderDocumentsReadingRoleRole = Roles.GetAll(n => n.Sid == Constants.Module.TenderDocumentsReadingRoleRole).SingleOrDefault();
      var tenderResponsible = Roles.GetAll(n => n.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.TenderResponsibleGuid).SingleOrDefault();
      var accreditationCommitteeProtocolReadingRole = Roles.GetAll(n => n.Sid == Constants.Module.AccreditationCommitteeProtocolReadingRole).SingleOrDefault();
      
      TenderCommitteeProtocols.AccessRights.Grant(tenderCommitteeProtocolCreatingRole, DefaultAccessRightsTypes.Create);
      TenderCommitteeProtocols.AccessRights.Grant(scanOfTenderCommitteeProtocolCreatingRole, DefaultAccessRightsTypes.Create);
      TenderCommitteeProtocols.AccessRights.Grant(tenderCommitteeProtocolReadingRole, DefaultAccessRightsTypes.Read);
      TenderCommitteeProtocols.AccessRights.Save();
      
      TenderDocuments.AccessRights.Grant(tenderCommitteeProtocolCreatingRole, DefaultAccessRightsTypes.Create);
      TenderDocuments.AccessRights.Grant(scanOfTenderCommitteeProtocolCreatingRole, DefaultAccessRightsTypes.Create);
      TenderDocuments.AccessRights.Grant(tenderCommitteeProtocolReadingRole, DefaultAccessRightsTypes.Read);
      TenderDocuments.AccessRights.Save();
      
      TenderAccreditationForms.AccessRights.Grant(tenderResponsible, DefaultAccessRightsTypes.Create);
      TenderAccreditationForms.AccessRights.Grant(tenderDocumentsReadingRoleRole, DefaultAccessRightsTypes.Read);
      TenderAccreditationForms.AccessRights.Save();
      
      AccreditationCommitteeProtocols.AccessRights.Grant(tenderResponsible, DefaultAccessRightsTypes.Create);
      AccreditationCommitteeProtocols.AccessRights.Grant(accreditationCommitteeProtocolReadingRole, DefaultAccessRightsTypes.Read);
      AccreditationCommitteeProtocols.AccessRights.Save();
    }
    
    /// <summary>
    /// Создание роли согласования.
    /// </summary>
    public static void CreateApprovalRole(Enumeration roleType, string description)
    {
      var role = lenspec.EtalonDatabooks.ComputedRoles.GetAll().Where(r => Equals(r.Type, roleType)).FirstOrDefault();
      // Проверяет наличие роли.
      if (role == null)
      {
        role = lenspec.EtalonDatabooks.ComputedRoles.Create();
        role.Type = roleType;
      }
      role.Description = description;
      role.Save();
      InitializationLogger.Debug($"Approval role '{description}' created");
    }
    
    public static void CreateTables()
    {
      InitializationLogger.Debug("Init: Create table MaterialsAndWorkKinds");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.SuppliersOfMaterialsAndServices.MaterialsAndWorkKindsTableName });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Tenders.Queries.SuppliersOfMaterialsAndServices.CreateMaterilasAndWorkKindsTable,
                                                                     new[] { Constants.SuppliersOfMaterialsAndServices.MaterialsAndWorkKindsTableName });
    }
    
    /// <summary>
    /// Выдать права на задачи.
    /// </summary>
    public static void GrantRightsOnTasks()
    {
      InitializationLogger.Debug("Init: Grant rights on tasks");
      
      var responsibleForQualificationRole = Roles.GetAll(n => n.Sid == Constants.Module.ResponsibleForCounterpartyQualificationRole).SingleOrDefault();
      
      ApprovalCounterpartyRegisterTasks.AccessRights.Grant(responsibleForQualificationRole, DefaultAccessRightsTypes.Create);
      ApprovalCounterpartyRegisterTasks.AccessRights.Save();
	}
	
		/// </summary>
    /// Выдать права на вычисляемые папки.
    /// </summary>
    public static void GrantRightsOnFolders()
    {
      InitializationLogger.Debug("Init: Grant rights on folders.");
      
      Tenders.SpecialFolders.QualificationDocuments.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      Tenders.SpecialFolders.QualificationDocuments.AccessRights.Save();
    }
    
  }
}
