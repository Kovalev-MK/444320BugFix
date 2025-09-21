using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.LocalRegulations.Server
{
  public partial class ModuleInitializer
  {

    /// <summary>
    /// Создание записи нового типа сценария "Прекращение действия устаревших приказов".
    /// </summary>
    public static void CreateTerminationOfObsoleteOrders()
    {
      InitializationLogger.DebugFormat("Init: Create stage for termination Of obsolete orders.");
      if (LocalRegulations.TerminationOfObsoleteOrderses.GetAll().Any())
        return;
      
      var stage = LocalRegulations.TerminationOfObsoleteOrderses.Create();
      stage.Name = "Прекращение действия устаревших приказов";
      stage.TimeoutInHours = 4;
      stage.TimeoutAction = LocalRegulations.UpdatingFieldsInDocumentBody.TimeoutAction.Repeat;
      stage.Save();         
    }
    
    //Добавлено Avis Expert
    
    /// <summary>
    /// Инициализация.
    /// </summary>
    /// <param name="e"></param>
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Вычисляемые папки.
      GrantRightsOnFolder();
      
      // Создание типов документов.
      CreateDocumentTypes();
      
      // Создание видов документов.
      CreateDocumentKinds();
      
      // Выдача прав на Документы.
      GrantRightsOnDocuments();
      
      // Создание таблиц для отчетов.
      CreateReportsTables();
      // Права на отчеты.
      GrantRightsOnReports();

      // Сценарии.
      CreateFunctionStages();      
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемую папку.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
      LocalRegulations.SpecialFolders.LocalRegulationsOfOrganization.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      LocalRegulations.SpecialFolders.LocalRegulationsOfOrganization.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Локальные акты организации'.");
      
      LocalRegulations.SpecialFolders.LocalRegulationOfCurrentUser.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      LocalRegulations.SpecialFolders.LocalRegulationOfCurrentUser.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Локальные акты текущего пользователя'.");
    }
    
    /// <summary>
    /// Создание типов документов.
    /// </summary>
    public static void CreateDocumentTypes()
    {
      // Создаём тип документа "Документ, утверждаемый приказом (приложение к приказу)".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.LocalRegulations.Resources.DocumentApprovedByOrderName,
                                                                              DocumentApprovedByOrder.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
    }
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      // Создание вида документа «Документ, утверждаемый приказом (приложение к приказу)».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.LocalRegulations.Resources.DocumentApprovedByOrderKindName,
                                                                              lenspec.LocalRegulations.Resources.DocumentApprovedByOrderKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              DocumentApprovedByOrder.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.DocumentApprovedByOrderKind,
                                                                              true);
    }
    
    /// <summary>
    /// Выдача прав на Документы.
    /// </summary>
    public static void GrantRightsOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant rights on documents to Local regulations module responsible.");
      
      lenspec.LocalRegulations.DocumentApprovedByOrders.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.FullAccess);
      lenspec.LocalRegulations.DocumentApprovedByOrders.AccessRights.Save();
    }
    
    #region Отчеты
    
    /// <summary>
    /// Создать таблицы для отчетов.
    /// </summary>
    public static void CreateReportsTables()
    {
      var acquaintanceReportTableName = Constants.AcquaintanceReport.SourceTableName;
      var acquaintanceFormReportTableName = Constants.AcquaintanceFormReport.SourceTableName;
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[]
                                                                  {
                                                                    acquaintanceReportTableName,
                                                                    acquaintanceFormReportTableName
                                                                  });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.AcquaintanceReport.CreateAcquaintanceReportTable, new[] { acquaintanceReportTableName });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.AcquaintanceReport.CreateAcquaintanceReportTable, new[] { acquaintanceFormReportTableName });
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемую папку.
    /// </summary>
    public static void GrantRightsOnReports()
    {
      InitializationLogger.Debug("Init: Grant right on reports to all users.");
      Reports.AccessRights.Grant(Reports.GetAcquaintanceReport().Info, Roles.AllUsers, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetAcquaintanceFormReport().Info, Roles.AllUsers, DefaultReportAccessRightsTypes.Execute);
    }
    
    #endregion
    
    /// <summary>
    /// Создание сценариев.
    /// </summary>
    public static void CreateFunctionStages()
    {
      CreateStartingAcquaintanceTask();
      CreateUpdatingFieldsInDocumentBody();
      CreateTerminationOfObsoleteOrders();
    }
    
    /// <summary>
    /// Создание записи нового типа сценария "Старт задачи на ознакомление с ЛНА".
    /// </summary>
    public static void CreateStartingAcquaintanceTask()
    {
      InitializationLogger.DebugFormat("Init: Create stage 'Старт задачи на ознакомление с ЛНА'.");
      if (LocalRegulations.StartingAcquaintanceTasks.GetAll().Any())
        return;

      var stage = LocalRegulations.StartingAcquaintanceTasks.Create();
      stage.Name = lenspec.LocalRegulations.Resources.StageStartingAcquaintanceTaskName;
      stage.TimeoutInHours = 8;
      stage.TimeoutAction = LocalRegulations.StartingAcquaintanceTask.TimeoutAction.Skip;
      stage.AcquaintanceTaskDeadline = 1;
      stage.IsElectronicAcquaintance = true;
      stage.Save();
    }
    
    /// <summary>
    /// Создание записи нового типа сценария "Обновление полей в теле документа".
    /// </summary>
    public static void CreateUpdatingFieldsInDocumentBody()
    {
      InitializationLogger.DebugFormat("Init: Create stage 'Обновление полей в теле документа'.");
      if (LocalRegulations.UpdatingFieldsInDocumentBodies.GetAll().Any())
        return;

      var stage = LocalRegulations.UpdatingFieldsInDocumentBodies.Create();
      stage.Name = "Обновление полей в теле документа";
      stage.TimeoutInHours = 8;
      stage.TimeoutAction = LocalRegulations.UpdatingFieldsInDocumentBody.TimeoutAction.Repeat;
      stage.Save();
    }
    
    //конец Добавлено Avis Expert
  }
}
