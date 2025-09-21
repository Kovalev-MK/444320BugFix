using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForPayment.ApplicationForPayment;

namespace lenspec.ApplicationsForPayment.Client
{
  partial class ApplicationForPaymentFunctions
  {

    /// <summary>
    /// Логгировать ошибку блокировки и вывести сообщение для пользователя.
    /// </summary>
    [Public]
    public void GetLockErrorMessage()
    {
      Logger.ErrorFormat("ApplicationForPayment. {0}", Locks.GetLockInfo(_obj).LockedMessage);
      Dialogs.ShowMessage(Locks.GetLockInfo(_obj).LockedMessage, MessageType.Error);
    }

    /// <summary>
    /// Функция для экспорта документов в 1С.
    /// </summary>
    [Public]
    public void Export1C()
    {
      bool documentUnloaded = true;
      documentUnloaded = PublicFunctions.ApplicationForPayment.UnloadingDocuments(_obj);
      if (documentUnloaded)
      {
        try
        {
          _obj.Export1CDate = Calendar.Now;
          _obj.Export1CState = Export1CState.Yes;
          
          if (_obj.State.IsChanged)
            _obj.Save();
          
          Dialogs.ShowMessage(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.DataExportStarted, MessageType.Information);
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("ApplicationForPayment. {0}", ex, _obj.Id);
          Dialogs.ShowMessage(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ErrorExport, MessageType.Error);
        }
        finally
        {
          if (Locks.GetLockInfo(_obj).IsLockedByMe)
            Locks.Unlock(_obj);
        }
      }
      else
        Dialogs.ShowMessage(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ErrorExport, MessageType.Error);
    }

    /// <summary>
    /// Показывать сводку по документу в заданиях на согласование и подписание.
    /// </summary>
    /// <returns>True, если в заданиях нужно показывать сводку по документу.</returns>
    [Public]
    public override bool NeedViewDocumentSummary()
    {
      var contract = lenspec.Etalon.Contracts.As(_obj.Contract);
      return contract != null && contract.IsDeferredPaymentlenspec == true;
    }
  }
}