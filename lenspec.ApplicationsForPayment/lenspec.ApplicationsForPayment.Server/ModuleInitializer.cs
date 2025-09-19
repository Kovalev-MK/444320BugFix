using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.ApplicationsForPayment.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      #region Роли
      
      CreateRoles();
      
      #endregion
      
      #region Документы
      
      CreateDocumentTypes();
      CreateDocumentKinds();
      GrantRightsOnDocuments();
      
      #endregion
      
      #region Вычисляемые папки
      
      GrantRightsOnFolder();
      
      #endregion
    }
    
    #region Роли
    
    /// <summary>
    /// Создание ролей.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create roles for ApplicationsForPayment");
      
      // Полные права на корректировку и выгрузку ЗНО в 1С.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.ApplicationsForPayment.Resources.UploadApplicationForPaymentRoleName,
                                                                      lenspec.ApplicationsForPayment.Resources.UploadApplicationForPaymentRoleDescription,
                                                                      Constants.Module.UploadApplicationForPaymentRoleGuid);
      // Ответственный бухгалтер НДФЛ (ЗНО).
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.ApplicationsForPayment.Resources.PitResponsibleAccountantRoleName,
                                                                      lenspec.ApplicationsForPayment.Resources.PitResponsibleAccountantRoleDescription,
                                                                      Constants.Module.PitResponsibleAccountantRoleGuid);
    }
    
    #endregion
    
    #region Документы
    
    /// <summary>
    /// Создание типов документов.
    /// </summary>
    public static void CreateDocumentTypes()
    {
      InitializationLogger.Debug("Init: Create DocumentType for ApplicationsForPayment");
      
      // Создание типа документа "Заявка на оплату".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.ApplicationsForPayment.Resources.ApplicationForPaymentTypeName,
                                                                              ApplicationsForPayment.Server.ApplicationForPayment.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts,
                                                                              true);
    }
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      InitializationLogger.Debug("Init: Create DocumentKind for ApplicationsForPayment");
      
      // Вид "Заявка на оплату".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.ApplicationsForPayment.Resources.ApplicationForPaymentKindName,
                                                                              lenspec.ApplicationsForPayment.Resources.ApplicationForPaymentKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true, true,
                                                                              ApplicationsForPayment.Server.ApplicationForPayment.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] 
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval
                                                                              },
                                                                              Constants.Module.ApplicationForPaymentKind,
                                                                              true);
    }
    
    /// <summary>
    /// Выдать права на документы.
    /// </summary>
    public static void GrantRightsOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant rights on documents for ApplicationsForPayment");
      
      var uploadApplicationForPaymentRole = Roles.GetAll(n => n.Sid == Constants.Module.UploadApplicationForPaymentRoleGuid).SingleOrDefault();
      if (uploadApplicationForPaymentRole != null)
        ApplicationForPayments.AccessRights.Grant(uploadApplicationForPaymentRole, DefaultAccessRightsTypes.Change);
      
      ApplicationForPayments.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Create);
      ApplicationForPayments.AccessRights.Save();
    }
    
    #endregion
    
    #region Вычисляемые папки
    
    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемую папку.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
      InitializationLogger.Debug("Init: Grant rights on folders for ApplicationsForPayment");
      
      ApplicationsForPayment.SpecialFolders.RegisterOfApplicationsForPayment.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ApplicationsForPayment.SpecialFolders.RegisterOfApplicationsForPayment.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Реестр заявок на оплату'.");
      
      ApplicationsForPayment.SpecialFolders.MyApplicationsForPayment.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ApplicationsForPayment.SpecialFolders.MyApplicationsForPayment.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Мои заявки'.");
    }
    
    #endregion
  }
}
