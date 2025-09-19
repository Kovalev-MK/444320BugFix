using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.ElectronicDigitalSignatures.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      //Создание типов документов.
      CreateDocumentTypes();
      
      //Создание видов документов.
      CreateDocumentKinds();
      
      // Создание ролей.
      CreateRoles();
      
      // Документы.
      GrantRightsOnDocuments();
      
      // Вычисляемые папки.
      GrantRightsOnFolder();
      
      // Роли согласования.
      CreateApprovalRoles();
      
      // Сценарии.
      CreateApprovalFunctionStages();
    }
    
    /// <summary>
    /// Создание типов документов в служебном справочнике "Типы документов".
    /// </summary>
    public static void CreateDocumentTypes()
    {
      InitializationLogger.Debug("Init: Create Document types");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.ElectronicDigitalSignatures.Resources.EDSApplication,
                                                                              EDSApplication.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.ElectronicDigitalSignatures.Resources.EDSDocument,
                                                                              EDSDocument.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
    }
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      InitializationLogger.Debug("Init: Create Document kinds");
      
      // Создание вида документа «Заявка УКЭП на носителе».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.ElectronicDigitalSignatures.Resources.EDSApplication,
                                                                              lenspec.ElectronicDigitalSignatures.Resources.EDSApplication,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              EDSApplication.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[]
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval,
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendActionItem
                                                                              },
                                                                              Constants.Module.EDSApplicationKind,
                                                                              true);
      var documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.EDSApplicationKind);
      documentKind.Code = lenspec.ElectronicDigitalSignatures.Resources.EDSApplicationKindCode;
      documentKind.Save();
      
      // Создание вида документа «Паспорт РФ».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.ElectronicDigitalSignatures.Resources.PassportDocumentKindName,
                                                                              lenspec.ElectronicDigitalSignatures.Resources.PassportDocumentKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              EDSDocument.ClassTypeGuid,
                                                                              null,
                                                                              Constants.Module.PassportKind,
                                                                              false);
      documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.PassportKind);
      documentKind.AvailableActions.Clear();
      documentKind.Save();
      
      // Создание вида документа «Согласие на обработку персональных данных».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.ElectronicDigitalSignatures.Resources.ConsentToProcessingDocumentKindName,
                                                                              lenspec.ElectronicDigitalSignatures.Resources.ConsentToProcessingDocumentKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              EDSDocument.ClassTypeGuid,
                                                                              null,
                                                                              Constants.Module.ConsentToProcessingKind,
                                                                              false);
      documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.ConsentToProcessingKind);
      documentKind.AvailableActions.Clear();
      documentKind.Save();
    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles");
      
      // Создать роль "Ответственный за выдачу, продление и аннулирование УКЭП по СПБ".
      lenspec.Etalon.Module.Docflow.PublicFunctions.Module.CreateRoleAvis(lenspec.ElectronicDigitalSignatures.Resources.QESResponsibleSpbRoleName,
                                                                          string.Empty,
                                                                          Constants.Module.QESResponsibleSpb,
                                                                          true);
      
      // Создать роль "Ответственный за выдачу, продление и аннулирование УКЭП по МСК и Регионам".
      lenspec.Etalon.Module.Docflow.PublicFunctions.Module.CreateRoleAvis(lenspec.ElectronicDigitalSignatures.Resources.QESResponsibleMskRoleName,
                                                                          string.Empty,
                                                                          Constants.Module.QESResponsibleMsk,
                                                                          true);
    }
    
    /// <summary>
    /// Выдать права на документы.
    /// </summary>
    public static void GrantRightsOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant rights on documents");
      
      EDSApplications.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Create);
      EDSApplications.AccessRights.Save();
      
      EDSDocuments.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Create);
      EDSDocuments.AccessRights.Save();
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемую папку.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
      ElectronicDigitalSignatures.SpecialFolders.MyEDSApplication.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ElectronicDigitalSignatures.SpecialFolders.MyEDSApplication.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Мои Завки УКЭП'.");
    }
    
    /// <summary>
    /// Функция инициализации для создания ролей согласования.
    /// </summary>
    public static void CreateApprovalRoles()
    {
      // Получатель УКЭП.
      lenspec.EtalonDatabooks.PublicInitializationFunctions.Module.CreateApprovalRole(lenspec.EtalonDatabooks.ComputedRole.Type.EDSOwner,
                                                                                      lenspec.EtalonDatabooks.ComputedRoles.Info.Properties.Type.GetLocalizedValue(lenspec.EtalonDatabooks.ComputedRole.Type.EDSOwner));
    }
    
    #region Сценарии
    
    /// <summary>
    /// Функция инициализации для сценариев.
    /// </summary>
    public static void CreateApprovalFunctionStages()
    {
      CreateDeletingContentOfEDSDocuments();
      CreateUpdateEDSApplicationState();
    }
    
    /// <summary>
    /// Создание записи нового типа сценария "Удаление содержимого документов сотрудника и содержимого заявки".
    /// </summary>
    public static void CreateDeletingContentOfEDSDocuments()
    {
      InitializationLogger.DebugFormat("Init: Create stage 'Удаление содержимого документов сотрудника и содержимого заявки'.");
      if (lenspec.ElectronicDigitalSignatures.DeletingContentOfEDSDocuments.GetAll().Any())
        return;

      var stage = lenspec.ElectronicDigitalSignatures.DeletingContentOfEDSDocuments.Create();
      stage.Name = lenspec.ElectronicDigitalSignatures.Resources.DeletingContentOfEDSDocumentsStageName;
      stage.TimeoutInHours = 4;
      stage.TimeoutAction = lenspec.ElectronicDigitalSignatures.DeletingContentOfEDSDocument.TimeoutAction.Repeat;
      stage.Save();
    }
    
    /// <summary>
    /// Создание записи нового типа сценария "Обновление поля Состояние/Согласование заявки для УКЭП, ЗНО".
    /// </summary>
    public static void CreateUpdateEDSApplicationState()
    {
      InitializationLogger.DebugFormat("Init: Create stage 'Обновление поля Состояние/Согласование заявки для УКЭП, ЗНО'.");
      if (lenspec.ElectronicDigitalSignatures.UpdateEDSApplicationStates.GetAll().Any())
        return;

      var stage = lenspec.ElectronicDigitalSignatures.UpdateEDSApplicationStates.Create();
      stage.Name = lenspec.ElectronicDigitalSignatures.Resources.UpdateEDSApplicationStateStageName;
      stage.TimeoutInHours = 4;
      stage.TimeoutAction = lenspec.ElectronicDigitalSignatures.UpdateEDSApplicationState.TimeoutAction.Repeat;
      stage.Save();
    }
    
    #endregion

  }
}
