using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ApplicationsForPayment.Server
{
  public class ModuleAsyncHandlers
  {

    /// <summary>
    /// Обновление информации об оплате в карточке ЗНО.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncUpdateApplicationForPayment(lenspec.ApplicationsForPayment.Server.AsyncHandlerInvokeArgs.AsyncUpdateApplicationForPaymentInvokeArgs args)
    {
    }

    /// <summary>
    /// Обновление поля Состояние во Вх. счете.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncUpdateIncomingInvoice(lenspec.ApplicationsForPayment.Server.AsyncHandlerInvokeArgs.AsyncUpdateIncomingInvoiceInvokeArgs args)
    {
    }

    /// <summary>
    /// Экспорт Заявки на оплату в интеграционную таблицу.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncExportApplicationForPayment(lenspec.ApplicationsForPayment.Server.AsyncHandlerInvokeArgs.AsyncExportApplicationForPaymentInvokeArgs args)
    {
      //      var isForcedLocked = false;
      //      Logger.DebugFormat("AsyncChangeSynchronizationStatus - обновление статуса синхронизации ИД = {0}.", args.DocumentId);
      //      var contract = lenspec.ApplicationsForPayment.ApplicationForPayments.Get(args.DocumentId);
      //      try
      //      {
      //        var lockinfo = Locks.GetLockInfo(contract);
      //        if (lockinfo != null && lockinfo.IsLocked)
      //        {
      //          args.Retry = true;
      //          Logger.ErrorFormat("AsyncChangeSynchronizationStatus - документ с ИД = {0} - заблокирован пользователем {1}.", contract.Id, lockinfo.OwnerName);
      //          return;
      //        }
      //        else
      //        {
      //          isForcedLocked = Locks.TryLock(contract);
      //        }
//
      //        if (!isForcedLocked)
      //        {
      //          throw new Exception($"не удалось заблокировать карточку документа в Directum RX.");
      //        }
//
      //        contract.SyncStatus1cavis = lenspec.ApplicationsForPayment.ApplicationForPayment.SyncStatus1cavis.Sync;
      //        contract.Save();
      //      }
      //      catch(Exception ex)
      //      {
      //        Logger.ErrorFormat("AsyncChangeSynchronizationStatus - не удалось сохранить изменения по документу с ИД = {0} - {1}.", contract.Id, ex.Message);
      //        args.Retry = false;
      //      }
      //      finally
      //      {
      //        if (isForcedLocked)
      //          Locks.Unlock(contract);
      //      }
    }
  }
}