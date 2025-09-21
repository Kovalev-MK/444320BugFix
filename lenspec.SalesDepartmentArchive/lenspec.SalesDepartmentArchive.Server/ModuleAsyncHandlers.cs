using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.SalesDepartmentArchive.Server
{
  public class ModuleAsyncHandlers
  {

    //��������� Avis Expert
    /// <summary>
    /// �������� ��������� ����������� ��������.
    /// </summary>
    /// <param name="args">��������� ��.</param>
    public virtual void AsyncUpdateClientContract(lenspec.SalesDepartmentArchive.Server.AsyncHandlerInvokeArgs.AsyncUpdateClientContractInvokeArgs args)
    {
      var isForcedLocked = false;
      Logger.DebugFormat("Avis - AsyncUpdateClientContract - ���������� ���������� ����������� �������� ��={0}", args.ClientContractId);
      var clientContract = SalesDepartmentArchive.SDAClientContracts.Get(args.ClientContractId);
      try
      {
        if (clientContract == null)
        {
          args.Retry = false;
          throw new Exception($"�� ������ ���������� ������� ��={args.ClientContractId}.");
        }
        
        var storageAddress = SalesDepartmentArchive.StorageAddresses.Get(args.StorageAddressId);
        if (storageAddress == null)
        {
          args.Retry = false;
          throw new Exception($"�� ������� ����� �������� ��={args.StorageAddressId}.");
        }
        
        var lockinfo = Locks.GetLockInfo(clientContract);
        if (lockinfo != null && lockinfo.IsLocked)
        {
          args.Retry = true;
          throw new Exception($"�������� ������������� ������������� {lockinfo.OwnerName}.");
        }
        else
          isForcedLocked = Locks.TryLock(clientContract);
        
        if (!isForcedLocked)
          throw new Exception($"�� ������� ������������� �������� ��������� � Directum RX.");
        
        clientContract.StorageAddress = storageAddress;
        // ����� �������������� �� ������ ������������� ������������ �����.
        foreach(var property in clientContract.State.Properties)
        {
          property.IsRequired = false;
        }
        clientContract.Save();
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - AsyncUpdateClientContract - ���������� ���������� ����������� �������� ��={0} - ", ex, args.ClientContractId);
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(clientContract);
      }
    }
    //����� ��������� Avis Expert

  }
}