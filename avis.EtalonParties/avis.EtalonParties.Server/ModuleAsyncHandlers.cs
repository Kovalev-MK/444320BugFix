using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonParties.Server
{
  public class ModuleAsyncHandlers
  {

    /// <summary>
    /// Закрытие Детализаций видов материалов, привязанных к неактуальному Виду материалов.
    /// </summary>
    /// <param name="args">Аргументы.</param>
    public virtual void AsyncCloseMaterials(avis.EtalonParties.Server.AsyncHandlerInvokeArgs.AsyncCloseMaterialsInvokeArgs args)
    {
    }

    /// <summary>
    /// Закрытие Детализаций видов работ, привязанных к неактуальному Виду работ.
    /// </summary>
    /// <param name="args">Аргументы.</param>
    public virtual void AsyncCloseWorkKinds(avis.EtalonParties.Server.AsyncHandlerInvokeArgs.AsyncCloseWorkKindsInvokeArgs args)
    {
    }

    #region Заполнить поля в Сведениях о контрагенте
    
    public virtual void AsyncFillCounterpartyDocument(avis.EtalonParties.Server.AsyncHandlerInvokeArgs.AsyncFillCounterpartyDocumentInvokeArgs args)
    {
      Logger.DebugFormat("Avis - AsyncFillCounterpartyDocument - заполнение Сведений о КА {0}.", args.CounterpartyDocumentId);
      
      var counterpartyDocument = Sungero.Docflow.CounterpartyDocuments.GetAll(x => x.Id == args.CounterpartyDocumentId).SingleOrDefault();
      if (counterpartyDocument == null)
      {
        Logger.ErrorFormat("Avis - AsyncFillCounterpartyDocument - не удалось найти Сведения о КА {0}.", args.CounterpartyDocumentId);
        args.Retry = false;
        return;
      }
      
      var counterparty = Sungero.Parties.Counterparties.GetAll(x => x.Id == args.CounterpartyId).SingleOrDefault();
      if (counterparty == null)
      {
        Logger.ErrorFormat("Avis - AsyncFillCounterpartyDocument - не удалось найти КА {0}.", args.CounterpartyId);
        args.Retry = false;
        return;
      }
      
      var lockInfo = Locks.GetLockInfo(counterpartyDocument);
      if (lockInfo.IsLocked)
      {
        args.Retry = true;
        return;
      }
      
      var counterpartyDocumentIsLocked = false;
      try
      {
        counterpartyDocumentIsLocked = Locks.TryLock(counterpartyDocument);
        if (counterpartyDocumentIsLocked)
        {
          counterpartyDocument.Counterparty = counterparty;
          counterpartyDocument.Save();
        }
        else
        {
          Logger.DebugFormat("Avis - AsyncFillCounterpartyDocument - не удалось заблокировать Сведения о КА {0}.", args.CounterpartyDocumentId);
          args.Retry = true;
          return;
        }
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - AsyncFillCounterpartyDocument - ", ex);
      }
      finally
      {
        if (counterpartyDocumentIsLocked)
          Locks.Unlock(counterpartyDocument);
      }
    }
    
    #endregion

    #region Закрытие всех записей в справочнике Банковские реквизиты
    
    public virtual void CloseBankDetails(avis.EtalonParties.Server.AsyncHandlerInvokeArgs.CloseBankDetailsInvokeArgs args)
    {
      var isAllClosed = TryCloseBankDetails();
      if (!isAllClosed)
      {
        if (args.RetryIteration >= 5)
        {
          args.Retry = false;
          SendNoticeUnsuccessfullyClosing();
        }
        else
          args.Retry = true;
      }
    }
    
    /// <summary>
    /// Закрыть все записи Банковские реквизиты
    /// </summary>
    /// <returns>true - все записи закрыты, false - остались незакрытые записи</returns>
    private static bool TryCloseBankDetails()
    {
      var bankDetails = BankDetails.GetAll(x => x.Status != avis.EtalonParties.BankDetail.Status.Closed);
      foreach (var bankDetail in bankDetails)
      {
        try
        {
          bankDetail.Status = avis.EtalonParties.BankDetail.Status.Closed;
          bankDetail.Save();
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Фоновый процесс Закрытие банковских реквизитов. Ошибка закрытия записи с Id={0} - {1}", bankDetail.Id, ex.Message);
        }
      }
      return bankDetails.All(x => x.Status == avis.EtalonParties.BankDetail.Status.Closed);
    }
    
    /// <summary>
    /// Отправить уведомление Администратору СЭД, если не все записи Банковских реквизитов закрыты
    /// </summary>
    private static void SendNoticeUnsuccessfullyClosing()
    {
      var unsuccessfullyEntries = avis.EtalonParties.BankDetails.GetAll(x => x.Status != avis.EtalonParties.BankDetail.Status.Closed);
      if (unsuccessfullyEntries.Any())
      {
        var subject = "Фоновый процесс 'Закрытие банковских реквизитов'. Не удалось закрыть эти записи, проверьте логи.";
        var role = Roles.GetAll(x => x.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).SingleOrDefault();
        var task = Sungero.Workflow.SimpleTasks.CreateWithNotices(subject, role);
        foreach (var entry in unsuccessfullyEntries)
        {
          task.Attachments.Add(entry);
        }
        task.Start();
      }
    }
    
    #endregion

  }
}