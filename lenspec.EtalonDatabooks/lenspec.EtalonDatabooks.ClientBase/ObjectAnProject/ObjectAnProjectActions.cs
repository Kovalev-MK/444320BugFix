using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnProject;

namespace lenspec.EtalonDatabooks.Client
{
  partial class ObjectAnProjectCollectionActions
  {
    
    //Добавлено Avis Expert
    public virtual bool CanObjectAnSaleAddressFilling(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _objs.Any();
    }

    public virtual void ObjectAnSaleAddressFilling(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Dialogs.NotifyMessage(lenspec.EtalonDatabooks.ObjectAnProjects.Resources.ObjectAnSaleAddressFillingStarted);
      foreach(var objectAnProject in _objs)
      {
        var asyncHandler = EtalonDatabooks.AsyncHandlers.AsyncObjectAnSaleAddressFilling.Create();
        asyncHandler.ObjectAnProjectId = objectAnProject.Id;
        asyncHandler.ExecuteAsync();
      }
    }
    //конец Добавлено Avis Expert
  }

  partial class ObjectAnProjectActions
  {
    public virtual void ShowClients(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var clients = lenspec.SalesDepartmentArchive.PublicFunctions.SDAClientContract.Remote.GetClients(_obj, false);
      clients.Show(lenspec.EtalonDatabooks.Resources.Clients);
    }

    public virtual bool CanShowClients(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowOwners(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var clients = lenspec.SalesDepartmentArchive.PublicFunctions.SDAClientContract.Remote.GetClients(_obj, true);
      var owners = avis.ManagementCompanyJKHArhive.PublicFunctions.ManagementContractMKD.Remote.GetOwners(_obj);
      var selection = clients.Concat(owners).ToList().Distinct();
      if (selection.Count() > EtalonDatabooks.PublicConstants.Module.MaxClientsOwnersSelectionSize)
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.ClientsOwnersSelectionLimitErrorFormat(
          EtalonDatabooks.PublicConstants.Module.MaxClientsOwnersSelectionSize,
          lenspec.EtalonDatabooks.ObjectAnSales.Info.LocalizedPluralName));
        return;
      }
      
      selection.Show(lenspec.EtalonDatabooks.Resources.Owners);
    }

    public virtual bool CanShowOwners(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    //Добавлено Avis Expert
    /// <summary>
    /// Действие "Клиентские договоры".
    /// </summary>
    /// <param name="e"></param>
    public virtual void ClientContractShow(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Открыть список клиентских договоров.
      lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(c => c.ObjectAnProject == _obj).Show();
    }

    public virtual bool CanClientContractShow(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void RequestPIBAndBTI(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var asyncHandler = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncUpdateObjectAnProjectavis.Create();
      asyncHandler.IdInvest = _obj.IdInvest;
      asyncHandler.EmployeeId = Sungero.Company.Employees.Current.Id.ToString();
      asyncHandler.ExecuteAsync();
      
      Dialogs.NotifyMessage(lenspec.EtalonDatabooks.ObjectAnProjects.Resources.RequestPIBAndBTIResultMessage);
    }

    public virtual bool CanRequestPIBAndBTI(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return (_obj.IsLinkToInvest == true && (Users.Current.IncludedIn(Roles.Administrators) || Users.Current.IncludedIn(lenspec.OutgoingLetters.PublicConstants.Module.CreatingMassMailingApplicationRole)));;
    }

    /// <summary>
    /// Кнопка отобразить "Помещения объекта".
    /// </summary>
    /// <param name="e"></param>
    public virtual void RoomAnObject(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      EtalonDatabooks.ObjectAnSales.GetAll(o => o.ObjectAnProject == _obj).Show();
    }

    public virtual bool CanRoomAnObject(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (_obj.State.IsInserted || _obj.State.IsChanged)
      {
        return false;
      }
      
      return true;
    }
    //конец Добавлено Avis Expert
  }
}