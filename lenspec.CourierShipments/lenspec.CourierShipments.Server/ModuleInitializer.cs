using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.CourierShipments.Server
{
  public partial class ModuleInitializer
  {

    //Добавлено Avis Expert
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Создание ролей.
      CreateRoles();
      
      // Справочники.
      GrantRightsOnDatabooks();
      
      //Создание типов документов.
      CreateDocumentTypes();
      
      //Создание видов документов.
      CreateDocumentKinds();
      
      // Документы.
      GrantRightsOnDocuments();
      
      // Вычисляемые папки.
      GrantRightsOnFolder();
    }
    
    /// <summary>
    /// Выдать права на справочники.
    /// </summary>
    public static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Grant rights on databooks.");
      
      CourierServices.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      
      var courierServiceResponsible = Roles.GetAll().FirstOrDefault(r => r.Sid == CourierShipments.Constants.Module.CourierServiceResponsible);
      if (courierServiceResponsible != null)
      {
        CourierServices.AccessRights.Grant(courierServiceResponsible, DefaultAccessRightsTypes.FullAccess);
      }
      
      CourierServices.AccessRights.Save();
    }
    
    /// <summary>
    /// Выдать права на документы.
    /// </summary>
    public static void GrantRightsOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant rights on documents.");
      
      CourierShipments.CourierShipmentsApplications.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Create);
      CourierShipments.CourierShipmentsApplications.AccessRights.Save();
    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.CourierShipments.Resources.RoleNameCourierServiceResponsible,
                                                                      lenspec.CourierShipments.Resources.RoleDescriptionCourierServiceResponsible,
                                                                      CourierShipments.Constants.Module.CourierServiceResponsible);
    }
    
    /// <summary>
    /// Создание типов документов.
    /// </summary>
    public static void CreateDocumentTypes()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.CourierShipments.Resources.CourierShipmentsApplicationTypeName,
                                                                              CourierShipmentsApplication.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
    }
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      // Создание вида документа «Заявка на курьерское отправление».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.CourierShipments.Resources.CourierShipmentsApplicationKindName,
                                                                              lenspec.CourierShipments.Resources.CourierShipmentsApplicationKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              CourierShipmentsApplication.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.CourierShipmentsApplicationKind,
                                                                              true);
    }
    
    /// <summary>
    /// Выдать права на вычисляемые папки.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
      InitializationLogger.Debug("Init: Grant rights on folders.");
      
      CourierShipments.SpecialFolders.CourierShipmentsApplicationList.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      CourierShipments.SpecialFolders.CourierShipmentsApplicationList.AccessRights.Save();
    }
    //конец Добавлено Avis Expert
  }
}
