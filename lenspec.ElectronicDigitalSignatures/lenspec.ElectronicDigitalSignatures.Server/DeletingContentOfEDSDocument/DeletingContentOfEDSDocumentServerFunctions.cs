using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.DeletingContentOfEDSDocument;

namespace lenspec.ElectronicDigitalSignatures.Server
{
  partial class DeletingContentOfEDSDocumentFunctions
  {

    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = base.Execute(approvalTask);
      
      var document = lenspec.ElectronicDigitalSignatures.EDSApplications.As(approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault());
      if (document == null)
        return this.GetErrorResult(lenspec.ElectronicDigitalSignatures.DeletingContentOfEDSDocuments.Resources.DocumentNotFound);
      
      try
      {
        if (document.HasVersions)
          DeleteAllVersions(document);
        
        var addendas = document.Relations.GetRelatedAndRelatedFromDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName)
          .Where(x => x.HasVersions && lenspec.ElectronicDigitalSignatures.EDSDocuments.Is(x))
          .Select(x => Sungero.Docflow.OfficialDocuments.As(x));
        foreach (var addenda in addendas)
        {
          DeleteAllVersions(addenda);
        }
        
        var certificates = Sungero.CoreEntities.Certificates.GetAll(x => x.Enabled == true && Equals(x.Owner, document.PreparedBy));
        foreach (var certificate in certificates)
        {
          certificate.Enabled = false;
          certificate.Save();
        }
        
        foreach (var text in approvalTask.Texts)
        {
          if (!string.IsNullOrEmpty(text.Body))
            text.Body = lenspec.ElectronicDigitalSignatures.DeletingContentOfEDSDocuments.Resources.SafetyRequirementsMessage;
        }
        if (approvalTask.State.IsChanged)
          approvalTask.Save();
        
        var assignments = Sungero.Workflow.Assignments.GetAll(x => x.Task == approvalTask && x.Texts.Any(t => t.Body != string.Empty && t.Body != null));
        foreach (var assignment in assignments)
        {
          foreach (var text in assignment.Texts)
          {
            text.Body = lenspec.ElectronicDigitalSignatures.DeletingContentOfEDSDocuments.Resources.SafetyRequirementsMessage;
          }
          if (assignment.State.IsChanged)
            assignment.Save();
        }
      }
      catch (Exception ex)
      {
        result = this.GetRetryResult(string.Empty);
        Logger.ErrorFormat("Avis - DeletingContentOfEDSDocument - approvalTask {0}", ex, approvalTask);
      }
      
      return result;
    }
    
    /// <summary>
    /// Удалить все версии документа.
    /// </summary>
    private void DeleteAllVersions(Sungero.Docflow.IOfficialDocument document)
    {
      var versions = document.Versions.ToList();
      foreach (var version in versions)
      {
        document.DeleteVersion(version);
      }
      document.Save();
    }
  }
}