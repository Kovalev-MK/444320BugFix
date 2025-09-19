using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using avis.PowerOfAttorneyModule.ExecutionPowerOfAttorney;

namespace avis.PowerOfAttorneyModule.Server
{
  //Avis-Expert>>
  partial class ExecutionPowerOfAttorneyRouteHandlers
  {

    public virtual void Script24Execute()
    {
      var poas = _obj.POAs.PowerOfAttorneys;
      foreach (var poa in poas)
      {
        foreach (var version in poa.Versions)
        {
          if (!version.Equals(poa.LastVersion))
            version.IsHidden = true;
        }
      }
    }

    public virtual void CompleteAssignment12(avis.PowerOfAttorneyModule.IIssuanceOriginalAssignment assignment, avis.PowerOfAttorneyModule.Server.IssuanceOriginalAssignmentArguments e)
    {
      foreach(var document in _obj.AllAttachments)
      {
        document.AccessRights.Revoke(assignment.Performer, DefaultAccessRightsTypes.Change);
        document.AccessRights.Grant(assignment.Performer, DefaultAccessRightsTypes.Read);
        document.AccessRights.Save();
      }
    }

    public virtual void CompleteAssignment7(avis.PowerOfAttorneyModule.IReworkAssignment assignment, avis.PowerOfAttorneyModule.Server.ReworkAssignmentArguments e)
    {
      foreach(var document in _obj.AllAttachments)
      {
        document.AccessRights.Revoke(assignment.Performer, DefaultAccessRightsTypes.Change);
        document.AccessRights.Revoke(assignment.Performer, DefaultAccessRightsTypes.FullAccess);
        document.AccessRights.Grant(assignment.Performer, DefaultAccessRightsTypes.Read);
        document.AccessRights.Save();
      }
    }

    public virtual void CompleteAssignment11(avis.PowerOfAttorneyModule.IScanningAssignment assignment, avis.PowerOfAttorneyModule.Server.ScanningAssignmentArguments e)
    {
      foreach(var document in _obj.AllAttachments)
      {
        document.AccessRights.Revoke(assignment.Performer, DefaultAccessRightsTypes.Change);
        document.AccessRights.Grant(assignment.Performer, DefaultAccessRightsTypes.Read);
        document.AccessRights.Save();
      }
    }

    public virtual void CompleteAssignment6(avis.PowerOfAttorneyModule.IRegistrationPowerOfAttorneyAssignment assignment, avis.PowerOfAttorneyModule.Server.RegistrationPowerOfAttorneyAssignmentArguments e)
    {
      foreach(var document in _obj.AllAttachments)
      {
        document.AccessRights.Revoke(assignment.Performer, DefaultAccessRightsTypes.Change);
        document.AccessRights.Grant(assignment.Performer, DefaultAccessRightsTypes.Read);
        document.AccessRights.Save();
      }
    }

    public virtual void StartAssignment11(avis.PowerOfAttorneyModule.IScanningAssignment assignment, avis.PowerOfAttorneyModule.Server.ScanningAssignmentArguments e)
    {
      var projectPOA = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      projectPOA.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.Signed;
      projectPOA.Save();
      
      var poas = _obj.POAs.PowerOfAttorneys.AsEnumerable();
      foreach (var poa in poas)
      {
        poa.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.Signed;
        poa.Save();
      }
    }

    public virtual void CompleteAssignment10(avis.PowerOfAttorneyModule.IPrintAndSigningAssignment assignment, avis.PowerOfAttorneyModule.Server.PrintAndSigningAssignmentArguments e)
    {
      var projectPOA = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      projectPOA.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.Signed;
      projectPOA.Save();
      
      var poas = _obj.POAs.PowerOfAttorneys.AsEnumerable();
      foreach (var poa in poas)
      {
        poa.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.Signed;
        poa.Save();
      }
      
      foreach(var document in _obj.AllAttachments)
      {
        document.AccessRights.Revoke(assignment.Performer, DefaultAccessRightsTypes.Change);
        document.AccessRights.Grant(assignment.Performer, DefaultAccessRightsTypes.Read);
        document.AccessRights.Save();
      }
    }

    public virtual void StartAssignment10(avis.PowerOfAttorneyModule.IPrintAndSigningAssignment assignment, avis.PowerOfAttorneyModule.Server.PrintAndSigningAssignmentArguments e)
    {
      var projectPOA = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      projectPOA.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.PendingSign;
      projectPOA.Save();
      
      var poas = _obj.POAs.PowerOfAttorneys.AsEnumerable();
      foreach (var poa in poas)
      {
        poa.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.PendingSign;
        poa.Save();
      }
    }

    public virtual void StartAssignment3(avis.PowerOfAttorneyModule.IApprovalManagerInitiatorAssignment assignment, avis.PowerOfAttorneyModule.Server.ApprovalManagerInitiatorAssignmentArguments e)
    {
      var projectPOA = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      projectPOA.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.OnApproval;
      projectPOA.Save();
      
      var poas = _obj.POAs.PowerOfAttorneys.AsEnumerable();
      foreach (var poa in poas)
      {
        poa.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.OnApproval;
        poa.Save();
      }
    }

    public virtual void StartAssignment7(avis.PowerOfAttorneyModule.IReworkAssignment assignment, avis.PowerOfAttorneyModule.Server.ReworkAssignmentArguments e)
    {
      var projectPOA = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      projectPOA.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.OnRework;
      projectPOA.Save();
      
      var poas = _obj.POAs.PowerOfAttorneys.AsEnumerable();
      foreach (var poa in poas)
      {
        poa.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.OnRework;
        poa.Save();
      }
    }

    public virtual void CompleteAssignment4(avis.PowerOfAttorneyModule.IExecutionLawyerAssignment assignment, avis.PowerOfAttorneyModule.Server.ExecutionLawyerAssignmentArguments e)
    {
      foreach(var document in _obj.AllAttachments)
      {
        document.AccessRights.Revoke(assignment.Performer, DefaultAccessRightsTypes.Change);
        document.AccessRights.Revoke(assignment.Performer, DefaultAccessRightsTypes.FullAccess);
        document.AccessRights.Grant(assignment.Performer, DefaultAccessRightsTypes.Read);
        
        if(assignment.Result != PowerOfAttorneyModule.ExecutionLawyerAssignment.Result.Reject &&
           assignment.Result != PowerOfAttorneyModule.ExecutionLawyerAssignment.Result.Rework)
        {
          document.AccessRights.RevokeAll(_obj.Author);
          document.AccessRights.Grant(_obj.Author, DefaultAccessRightsTypes.Read);
        }
        document.AccessRights.Save();
      }
      if(assignment.Result == PowerOfAttorneyModule.ExecutionLawyerAssignment.Result.Forward)
      {
        assignment.Forward(assignment.Forward);
      }
    }

    public virtual void Script22Execute()
    {
      var projectPOA = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      projectPOA.InternalApprovalState = lenspec.Etalon.PowerOfAttorney.InternalApprovalState.Aborted;
      projectPOA.Save();
      
      var poas = _obj.POAs.PowerOfAttorneys;
      foreach (var poa in poas)
      {
        poa.LifeCycleState = lenspec.Etalon.PowerOfAttorney.LifeCycleState.Active;
        poa.Save();
      }
      
      var documentsAttorney = _obj.AttorneyDocuments.SimpleDocuments.Where(x => x.DocumentKind.Equals(Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(PublicConstants.Module.DocumentAttorneyKindGuid)));
      if(documentsAttorney != null && documentsAttorney.Any())
      {
        foreach(var sd in documentsAttorney)
        {
          sd.Versions.Clear();
        }
      }
    }

    public virtual void StartBlock20(Sungero.Workflow.Server.NoticeArguments e)
    {
      if(_obj.ProjectPOA.PowerOfAttorneys.Any())
      {
        e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.SubjectNoticeRejectPowerOfAttorney, _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault().Name);
      }
      var performer = _obj.Author;
      e.Block.Performers.Add(performer);
    }

    public virtual void StartBlock19(Sungero.Workflow.Server.NoticeArguments e)
    {
      if(_obj.ProjectPOA.PowerOfAttorneys.Any())
      {
        e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.SubjectNoticeAuthoritiesSigning, _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault().Name);
      }
      
      var roleKindObservers = lenspec.EtalonDatabooks.RoleKinds.GetAll(x => x.Name == avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.RoleKindNameObservers).FirstOrDefault();
      if (roleKindObservers != null)
      {
        var projectPOA = _obj.ProjectPOA.PowerOfAttorneys.ToList();
        foreach (var item in projectPOA)
        {
          if (item.OurSignatory != null && item.OurSignatory.Department.BusinessUnit != null)
          {
            var bu = lenspec.Etalon.BusinessUnits.As(item.OurSignatory.Department.BusinessUnit);
            var observer = bu.RoleKindEmployeelenspec.Where(x => roleKindObservers.Equals(x.RoleKind)).FirstOrDefault();
            if (observer != null)
              e.Block.Performers.Add(observer.Employee);
          }
        }
      }
      if (!e.Block.Performers.Any())
      {
        var performers = _obj.Observers.Select(i => i.Observer);
        foreach(var empl in performers)
        {
          e.Block.Performers.Add(empl);
        }
      }
    }

    public virtual void StartBlock18(avis.PowerOfAttorneyModule.Server.SettingUpAssignmentArguments e)
    {
      if(_obj.ProjectPOA.PowerOfAttorneys.Any())
      {
        e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.SubjectSettingUpAssignment, _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault().Name);
      }
      var performers = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
      e.Block.Performers.Add(performers);
    }

    public virtual void StartBlock17(avis.PowerOfAttorneyModule.Server.NoticeIssuanceOriginalArguments e)
    {
//      if(_obj.ProjectPOA.PowerOfAttorneys.Any())
//      {
//        e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.SubjectNoticeGetOriginal, _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault().Name);
//      }
//      var performers = Functions.ExecutionPowerOfAttorney.GetAttorneyPerformers(_obj);
//      foreach(var empl in performers)
//      {
//        e.Block.Performers.Add(empl);
//      }
    }

    public virtual void StartBlock15(Sungero.Workflow.Server.NoticeArguments e)
    {
      if(_obj.ProjectPOA.PowerOfAttorneys.Any())
      {
        e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.SubjectNoticeEndingExecution, _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault().Name);
      }
      var performer = _obj.Author;
      e.Block.Performers.Add(performer);
    }

    public virtual bool Decision14Result()
    {
      var document = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      return document.AuthorityKindsavis.Any(x => x.AuthKind.Name.Equals(avis.PowerOfAttorneyModule.Resources.EnterIntoContractName));
    }

    public virtual bool Decision13Result()
    {
      //      var document = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      //      var attorneys = document.Attorneyavis.Where(x => lenspec.Etalon.People.Is(x.Attorn));
      //      attorneys = attorneys.Where(x => lenspec.Etalon.People.As(x.Attorn).IsEmployeeGKavis == true);
      //      return attorneys.Any();
      return true;
    }

    public virtual void StartBlock12(avis.PowerOfAttorneyModule.Server.IssuanceOriginalAssignmentArguments e)
    {
      if(_obj.ProjectPOA.PowerOfAttorneys.Any())
      {
        e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.SubjectIssuanceOriginalAssignment, _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault().Name);
      }
      var document = Sungero.Docflow.OfficialDocuments.As(_obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault());
      var performer = lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovalRoleKindPerformer(null, document, lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk);
      e.Block.Performers.Add(performer);
    }

    public virtual void StartBlock11(avis.PowerOfAttorneyModule.Server.ScanningAssignmentArguments e)
    {
      if(_obj.ProjectPOA.PowerOfAttorneys.Any())
      {
        e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.SubjectScanningAssignment, _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault().Name);
      }
      var document = Sungero.Docflow.OfficialDocuments.As(_obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault());
      var performer = lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovalRoleKindPerformer(null, document, lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk);
      e.Block.Performers.Add(performer);
    }

    public virtual void StartBlock10(avis.PowerOfAttorneyModule.Server.PrintAndSigningAssignmentArguments e)
    {
      e.Block.RelativeDeadlineDays = 2;
      if(_obj.ProjectPOA.PowerOfAttorneys.Any())
      {
        e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.SubjectPrintAndSigningAssignment, _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault().Name);
      }
      var document = Sungero.Docflow.OfficialDocuments.As(_obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault());
      var performer = lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovalRoleKindPerformer(null, document, lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk);
      e.Block.Performers.Add(performer);
    }
    
    public virtual void Script9Execute()
    {
      var powerOfAttorneys = _obj.POAs.PowerOfAttorneys.ToList();
      if(powerOfAttorneys == null || !powerOfAttorneys.Any())
      {
        Logger.Error(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.ErrorLogMessageEmptyDocumentCollection);
        return;
      }
      Functions.ExecutionPowerOfAttorney.ConvertToPdfPowerOfAttorneys(_obj, powerOfAttorneys);
      
      // Скрыть все версии, кроме последней, у Доверенностей и Проектов доверенностей.
      var documentsToHideVersion = new List<lenspec.Etalon.IPowerOfAttorney>();
      documentsToHideVersion.AddRange(_obj.POAs.PowerOfAttorneys.ToList());
      documentsToHideVersion.AddRange(_obj.ProjectPOA.PowerOfAttorneys.ToList());
      foreach (var poa in documentsToHideVersion)
      {
        foreach (var version in poa.Versions)
        {
          if (!version.Equals(poa.LastVersion) && version.IsHidden != true)
            version.IsHidden = true;
        }
      }
    }

    public virtual void StartBlock6(avis.PowerOfAttorneyModule.Server.RegistrationPowerOfAttorneyAssignmentArguments e)
    {
      e.Block.RelativeDeadlineDays = 2;
      var document = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.RegistrationPowerOfAttorneyAssignments.Resources.Subject, document.Name);
      var offDocument = Sungero.Docflow.OfficialDocuments.As(document);
      var performer = lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovalRoleKindPerformer(null, offDocument, lenspec.EtalonDatabooks.Resources.RegistrarPowerOfAttorneyRoleName);
      e.Block.Performers.Add(performer);
    }

    public virtual void StartBlock7(avis.PowerOfAttorneyModule.Server.ReworkAssignmentArguments e)
    {
      var document = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      if(document != null)
      {
        e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.SubjectReworkAssignment, document.Name);
      }
      var performer = _obj.Author;
      e.Block.Performers.Add(performer);
    }

    public virtual void StartBlock4(avis.PowerOfAttorneyModule.Server.ExecutionLawyerAssignmentArguments e)
    {
      e.Block.RelativeDeadlineDays = 2;
      var document = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionLawyerAssignments.Resources.Subject, document.Name);
      e.Block.Amount = document.Amountavis;
//      foreach(var item in document.Attorneyavis)
//      {
//        var line = e.Block.Attorneys.AddNew();
//        line.Attorney = item.Attorn;
//      }
      foreach(var item in document.DocKindsavis)
      {
        var line = e.Block.DocKindsavis.AddNew();
        line.Kind = item.Kind;
      }
      foreach(var item in document.ContractCategoryavis)
      {
        var line = e.Block.ContractCategoryavis.AddNew();
        line.Category = item.Category;
      }
      
      var offDocument = Sungero.Docflow.OfficialDocuments.As(document);
      var performer = lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovalRoleKindPerformer(null, offDocument, avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.POAResponsibleLawyerRoleName);
      e.Block.Performers.Add(performer);
    }
    
    public virtual void StartBlock3(avis.PowerOfAttorneyModule.Server.ApprovalManagerInitiatorAssignmentArguments e)
    {
      if(_obj.ProjectPOA.All.Any())
      {
        var docName = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault().Name;
        e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.ApprovalManagerInitiatorAssignments.Resources.Subject, docName);
      }
      var performer = _obj.ManagerInitiator;
      e.Block.Performers.Add(performer);
    }

    public virtual bool Decision5Result()
    {
      var mainDoc = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      return mainDoc.DocumentKind.Id == Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DocumentNotarialKindGuid).Id;
    }
    //<<Avis-Expert
  }
}