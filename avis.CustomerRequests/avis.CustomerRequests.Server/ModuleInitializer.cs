using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace avis.CustomerRequests.Server
{
  public partial class ModuleInitializer
  {

    public override bool IsModuleVisible()
    {
      return Users.Current.IncludedIn(Roles.AllUsers);
    }
    
    //Добавлено Avis Expert
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Создание ролей.
      CreateRoles();
      
      //Создание типов документов.
      CreateDocumentTypes();
      
      //Создание видов документов.
      CreateDocumentKinds();
      
      // Документы.
      GrantRightsOnDocuments();
      
      // Справочники.
      GrantRightsOnDatabooks();

      // Вычисляемые папки.
      GrantRightsOnFolder();
    }
    
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles for Customer Requests");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(Resources.RoleNameForDataBook,
                                                                      Resources.RoleDescriptionForDataBook,
                                                                      Constants.Module.RoleAccessToDatabook);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(Resources.RoleSeeCustRequests,
                                                                      Resources.RoleSeeCustRequests,
                                                                      Constants.Module.RoleAccessToRequests);
    }

    public static void CreateDocumentTypes()
    {
      // Создание типа документа Обращения клиентов.
      InitializationLogger.Debug("Init: Create DocumentType for Customer Requests");
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(Resources.CustomerRequestType,
                                                                              Constants.Module.CustReqType,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Incoming,
                                                                              true);
    }
    
    public static void CreateDocumentKinds()
    {
      // Создание вида документа «Обращение клиента».
      InitializationLogger.Debug("Init: Create DocumentKind for Customer Requests");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(Resources.CustomerRequestDocKind,
                                                                              Resources.CustomerRequestDocKind,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Registrable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Incoming, true, false,
                                                                              Constants.Module.CustReqType,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendActionItem },
                                                                              Constants.Module.CustReqDocKind,
                                                                              true);
    }
    
    public static void GrantRightsOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant rights on documents to Customer Requests.");
      
      var roleSeeRequests= Roles.GetAll(n => n.Sid == Constants.Module.RoleAccessToRequests).SingleOrDefault();

      CustomerRequests.AccessRights.RevokeAll(Roles.AllUsers);
      CustomerRequests.AccessRights.Grant(roleSeeRequests, DefaultAccessRightsTypes.Read);

      var clerks = Sungero.Docflow.PublicFunctions.DocumentRegister.Remote.GetClerks();
      if (clerks != null)
      {
        CustomerRequests.AccessRights.Grant(clerks, DefaultAccessRightsTypes.Create);
      }
      CustomerRequests.AccessRights.Save();
    }

    public static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Grant rights on databooks for Custom Requests.");
      
      var roleDataBook = Roles.GetAll(n => n.Sid == Constants.Module.RoleAccessToDatabook).SingleOrDefault();
      var roleSeeRequests= Roles.GetAll(n => n.Sid == Constants.Module.RoleAccessToRequests).SingleOrDefault();
      
      CategoryRequests.AccessRights.RevokeAll(Roles.AllUsers);
      CategoryRequests.AccessRights.Grant(roleDataBook, DefaultAccessRightsTypes.FullAccess);
      CategoryRequests.AccessRights.Grant(roleSeeRequests, DefaultAccessRightsTypes.Read);

      CustomerRequests.AccessRights.RevokeAll(Roles.AllUsers);
      CustReqSetups.AccessRights.Grant(roleDataBook, DefaultAccessRightsTypes.FullAccess);
      CustReqSetups.AccessRights.Grant(roleSeeRequests, DefaultAccessRightsTypes.Read);
      
      var clerks = Sungero.Docflow.PublicFunctions.DocumentRegister.Remote.GetClerks();
      if (clerks != null)
      {
        CategoryRequests.AccessRights.Grant(clerks, DefaultAccessRightsTypes.Read);
        CustReqSetups.AccessRights.Grant(clerks, DefaultAccessRightsTypes.Read);
      }
      CategoryRequests.AccessRights.Save();
      CustReqSetups.AccessRights.Save();
      
      lenspec.SalesDepartmentArchive.DocumentKindsOfRealEstateSales.AccessRights.Grant(roleDataBook, DefaultAccessRightsTypes.Read);
      lenspec.SalesDepartmentArchive.DocumentKindsOfRealEstateSales.AccessRights.Save();
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемую папку.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
      InitializationLogger.Debug("Init: Grant rights on folders for Custom Requests.");
      var accessToDatabook = Roles.GetAll(n => n.Sid == Constants.Module.RoleAccessToDatabook).SingleOrDefault();
      var accessToRequests = Roles.GetAll(n => n.Sid == Constants.Module.RoleAccessToRequests).SingleOrDefault();
      var clerks = Sungero.Docflow.PublicFunctions.DocumentRegister.Remote.GetClerks();
      
      avis.CustomerRequests.SpecialFolders.CustRequestFolder.AccessRights.Grant(accessToDatabook, DefaultAccessRightsTypes.Read);
      avis.CustomerRequests.SpecialFolders.CustRequestFolder.AccessRights.Grant(accessToRequests, DefaultAccessRightsTypes.Read);
      avis.CustomerRequests.SpecialFolders.CustRequestFolder.AccessRights.Grant(clerks, DefaultAccessRightsTypes.Read);
      avis.CustomerRequests.SpecialFolders.CustRequestFolder.AccessRights.Save();
    }
  }
}
