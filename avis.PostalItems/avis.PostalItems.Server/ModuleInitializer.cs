using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace avis.PostalItems.Server
{
  // Добавлено avis.
  
  public partial class ModuleInitializer
  {
    /// <summary>
    /// Инциализация.
    /// </summary>
    /// <param name="e"></param>
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Создание типов документов.
      CreateDocumentTypes();
      
      // Создание видов документов.
      CreateDocumentKinds();
      
      // Выдача прав, делопроизводителям.
      GrantRightsToClerksRole();
      
      // Заполнение справочника "Категории РПО".
      CreateCategpryRPOs();
    }
    
    /// <summary>
    /// Создание типов документов в служебном справочнике "Типы документов".
    /// </summary>
    private static void CreateDocumentTypes()
    {
      InitializationLogger.Debug("Init: Create Document Types for PostalItems.");
      
      // Компонент письма.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(avis.PostalItems.Resources.LetterComplonentDocumentTypeName,
                                                                              LetterComponentDocument.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
    }
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    private static void CreateDocumentKinds()
    {
      InitializationLogger.Debug("Init: Creat eDocument Kinds for PostalItems.");
      
      // Создание вида документа «Конверт».
      // В качестве ИД вида документа используется константа PostalItemEnvelope.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.PostalItems.Resources.PostalItemEnvelopeName,
                                                                              avis.PostalItems.Resources.PostalItemEnvelopeShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              LetterComponentDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.PostalItemEnvelope,
                                                                              true);
      
      // Создание вида документа «Чек».
      // В качестве ИД вида документа используется константа PostalItemCheck.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.PostalItems.Resources.PostalItemCheckName,
                                                                              avis.PostalItems.Resources.PostalItemCheckShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              LetterComponentDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.PostalItemCheck,
                                                                              true);
      
      // Создание вида документа «Уведомление».
      // В качестве ИД вида документа используется константа PostalItemNotification.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.PostalItems.Resources.PostalItemNotificationName,
                                                                              avis.PostalItems.Resources.PostalItemNotificationShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              LetterComponentDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.PostalItemNotification,
                                                                              true);
    }
    
    /// <summary>
    ///  Выдача прав, делопроизводителям.
    /// </summary>
    private static void GrantRightsToClerksRole()
    {
      InitializationLogger.Debug("Init: Grant rights on documents to clerks for PostalItems.");
      
      var clerks = Roles.GetAll(r => r.Sid == Sungero.Docflow.PublicConstants.Module.RoleGuid.ClerksRole).FirstOrDefault();
      if (clerks == null)
        return;
     
      LetterComponentDocuments.AccessRights.Grant(clerks, DefaultAccessRightsTypes.FullAccess);
      LetterComponentDocuments.AccessRights.Save();
    }
    
    /// <summary>
    /// Заполнение справочника "Категории РПО".
    /// </summary>
    private static void CreateCategpryRPOs()
    {
      InitializationLogger.Debug("Init: Create Categpry RPOs for PostalItems.");
      
      CreateCategoryRPO(avis.PostalItems.Resources.RegisteredCategoryRPOName, avis.PostalItems.Constants.Module.RPOCustomGuid.ToString());
      CreateCategoryRPO(avis.PostalItems.Resources.AbroadCategoryRPOName, avis.PostalItems.Constants.Module.RPOAbroadGuid.ToString());
      CreateCategoryRPO(avis.PostalItems.Resources.SimpleCategoryRPOName, avis.PostalItems.Constants.Module.RPOSimpleGuid.ToString());
      CreateCategoryRPO(avis.PostalItems.Resources.ValuableCategoryRPOName, avis.PostalItems.Constants.Module.RPOValuableGuid.ToString());
    }
    
    /// <summary>
    /// Создать нового рпо.
    /// </summary>
    /// <param name="name">Название.</param>
    /// <param name="sid">Уникальный ИД, регистрозависимый.</param>
    private static void CreateCategoryRPO(string name, string sid)
    {
      // Проверяем что рпо с таким сид не существует.
      var rpo = avis.PostalItems.CategoryRPOs.GetAll(c => c.Sid == sid).FirstOrDefault();
      
      if (rpo != null)
        return;
      
      // Создаём новое РПО.
      rpo = avis.PostalItems.CategoryRPOs.Create();
      rpo.Name = name;
      rpo.Sid = sid;
      rpo.Save();
    }
  }
  
  // Конец добавлено avis.
}