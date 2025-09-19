using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApprovalNewVersionConvertPDF;

namespace lenspec.EtalonDatabooks.Server
{
  partial class ApprovalNewVersionConvertPDFFunctions
  {
    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      Logger.DebugFormat("ApprovalConvertPdfStage. Start execute convert to pdf for task id: {0}, start id: {1}.", approvalTask.Id, approvalTask.StartId);
      
      var result = base.Execute(approvalTask);
      
      var documents = new List<Sungero.Docflow.IOfficialDocument>();
      
      var documentFromTask = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (documentFromTask == null)
      {
        Logger.ErrorFormat("ApprovalConvertPdfStage. Primary document not found. task id: {0}, start id: {1}", approvalTask.Id, approvalTask.StartId);
        return this.GetErrorResult(Sungero.Docflow.Resources.PrimaryDocumentNotFoundError);
      }
      
      documents.Add(documentFromTask);
      if (_obj.IsConvertAddendums == true)
      {
        var addenda = approvalTask.AddendaGroup.OfficialDocuments.ToList();
        documents.AddRange(addenda);
      }
      
      var documentsToConvert = new List<Sungero.Docflow.IOfficialDocument>();
      foreach (var document in documents)
      {
        if (!document.HasVersions)
        {
          Logger.DebugFormat("ApprovalConvertPdfStage. Document with Id {0} has no version.", document.Id);
          continue;
        }
        
        // Формат не поддерживается.
        var versionExtension = document.LastVersion.BodyAssociatedApplication.Extension.ToLower();
        var versionExtensionIsSupported = Sungero.AsposeExtensions.Converter.CheckIfExtensionIsSupported(versionExtension);
        if (!versionExtensionIsSupported)
        {
          Logger.DebugFormat("ApprovalConvertPdfStage. Document with Id {0} unsupported format {1}.", document.Id, versionExtension);
          continue;
        }
        
        var lockInfo = Locks.GetLockInfo(document.LastVersion.Body);
        if (lockInfo.IsLocked)
        {
          Logger.DebugFormat("ApprovalConvertPdfStage. Document with Id {0} locked {1}.", document.Id, lockInfo.OwnerName);
          return this.GetRetryResult(string.Format(lenspec.EtalonDatabooks.ApprovalNewVersionConvertPDFs.Resources.ConvertPdfLockError, document.Name, document.Id, lockInfo.OwnerName));
        }
        
        documentsToConvert.Add(document);
      }
      
      foreach (var document in documentsToConvert)
      {
        try
        {
          Logger.DebugFormat("ApprovalConvertPdfStage. Start convert to pdf for document id {0}.", document.Id);
          // Конвертируем в pdf.
          lenspec.Etalon.Module.Docflow.PublicFunctions.Module.ConvertToPdfAvis(document, _obj.RewriteOriginal == true);
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("ApprovalConvertPdfStage. Convert to pdf error. Document Id {0}, Version Id {1}", ex, document.Id, document.LastVersion.Id);
          result = this.GetRetryResult(string.Empty);
        }
      }
      
      Logger.DebugFormat("ApprovalConvertPdfStage. Done execute convert to pdf for task id {0}, success: {1}, retry: {2}", approvalTask.Id, result.Success, result.Retry);
      
      return result;
    }
  }
}