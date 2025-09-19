using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.OutgoingLetters.Server
{
  public partial class ModuleInitializer
  {

    //Добавлено Avis Expert
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      #region Роли
      
      CreateRoles();
      
      #endregion
      
      #region Справочники
      
      GrantRightsOnDatabooks();
      
      #endregion
      
      #region Документы
      
      CreateDocumentTypes();
      CreateDocumentKinds();
      GrantRightsOnDocuments();
      
      #endregion
      
      #region Вычисляемые папки
      
      GrantRightsOnFolder();
      
      #endregion
      
      #region Отчеты
      
      CreateReportsTables();
      GrantRightsOnReport();
      
      #endregion
      
      #region Сценарии
      
      CreateFormationIncomingLettersWithinGroup();
      CreateMassDocumentRegistration();
      
      #endregion
      
    }
    
    /// <summary>
    /// Выдать права на справочники.
    /// </summary>
    public static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Grant rights on databooks.");
      
      MailingTypes.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      
      var registrationManagers = Roles.GetAll(n => n.Sid == Sungero.Docflow.PublicConstants.Module.RoleGuid.RegistrationManagersRole).SingleOrDefault();
      if (registrationManagers != null)
      {
        MailingTypes.AccessRights.Grant(registrationManagers, DefaultAccessRightsTypes.Change);
      }
      
      MailingTypes.AccessRights.Save();
    }
    
    /// <summary>
    /// Выдать права на документы.
    /// </summary>
    public static void GrantRightsOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant rights on documents.");
      
      var creatingMassMailingApplicationRole = Roles.GetAll(n => n.Sid == Constants.Module.CreatingMassMailingApplicationRole).SingleOrDefault();
      if (creatingMassMailingApplicationRole != null)
      {
        MassMailingApplications.AccessRights.Grant(creatingMassMailingApplicationRole, DefaultAccessRightsTypes.Create);
        MassMailingApplications.AccessRights.Save();
      }
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемую папку.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
      InitializationLogger.Debug("Init: Grant rights on folders.");
      var allUsers = Roles.AllUsers;
      
      OutgoingLetters.SpecialFolders.OutgoingLettersOfCurrentUser.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      OutgoingLetters.SpecialFolders.MassMailingTasksOfCurrentUser.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      
      OutgoingLetters.SpecialFolders.OutgoingLettersOfCurrentUser.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Исходящие письма текущего пользователя'.");
      OutgoingLetters.SpecialFolders.MassMailingTasksOfCurrentUser.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Задачи по массовым рассылкам текущего пользователя'.");
    }
    
    /// <summary>
    /// Создание новых типов документов в служебном справочнике.
    /// </summary>
    public static void CreateDocumentTypes()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.OutgoingLetters.Resources.MassMailingApplicationName,
                                                                              MassMailingApplication.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
    }
    
    /// <summary>
    /// Создание видов документов для Заявок на создание Нашей организации.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      // Создание вида документа «Заявка на рассылку массовых уведомлений».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.OutgoingLetters.Resources.MassMailingApplicationName,
                                                                              lenspec.OutgoingLetters.Resources.MassMailingApplicationDescription,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              MassMailingApplication.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.MassMailingApplicationKind,
                                                                              true);
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на отчеты.
    /// </summary>
    public static void GrantRightsOnReport()
    {
      InitializationLogger.Debug("Init: Grant right on reports.");
      
      var clerks = Sungero.Docflow.PublicFunctions.DocumentRegister.Remote.GetClerks();
      if (clerks != null)
      {
        Reports.AccessRights.Grant(Reports.GetRusPostReport().Info, clerks, DefaultReportAccessRightsTypes.Execute);
        Reports.AccessRights.Grant(Reports.GetRusPostMailListNew().Info, clerks, DefaultReportAccessRightsTypes.Execute);
        Reports.AccessRights.Grant(Reports.GetRusPostMailListShort().Info, clerks, DefaultReportAccessRightsTypes.Execute);
      }
      
      Reports.AccessRights.Grant(Reports.GetUnsentMassEmailsReport().Info, Roles.AllUsers, DefaultReportAccessRightsTypes.Execute);
    }
    
    /// <summary>
    /// Создать таблицы для отчетов.
    /// </summary>
    public static void CreateReportsTables()
    {
      InitializationLogger.Debug("Init: Create temp tables for reports.");
      
      var rusPostReportTableName = Constants.RusPostReport.SourceTableName;
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { rusPostReportTableName });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.RusPostReport.CreateSourceTable, new[] { rusPostReportTableName });
      
      var checkLetterDataReportTableName = Constants.CheckLetterDataReport.SourceTableName;
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { checkLetterDataReportTableName });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.CheckLetterDataReport.CreateSourceTable, new[] { checkLetterDataReportTableName });
      
      var unsentMassEmailsReportTableName = Constants.UnsentMassEmailsReport.SourceTableName;
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { unsentMassEmailsReportTableName });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.UnsentMassEmailsReport.CreateSourceTable, new[] { unsentMassEmailsReportTableName });
    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.OutgoingLetters.Resources.RoleNameCreatingMassMailingApplication,
                                                                      lenspec.OutgoingLetters.Resources.RoleDescriptionCreatingMassMailingApplication,
                                                                      Constants.Module.CreatingMassMailingApplicationRole);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.OutgoingLetters.Resources.RoleRightsToReportExecutionOrdersIncommingLetters,
                                                                      lenspec.OutgoingLetters.Resources.RoleDescriptionToReportExecutionOrdersIncommingLetters,
                                                                      Constants.Module.RightsToReportExecutionOrdersIncommingLetters);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.OutgoingLetters.Resources.RoleNameRightsAttachScansOutgoingEmails,
                                                                      lenspec.OutgoingLetters.Resources.RoleDescriptionRightsAttachScansOutgoingEmails,
                                                                      Constants.Module.RightsToAttachScansOfOutgoingLetters);
    }
    
    /// <summary>
    /// Создание записи нового типа сценария "Формирование входящего письма внутри группы".
    /// </summary>
    public static void CreateFormationIncomingLettersWithinGroup()
    {
      InitializationLogger.DebugFormat("Init: Create stage 'Формирование входящего письма внутри группы'.");
      if (OutgoingLetters.FormationIncomingLettersWithinGroups.GetAll().Any())
        return;
      var stage = OutgoingLetters.FormationIncomingLettersWithinGroups.Create();
      stage.Name = lenspec.OutgoingLetters.Resources.FormationIncomingLettersWithinGroupName;
      stage.TimeoutInHours = 8;
      stage.Save();
    }
    
    /// <summary>
    /// Создание записи нового типа сценария "Массовая регистрация документов".
    /// </summary>
    public static void CreateMassDocumentRegistration()
    {
      InitializationLogger.DebugFormat("Init: Create stage 'Массовая регистрация документов'.");
      if (OutgoingLetters.MassDocumentRegistrations.GetAll().Any())
        return;
      var stage = OutgoingLetters.MassDocumentRegistrations.Create();
      stage.Name = "Массовая регистрация документов";
      stage.TimeoutInHours = 4;
      stage.TimeoutAction = OutgoingLetters.MassDocumentRegistration.TimeoutAction.Skip;
      stage.Save();
    }
    //конец Добавлено Avis Expert
  }
}
