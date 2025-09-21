using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonContracts.Server
{
  public class ModuleAsyncHandlers
  {

    public virtual void TerminateContractBySupAgreement(avis.EtalonContracts.Server.AsyncHandlerInvokeArgs.TerminateContractBySupAgreementInvokeArgs args)
    {
    }

    // Посчитать остаточную сумму по договору
    public virtual void CalculateRemainingAmount(avis.EtalonContracts.Server.AsyncHandlerInvokeArgs.CalculateRemainingAmountInvokeArgs args)
    {
      var contract = lenspec.Etalon.Contracts.Get(args.ContractId);
      try
      {
        var lockInfo = Locks.GetLockInfo(contract);
        if (lockInfo.IsLocked)
          throw AppliedCodeException.Create(string.Format("Договор заблокирован пользователем {0}", lockInfo.OwnerName));
        Locks.Lock(contract);
        lenspec.Etalon.PublicFunctions.Contract.CalculateRemainingAmount(contract);
        contract.Save();
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Avis - CalculateRemainingAmount. Ошибка вычиления остаточной суммы по договору с ИД {0}: {1}. {2}", args.ContractId, ex.Message, innerMessage);
        args.Retry = true;
      }
      finally
      {
        Locks.Unlock(contract);
      }
    }

    public virtual void AsyncChangeSynchronizationStatus(avis.EtalonContracts.Server.AsyncHandlerInvokeArgs.AsyncChangeSynchronizationStatusInvokeArgs args)
    {
      var isForcedLocked = false;
      Logger.DebugFormat("AsyncChangeSynchronizationStatus - обновление статуса синхронизации ИД = {0}.", args.ContractualDocumentId);
      var contract = lenspec.Etalon.ContractualDocuments.Get(args.ContractualDocumentId);
      try
      {
        var lockinfo = Locks.GetLockInfo(contract);
        if (lockinfo != null && lockinfo.IsLocked)
        {
          args.Retry = true;
          Logger.ErrorFormat("AsyncChangeSynchronizationStatus - документ с ИД = {0} - заблокирован пользователем {1}.", contract.Id, lockinfo.OwnerName);
          return;
        }
        else
        {
          isForcedLocked = Locks.TryLock(contract);
        }
        
        if (!isForcedLocked)
        {
          throw new Exception($"не удалось заблокировать карточку документа в Directum RX.");
        }
        
        contract.SyncStatus1cavis = lenspec.Etalon.ContractualDocument.SyncStatus1cavis.Sync;
        contract.Save();
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("AsyncChangeSynchronizationStatus - не удалось сохранить изменения по документу с ИД = {0} - {1}.", contract.Id, ex.Message);
        args.Retry = false;
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(contract);
      }

    }

  }
}