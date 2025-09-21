using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace avis.PowerOfAttorneyModule.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateRoles();
      GrantAccessRightOnDatabooks();
      GrantAccessRightOnDocuments();
      GrantAccessRightOnFolders();
      CreateDocumentTypes();
      CreateDocumentKinds();
      CreateAuthorityKinds();
      CreateApprovalFunctions();
    }
    
    public void GrantAccessRightOnDatabooks()
    {
      InitializationLogger.Debug("init: Grant access right on databook 'Authority'");
      Authorities.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      var responsibleForAuthorities = Roles.GetAll(x => x.Sid == Constants.Module.RoleGuidResponsibleForAuthorities).SingleOrDefault();
      Authorities.AccessRights.Grant(responsibleForAuthorities, DefaultAccessRightsTypes.FullAccess);
      Authorities.AccessRights.Save();
      
      InitializationLogger.Debug("init: Grant access right on databook 'AuthorityKind'");
      AuthorityKinds.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      AuthorityKinds.AccessRights.Save();
    }
    
    public void CreateRoles()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.PowerOfAttorneyModule.Resources.RoleNameResponsibleForAuthorities, string.Empty, Constants.Module.RoleGuidResponsibleForAuthorities);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.PowerOfAttorneyModule.Resources.RoleNameResponsibleForRightSign, string.Empty, Constants.Module.RoleGuidResponsibleRightSign);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.PowerOfAttorneyModule.Resources.RoleNameRightChangePowerOfAttorneyGroup, string.Empty, Constants.Module.RoleGuidRightChangePowerOfAttorneysGroup);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.PowerOfAttorneyModule.Resources.RoleNameRightCreatePowerOfAttorneys, string.Empty, Constants.Module.RoleGuidRightCreatePowerOfAttorneys);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.PowerOfAttorneyModule.Resources.RoleNameResponsibleRecallPowerOfAttorney, string.Empty, Constants.Module.RoleGuidResponsibleRecallPowerOfAttorneys);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.PowerOfAttorneyModule.Resources.RoleNameRightAllPowerOfAttorneysGroup, string.Empty, Constants.Module.RoleGuidRightAllPowerOfAttorneysGroup);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.PowerOfAttorneyModule.Resources.RoleNameResponsibleLawyer, string.Empty, Constants.Module.RoleGuidResponsibleLawyer);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.PowerOfAttorneyModule.Resources.RoleNameRightReadPowerOfAttorneyGroup, string.Empty, Constants.Module.RoleGuidRightReadAllPowerOfAttorneysGroup);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.PowerOfAttorneyModule.Resources.RoleNameRightsToAttachScans, string.Empty, Constants.Module.RoleGuidRightsToAttachScans);
    }
    
    public void GrantAccessRightOnDocuments()
    {
      var roleRightAllPoasGroup = Roles.GetAll(x => x.Sid == Constants.Module.RoleGuidRightAllPowerOfAttorneysGroup).SingleOrDefault();
      var roleRightChangePoas = Roles.GetAll(x => x.Sid == Constants.Module.RoleGuidRightChangePowerOfAttorneysGroup).SingleOrDefault();
      
      InitializationLogger.Debug("Grant access right on document type 'FormalizedPowerOfAttorney");
      lenspec.Etalon.FormalizedPowerOfAttorneys.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Create);
      lenspec.Etalon.FormalizedPowerOfAttorneys.AccessRights.Grant(roleRightChangePoas, DefaultAccessRightsTypes.Change);
      lenspec.Etalon.FormalizedPowerOfAttorneys.AccessRights.Save();
      
      InitializationLogger.Debug("Grant access right on document type 'ApplicationRelinquishmentAuthority");
      avis.PowerOfAttorneyModule.ApplicationRelinquishmentAuthorities.AccessRights.Grant(roleRightAllPoasGroup, DefaultAccessRightsTypes.FullAccess);
      avis.PowerOfAttorneyModule.ApplicationRelinquishmentAuthorities.AccessRights.Save();
    }
    
    public void GrantAccessRightOnFolders()
    {
      SpecialFolders.RegistegOfPowerOfAttorneyFolder.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      SpecialFolders.RegistegOfPowerOfAttorneyFolder.AccessRights.Save();
      
      var foldersOfficeRole = Roles.GetAll(x => x.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.FoldersOffice).SingleOrDefault();
      
      SpecialFolders.IssuingPowerOfAttorney.AccessRights.Grant(foldersOfficeRole, DefaultAccessRightsTypes.Read);
      SpecialFolders.IssuingPowerOfAttorney.AccessRights.Save();
      
      SpecialFolders.PrintingAndSigningPowerOfAttorney.AccessRights.Grant(foldersOfficeRole, DefaultAccessRightsTypes.Read);
      SpecialFolders.PrintingAndSigningPowerOfAttorney.AccessRights.Save();
      
      SpecialFolders.RegistrationPowerOfAttorney.AccessRights.Grant(foldersOfficeRole, DefaultAccessRightsTypes.Read);
      SpecialFolders.RegistrationPowerOfAttorney.AccessRights.Save();
      
      SpecialFolders.ScanningPowerOfAttorney.AccessRights.Grant(foldersOfficeRole, DefaultAccessRightsTypes.Read);
      SpecialFolders.ScanningPowerOfAttorney.AccessRights.Save();
      
      SpecialFolders.FormalizedPowerOfAttorneys.AccessRights.Grant(foldersOfficeRole, DefaultAccessRightsTypes.Read);
      SpecialFolders.FormalizedPowerOfAttorneys.AccessRights.Save();
      
      SpecialFolders.RegistrationPowerOfAttorneysForTask.AccessRights.Grant(foldersOfficeRole, DefaultAccessRightsTypes.Read);
      SpecialFolders.RegistrationPowerOfAttorneysForTask.AccessRights.Save();
      
      SpecialFolders.PowerOfAttorneyRequests.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      SpecialFolders.PowerOfAttorneyRequests.AccessRights.Save();
    }
    
    public void CreateDocumentKinds()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.PowerOfAttorneyModule.Resources.DocumentAttorneyKindName, avis.PowerOfAttorneyModule.Resources.DocumentAttorneyKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Inner,
                                                                              false, false, Guid.Parse("09584896-81e2-4c83-8f6c-70eb8321e1d0"),
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.DocumentAttorneyKindGuid, false);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.PowerOfAttorneyModule.Resources.ApplicationRelinquishmentAuthorityKindName, avis.PowerOfAttorneyModule.Resources.ApplicationRelinquishmentAuthorityKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Inner,
                                                                              true, false, Guid.Parse("99ac6696-d9d6-43c4-92af-42cc2ed9194c"),
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.ApplicationRelinquishmentAuthorityKindGuid, false);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.PowerOfAttorneyModule.Resources.DocumentNotarialKindName, avis.PowerOfAttorneyModule.Resources.DocumentNotarialKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Inner,
                                                                              true, false, Guid.Parse("be859f9b-7a04-4f07-82bc-441352bce627"),
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval, Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.DocumentNotarialKindGuid, false);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.PowerOfAttorneyModule.Resources.RequestToCreatePowerOfAttorneyKindName, avis.PowerOfAttorneyModule.Resources.RequestToCreatePowerOfAttorneyKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Inner,
                                                                              true, false, Guid.Parse("be859f9b-7a04-4f07-82bc-441352bce627"),
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval, Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.RequestToCreatePowerOfAttorneyKindGuid, false);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.PowerOfAttorneyModule.Resources.RequestToCreateNotarialPowerOfAttorneyKindName, avis.PowerOfAttorneyModule.Resources.RequestToCreateNotarialPowerOfAttorneyKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Inner,
                                                                              true, false, Guid.Parse("be859f9b-7a04-4f07-82bc-441352bce627"),
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval, Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid, false);
    }
    
    public void CreateDocumentTypes()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(avis.PowerOfAttorneyModule.Resources.ApplicationRelinquishmentAuthorityKindName,
                                                                              Guid.Parse("99ac6696-d9d6-43c4-92af-42cc2ed9194c"),
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Inner, true);
    }
    
    public void CreateAuthorityKinds()
    {
      InitializationLogger.Debug($"init: Create AuthorityKind '{avis.PowerOfAttorneyModule.Resources.EnterIntoContractName}'");
      if (!AuthorityKinds.GetAll(x => x.Name == avis.PowerOfAttorneyModule.Resources.EnterIntoContractName).Any())
      {
        var authorityKind = AuthorityKinds.Create();
        authorityKind.Name = avis.PowerOfAttorneyModule.Resources.EnterIntoContractName;
        authorityKind.Save();
      }
    }
    
    public void CreateApprovalFunctions()
    {
      InitializationLogger.Debug("Init: Create Hiding versions approval function");
      if (avis.PowerOfAttorneyModule.HidingVersionsFunctions.GetAll().Any())
        return;
      var hidingVersionsFunction = avis.PowerOfAttorneyModule.HidingVersionsFunctions.Create();
      hidingVersionsFunction.Name = avis.PowerOfAttorneyModule.Resources.HidingVersionsFunctionName;
      hidingVersionsFunction.TimeoutInHours = 4;
      hidingVersionsFunction.TimeoutAction = Sungero.Docflow.ApprovalFunctionStageBase.TimeoutAction.Skip;
      hidingVersionsFunction.Save();
      
      InitializationLogger.Debug("Init: Create Removing body power of attorney function");
      if (avis.PowerOfAttorneyModule.RemovingBodyFunctions.GetAll().Any())
        return;
      var removingBodyFunction = avis.PowerOfAttorneyModule.RemovingBodyFunctions.Create();
      removingBodyFunction.Name = avis.PowerOfAttorneyModule.Resources.RemovingBodyFunctionName;
      removingBodyFunction.TimeoutInHours = 4;
      removingBodyFunction.TimeoutAction = Sungero.Docflow.ApprovalFunctionStageBase.TimeoutAction.Skip;
      removingBodyFunction.Save();
    }
  }
}