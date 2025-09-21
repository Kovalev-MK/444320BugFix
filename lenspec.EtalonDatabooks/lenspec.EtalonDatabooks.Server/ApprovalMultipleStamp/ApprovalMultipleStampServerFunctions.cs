using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApprovalMultipleStamp;

namespace lenspec.EtalonDatabooks.Server
{
  partial class ApprovalMultipleStampFunctions
  {
    /// <summary>
    /// Получить утверждающую подпись.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Электронная подпись.</returns>
    private static Sungero.Domain.Shared.ISignature GetSignatureForApprovalMark(Sungero.Docflow.IOfficialDocument document)
    {
      var version = document.Versions.FirstOrDefault(x => x.Id == document.LastVersion.Id);
      if (version == null)
        return null;
      
      // Только утверждающие подписи.
      var versionSignatures = Signatures.Get(version)
        .Where(s => s.IsExternal != true && s.SignatureType == SignatureType.Approval)
        .ToList();
      if (!versionSignatures.Any())
        return null;
      
      // В приоритете подпись сотрудника из поля "Подписал". Квалифицированная ЭП приоритетнее простой.
      return versionSignatures
        .OrderByDescending(s => Equals(s.Signatory, document.OurSignatory))
        .ThenBy(s => s.SignCertificate == null)
        .ThenByDescending(s => s.SigningDate)
        .FirstOrDefault();
    }
    
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      Logger.DebugFormat("ApprovalMultipleStamp. Start execute convert to pdf for task id: {0}, start id: {1}.", approvalTask.Id, approvalTask.StartId);
      
      var result = base.Execute(approvalTask);
    
      var documents = new List<Sungero.Docflow.IOfficialDocument>();
      
      var documentFromTask = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (documentFromTask == null)
      {
        Logger.ErrorFormat("ApprovalConvertPdfStage. Primary document not found. task id: {0}, start id: {1}", approvalTask.Id, approvalTask.StartId);
        return this.GetErrorResult(Sungero.Docflow.Resources.PrimaryDocumentNotFoundError);
      }
      
      documents.Add(documentFromTask);
 
      var addenda = approvalTask.AddendaGroup.OfficialDocuments.ToList();
      documents.AddRange(addenda);
      
     // Количество документов без подписи.
      var notSingnatureCount = 0;
      
      var documentsToConvert = new List<Sungero.Docflow.IOfficialDocument>();
      foreach (var document in documents)
      {
        if (!document.HasVersions)
        {
          Logger.DebugFormat("ApprovalConvertPdfStage. Document with Id {0} has no version.", document.Id);
          continue;
        }
        
        var lockInfo = Locks.GetLockInfo(document.LastVersion.Body);
        if (lockInfo.IsLocked)
        {
          Logger.DebugFormat("ApprovalConvertPdfStage. Document with Id {0} locked {1}.", document.Id, lockInfo.OwnerName);
          return this.GetRetryResult(string.Format(lenspec.EtalonDatabooks.ApprovalNewVersionConvertPDFs.Resources.ConvertPdfLockError, document.Name, document.Id, lockInfo.OwnerName));
        }

        var approvalMark = GetSignatureForApprovalMark(document);
        if (approvalMark == null)
        {
          notSingnatureCount++;
          continue;
        }
        
        documentsToConvert.Add(document);
      }

      foreach (var document in documentsToConvert)
      {
        try
        {
          Logger.DebugFormat("ApprovalMultipleStamp. Start convert to pdf for document id {0}.", document.Id);
          var convertionResult = lenspec.Etalon.Module.Docflow.PublicFunctions.Module.Remote.SearchAndReplaceAnchor(document);
          if (convertionResult != "")
          {
            Logger.ErrorFormat("ApprovalMultipleStamp. Convert to pdf error {0}. Document Id {1}, Version Id {2}", convertionResult, document.Id, document.LastVersion.Id);
            result = this.GetRetryResult(string.Empty);
          }
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("ApprovalMultipleStamp. Convert to pdf error. Document Id {0}, Version Id {1}", ex, document.Id, document.LastVersion.Id);
          result = this.GetRetryResult(string.Empty);
        }
      }
      
      Logger.DebugFormat("ApprovalMultipleStamp. Done execute convert to pdf for task id {0}, success: {1}, retry: {2}", approvalTask.Id, result.Success, result.Retry);
      
      // Отправляем уведомление автору задачи, с количеством не преобразованных документов.
      if (notSingnatureCount != 0)
      {
        var task = Sungero.Workflow.SimpleTasks.CreateAsSubtask(approvalTask.MainTask);
        task.Subject = $"Количество не преобразованных документов {notSingnatureCount}";
        task.ThreadSubject = $"Количество не преобразованных документов {notSingnatureCount}";
        task.Author = approvalTask.Author;
        task.NeedsReview = false;
        var routeStep = task.RouteSteps.AddNew();
        routeStep.AssignmentType = Sungero.Workflow.SimpleTaskRouteSteps.AssignmentType.Notice;
        routeStep.Performer = approvalTask.Author;
        routeStep.Deadline = null;
        task.Start();
      }
      
      return result;
    }
  }
}