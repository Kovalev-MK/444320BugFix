using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.UpdateEDSApplicationState;

namespace lenspec.ElectronicDigitalSignatures.Server
{
  partial class UpdateEDSApplicationStateFunctions
  {

    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = base.Execute(approvalTask);
      
      var document = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (document == null)
        return this.GetErrorResult(lenspec.ElectronicDigitalSignatures.UpdateEDSApplicationStates.Resources.DocumentNotFound);
      
      if (!lenspec.ElectronicDigitalSignatures.EDSApplications.Is(document) && !lenspec.ApplicationsForPayment.ApplicationForPayments.Is(document))
        return this.GetErrorResult(lenspec.ElectronicDigitalSignatures.UpdateEDSApplicationStates.Resources.DocumentTypeError);
      
      if (Locks.GetLockInfo(document).IsLocked)
      {
        result = this.GetRetryResult(string.Empty);
        return result;
      }
      
      var isForcedLocked = false;
      try
      {
        isForcedLocked = Locks.TryLock(document);
        if (isForcedLocked)
        {
          var edsApplication = lenspec.ElectronicDigitalSignatures.EDSApplications.As(document);
          if (edsApplication != null)
          {
            edsApplication.ApplicationState = _obj.NewApplicationState;
            edsApplication.Save();
          }
          
          var applicationForPayment = lenspec.ApplicationsForPayment.ApplicationForPayments.As(document);
          if (applicationForPayment != null)
          {
            applicationForPayment.InternalApprovalState = _obj.NewInternalApprovalState;
            applicationForPayment.Save();
          }
        }
        else
        {
          result = this.GetRetryResult(string.Empty);
          Logger.ErrorFormat("Avis - UpdateEDSApplicationState - approvalTask {0} - не удалось заблокировать картоку документа.", approvalTask);
          return result;
        }
      }
      catch (Exception ex)
      {
        result = this.GetRetryResult(string.Empty);
        Logger.ErrorFormat("Avis - UpdateEDSApplicationState - approvalTask {0}", ex, approvalTask);
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(document);
      }
      
      return result;
    }
  }
}