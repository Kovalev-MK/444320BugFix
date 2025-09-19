using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class PowerOfAttorneyRequestsFolderHandlers
  {

    public virtual IQueryable<Sungero.Docflow.IPowerOfAttorney> PowerOfAttorneyRequestsDataQuery(IQueryable<Sungero.Docflow.IPowerOfAttorney> query)
    {
      var requestCreatePOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
      var requestCreateNPOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
      query = query.Where(x => Equals(Sungero.Company.Employees.Current, x.Author) && (Equals(x.DocumentKind, requestCreatePOAKind) || Equals(x.DocumentKind, requestCreateNPOAKind)));
      return query;
    }
  }

  partial class RegistegOfPowerOfAttorneyFolderFolderHandlers
  {

    public virtual IQueryable<Sungero.Docflow.IPowerOfAttorneyBase> RegistegOfPowerOfAttorneyFolderDataQuery(IQueryable<Sungero.Docflow.IPowerOfAttorneyBase> query)
    {
      var requestCreatePOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
      var requestCreateNPOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
      query = query.Where(x => !Equals(x.DocumentKind, requestCreatePOAKind) && !Equals(x.DocumentKind, requestCreateNPOAKind));
      return query;
    }
  }

  partial class RegistrationPowerOfAttorneysForTaskFolderHandlers
  {

    public virtual bool IsRegistrationPowerOfAttorneysForTaskVisible()
    {
      var foldersOfficeRole = Roles.GetAll(x => x.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.FoldersOffice).SingleOrDefault();
      return Users.Current.IncludedIn(foldersOfficeRole);
    }

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> RegistrationPowerOfAttorneysForTaskDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = lenspec.EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, lenspec.EtalonDatabooks.FlowFolderSetting.FolderName.RegPOAExecTask);
      return result;
    }
  }

  partial class FormalizedPowerOfAttorneysFolderHandlers
  {

    public virtual bool IsFormalizedPowerOfAttorneysVisible()
    {
      var foldersOfficeRole = Roles.GetAll(x => x.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.FoldersOffice).SingleOrDefault();
      return Users.Current.IncludedIn(foldersOfficeRole);
    }

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> FormalizedPowerOfAttorneysDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = lenspec.EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, lenspec.EtalonDatabooks.FlowFolderSetting.FolderName.FormalizedPOA);
      return result;
    }
  }

  partial class ScanningPowerOfAttorneyFolderHandlers
  {

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> ScanningPowerOfAttorneyDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = lenspec.EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, lenspec.EtalonDatabooks.FlowFolderSetting.FolderName.ScanningPOA);
      return result;
    }
  }

  partial class RegistrationPowerOfAttorneyFolderHandlers
  {

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> RegistrationPowerOfAttorneyDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = lenspec.EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, lenspec.EtalonDatabooks.FlowFolderSetting.FolderName.RegistrationPOA);
      return result;
    }
  }

  partial class PrintingAndSigningPowerOfAttorneyFolderHandlers
  {

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> PrintingAndSigningPowerOfAttorneyDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = lenspec.EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, lenspec.EtalonDatabooks.FlowFolderSetting.FolderName.PrintSignPOA);
      return result;
    }
  }

  partial class IssuingPowerOfAttorneyFolderHandlers
  {

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> IssuingPowerOfAttorneyDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = lenspec.EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, lenspec.EtalonDatabooks.FlowFolderSetting.FolderName.IssuingPOA);
      return result;
    }
  }

  partial class PowerOfAttorneyModuleHandlers
  {
  }
}