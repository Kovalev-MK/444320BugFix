using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.SalesDepartmentArchive.Server
{
  public class ModuleAsyncHandlers
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Обновить реквизиты клиентского договора.
    /// </summary>
    /// <param name="args">Аргументы АО.</param>
    public virtual void AsyncUpdateClientContract(lenspec.SalesDepartmentArchive.Server.AsyncHandlerInvokeArgs.AsyncUpdateClientContractInvokeArgs args)
    {
      var isForcedLocked = false;
      Logger.DebugFormat("Avis - AsyncUpdateClientContract - обновление реквизитов клиентского договора ИД={0}", args.ClientContractId);
      var clientContract = SalesDepartmentArchive.SDAClientContracts.Get(args.ClientContractId);
      try
      {
        if (clientContract == null)
        {
          args.Retry = false;
          throw new Exception($"не найден клиентский договор ИД={args.ClientContractId}.");
        }
        
        var storageAddress = SalesDepartmentArchive.StorageAddresses.Get(args.StorageAddressId);
        if (storageAddress == null)
        {
          args.Retry = false;
          throw new Exception($"не найдено место хранения ИД={args.StorageAddressId}.");
        }
        
        var lockinfo = Locks.GetLockInfo(clientContract);
        if (lockinfo != null && lockinfo.IsLocked)
        {
          args.Retry = true;
          throw new Exception($"карточка заблокирована пользователем {lockinfo.OwnerName}.");
        }
        else
          isForcedLocked = Locks.TryLock(clientContract);
        
        if (!isForcedLocked)
          throw new Exception($"не удалось заблокировать карточку документа в Directum RX.");
        
        clientContract.StorageAddress = storageAddress;
        // Снять обязательность на случай незаполненных обязательных полей.
        foreach(var property in clientContract.State.Properties)
        {
          property.IsRequired = false;
        }
        clientContract.Save();
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - AsyncUpdateClientContract - обновление реквизитов клиентского договора ИД={0} - ", ex, args.ClientContractId);
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(clientContract);
      }
    }
    //конец Добавлено Avis Expert

  }
}