using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace avis.ApprovingCounterpartyDEB.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateDocumentKinds();
      GrantAccessRightsToDocuments();
      CreateRoles();
      GrantAccessRightsToReports();
      GrantAccessRightsToTasks();
    }
    
    public void CreateRoles()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.ApprovingCounterpartyDEB.Resources.RightsOnReportApprovalCounterparty, avis.ApprovingCounterpartyDEB.Resources.RightsOnReportApprovalCounterparty, Constants.Module.RightsReportOnApprovalCounterpartyRoleGuid);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.ApprovingCounterpartyDEB.Resources.DebReportRoleName, avis.ApprovingCounterpartyDEB.Resources.DebReportRoleDescription, Constants.Module.DebReportRoleGuid);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.ApprovingCounterpartyDEB.Resources.ExpressReportKFRoleName, avis.ApprovingCounterpartyDEB.Resources.ExpressReportKFRoleDescription, Constants.Module.ExpressReportKFRoleGuid);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.ApprovingCounterpartyDEB.Resources.ResponsibleLimitEconomistRoleName, avis.ApprovingCounterpartyDEB.Resources.ResponsibleLimitEconomistRoleDescription, Constants.Module.ResponsibleLimitEconomist);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.ApprovingCounterpartyDEB.Resources.NotifiedMembersApprovalDEBRoleName, avis.ApprovingCounterpartyDEB.Resources.NotifiedMembersApprovalDEBRoleDescription, Constants.Module.NotifiedMembersApprovalDEB);
    }

    /// <summary>
    /// Создать виды документы модуля
    /// </summary>
    public void CreateDocumentKinds()
    {
      InitializationLogger.Debug("Init: Create document kinds");
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(avis.ApprovingCounterpartyDEB.Resources.ApprovalCounterpartyBankTypeAndKindName,
                                                                              Constants.ApprovalCounterpartyBankDEB.ApprovalCounterpartyTypeGuid, Sungero.Docflow.DocumentKind.DocumentFlow.Inner, false);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.ApprovingCounterpartyDEB.Resources.ApprovalCounterpartyBankTypeAndKindName,
                                                                              avis.ApprovingCounterpartyDEB.Resources.ApprovalCounterpartyBankTypeAndKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Inner,
                                                                              true, false, Constants.ApprovalCounterpartyBankDEB.ApprovalCounterpartyTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.ApprovalCounterpartyBankDEB.ApprovalCounterpartyKindGuid, true);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentType(avis.ApprovingCounterpartyDEB.Resources.ApprovalPersonTypeAndKindName,
                                                                              Constants.ApprovalPersonDEB.ApprovalPersonTypeGuid, Sungero.Docflow.DocumentKind.DocumentFlow.Inner, false);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(avis.ApprovingCounterpartyDEB.Resources.ApprovalPersonTypeAndKindName,
                                                                              avis.ApprovingCounterpartyDEB.Resources.ApprovalPersonTypeAndKindName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentKind.DocumentFlow.Inner,
                                                                              true, false, Constants.ApprovalPersonDEB.ApprovalPersonTypeGuid,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.ApprovalPersonDEB.ApprovalPersonKindGuid, true);
    }
    
    /// <summary>
    /// Выдать права на документы модуля
    /// </summary>
    public void GrantAccessRightsToDocuments()
    {
      InitializationLogger.Debug("Init: Grant acess rights on documents");
      ApprovalCounterpartyBankDEBs.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ApprovalCounterpartyBankDEBs.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Create);
      ApprovalCounterpartyBankDEBs.AccessRights.Save();
      
      ApprovalPersonDEBs.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ApprovalPersonDEBs.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Create);
      ApprovalPersonDEBs.AccessRights.Save();
    }
    
    /// <summary>
    /// Выдача прав на отчеты модуля
    /// </summary>
    public void GrantAccessRightsToReports()
    {
      InitializationLogger.Debug("Init: Grant acess rights on reports");
      Reports.AccessRights.Grant(Reports.GetCounterpartiesOnApproval().Info, Roles.AllUsers, DefaultReportAccessRightsTypes.Execute);
      Reports.AccessRights.Grant(Reports.GetApprovalSheetDefault().Info, Roles.AllUsers, DefaultReportAccessRightsTypes.Execute);
    }
    
    public void GrantAccessRightsToTasks()
    {
      InitializationLogger.Debug("Init: Grant acess rights on tasks");
      ApprovalCounterpartyPersonDEBs.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Create);
      ApprovalCounterpartyPersonDEBs.AccessRights.Save();
    }

  }
}
