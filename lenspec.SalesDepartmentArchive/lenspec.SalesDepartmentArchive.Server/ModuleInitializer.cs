using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.SalesDepartmentArchive.Server
{
  public partial class ModuleInitializer
  {

    public override bool IsModuleVisible()
    {
      return true;
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
      
      // Отчеты.
      GrantRightsOnReports();
      
      // Сценарии.
      CreateFillingDocumentKindsOfRealEstateSale();
    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles");
      
      // Полные права на документы УК.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.SalesDepartmentArchive.Resources.RoleNameSalesDepartmentArchiveResponsible,
                                                                      lenspec.SalesDepartmentArchive.Resources.RoleDescriptionSalesDepartmentArchiveResponsible,
                                                                      SalesDepartmentArchive.Constants.Module.SalesDepartmentArchiveResponsible);
      
      // Полные права на Акты при приемке объектов долевого строительства и Акты в гарантийный период (старое название "Полные права на акты передачи квартир покупателям").
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.SalesDepartmentArchive.Resources.RoleNameFullRightsActsApartmentsBuyers,
                                                                      lenspec.SalesDepartmentArchive.Resources.RoleDescriptionFullRightsActsApartmentsBuyers,
                                                                      SalesDepartmentArchive.Constants.Module.FullRightsActsApartmentsBuyers);

      //Полные права на акты управляющих компаний.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.SalesDepartmentArchive.Resources.RoleNameFullRightsActsOfManagementCompany,
                                                                      lenspec.SalesDepartmentArchive.Resources.RoleDescriptionFullRightsActsOfManagementCompany,
                                                                      SalesDepartmentArchive.Constants.Module.FullRightsActsOfManagementCompany);

      // Права на создание клиентских договоров.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(lenspec.SalesDepartmentArchive.Resources.RoleNameClientContractsCreatingRole,
                                                                      lenspec.SalesDepartmentArchive.Resources.RoleDescriptionClientContractsCreatingRole,
                                                                      SalesDepartmentArchive.Constants.Module.ClientContractsCreatingRole);
    }
    
    
    #region Документы
    
    /// <summary>
    /// Создание типов документов в служебном справочнике "Типы документов".
    /// </summary>
    public static void CreateDocumentTypes()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.SalesDepartmentArchive.Resources.ClientContractTypeName,
                                                                              SDAClientContract.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.SalesDepartmentArchive.Resources.ClientDocumentTypeName,
                                                                              SDAClientDocument.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.SalesDepartmentArchive.Resources.RequestSubmissionToArchiveTypeName,
                                                                              SDARequestSubmissionToArchive.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.SalesDepartmentArchive.Resources.RequestIssuanceFromArchiveTypeName,
                                                                              SDARequestIssuanceFromArchive.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.SalesDepartmentArchive.Resources.ActsAcceptanceOfAppartments,
                                                                              SDAActAcceptanceOfApartment.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.SalesDepartmentArchive.Resources.ActWarrantyPeriod,
                                                                              SDAActWarrantyPeriod.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(lenspec.SalesDepartmentArchive.Resources.ActsOfManagementCompany,
                                                                              ActsOfManagementCompany.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true);
    }
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      // Создание вида документа «Клиентский договор».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.ClientContractKindName,
                                                                              lenspec.SalesDepartmentArchive.Resources.ClientContractKindShotName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              SDAClientContract.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.SDAClientContractKind,
                                                                              true);
      // Создание вида документа «Доп. соглашение к клиентскому договору».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.ClientDocumentAgreementKindName,
                                                                              lenspec.SalesDepartmentArchive.Resources.ClientDocumentAgreementKindShotName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              SDAClientDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.SDAClientDocumentAgreementKind,
                                                                              true);
      // Создание вида документа «Приложение к клиентскому договору».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.ClientDocumentAddendumKindName,
                                                                              lenspec.SalesDepartmentArchive.Resources.ClientDocumentAddendumKindShotName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              SDAClientDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.SDAClientDocumentAddendumKind,
                                                                              false);
      // Создание вида документа «Прочий документ к клиентскому договору».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.ClientDocumentOtherKindName,
                                                                              lenspec.SalesDepartmentArchive.Resources.ClientDocumentOtherKindShotName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              SDAClientDocument.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.SDAClientDocumentOtherKind,
                                                                              false);
      // Создание вида документа «Заявка на сдачу в архив».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.RequestSubmissionToArchiveKindName,
                                                                              lenspec.SalesDepartmentArchive.Resources.RequestSubmissionToArchiveKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              SDARequestSubmissionToArchive.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.SDARequestSubmissionToArchiveKind,
                                                                              true);
      // Создание вида документа «Заявка на выдачу из архива».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.RequestIssuanceFromArchiveKindName,
                                                                              lenspec.SalesDepartmentArchive.Resources.RequestIssuanceFromArchiveKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              SDARequestIssuanceFromArchive.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.SDARequestIssuanceFromArchiveKind,
                                                                              true);
      
      // Создание вида документа «Акт осмотра».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.InspectionActName,
                                                                              lenspec.SalesDepartmentArchive.Resources.InspectionActShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              SDAActAcceptanceOfApartment.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.InspectionAct,
                                                                              true);
      
      // Создание вида документа "Акт выполненных работ (приемка)" (старое название «Акт выполненных работ»).
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.CertificateOfCompletionName,
                                                                              lenspec.SalesDepartmentArchive.Resources.CertificateOfCompletionShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              SDAActAcceptanceOfApartment.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.CertificateOfCompletion,
                                                                              true);
      
      // Создание вида документа «Акт строительной готовности».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.ConstructionReadinessActName,
                                                                              lenspec.SalesDepartmentArchive.Resources.ConstructionReadinessActShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              SDAActAcceptanceOfApartment.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.ConstructionReadinessAct,
                                                                              true);
      
      // Создание вида документа «Акт приема-передачи помещения».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.AcceptanceTransferApartmentName,
                                                                              lenspec.SalesDepartmentArchive.Resources.AcceptanceTransferApartmentShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              SDAActAcceptanceOfApartment.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.AcceptanceTransferApartmentKind,
                                                                              true);
      
      // Создание вида документа "Акт комиссионного осмотра помещения" (старое название "Акт комиссионного обследования").
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.CommissionInspectionActName,
                                                                              lenspec.SalesDepartmentArchive.Resources.CommissionInspectionActShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              SDAActWarrantyPeriod.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.CommissionInspectionActKind,
                                                                              false);
      
      // Создание вида документа "Акт об устранении дефектов".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.DefectRemedyActName,
                                                                              lenspec.SalesDepartmentArchive.Resources.DefectRemedyActShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              SDAActWarrantyPeriod.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.DefectRemedyActKind,
                                                                              false);
      
      // Создание вида документа "Акт выполненных работ (гарантия)" (старое название "Акт комиссионного обследования").
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.CertificateOfCompletionActName,
                                                                              lenspec.SalesDepartmentArchive.Resources.CertificateOfCompletionActShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              SDAActWarrantyPeriod.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.CertificateOfCompletionActKind,
                                                                              true);
      
      // Создание вида документа "Акт комиссионного осмотра мест общего пользования".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.CommissionInspectionReportPublicName,
                                                                              lenspec.SalesDepartmentArchive.Resources.CommissionInspectionReportPublicShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              SDAActWarrantyPeriod.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.CommissionInspectionReportPublicKind,
                                                                              false);
      
      // Создание вида документа "Акт комиссионного обследования (УК)".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.CommissionInspectionActUkName,
                                                                              lenspec.SalesDepartmentArchive.Resources.CommissionInspectionActUkShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              ActsOfManagementCompany.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.CommissionInspectionActUkKind,
                                                                              false);
      
      // Создание вида документа "Акт об устранении дефектов (УК)".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.DefectRemedyActUkName,
                                                                              lenspec.SalesDepartmentArchive.Resources.DefectRemedyActUkShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              ActsOfManagementCompany.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.DefectRemedyActUkKind,
                                                                              false);
      
      // Создание вида документа "Акт выполненных работ (УК)".
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.SalesDepartmentArchive.Resources.CertificateOfCompletionActUkName,
                                                                              lenspec.SalesDepartmentArchive.Resources.CertificateOfCompletionActUkShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, true,
                                                                              ActsOfManagementCompany.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.CertificateOfCompletionActUkKind,
                                                                              false);
    }
    
    /// <summary>
    /// Выдать права на документы.
    /// </summary>
    public static void GrantRightsOnDocuments()
    {
      InitializationLogger.Debug("Init: Grant rights on documents to Sales department archive module responsible.");
      
      var salesDepartmentArchiveResponsible = Roles.GetAll(n => n.Sid == SalesDepartmentArchive.Constants.Module.SalesDepartmentArchiveResponsible).SingleOrDefault();
      // Роль "Полные права на акты передачи квартир покупателям".
      var fullRightsActsApartmentsBuyers = Roles.GetAll(n => n.Sid == SalesDepartmentArchive.Constants.Module.FullRightsActsApartmentsBuyers).SingleOrDefault();
      // Роль "Полные права на акты управляющих компаний".
      var fullRightsActsOfManagementCompany = Roles.GetAll(n => n.Sid == SalesDepartmentArchive.Constants.Module.FullRightsActsOfManagementCompany).SingleOrDefault();
      // Роль "Права на создание клиентских договоров".
      var clientContractsCreatingRole = Roles.GetAll(n => n.Sid == SalesDepartmentArchive.Constants.Module.ClientContractsCreatingRole).SingleOrDefault();
      
      SalesDepartmentArchive.SDAClientContracts.AccessRights.Grant(clientContractsCreatingRole, DefaultAccessRightsTypes.Change);
      // По требованию ТЗ: убрать права доступа для роли Полные права на документы УК.
      if (SalesDepartmentArchive.SDAClientContracts.AccessRights.IsGranted(DefaultAccessRightsTypes.Create, salesDepartmentArchiveResponsible))
        SalesDepartmentArchive.SDAClientContracts.AccessRights.Revoke(salesDepartmentArchiveResponsible, DefaultAccessRightsTypes.Create);
      
      SalesDepartmentArchive.ActsOfManagementCompanies.AccessRights.Grant(fullRightsActsOfManagementCompany, DefaultAccessRightsTypes.Create);
      SalesDepartmentArchive.SDAClientDocuments.AccessRights.Grant(salesDepartmentArchiveResponsible, DefaultAccessRightsTypes.Create);
      SalesDepartmentArchive.SDARequestSubmissionToArchives.AccessRights.Grant(salesDepartmentArchiveResponsible, DefaultAccessRightsTypes.Create);
      SalesDepartmentArchive.SDARequestIssuanceFromArchives.AccessRights.Grant(salesDepartmentArchiveResponsible, DefaultAccessRightsTypes.Create);
      SDAActAcceptanceOfApartments.AccessRights.Grant(fullRightsActsApartmentsBuyers, DefaultAccessRightsTypes.Create);
      SDAActWarrantyPeriods.AccessRights.Grant(fullRightsActsApartmentsBuyers, DefaultAccessRightsTypes.Create);
      
      SalesDepartmentArchive.SDAClientContracts.AccessRights.Save();
      SalesDepartmentArchive.ActsOfManagementCompanies.AccessRights.Save();
      SalesDepartmentArchive.SDAClientDocuments.AccessRights.Save();
      SalesDepartmentArchive.SDARequestSubmissionToArchives.AccessRights.Save();
      SalesDepartmentArchive.SDARequestIssuanceFromArchives.AccessRights.Save();
      SDAActAcceptanceOfApartments.AccessRights.Save();
      SDAActWarrantyPeriods.AccessRights.Save();
    }
    
    #endregion
    
    /// <summary>
    /// Выдать права на справочники.
    /// </summary>
    public static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Grant rights on databooks to Sales department archive module responsible.");
      
      var salesDepartmentArchiveResponsible = Roles.GetAll(n => n.Sid == SalesDepartmentArchive.Constants.Module.SalesDepartmentArchiveResponsible).SingleOrDefault();
      // Роль "Полные права на акты передачи квартир покупателям".
      var fullRightsActsApartmentsBuyers = Roles.GetAll(n => n.Sid == SalesDepartmentArchive.Constants.Module.FullRightsActsApartmentsBuyers).SingleOrDefault();
      
      DocumentKindsOfRealEstateSales.AccessRights.Grant(salesDepartmentArchiveResponsible, DefaultAccessRightsTypes.FullAccess);
      DocumentKindsOfRealEstateSales.AccessRights.Grant(fullRightsActsApartmentsBuyers, DefaultAccessRightsTypes.Read);
      StorageAddresses.AccessRights.Grant(salesDepartmentArchiveResponsible, DefaultAccessRightsTypes.FullAccess);
      
      DocumentKindsOfRealEstateSales.AccessRights.Save();
      StorageAddresses.AccessRights.Save();
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемую папку.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
    }
    
    /// <summary>
    /// Создание записи нового типа сценария "Заполнение поля Вид док. недвижимости в карточке Клиентского договора".
    /// </summary>
    public static void CreateFillingDocumentKindsOfRealEstateSale()
    {
      InitializationLogger.DebugFormat("Init: Create stage for Starting the Local regulations Acquaintance Task.");
      if (SalesDepartmentArchive.FillingDocumentKindsOfRealEstateSales.GetAll().Any())
        return;

      var stage = SalesDepartmentArchive.FillingDocumentKindsOfRealEstateSales.Create();
      stage.Name = lenspec.SalesDepartmentArchive.Resources.FillingDocumentKindsOfRealEstateSaleName;
      stage.TimeoutInHours = 4;
      stage.TimeoutAction = LocalRegulations.StartingAcquaintanceTask.TimeoutAction.Repeat;
      stage.Save();
    }
    
    /// <summary>
    /// Функция инициализации для выдачи прав на отчеты.
    /// </summary>
    public static void GrantRightsOnReports()
    {
      
      InitializationLogger.Debug("Init: Grant right on reports.");
      
      var clerks = Sungero.Docflow.PublicFunctions.DocumentRegister.Remote.GetClerks();
      Reports.AccessRights.Grant(Reports.GetClientDocumentsWithAnEmptyClientFieldReport().Info, clerks, DefaultReportAccessRightsTypes.Execute);

    }
    //конец Добавлено Avis Expert
  }
}
