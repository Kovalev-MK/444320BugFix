using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace avis.EtalonContracts.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      try
      {
        // Создание ролей.
        CreateRoles();
        // Выдача прав на справочники.
        GrantRightsOnDatabooks();
        //Права на отчеты
        GrantRightsToReports();
        // Создание типов документов.
        CreateDocumentTypes();
        // Создание видов документов.
        CreateDocumentKinds();
        // Выдача прав на документы
        GrantAccessRightOnDocuments();
        // Инициализация сценариев.
        CreateApprovalFunctionStages();
        // Создание временных таблиц
        CreateTables();
      }
      catch(Exception)
      {
        
      }
    }
    
    /// <summary>
    /// Создание типов документов.
    /// </summary>
    private static void CreateDocumentTypes()
    {
      // Создание типа документа "Приложение к договорному документу".
      InitializationLogger.Debug("Init: Create DocumentType for Etalon Contracts");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(avis.EtalonContracts.Resources.AttachmentContractDocumentTypeName,
                                                                              Constants.Module.AttachmentContractDocumentTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts,
                                                                              true);
    }
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      // Создание вида документа «Обращение клиента».
      InitializationLogger.Debug("Init: Create DocumentKind for Enalon Contracts");
      
      // Вид "Приложение к договорному документу".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.EtalonContracts.Resources.AttachmentDocumentKindName,
                                                                              avis.EtalonContracts.Resources.AttachmentDocumentKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, false, false,
                                                                              Constants.Module.AttachmentContractDocumentTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendActionItem },
                                                                              Constants.Module.AttachmentDocumentKindGuid,
                                                                              false);
      
      // Вид "Протокол разногласий".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.EtalonContracts.Resources.ProtocolDisagreementKindName,
                                                                              avis.EtalonContracts.Resources.ProtocolDisagreementKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, false, false,
                                                                              Constants.Module.AttachmentContractDocumentTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendActionItem },
                                                                              Constants.Module.ProtocolDisagreementKindGuid,
                                                                              false);
    }
    
    /// <summary>
    /// Выдача прав на справочники.
    /// </summary>
    private static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Grant Rights On Databooks for Etalon Contracts");
      
      var allUsers = Sungero.CoreEntities.Roles.AllUsers;
      var responsibleContracDocuments = Roles.GetAll(r => r.Sid == EtalonContracts.Constants.Module.ResponsibleContracDocuments).FirstOrDefault();
      
      ContractTypes.AccessRights.Grant(responsibleContracDocuments, DefaultAccessRightsTypes.FullAccess);
      ContractTypes.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      ContractTypes.AccessRights.Save();
      
      GroupContractTypes.AccessRights.Grant(responsibleContracDocuments, DefaultAccessRightsTypes.FullAccess);
      GroupContractTypes.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      GroupContractTypes.AccessRights.Save();
      
      ContractKinds.AccessRights.Grant(responsibleContracDocuments, DefaultAccessRightsTypes.FullAccess);
      ContractKinds.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      ContractKinds.AccessRights.Save();
      
      lenspec.Etalon.ContractCategories.AccessRights.Grant(responsibleContracDocuments, DefaultAccessRightsTypes.FullAccess);
      lenspec.Etalon.ContractCategories.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      lenspec.Etalon.ContractCategories.AccessRights.Save();
      
      WorkTypes.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      WorkTypes.AccessRights.Save();
      
      DetailingWorkTypes.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      DetailingWorkTypes.AccessRights.Save();
      
      PresenceRegions.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      PresenceRegions.AccessRights.Save();
    }
    
    /// <summary>
    /// Создание ролейю.
    /// </summary>
    private static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create role for Etalon Contracts");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(EtalonContracts.Resources.ResponsibleContracDocumentsName,
                                                                      EtalonContracts.Resources.ResponsibleContracDocumentsName,
                                                                      EtalonContracts.Constants.Module.ResponsibleContracDocuments);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.EtalonContracts.Resources.FillingBudgetFormedRoleName, string.Empty, Constants.Module.FillingBudgetFormed);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.EtalonContracts.Resources.AllEmployeesDirectionLegalIssuesName,
                                                                      avis.EtalonContracts.Resources.AllEmployeesDirectionLegalIssuesName,
                                                                      EtalonContracts.Constants.Module.AllEmployeesDirectionLegalIssues);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.EtalonContracts.Resources.FullAcessContractualDocumentName,
                                                                      avis.EtalonContracts.Resources.FullAcessContractualDocumentName,
                                                                      EtalonContracts.Constants.Module.FullAcessContractualDocuments);
      
      // Создание роли "Права на отчет по использованию шаблонов ДД"
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.EtalonContracts.Resources.RightsToReportForContractTemplatesName,
                                                                      avis.EtalonContracts.Resources.RightsToReportForContractTemplatesDesc,
                                                                      EtalonContracts.Constants.Module.RightsToReportForContractTemplates);
    }
    
    private static void GrantAccessRightOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant Right To Contracts Responsible for Etalon Contracts");
      var contractsResponsible = Roles.GetAll().Where(n => n.Sid == Sungero.Docflow.Constants.Module.RoleGuid.ContractsResponsible).FirstOrDefault();
      AttachmentContractDocuments.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Create);
      AttachmentContractDocuments.AccessRights.Save();
      
      InitializationLogger.Debug("Init: Grant Right To AllEmployeesDirectionLegalIssues for ContractualDocuments");
      var allEmployeesDirectionLegalIssues = Roles.GetAll().Where(x => x.Sid == Constants.Module.AllEmployeesDirectionLegalIssues).FirstOrDefault();
      AttachmentContractDocuments.AccessRights.Grant(allEmployeesDirectionLegalIssues, DefaultAccessRightsTypes.Read);
      AttachmentContractDocuments.AccessRights.Save();
      Sungero.Contracts.ContractualDocuments.AccessRights.Grant(allEmployeesDirectionLegalIssues, DefaultAccessRightsTypes.Read);
      Sungero.Contracts.ContractualDocuments.AccessRights.Save();
      
      InitializationLogger.Debug("Init: Grant Right To FullAcessContractualDocuments for ContractualDocuments");
      var fullAcessContractualDocuments = Roles.GetAll().Where(x => x.Sid == Constants.Module.FullAcessContractualDocuments).FirstOrDefault();
      AttachmentContractDocuments.AccessRights.Grant(fullAcessContractualDocuments, DefaultAccessRightsTypes.FullAccess);
      AttachmentContractDocuments.AccessRights.Save();
      Sungero.Contracts.ContractualDocuments.AccessRights.Grant(fullAcessContractualDocuments, DefaultAccessRightsTypes.FullAccess);
      Sungero.Contracts.ContractualDocuments.AccessRights.Save();
      
      //TODO Отобрать полный доступ у роли allEmployeesDirectionLegalIssues (выданы по ошибке, убрать поле переноса на тест)
      if (AttachmentContractDocuments.AccessRights.IsGranted(DefaultAccessRightsTypes.FullAccess, allEmployeesDirectionLegalIssues))
      {
        AttachmentContractDocuments.AccessRights.Revoke(allEmployeesDirectionLegalIssues, DefaultAccessRightsTypes.FullAccess);
        AttachmentContractDocuments.AccessRights.Save();
      }
      if (Sungero.Contracts.ContractualDocuments.AccessRights.IsGranted(DefaultAccessRightsTypes.FullAccess, allEmployeesDirectionLegalIssues))
      {
        Sungero.Contracts.ContractualDocuments.AccessRights.Revoke(allEmployeesDirectionLegalIssues, DefaultAccessRightsTypes.FullAccess);
        Sungero.Contracts.ContractualDocuments.AccessRights.Save();
      }
    }
    
    /// <summary>
    /// Создание сценариев.
    /// </summary>
    private static void CreateApprovalFunctionStages()
    {
      
    }
    
    public static void GrantRightsToReports()
    {
      avis.EtalonContracts.Reports.AccessRights.Grant(avis.EtalonContracts.Reports.GetAmountContractualDocument().Info, Roles.AllUsers, DefaultReportAccessRightsTypes.Execute);
      
      // Выдача прав на отчет "Отчет по использованию шаблонов ДД"
      var role = Roles.GetAll(x => x.Sid == EtalonContracts.Constants.Module.RightsToReportForContractTemplates).FirstOrDefault();
      avis.EtalonContracts.Reports.AccessRights.Grant(avis.EtalonContracts.Reports.GetUsingContractTemplatesReport().Info, role, DefaultReportAccessRightsTypes.Execute);
    }
    
    public static void CreateTables()
    {
      InitializationLogger.Debug("Init: Create table UsingContractTemplatesTable");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.UsingContractTemplatesReport.UsingContractTemplatesReportTableName });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(EtalonContracts.Queries.UsingContractTemplatesReport.CreateUsingContractTemplatesTable,
                                                                     new[] { Constants.UsingContractTemplatesReport.UsingContractTemplatesReportTableName });
    }
  }
}
