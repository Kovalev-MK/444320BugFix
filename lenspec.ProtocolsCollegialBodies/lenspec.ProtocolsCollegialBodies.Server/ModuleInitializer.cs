using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.ProtocolsCollegialBodies.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      //Создание ролей.
      CreateRoles();
      
      //Создание вычисляемых ролей
      CreateApprovalRole(ProtocolsCollegialBodies.CompRoleCollegialBody.Type.Chairman, "Председатель коллегиального органа.");
      CreateApprovalRole(ProtocolsCollegialBodies.CompRoleCollegialBody.Type.Members, "Члены коллегиального органа.");
      
      //Создание типов документов.
      CreateDocumentTypes();
      
      //Создание видов документов.
      CreateDocumentKinds();
      
      //Права на создание документов
      GrantCreateRightsOnInfoDocument();
      
      //Права на справочники.
      GrantRightsOnDatabooks();
      
      //Создания сценария.
      CreateStartingIntroductionTask();
    }
    
    /// <summary>
    /// Создание типа документа "Протокол коллегиальных органов"
    /// в служебном справочнике "Типы документов".
    /// </summary>
    public static void CreateDocumentTypes()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.ProtocolsCollegialBodies.Resources.ProtocolCollegialDocumentTypeName,
                                                                              ProtocolCollegialBody.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true);
    }

    /// <summary>
    /// Создание видов для Протоколов коллегиальных органов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      // Создание вида документа «Протокол коллегиальных органов».
      // Чтобы документы можно было регистрировать, задается свойство Registrable.
      // В качестве ИД вида документа используется константа ProtocolCollegialBody.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.ProtocolsCollegialBodies.Resources.ProtocolCollegialBodyName, 
                                                                              lenspec.ProtocolsCollegialBodies.Resources.ProtocolCollegialBodyShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              ProtocolCollegialBody.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              lenspec.ProtocolsCollegialBodies.Constants.Module.ProtocolCollegialBodyKind);
    }

    /// <summary>
    /// Выдача прав на создание Протоколов коллегиальных органов.
    /// </summary>
    public static void GrantCreateRightsOnInfoDocument()
    {
      InitializationLogger.DebugFormat("Init: Grant Create Rights On Info Document for ProtocolsCollegialBodies.");
      
      var allUsers = Roles.AllUsers;
      ProtocolsCollegialBodies.ProtocolCollegialBodies.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Create);
      ProtocolsCollegialBodies.ProtocolCollegialBodies.AccessRights.Save();
    }

    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles for ProtocolsCollegialBodies.");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.ProtocolsCollegialBodies.Resources.RoleNameClerksGK,
                                                                      lenspec.ProtocolsCollegialBodies.Resources.RoleDescriptionClerksGK,
                                                                      lenspec.ProtocolsCollegialBodies.Constants.Module.ClerksGKResponsible);
    }

    /// <summary>
    /// Выдать права на справочник, всем пользователям на Просмотр, группе Делопроизводители ГК полные.
    /// </summary>
    public static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Grant rights on databooks to Sales department archive module responsible for ProtocolsCollegialBodies.");
      
      var ClerksGKResponsibl = Roles.GetAll().SingleOrDefault(n => n.Sid == ProtocolsCollegialBodies.Constants.Module.ClerksGKResponsible);
      var allUsers = Roles.AllUsers;
      
      CollegialBodies.AccessRights.Grant(ClerksGKResponsibl, DefaultAccessRightsTypes.FullAccess);
      CollegialBodies.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      
      CollegialBodies.AccessRights.Save();
    }

    /// <summary>
    /// Создание роли “Председатель коллегиального органа”.
    /// </summary>
    public static void CreateApprovalRole(Enumeration roleType, string description)
    {
      InitializationLogger.DebugFormat("Init: Create Approval role for ProtocolsCollegialBodies.");
      
      var role = CompRoleCollegialBodies.GetAll().Where(r => Equals(r.Type, roleType)).FirstOrDefault();
      // Проверяет наличие роли.
      if (role == null)
      {
        role = CompRoleCollegialBodies.Create();
        role.Type = roleType;
      }
      role.Description = description;
      role.Save();
      InitializationLogger.Debug("Создана роль \"Председатель коллегиального органа\"");
    }

    /// <summary>
    /// Создание записи нового типа сценария "Старт задачи на ознакомление с ПКО".
    /// </summary>
    public static void CreateStartingIntroductionTask()
    {
      InitializationLogger.DebugFormat("Init: Create stage for Start of the task for familiarization with the PKO for ProtocolsCollegialBodies.");
      
      if (ProtocolsCollegialBodies.StartingIntroductionTasks.GetAll().Any())
        return;

      var stage = ProtocolsCollegialBodies.StartingIntroductionTasks.Create();
      stage.Name = ProtocolsCollegialBodies.Resources.StageStartingIntroductionTaskName;
      stage.TimeoutInHours = 8;
      stage.TimeoutAction = ProtocolsCollegialBodies.StartingIntroductionTask.TimeoutAction.Skip;
      stage.TaskDeadline = 3;
      stage.IsElectronic = true;
      stage.Save();
    }
  }
}
