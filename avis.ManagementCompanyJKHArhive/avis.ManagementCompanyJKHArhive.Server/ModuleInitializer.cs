using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace avis.ManagementCompanyJKHArhive.Server
{
  public partial class ModuleInitializer
  {

    public override bool IsModuleVisible()
    {
      return Users.Current.IncludedIn(Roles.AllUsers);
    }
    
    /// <summary>
    /// Инициализация.
    /// </summary>
    /// <param name="e"></param>
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Создание типов документов в служебном справочнике "Типы документов".
      CreateDocumentTypes();
      // Создание видов документов.
      CreateDocumentKinds();
      // Выдать права на документы.
      GrantRightsOnDocuments();
      // Создать типы связей.
      CreateRelationTypes();
    }
    
    /// <summary>
    /// Создание типов документов в служебном справочнике "Типы документов".
    /// </summary>
    public static void CreateDocumentTypes()
    {
      InitializationLogger.Debug("Init: CreateDocumentTypes for ManagementCompanyJKHArhive.");
      
      // Договор управления МКД.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(avis.ManagementCompanyJKHArhive.Resources.ManagementContractMKDName,
                                                                              ManagementContractMKD.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
      // Документы для УК.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(avis.ManagementCompanyJKHArhive.Resources.DocumentForManagementCompanyName,
                                                                              DocumentForManagementCompany.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
      // Документы для УК.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(avis.ManagementCompanyJKHArhive.Resources.BasisForManagementContractMKDName,
                                                                              BasisForManagementContractMKD.ClassTypeGuid,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              false);
    }
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      InitializationLogger.Debug("Init: CreateDocumentKinds for ManagementCompanyJKHArhive.");
      
      // Создание вида документа «Договор управления МКД».
      // В качестве ИД вида документа используется константа MKDManagementAgreement.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.ManagementCompanyJKHArhive.Resources.MKDDocumentKindName,
                                                                              avis.ManagementCompanyJKHArhive.Resources.MKDDocumentKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              ManagementContractMKD.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval },
                                                                              Constants.Module.MKDManagementAgreementKind,
                                                                              true);
      
      // Создание вида документа «Направление от застройщика (при заселении в дом)».
      // В качестве ИД вида документа используется константа DocumentsForUK.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.ManagementCompanyJKHArhive.Resources.DirectionFromDeveloperName,
                                                                              avis.ManagementCompanyJKHArhive.Resources.DirectionFromDeveloperShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              BasisForManagementContractMKD.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { },
                                                                              Constants.Module.DirectionFromDeveloperBaseForManagementAgreementKind,
                                                                              true);
      
      // Создание вида документа «Правоустанавливающий документ (выписка из ЕГРН)».
      // В качестве ИД вида документа используется константа DocumentsForUK.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.ManagementCompanyJKHArhive.Resources.TitleDocumentName,
                                                                              avis.ManagementCompanyJKHArhive.Resources.TitleDocumentShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              BasisForManagementContractMKD.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { },
                                                                              Constants.Module.TitleDocumentBaseForManagementAgreementKind,
                                                                              true);
      
      // Создание вида документа «Прочие документы ЖКХ».
      // В качестве ИД вида документа используется константа DocumentsForUK.
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.ManagementCompanyJKHArhive.Resources.OtherDocJKHName,
                                                                              avis.ManagementCompanyJKHArhive.Resources.OtherDocJKHShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner, true, false,
                                                                              DocumentForManagementCompany.ClassTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { },
                                                                              Constants.Module.OtherDocJKHKind,
                                                                              true);
    }
    
    /// <summary>
    /// Выдать права на документы.
    /// </summary>
    public static void GrantRightsOnDocuments()
    {
      var fullPermitArhivJKHGuid = lenspec.Etalon.Module.Parties.PublicConstants.Module.FullPermitArhivJKH;
      var fullPermitArhivJKHRole = Roles.GetAll(r => Equals(r.Sid, fullPermitArhivJKHGuid)).FirstOrDefault();
      
      if (fullPermitArhivJKHRole == null)
        return;
      
      InitializationLogger.Debug("Init: Grant rights on documents to role \"Полные права на управление архивом для управляющих компаний (ЖКХ)\" for ManagementCompanyJKHArhive.");
      
      DocumentForManagementCompanies.AccessRights.Grant(fullPermitArhivJKHRole, DefaultAccessRightsTypes.FullAccess);
      DocumentForManagementCompanies.AccessRights.Save();
      
      BasisForManagementContractMKDs.AccessRights.Grant(fullPermitArhivJKHRole, DefaultAccessRightsTypes.FullAccess);
      BasisForManagementContractMKDs.AccessRights.Save();
      
      ManagementContractMKDs.AccessRights.Grant(fullPermitArhivJKHRole, DefaultAccessRightsTypes.FullAccess);
      ManagementContractMKDs.AccessRights.Save();
      
      ManagementCompanyJKHArhive.SpecialFolders.MKDFolder.AccessRights.Grant(fullPermitArhivJKHRole, DefaultAccessRightsTypes.FullAccess);
      ManagementCompanyJKHArhive.SpecialFolders.MKDFolder.AccessRights.Save();
    }
    
    /// <summary>
    /// Создание типов связей.
    /// </summary>
    public void CreateRelationTypes()
    {
      InitializationLogger.Debug("Init: Create ArchivJKH relation types for ManagementCompanyJKHArhive.");
      
      // Документ-основание для создания договора управления МКД - Договор управления МКД.
      var basisMKD = RelationTypes.GetAll(r => r.Name == Constants.Module.DocumentBasisToManagementAgreementMKDRelationName).FirstOrDefault() ?? RelationTypes.Create();
      basisMKD.Name = Constants.Module.DocumentBasisToManagementAgreementMKDRelationName;
      basisMKD.SourceDescription = avis.ManagementCompanyJKHArhive.Resources.BasisMKDDescription;
      basisMKD.TargetDescription = avis.ManagementCompanyJKHArhive.Resources.BasisMKDTitle;
      basisMKD.TargetTitle = avis.ManagementCompanyJKHArhive.Resources.BasisMKDTitle;
      basisMKD.SourceTitle = avis.ManagementCompanyJKHArhive.Resources.BasisMKDDescription;
      basisMKD.HasDirection = true;
      basisMKD.NeedSourceUpdateRights = false;
      basisMKD.UseSource = true;
      basisMKD.UseTarget = true;
      
      basisMKD.Mapping.Clear();
      var basisMKDRow = basisMKD.Mapping.AddNew();
      basisMKDRow.Source = ManagementCompanyJKHArhive.BasisForManagementContractMKDs.Info;
      basisMKDRow.Target = ManagementCompanyJKHArhive.ManagementContractMKDs.Info;
      basisMKD.Save();
    }
  }
}