using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;
using Sungero.Domain.Shared;

namespace lenspec.EtalonDatabooks.Server
{
  public partial class ModuleInitializer
  {
    //Добавлено Avis Expert.
    
    /// <summary>
    /// Инициализация.
    /// </summary>
    /// <param name="e"></param>
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Создание констант в справочнике ConstantDatabook.
      InitConstantDatabooks();
      
      // Создание ролей.
      CreateRoles();
      
      //Создание записей справочника Виды ролей
      CreateRoleKinds();
      
      //Создание ролей согласования.
      CreateApprovalRoles();
      
      //Создание типов документов.
      CreateDocumentTypes();
      
      //Создание видов документов.
      CreateDocumentKinds();
      
      // Выдача прав на документы.
      GrantRightsOnDocuments();
      
      // Выдача прав на справочники.
      GrantRightsOnDatabooks();
      
      // Вычисляемые папки.
      GrantRightsOnFolder();
      
      #region Настройки папок потока
      
      InitEtalonTaskType();
      InitEtalonAssignmentType();
      
      #endregion
      
      // Справочник Место работы.
      InitPlaceOfWork();
      
      #region Сценарии
      
      // Создать этап преобразования в PDF с простановкой множественных штампов.
      CreateApprovalMultipleStamps();
      
      // Создание записи нового типа сценария "Создавать промежуточную версию в PDF".
      CreateApprovalNewVersionConvertPDF();
      
      #endregion
    }
    
    /// <summary>
    /// Инициализация справочника "ConstantDatabook".
    /// </summary>
    public static void InitConstantDatabooks()
    {
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.AutonumerationAttachmentPostalItemCode, lenspec.EtalonDatabooks.Resources.AutonumerationAttachmentPostalItemConstantName, string.Empty, true, "0");
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.InvestInegrationLogCode, lenspec.EtalonDatabooks.Resources.InvestInegrationLogConstantName, lenspec.EtalonDatabooks.Resources.InvestInegrationLogConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.MassMailingEmailAddressCode, lenspec.EtalonDatabooks.Resources.MassMailingEmailAddressConstantName, lenspec.EtalonDatabooks.Resources.MassMailingEmailAddressConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.BarcodePathCode, lenspec.EtalonDatabooks.Resources.BarcodePathConstantName, lenspec.EtalonDatabooks.Resources.BarcodePathConstantComment, false, @"C:\\Barcode");
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.FormationIncomingLettersWithinGroupFunctionsCode, lenspec.EtalonDatabooks.Resources.FormationIncomingLettersWithinGroupFunctionsConstantName, lenspec.EtalonDatabooks.Resources.FormationIncomingLettersWithinGroupFunctionsConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.FlowFoldersSalesDepartmentsFixCode, lenspec.EtalonDatabooks.Resources.FlowFoldersSalesDepartmentsFixConstantName, lenspec.EtalonDatabooks.Resources.FlowFoldersSalesDepartmentsFixConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.MaxNumberOfAcquaintanceTaskPerformers, lenspec.EtalonDatabooks.Resources.MaxNumberOfAcquaintanceTaskPerformersConstantName, lenspec.EtalonDatabooks.Resources.MaxNumberOfAcquaintanceTaskPerformersConstantComment, false, "500");
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.MassMailingBCCEmailCode, lenspec.EtalonDatabooks.Resources.MassMailingBCCEmailConstantName, lenspec.EtalonDatabooks.Resources.MassMailingBCCEmailConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.DepartmentForClosedEmployees, lenspec.EtalonDatabooks.Resources.DepartmentForClosedEmployeesConstantName, lenspec.EtalonDatabooks.Resources.DepartmentForClosedEmployeesConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.ApplicationForPaymentBusinessUnitId, lenspec.EtalonDatabooks.Resources.ApplicationForPaymentBusinessUnitIdConstantName, string.Empty, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.RenewalInstruction, lenspec.EtalonDatabooks.Resources.RenewalInstructionConstantsName, lenspec.EtalonDatabooks.Resources.RenewalInstructionConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.CancellationInstruction, lenspec.EtalonDatabooks.Resources.CancellationInstructionConstantName, lenspec.EtalonDatabooks.Resources.CancellationInstructionConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.SettingInstruction, lenspec.EtalonDatabooks.Resources.SettingInstructionConstantName, lenspec.EtalonDatabooks.Resources.SettingInstructionConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.TenderDocsMigrationUserId, lenspec.EtalonDatabooks.Resources.TenderDocsMigrationUserIdConstName, lenspec.EtalonDatabooks.Resources.TenderDocsMigrationUserIdConstComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.BussinessUnitsForFilteringByBudget, lenspec.EtalonDatabooks.Resources.BussinessUnitsForFilteringByBudgetName, lenspec.EtalonDatabooks.Resources.BussinessUnitsForFilteringByBudgetComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.SharedConstructionBudgetItemTransit, lenspec.EtalonDatabooks.Resources.SharedConstructionBudgetItemTransitConstantName, lenspec.EtalonDatabooks.Resources.SharedConstructionBudgetItemTransitConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.SharedConstructionBudgetItemTransitDecoding, lenspec.EtalonDatabooks.Resources.SharedConstructionBudgetItemTransitDecodingConstantName, lenspec.EtalonDatabooks.Resources.SharedConstructionBudgetItemTransitDecodingConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.DirectorateTransit, lenspec.EtalonDatabooks.Resources.DirectorateTransitConstantName, lenspec.EtalonDatabooks.Resources.DirectorateTransitConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.DepartmentTransit, lenspec.EtalonDatabooks.Resources.DepartmentTransitConstantName, lenspec.EtalonDatabooks.Resources.DepartmentTransitConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.DirectorateRegionTransit, lenspec.EtalonDatabooks.Resources.DirectorateRegionTransitConstantName, lenspec.EtalonDatabooks.Resources.DirectorateRegionTransitConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.SharedConstructionBudgetItemSubsidy, lenspec.EtalonDatabooks.Resources.SharedConstructionBudgetItemSubsidyConstantName, lenspec.EtalonDatabooks.Resources.SharedConstructionBudgetItemSubsidyConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.DirectorateSubsidy, lenspec.EtalonDatabooks.Resources.DirectorateSubsidyConstantName, lenspec.EtalonDatabooks.Resources.DirectorateSubsidyConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.ContractStatementC2DocumentKind, lenspec.EtalonDatabooks.Resources.ContractStatementC2DocumentKindConstantName, lenspec.EtalonDatabooks.Resources.ContractStatementC2DocumentKindConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.ContractStatementC3DocumentKind, lenspec.EtalonDatabooks.Resources.ContractStatementC3DocumentKindConstantName, lenspec.EtalonDatabooks.Resources.ContractStatementC3DocumentKindConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.IncomeContractType, lenspec.EtalonDatabooks.Resources.IncomeContractTypeConstantName, lenspec.EtalonDatabooks.Resources.IncomeContractTypeConstantComment, false, string.Empty);
      CreateConstants(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.ExpensiveContractType, lenspec.EtalonDatabooks.Resources.ExpensiveContractTypeConstantName, lenspec.EtalonDatabooks.Resources.ExpensiveContractTypeConstantComment, false, string.Empty);
      // Номер первой константы.
      var n = 1;
      // Код первой константы.
      var code = 11;
      // 30 Констант под гиперссылки.
      while (n <= 30)
        CreateConstants(code++.ToString("0000"), ConstantDatabooks.Resources.EditableHyperlinkFormat(n++), string.Empty, false, string.Empty);
    }
    
    /// <summary>
    /// Создание записи контанты в справочнике "ConstantDatabook".
    /// </summary>
    /// <param name="code">Уникальный код, из константы справочника "ConstantDatabook".</param>
    /// <param name="name">Название.</param>
    /// <param name="comment">Комментарий.</param>
    /// <param name="isNumeration">Автонумеруемый.</param>
    /// /// <param name="isNumeration">Знчение.</param>
    private static void CreateConstants(string code, string name, string comment, bool isNumeration, string value)
    {
      var constant = ConstantDatabooks.GetAll(c => c.Code == code).FirstOrDefault();
      
      if (constant == null)
      {
        var newConstant = ConstantDatabooks.Create();
        newConstant.Code = code;
        newConstant.Name = name;
        newConstant.Comment = comment;
        newConstant.IsNumeration = isNumeration;
        newConstant.Value = value;
        
        if (isNumeration == true)
          newConstant.Value = "0";
        
        newConstant.Save();
      }
    }
    
    /// <summary>
    /// Выдать прав на справочники.
    /// </summary>
    public static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Grant rights on databooks.");
      
      // Полные права на справочник ИСП и документы РНС, РНВ.
      var roleFullPermitProject = Roles
        .GetAll(r => r.Sid == Constants.Module.FullPermitISPAndRNSAndPNV)
        .FirstOrDefault();
      // Права на создание/изменение контрагентов, персон и контактных лиц.
      var roleCreateCounterparty = Roles
        .GetAll(r => r.Sid == avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid)
        .FirstOrDefault();
      // Ответственные за контрагентов.
      var roleCounterpartiesResponsible = Roles
        .GetAll(r => r.Sid == Sungero.Docflow.PublicConstants.Module.RoleGuid.CounterpartiesResponsibleRole)
        .FirstOrDefault();
      
      // Районы.
      lenspec.EtalonDatabooks.Areas.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      lenspec.EtalonDatabooks.Areas.AccessRights.Grant(roleCreateCounterparty, DefaultAccessRightsTypes.FullAccess);
      // Регионы.
      Sungero.Commons.Regions.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      Sungero.Commons.Regions.AccessRights.Grant(roleCreateCounterparty, DefaultAccessRightsTypes.FullAccess);
      Sungero.Commons.Regions.AccessRights.RevokeAll(roleCounterpartiesResponsible);
      Sungero.Commons.Regions.AccessRights.Grant(roleCounterpartiesResponsible, DefaultAccessRightsTypes.Read);
      // Округа.
      Districts.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      Districts.AccessRights.Grant(roleCreateCounterparty, DefaultAccessRightsTypes.FullAccess);
      // Населенные пункты.
      Sungero.Commons.Cities.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      Sungero.Commons.Cities.AccessRights.Grant(roleCreateCounterparty, DefaultAccessRightsTypes.FullAccess);
      Sungero.Commons.Cities.AccessRights.RevokeAll(roleCounterpartiesResponsible);
      Sungero.Commons.Cities.AccessRights.Grant(roleCounterpartiesResponsible, DefaultAccessRightsTypes.Read);
      // Объекты проектов.
      ObjectAnProjects.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ObjectAnProjects.AccessRights.Grant(roleFullPermitProject, DefaultAccessRightsTypes.FullAccess);
      // ИСП.
      OurCFs.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      OurCFs.AccessRights.Grant(roleFullPermitProject, DefaultAccessRightsTypes.FullAccess);
      
      JobTitleKinds.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ApprovalStageInstructions.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      RoleKinds.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ApprovalSettings.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      PurposeOfPremiseses.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ObjectAnSales.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ConstantDatabooks.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      FlowFolderSettings.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      PlaceOfWorks.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      CompanyProfiles.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      
      lenspec.EtalonDatabooks.Areas.AccessRights.Save();
      Sungero.Commons.Regions.AccessRights.Save();
      Districts.AccessRights.Save();
      Sungero.Commons.Cities.AccessRights.Save();
      ObjectAnProjects.AccessRights.Save();
      OurCFs.AccessRights.Save();
      JobTitleKinds.AccessRights.Save();
      ApprovalStageInstructions.AccessRights.Save();
      RoleKinds.AccessRights.Save();
      ApprovalSettings.AccessRights.Save();
      OurCFs.AccessRights.Save();
      PurposeOfPremiseses.AccessRights.Save();
      ObjectAnProjects.AccessRights.Save();
      ObjectAnSales.AccessRights.Save();
      ConstantDatabooks.AccessRights.Save();
      FlowFolderSettings.AccessRights.Save();
      PlaceOfWorks.AccessRights.Save();
      CompanyProfiles.AccessRights.Save();
    }
    
    /// <summary>
    /// Выдать права всем пользователям на документы.
    /// </summary>
    public static void GrantRightsOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant rights on documents.");
      var roleFullPermitProject = Sungero.CoreEntities.Roles
        .GetAll(r => r.Sid == Constants.Module.FullPermitISPAndRNSAndPNV)
        .FirstOrDefault();
      var roleResponsibleContractualTemplates = Sungero.CoreEntities.Roles
        .GetAll(r => r.Sid == EtalonDatabooks.Constants.Module.ResponsibleContractualTemplates)
        .FirstOrDefault();
      
      Sungero.Docflow.DocumentTemplates.AccessRights.Grant(roleResponsibleContractualTemplates, DefaultAccessRightsTypes.Create);
      EtalonDatabooks.ApplicationBUCreationDocuments.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.ApplicationBUEditingDocuments.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.ProjectObjectPermitDocuments.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.ProjectBuildingPermitDocuments.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ProjectObjectPermitDocuments.AccessRights.Grant(roleFullPermitProject, DefaultAccessRightsTypes.FullAccess);
      ProjectBuildingPermitDocuments.AccessRights.Grant(roleFullPermitProject, DefaultAccessRightsTypes.FullAccess);
      
      Sungero.Docflow.DocumentTemplates.AccessRights.Save();
      EtalonDatabooks.ApplicationBUCreationDocuments.AccessRights.Save();
      EtalonDatabooks.ApplicationBUEditingDocuments.AccessRights.Save();
      EtalonDatabooks.ProjectObjectPermitDocuments.AccessRights.Save();
      EtalonDatabooks.ProjectBuildingPermitDocuments.AccessRights.Save();
      ProjectObjectPermitDocuments.AccessRights.Save();
      ProjectBuildingPermitDocuments.AccessRights.Save();
    }
    
    /// <summary>
    /// Создание типов документов.
    /// </summary>
    public static void CreateDocumentTypes()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.EtalonDatabooks.Resources.ApplicationBUCreationDocumentTypeName,
                                                                              ApplicationBUCreationDocument.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.EtalonDatabooks.Resources.ApplicationBUEditingDocumentTypeName,
                                                                              ApplicationBUEditingDocument.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.EtalonDatabooks.Resources.ProjectObjectPermitDocumentTypeName,
                                                                              ProjectObjectPermitDocument.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.EtalonDatabooks.Resources.ProjectBuildingPermitDocumentTypeName,
                                                                              ProjectBuildingPermitDocument.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
    }
    
    /// <summary>
    /// Создание видов документов.
    /// А так же для проектов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      // Создание вида документа «Заявка на создание Нашей организации (в т.ч. в базе 1С)».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.EtalonDatabooks.Resources.ApplicationBUCreationKindName,
                                                                              lenspec.EtalonDatabooks.Resources.ApplicationBUCreationKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              ApplicationBUCreationDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.ApplicationBUCreationKind);
      // Создание вида документа «Заявка на изменение Нашей организации (в т.ч. в базе 1С)».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.EtalonDatabooks.Resources.ApplicationBUEditingKindName,
                                                                              lenspec.EtalonDatabooks.Resources.ApplicationBUEditingKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              ApplicationBUEditingDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.ApplicationBUEditingKind);
      // Создание вида документа «Разрешение на ввод объекта».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.EtalonDatabooks.Resources.BuildingPermitKindName,
                                                                              lenspec.EtalonDatabooks.Resources.BuildingPermitKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              ProjectBuildingPermitDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.BuildingPermit);
      // Создание вида документа «Разрешение на ввод объекта».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.EtalonDatabooks.Resources.ObjectPermitKindName,
                                                                              lenspec.EtalonDatabooks.Resources.ObjectPermitKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              ProjectObjectPermitDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.ObjectPermit);
    }
    
    /// <summary>
    /// Создание вычисляемых ролей.
    /// </summary>
    public static void CreateApprovalRoles()
    {
      CreateApprovalRole(EtalonDatabooks.ComputedRole.Type.ApprovRoleKind, lenspec.EtalonDatabooks.Resources.RoleDescriptionApproverByRoleKind);
      CreateApprovalRole(EtalonDatabooks.ComputedRole.Type.ManagerTaskCard, lenspec.EtalonDatabooks.Resources.RoleDescriptionManagerFromTaskCard);
      CreateApprovalRole(EtalonDatabooks.ComputedRole.Type.ResponsibleClerk, lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk);
      CreateApprovalRole(EtalonDatabooks.ComputedRole.Type.MailingSender, lenspec.EtalonDatabooks.Resources.RoleDescriptionMailingSender);
      CreateApprovalRole(EtalonDatabooks.ComputedRole.Type.ManagAttornDept, lenspec.EtalonDatabooks.Resources.RoleDescriptionManagerAttorneyDepartment);
      CreateApprovalRole(EtalonDatabooks.ComputedRole.Type.RespClerkPOA, lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerkPOA);
      CreateApprovalRole(EtalonDatabooks.ComputedRole.Type.Attorney, lenspec.EtalonDatabooks.Resources.RoleDescriptionAttorney);
    }
    
    /// <summary>
    /// Создание роли согласования.
    /// </summary>
    [Public]
    public static void CreateApprovalRole(Enumeration roleType, string description)
    {
      InitializationLogger.DebugFormat("Init: Create approval role {0}", description);
      
      var role = ComputedRoles.GetAll().Where(x => Equals(x.Type, roleType)).FirstOrDefault();
      if (role == null)
        role = ComputedRoles.Create();
      
      role.Type = roleType;
      role.Description = description;
      role.Save();

    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.EtalonDatabooks.Resources.RoleNameResponsibleContractualTemplates,
                                                                      lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleContractualTemplates,
                                                                      EtalonDatabooks.Constants.Module.ResponsibleContractualTemplates);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.EtalonDatabooks.Resources.RoleNameAdministratorEDMS,
                                                                      lenspec.EtalonDatabooks.Resources.RoleDescriptionAdministratorEDMS,
                                                                      EtalonDatabooks.Constants.Module.AdministratorEDMS);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.EtalonDatabooks.Resources.RoleNameResponsibleFillingISP,
                                                                      lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleFillingISP,
                                                                      EtalonDatabooks.Constants.Module.ResponsibleFillingISP);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.EtalonDatabooks.Resources.RoleNameForOurCF,
                                                                      string.Empty,
                                                                      EtalonDatabooks.Constants.Module.FullPermitISPAndRNSAndPNV);
      
      lenspec.Etalon.Module.Docflow.PublicFunctions.Module.CreateRoleAvis(lenspec.EtalonDatabooks.Resources.RoleNameOfficeGK,
                                                                          lenspec.EtalonDatabooks.Resources.RoleDescriptionOfficeGK,
                                                                          EtalonDatabooks.Constants.Module.OfficeGK, true);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.EtalonDatabooks.Resources.RoleNameOfficeAssignment,
                                                                      lenspec.EtalonDatabooks.Resources.RoleDescriptionOfficeAssignment,
                                                                      EtalonDatabooks.Constants.Module.OfficeAssignment);

      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.EtalonDatabooks.Resources.RoleNameFoldersOffice,
                                                                      lenspec.EtalonDatabooks.Resources.RoleDescriptionFoldersOffice,
                                                                      EtalonDatabooks.Constants.Module.FoldersOffice);

      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.EtalonDatabooks.Resources.RoleNameViewPersonalData,
                                                                      lenspec.EtalonDatabooks.Resources.RoleDescriptionViewPersonalData,
                                                                      EtalonDatabooks.Constants.Module.ViewPersonalData);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.EtalonDatabooks.Resources.RoleNameRightsToSelectAnyEmployees,
                                                                      lenspec.EtalonDatabooks.Resources.RoleDescriptionRightsToSelectAnyEmployees,
                                                                      EtalonDatabooks.Constants.Module.RightsToSelectAnyEmployees);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.EtalonDatabooks.Resources.RoleNameRightsToSaveWithoutConstructionObjects,
                                                                      lenspec.EtalonDatabooks.Resources.RoleDescriptionRightsToSaveWithoutConstructionObjects,
                                                                      EtalonDatabooks.Constants.Module.RightsToSaveWithoutConstructionObjects);
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемык папки.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
      InitializationLogger.Debug("Init: Grant rights on folders");
      var foldersOffice = Roles.GetAll(n => n.Sid == EtalonDatabooks.Constants.Module.FoldersOffice).SingleOrDefault();
      
      EtalonDatabooks.SpecialFolders.AppDispatchCourierMail.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.AppDispatchCourierMail.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.CheckingDesignOutgoingEmails.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.CheckingDesignOutgoingEmails.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.RegistrationOutgoingEmails.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.RegistrationOutgoingEmails.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.SendingOutgoingEmails.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.SendingOutgoingEmails.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.ApprovalLocalAct.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.ApprovalLocalAct.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.CheckingLocalAct.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.CheckingLocalAct.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.RegistrationLocalAct.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.RegistrationLocalAct.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.VerificationProtocolCollegialBody.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.VerificationProtocolCollegialBody.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.RegistrationProtocolCollegialBody.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.RegistrationProtocolCollegialBody.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.ApplicationCreatingChangingCounterparties.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.ApplicationCreatingChangingCounterparties.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.SendingOutgoingOtherDeliveryMethods.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.SendingOutgoingOtherDeliveryMethods.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.RegistrationMassMailingApplication.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.RegistrationMassMailingApplication.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.RegistrationIncomingLetter.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.RegistrationIncomingLetter.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.MassSendingOutgoingEmails.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.MassSendingOutgoingEmails.AccessRights.Save();
      
      EtalonDatabooks.SpecialFolders.MassSendingOutgoingOtherDeliveryMethods.AccessRights.Grant(foldersOffice, DefaultAccessRightsTypes.Read);
      EtalonDatabooks.SpecialFolders.MassSendingOutgoingOtherDeliveryMethods.AccessRights.Save();
    }
    
    #region Настройки папок потока
    
    /// <summary>
    /// Инициализация справочника Типы задач.
    /// </summary>
    private static void InitEtalonTaskType()
    {
      InitializationLogger.Debug("Init: Create EtalonTaskType records.");
      // Задача на согласование по регламенту.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.Docflow.ApprovalTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.ApprovalTaskGuid,
                             taskTypeName);
      }
      // Контроль возврата документа.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CheckReturnTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.CheckReturnTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.Docflow.CheckReturnTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.CheckReturnTaskGuid,
                             taskTypeName);
      }
      // Запрос на продление срока.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.DocflowDeadlineExtensionTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.DocflowDeadlineExtensionTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.Docflow.DeadlineExtensionTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.DocflowDeadlineExtensionTaskGuid,
                             taskTypeName);
      }
      // Задача на свободное согласование.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.FreeApprovalTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.FreeApprovalTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.Docflow.FreeApprovalTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.FreeApprovalTaskGuid,
                             taskTypeName);
      }
      // Задача на обработку входящих документов эл. обмена.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ExchangeDocumentProcessingTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ExchangeDocumentProcessingTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.Exchange.ExchangeDocumentProcessingTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.ExchangeDocumentProcessingTaskGuid,
                             taskTypeName);
      }
      // Задача на отправку извещений о получении документов.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReceiptNotificationSendingTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReceiptNotificationSendingTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.Exchange.ReceiptNotificationSendingTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.ReceiptNotificationSendingTaskGuid,
                             taskTypeName);
      }
      // Задача на обработку конфликтов синхронизации контрагентов.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CounterpartyConflictProcessingTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.CounterpartyConflictProcessingTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.ExchangeCore.CounterpartyConflictProcessingTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.CounterpartyConflictProcessingTaskGuid,
                             taskTypeName);
      }
      // Задача на обработку приглашения к эл. обмену от контрагента.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.IncomingInvitationTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.IncomingInvitationTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.ExchangeCore.IncomingInvitationTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.IncomingInvitationTaskGuid,
                             taskTypeName);
      }
      // Задача на ознакомление с документом.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.AcquaintanceTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.AcquaintanceTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.RecordManagement.AcquaintanceTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.AcquaintanceTaskGuid,
                             taskTypeName);
      }
      // Задача на исполнение поручения.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ActionItemExecutionTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ActionItemExecutionTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.RecordManagement.ActionItemExecutionTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.ActionItemExecutionTaskGuid,
                             taskTypeName);
      }
      // Продление срока.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.RecordManagmentDeadlineExtensionTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.RecordManagmentDeadlineExtensionTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.RecordManagement.DeadlineExtensionTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.RecordManagmentDeadlineExtensionTaskGuid,
                             taskTypeName);
      }
      // Задача на рассмотрение документа.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.DocumentReviewTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.DocumentReviewTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.RecordManagement.DocumentReviewTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.DocumentReviewTaskGuid,
                             taskTypeName);
      }
      // Запрос отчета по поручению.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.StatusReportRequestTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.StatusReportRequestTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.RecordManagement.StatusReportRequestTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.StatusReportRequestTaskGuid,
                             taskTypeName);
      }
      // Задача на верификацию комплекта документов.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.VerificationTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.VerificationTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.SmartProcessing.VerificationTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.VerificationTaskGuid,
                             taskTypeName);
      }
      // Заявка на изменение компонентов Directum RX.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.EditComponentRXRequestTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.EditComponentRXRequestTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(lenspec.AutomatedSupportTickets.EditComponentRXRequestTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.EditComponentRXRequestTaskGuid,
                             taskTypeName);
      }
      // Заявка на формирование замещения.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.SubstitutionRequestTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.SubstitutionRequestTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.SubstitutionRequestTaskGuid,
                             taskTypeName);
      }
      // Заявка на редактирование справочника Контрагенты.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CreateCompanyTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.CreateCompanyTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(avis.EtalonParties.CreateCompanyTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.CreateCompanyTaskGuid,
                             taskTypeName);
      }
      // Простая задача
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.SimpleTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.SimpleTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(Sungero.Workflow.SimpleTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.SimpleTaskGuid,
                             taskTypeName);
      }
      // Задача на согласование включения в реестр квалифицированных контрагентов/список дисквалифицированных контрагентов.
      if (!EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalCounterpartyRegisterTaskGuid)).Any())
      {
        var taskTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalCounterpartyRegisterTaskGuid).GetTypeByGuid().ToString();
        CreateEtalonTaskType(lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Info.LocalizedName,
                             Constants.Module.FlowFoldersSetting.ApprovalCounterpartyRegisterTaskGuid,
                             taskTypeName);
      }
    }
    
    private static void CreateEtalonTaskType(string name, string typeGuid, string entityName)
    {
      var taskType = EtalonTaskTypes.Create();
      taskType.Name = name;
      taskType.TaskTypeGuid = typeGuid;
      taskType.EntityName = entityName;
      taskType.Save();
    }
    
    /// <summary>
    /// Инициализация справочника Типы заданий.
    /// </summary>
    private static void InitEtalonAssignmentType()
    {
      InitializationLogger.Debug("Init: Create EtalonAssignmentType records.");
      try
      {
        #region ApprovalTask
        
        var approvalTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalTaskGuid)).SingleOrDefault();
        if (approvalTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalCheckingAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalCheckingAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalCheckingAssignments.Info.LocalizedName + " (с возможностью доработки)",
                                       Constants.Module.FlowFoldersSetting.ApprovalCheckingAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalCheckReturnAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalCheckReturnAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalCheckReturnAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalCheckReturnAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalExecutionAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalExecutionAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalExecutionAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalExecutionAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalManagerAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalManagerAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalManagerAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalManagerAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalPrintingAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalPrintingAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalPrintingAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalPrintingAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalRegistrationAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalRegistrationAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalRegistrationAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalRegistrationAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalReviewAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalReviewAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalReviewAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalReviewAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalReworkAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalReworkAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalReworkAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalReworkAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalSendingAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalSendingAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalSendingAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalSendingAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalSigningAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalSigningAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalSigningAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalSigningAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalSimpleAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalSimpleAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.ApprovalSimpleAssignments.Info.LocalizedName + " (без возможности доработки)",
                                       Constants.Module.FlowFoldersSetting.ApprovalSimpleAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType("Уведомление согласующим о том, что документ был повторно отправлен на согласование",
                                       Constants.Module.FlowFoldersSetting.ApprovalNotificationGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalSimpleNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalSimpleNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType("Уведомление по документу с предопределенной темой",
                                       Constants.Module.FlowFoldersSetting.ApprovalSimpleNotificationGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CheckReturnAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.CheckReturnAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType("Уведомление по документу с предопределенной темой",
                                       Constants.Module.FlowFoldersSetting.CheckReturnAssignmentGuid,
                                       assignmentTypeName,
                                       approvalTask);
          }
          InitializationLogger.Debug("Init: records for ApprovalTask created.");
        }
        
        #endregion
        
        #region CheckReturnTask
        
        var checkReturnTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CheckReturnTaskGuid)).SingleOrDefault();
        if (checkReturnTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CheckReturnAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.CheckReturnAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.CheckReturnAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.CheckReturnAssignmentGuid,
                                       assignmentTypeName,
                                       checkReturnTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CheckReturnCheckAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.CheckReturnCheckAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.CheckReturnCheckAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.CheckReturnCheckAssignmentGuid,
                                       assignmentTypeName,
                                       checkReturnTask);
          }
          InitializationLogger.Debug("Init: records for CheckReturnTask created.");
        }
        
        #endregion
        
        #region DocflowDeadlineExtensionTask
        
        var docflowDeadlineExtensionTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.DocflowDeadlineExtensionTaskGuid)).SingleOrDefault();
        if (docflowDeadlineExtensionTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.DeadlineExtensionAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.DeadlineExtensionAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.DeadlineExtensionAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.DeadlineExtensionAssignmentGuid,
                                       assignmentTypeName,
                                       docflowDeadlineExtensionTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.DeadlineRejectionAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.DeadlineRejectionAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.DeadlineRejectionAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.DeadlineRejectionAssignmentGuid,
                                       assignmentTypeName,
                                       docflowDeadlineExtensionTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.DeadlineExtensionNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.DeadlineExtensionNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.DeadlineExtensionNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.DeadlineExtensionNotificationGuid,
                                       assignmentTypeName,
                                       docflowDeadlineExtensionTask);
          }
          InitializationLogger.Debug("Init: records for DocflowDeadlineExtensionTask created.");
        }
        
        #endregion
        
        #region FreeApprovalTask
        
        var freeApprovalTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.FreeApprovalTaskGuid)).SingleOrDefault();
        if (freeApprovalTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.FreeApprovalAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.FreeApprovalAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.FreeApprovalAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.FreeApprovalAssignmentGuid,
                                       assignmentTypeName,
                                       freeApprovalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.FreeApprovalFinishAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.FreeApprovalFinishAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.FreeApprovalFinishAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.FreeApprovalFinishAssignmentGuid,
                                       assignmentTypeName,
                                       freeApprovalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.FreeApprovalReworkAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.FreeApprovalReworkAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.FreeApprovalReworkAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.FreeApprovalReworkAssignmentGuid,
                                       assignmentTypeName,
                                       freeApprovalTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.FreeApprovalNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.FreeApprovalNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Docflow.FreeApprovalNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.FreeApprovalNotificationGuid,
                                       assignmentTypeName,
                                       freeApprovalTask);
          }
          InitializationLogger.Debug("Init: records for FreeApprovalTask created.");
        }
        
        #endregion
        
        #region ExchangeDocumentProcessingTask
        
        var exchangeDocumentProcessingTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ExchangeDocumentProcessingTaskGuid)).SingleOrDefault();
        if (exchangeDocumentProcessingTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ExchangeDocumentProcessingAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ExchangeDocumentProcessingAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Exchange.ExchangeDocumentProcessingAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ExchangeDocumentProcessingAssignmentGuid,
                                       assignmentTypeName,
                                       exchangeDocumentProcessingTask);
          }
          InitializationLogger.Debug("Init: records for ExchangeDocumentProcessingTask created.");
        }
        
        #endregion
        
        #region ReceiptNotificationSendingTask
        
        var receiptNotificationSendingTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReceiptNotificationSendingTaskGuid)).SingleOrDefault();
        if (receiptNotificationSendingTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReceiptNotificationSendingAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReceiptNotificationSendingAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Exchange.ReceiptNotificationSendingAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ReceiptNotificationSendingAssignmentGuid,
                                       assignmentTypeName,
                                       receiptNotificationSendingTask);
          }
          InitializationLogger.Debug("Init: records for ReceiptNotificationSendingTask created.");
        }
        
        #endregion
        
        #region CounterpartyConflictProcessingTask
        
        var counterpartyConflictProcessingTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CounterpartyConflictProcessingTaskGuid)).SingleOrDefault();
        if (counterpartyConflictProcessingTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CounterpartyConflictProcessingAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.CounterpartyConflictProcessingAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.ExchangeCore.CounterpartyConflictProcessingAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.CounterpartyConflictProcessingAssignmentGuid,
                                       assignmentTypeName,
                                       counterpartyConflictProcessingTask);
          }
          InitializationLogger.Debug("Init: records for CounterpartyConflictProcessingTask created.");
        }
        
        #endregion
        
        #region IncomingInvitationTask
        
        var incomingInvitationTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.IncomingInvitationTaskGuid)).SingleOrDefault();
        if (incomingInvitationTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.IncomingInvitationAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.IncomingInvitationAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.ExchangeCore.IncomingInvitationAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.IncomingInvitationAssignmentGuid,
                                       assignmentTypeName,
                                       incomingInvitationTask);
          }
          InitializationLogger.Debug("Init: records for IncomingInvitationTask created.");
        }
        
        #endregion
        
        #region AcquaintanceTask
        
        var acquaintanceTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.AcquaintanceTaskGuid)).SingleOrDefault();
        if (acquaintanceTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.AcquaintanceAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.AcquaintanceAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.AcquaintanceAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.AcquaintanceAssignmentGuid,
                                       assignmentTypeName,
                                       acquaintanceTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.AcquaintanceFinishAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.AcquaintanceFinishAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.AcquaintanceFinishAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.AcquaintanceFinishAssignmentGuid,
                                       assignmentTypeName,
                                       acquaintanceTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.AcquaintanceCompleteNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.AcquaintanceCompleteNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.AcquaintanceCompleteNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.AcquaintanceCompleteNotificationGuid,
                                       assignmentTypeName,
                                       acquaintanceTask);
          }
          InitializationLogger.Debug("Init: records for AcquaintanceTask created.");
        }
        
        #endregion
        
        #region ActionItemExecutionTask
        
        var actionItemExecutionTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ActionItemExecutionTaskGuid)).SingleOrDefault();
        if (actionItemExecutionTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ActionItemExecutionAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ActionItemExecutionAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ActionItemExecutionAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ActionItemExecutionAssignmentGuid,
                                       assignmentTypeName,
                                       actionItemExecutionTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ActionItemSupervisorAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ActionItemSupervisorAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ActionItemSupervisorAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ActionItemSupervisorAssignmentGuid,
                                       assignmentTypeName,
                                       actionItemExecutionTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ActionItemExecutionNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ActionItemExecutionNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ActionItemExecutionNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ActionItemExecutionNotificationGuid,
                                       assignmentTypeName,
                                       actionItemExecutionTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ActionItemObserversNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ActionItemObserversNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ActionItemObserversNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ActionItemObserversNotificationGuid,
                                       assignmentTypeName,
                                       actionItemExecutionTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ActionItemSupervisorNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ActionItemSupervisorNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ActionItemSupervisorNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ActionItemSupervisorNotificationGuid,
                                       assignmentTypeName,
                                       actionItemExecutionTask);
          }
          InitializationLogger.Debug("Init: records for ActionItemExecutionTask created.");
        }
        
        #endregion
        
        #region RecordManagmentDeadlineExtensionTask
        
        var rmDeadlineExtensionTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.RecordManagmentDeadlineExtensionTaskGuid)).SingleOrDefault();
        if (rmDeadlineExtensionTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.RMDeadlineExtensionAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.RMDeadlineExtensionAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.DeadlineExtensionAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.RMDeadlineExtensionAssignmentGuid,
                                       assignmentTypeName,
                                       rmDeadlineExtensionTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.RMDeadlineRejectionAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.RMDeadlineRejectionAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.DeadlineRejectionAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.RMDeadlineRejectionAssignmentGuid,
                                       assignmentTypeName,
                                       rmDeadlineExtensionTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.RMDeadlineExtensionNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.RMDeadlineExtensionNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.DeadlineExtensionNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.RMDeadlineExtensionNotificationGuid,
                                       assignmentTypeName,
                                       rmDeadlineExtensionTask);
          }
          InitializationLogger.Debug("Init: records for RecordManagmentDeadlineExtensionTask created.");
        }
        
        #endregion
        
        #region DocumentReviewTask
        
        var documentReviewTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.DocumentReviewTaskGuid)).SingleOrDefault();
        if (documentReviewTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.PreparingDraftResolutionAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.PreparingDraftResolutionAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.PreparingDraftResolutionAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.PreparingDraftResolutionAssignmentGuid,
                                       assignmentTypeName,
                                       documentReviewTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReviewDraftResolutionAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReviewDraftResolutionAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ReviewDraftResolutionAssignments.Info.LocalizedName + " (с подготовленным проектом резолюции)",
                                       Constants.Module.FlowFoldersSetting.ReviewDraftResolutionAssignmentGuid,
                                       assignmentTypeName,
                                       documentReviewTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReviewManagerAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReviewManagerAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ReviewManagerAssignments.Info.LocalizedName + " (без проекта резолюции)",
                                       Constants.Module.FlowFoldersSetting.ReviewManagerAssignmentGuid,
                                       assignmentTypeName,
                                       documentReviewTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReviewResolutionAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReviewResolutionAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ReviewResolutionAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ReviewResolutionAssignmentGuid,
                                       assignmentTypeName,
                                       documentReviewTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReviewReworkAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReviewReworkAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ReviewReworkAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ReviewReworkAssignmentGuid,
                                       assignmentTypeName,
                                       documentReviewTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReviewClerkNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReviewClerkNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ReviewClerkNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ReviewClerkNotificationGuid,
                                       assignmentTypeName,
                                       documentReviewTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReviewObserverNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReviewObserverNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ReviewObserverNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ReviewObserverNotificationGuid,
                                       assignmentTypeName,
                                       documentReviewTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReviewObserversNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReviewObserversNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ReviewObserversNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ReviewObserversNotificationGuid,
                                       assignmentTypeName,
                                       documentReviewTask);
          }
          InitializationLogger.Debug("Init: records for DocumentReviewTask created.");
        }
        
        #endregion
        
        #region StatusReportRequestTask
        
        var statusReportRequestTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.StatusReportRequestTaskGuid)).SingleOrDefault();
        if (statusReportRequestTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReportRequestAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReportRequestAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ReportRequestAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ReportRequestAssignmentGuid,
                                       assignmentTypeName,
                                       statusReportRequestTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ReportRequestCheckAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ReportRequestCheckAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.RecordManagement.ReportRequestCheckAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ReportRequestCheckAssignmentGuid,
                                       assignmentTypeName,
                                       statusReportRequestTask);
          }
          InitializationLogger.Debug("Init: records for StatusReportRequestTask created.");
        }
        
        #endregion
        
        #region VerificationTask
        
        var verificationTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.VerificationTaskGuid)).SingleOrDefault();
        if (verificationTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.VerificationAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.VerificationAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.SmartProcessing.VerificationAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.VerificationAssignmentGuid,
                                       assignmentTypeName,
                                       verificationTask);
          }
          InitializationLogger.Debug("Init: records for VerificationTask created.");
        }
        
        #endregion
        
        #region EditComponentRXRequestTask
        
        var editComponentRXRequestTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.EditComponentRXRequestTaskGuid)).SingleOrDefault();
        if (editComponentRXRequestTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalAdministratorGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalAdministratorGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(lenspec.AutomatedSupportTickets.ApprovalAdministrators.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalAdministratorGuid,
                                       assignmentTypeName,
                                       editComponentRXRequestTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalManagerGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalManagerGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(lenspec.AutomatedSupportTickets.ApprovalManagers.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalManagerGuid,
                                       assignmentTypeName,
                                       editComponentRXRequestTask);
          }
          InitializationLogger.Debug("Init: records for EditComponentRXRequestTask created.");
        }
        
        #endregion
        
        #region SubstitutionRequestTask
        
        var substitutionRequestTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.SubstitutionRequestTaskGuid)).SingleOrDefault();
        if (substitutionRequestTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalSubstitutionAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApprovalSubstitutionAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(lenspec.AutomatedSupportTickets.ApprovalSubstitutionAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApprovalSubstitutionAssignmentGuid,
                                       assignmentTypeName,
                                       substitutionRequestTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.SubstitutionRequestNotificationGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.SubstitutionRequestNotificationGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(lenspec.AutomatedSupportTickets.SubstitutionRequestNotifications.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.SubstitutionRequestNotificationGuid,
                                       assignmentTypeName,
                                       substitutionRequestTask);
          }
          InitializationLogger.Debug("Init: records for SubstitutionRequestTask created.");
        }
        
        #endregion
        
        #region CreateCompanyTask
        
        var createCompanyTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CreateCompanyTaskGuid)).SingleOrDefault();
        if (createCompanyTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApproveCounterpartyAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApproveCounterpartyAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(avis.EtalonParties.ApproveCounterpartyAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApproveCounterpartyAssignmentGuid,
                                       assignmentTypeName,
                                       createCompanyTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApproveRevisionGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ApproveRevisionGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(avis.EtalonParties.ApproveRevisions.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ApproveRevisionGuid,
                                       assignmentTypeName,
                                       createCompanyTask);
          }
          InitializationLogger.Debug("Init: records for CreateCompanyTask created.");
        }
        
        #endregion
        
        #region SimpleTask
        
        var simpleTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.SimpleTaskGuid)).SingleOrDefault();
        if (simpleTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.SimpleAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.SimpleAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(Sungero.Workflow.SimpleAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.SimpleAssignmentGuid,
                                       assignmentTypeName,
                                       simpleTask);
          }
          InitializationLogger.Debug("Init: records for SimpleTask created.");
        }
        
        #endregion
        
        #region ApprovalCounterpartyRegisterTask
        
        var approvalCounterpartyRegisterTask = EtalonTaskTypes.GetAll(x => x.TaskTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ApprovalCounterpartyRegisterTaskGuid)).SingleOrDefault();
        if (approvalCounterpartyRegisterTask != null)
        {
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.CommitteeApprovalAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.CommitteeApprovalAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(lenspec.Tenders.CommitteeApprovalAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.CommitteeApprovalAssignmentGuid,
                                       assignmentTypeName,
                                       approvalCounterpartyRegisterTask);
          }
          if (!EtalonAssignmentTypes.GetAll(x => x.AssignmentTypeGuid.Equals(Constants.Module.FlowFoldersSetting.ProcessingOfApprovalResultsAssignmentGuid)).Any())
          {
            var assignmentTypeName = new System.Guid(Constants.Module.FlowFoldersSetting.ProcessingOfApprovalResultsAssignmentGuid).GetTypeByGuid().ToString();
            CreateEtalonAssignmentType(lenspec.Tenders.ProcessingOfApprovalResultsAssignments.Info.LocalizedName,
                                       Constants.Module.FlowFoldersSetting.ProcessingOfApprovalResultsAssignmentGuid,
                                       assignmentTypeName,
                                       approvalCounterpartyRegisterTask);
          }
          InitializationLogger.Debug("Init: records for ApprovalCounterpartyRegisterTask created.");
        }
        
        #endregion
      }
      catch(Exception ex)
      {
        InitializationLogger.Debug(string.Format("Error: {0}", ex.Message));
      }
    }
    
    private static void CreateEtalonAssignmentType(string name, string typeGuid, string entityName, IEtalonTaskType taskType)
    {
      var assignmentType = EtalonAssignmentTypes.Create();
      assignmentType.Name = name;
      assignmentType.AssignmentTypeGuid = typeGuid;
      assignmentType.EntityName = entityName;
      assignmentType.TaskType = taskType;
      assignmentType.Save();
    }
    
    #endregion
    
    #region Сценарии
    
    /// <summary>
    /// Создать этап преобразования в PDF с простановкой множественных штампов.
    /// </summary>
    public static void CreateApprovalMultipleStamps()
    {
      InitializationLogger.DebugFormat("Init: Create stage '{0}'.", lenspec.EtalonDatabooks.Resources.ApprovalMultipleStampsStageName);
      if (lenspec.EtalonDatabooks.ApprovalMultipleStamps.GetAll().Any())
        return;
      
      var stage = lenspec.EtalonDatabooks.ApprovalMultipleStamps.Create();
      stage.Name = lenspec.EtalonDatabooks.Resources.ApprovalMultipleStampsStageName;
      stage.TimeoutInHours = 4;
      stage.Save();
    }
    
    /// <summary>
    /// Создание записи нового типа сценария "Создавать промежуточную версию в PDF".
    /// </summary>
    private static void CreateApprovalNewVersionConvertPDF()
    {
      InitializationLogger.DebugFormat("Init: Create stage '{0}'.", lenspec.EtalonDatabooks.Resources.ApprovalNewVersionConvertPDFStageName);

      if (lenspec.EtalonDatabooks.ApprovalNewVersionConvertPDFs.GetAll().Any())
        return;

      var stage = lenspec.EtalonDatabooks.ApprovalNewVersionConvertPDFs.Create();
      stage.Name = lenspec.EtalonDatabooks.Resources.ApprovalNewVersionConvertPDFStageName;
      stage.RewriteOriginal = false;
      stage.IsConvertAddendums = true;
      stage.TimeoutInHours = 4;
      stage.Save();
    }
    
    #endregion
    
    /// <summary>
    /// Инициализация справочника Место работы.
    /// </summary>
    public static void InitPlaceOfWork()
    {
      CreatePlaceOfWork("Основное место работы", 0);
      CreatePlaceOfWork("Внешнее совместительство", 1);
      CreatePlaceOfWork("Внутреннее совместительство", 2);
    }
    
    /// <summary>
    /// Создание записи в справочнике Место работы.
    /// </summary>
    /// <param name="name">Имя.</param>
    /// <param name="code">Код 1С.</param>
    private static void CreatePlaceOfWork(string name, int code)
    {
      var placeOfWork = PlaceOfWorks.GetAll(x => x.Name.Equals(name) && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).FirstOrDefault();
      if (placeOfWork == null)
      {
        placeOfWork = PlaceOfWorks.Create();
        placeOfWork.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
        placeOfWork.Name = name;
        placeOfWork.Code1C = code;
        placeOfWork.Save();
      }
    }
    
    public static void CreateRoleKinds()
    {

    }
    
    /// <summary>
    /// Создать вид роли
    /// </summary>
    /// <param name="name">Имя</param>
    /// <param name="note">Описание</param>
    /// <remarks>Есть проверка на уникальность по имени</remarks>
    [Public]
    public static void CreateRoleKind(string name, string note)
    {
      InitializationLogger.Debug($"Init: Create role kind {name}");
      
      if (lenspec.EtalonDatabooks.RoleKinds.GetAll(x => x.Name == name).Any())
        return;
      
      var roleKind = lenspec.EtalonDatabooks.RoleKinds.Create();
      roleKind.Name = name;
      roleKind.Note = note;
      roleKind.Save();
    }
    
    //конец Добавлено Avis Expert.
  }
}