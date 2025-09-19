using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.AutomatedSupportTickets.Server
{
  public partial class ModuleInitializer
  {
    //Добавлено Avis Expert
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Создание ролей.
      CreateRoles();
      
      // Задачи.
      GrantRightsOnTasks();
      
      #region Отчеты
      
      CreateReportsTables();
      GrantRightsOnReport();
      
      #endregion
    }

    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles");
      
      // Завершение Задач и Заданий.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.AutomatedSupportTickets.Resources.RoleNameCompleteAssignmentsResponsible,
                                                                      lenspec.AutomatedSupportTickets.Resources.RoleDescriptionCompleteAssignmentsResponsible,
                                                                      Constants.Module.CompleteAssignmentsResponsible);
      
      // Права доступа к отключению учетных записей и отчету Указание пользователей в объектах системы
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.AutomatedSupportTickets.Resources.RoleNameAccessAccountDisconnectionAndUserReport,
                                                                      lenspec.AutomatedSupportTickets.Resources.RoleDescriptionAccessAccountDisconnectionAndUserReport,
                                                                      Constants.Module.AccessAccountDisconnectionAndUserReport);
    }
    
    /// <summary>
    /// Выдать права на задачи.
    /// </summary>
    public static void GrantRightsOnTasks()
    {
      InitializationLogger.Debug("Init: Grant rights on tasks.");
      
      var allUsers = Roles.AllUsers;
      AutomatedSupportTickets.SubstitutionRequestTasks.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      AutomatedSupportTickets.SubstitutionRequestTasks.AccessRights.Save();
    }
    
    /// <summary>
    /// Создать таблицы для отчетов.
    /// </summary>
    public static void CreateReportsTables()
    {
      InitializationLogger.Debug("Init: Create temp tables for reports.");
      
      var specifyingUsersSystemObjectsReportTableName = lenspec.AutomatedSupportTickets.Constants.SpecifyingUsersSystemObjects.SpecifyingUsersSystemObjectsTableName;
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { specifyingUsersSystemObjectsReportTableName });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.SpecifyingUsersSystemObjects.CreateResponsibilitiesReportTable, new[] { specifyingUsersSystemObjectsReportTableName });
      
      var reportExecutionOrders = Constants.ReportExecutionOrders.SourceTableName;
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { reportExecutionOrders });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.ReportExecutionOrders.CreateSourceTable, new[] { reportExecutionOrders });
      
      var reconciliationSettingsReportTableName = lenspec.AutomatedSupportTickets.Constants.ReconciliationSettings.ReconciliationSettingsTableName;
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { reconciliationSettingsReportTableName });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.ReconciliationSettings.CreateReconciliationSettingsReportTable, new[] { reconciliationSettingsReportTableName });
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на отчеты.
    /// </summary>
    public static void GrantRightsOnReport()
    {
      InitializationLogger.Debug("Init: Grant right on reports.");
      
      var allUsers = Roles.AllUsers;
      var usersRoleRightsToReportExecutionOrdersIncommingLetters = Roles.GetAll(r => r.Sid == lenspec.OutgoingLetters.PublicConstants.Module.RightsToReportExecutionOrdersIncommingLetters).SingleOrDefault();
      var accessAccountDisconnectionAndUserReport = Roles.GetAll(r => r.Sid == Constants.Module.AccessAccountDisconnectionAndUserReport).SingleOrDefault();
      
      Reports.AccessRights.Grant(Reports.GetReportExecutionOrders().Info, usersRoleRightsToReportExecutionOrdersIncommingLetters, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetCompanyStafWithHierarchy().Info, allUsers, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetSpecifyingUsersSystemObjects().Info, accessAccountDisconnectionAndUserReport, DefaultReportAccessRightsTypes.Execute);
    }
  }
}
