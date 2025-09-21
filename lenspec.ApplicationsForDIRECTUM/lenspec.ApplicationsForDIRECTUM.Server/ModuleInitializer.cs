using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.ApplicationsForDIRECTUM.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Создание типов документов.
      CreateDocumentTypes();
      
      // Создание видов документов.
      CreateDocumentKinds();
      
      // Права на создание документов.
      GrantRightsOnDocuments();
      
      // Вычисляемые папки.
      GrantRightsOnFolder();
    }

    /// <summary>
    /// Создание типов документов.
    /// </summary>
    public static void CreateDocumentTypes()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.ApplicationsForDIRECTUM.Resources.ApplicationOpeningRSBUandUUTypeName,
                                                                              ApplicationOpeningRSBUandUU.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true);
    }

    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      // Создание вида документа "Заявка на открытие периодов РСБУ и УУ".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.ApplicationsForDIRECTUM.Resources.ApplicationOpeningRSBUandUUKindName,
                                                                              lenspec.ApplicationsForDIRECTUM.Resources.ApplicationOpeningRSBUandUUKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              ApplicationOpeningRSBUandUU.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              lenspec.ApplicationsForDIRECTUM.Constants.Module.ApplicationOpeningRSBUandUUKind);
    }

    /// <summary>
    /// Выдача прав на документы.
    /// </summary>
    public static void GrantRightsOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant rights on documents.");
      
      ApplicationsForDIRECTUM.ApplicationOpeningRSBUandUUs.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Create);
      ApplicationsForDIRECTUM.ApplicationOpeningRSBUandUUs.AccessRights.Save();
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемую папку.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
      InitializationLogger.Debug("Init: Grant rights on folders.");
      
      ApplicationsForDIRECTUM.SpecialFolders.MyRequests.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ApplicationsForDIRECTUM.SpecialFolders.MyRequests.AccessRights.Save();
    }
  }
}
