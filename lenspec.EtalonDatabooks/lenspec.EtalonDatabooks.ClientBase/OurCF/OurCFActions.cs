using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.OurCF;

namespace lenspec.EtalonDatabooks.Client
{
  // Добавлено avis.
  
  partial class OurCFActions
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
          lenspec.EtalonDatabooks.ObjectAnProjects.Info.LocalizedPluralName));
        return;
      }
      
      selection.Show(lenspec.EtalonDatabooks.Resources.Owners);
    }

    public virtual bool CanShowOwners(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void SaveAndClose(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (string.IsNullOrEmpty(_obj.Name) && string.IsNullOrEmpty(_obj.CommercialName))
      {
        e.AddError(lenspec.EtalonDatabooks.OurCFs.Resources.NeedFillName);
        return;
      }
      base.SaveAndClose(e);
    }

    public override bool CanSaveAndClose(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSaveAndClose(e);
    }

    public override void Save(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (string.IsNullOrEmpty(_obj.Name) && string.IsNullOrEmpty(_obj.CommercialName))
      {
        e.AddError(lenspec.EtalonDatabooks.OurCFs.Resources.NeedFillName);
        return;
      }
      base.Save(e);
    }

    public override bool CanSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSave(e);
    }

    /// <summary>
    /// Кнопка "Объекты проекта".
    /// </summary>
    /// <param name="e"></param>
    public virtual void OpenObjectAnProject(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      EtalonDatabooks.ObjectAnProjects.GetAll(o => o.OurCF == _obj).Show();
    }

    public virtual bool CanOpenObjectAnProject(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Если не сохранена.
      if (_obj.State.IsInserted)
        return false;
      
      // Если сотрудник входит в роль "Полные права на справочник ИСП" или "Администратор".
      return Etalon.Employees.Current.IncludedIn(EtalonDatabooks.Constants.Module.FullPermitISPAndRNSAndPNV) || Etalon.Employees.Current.IncludedIn(Roles.Administrators);
    }
  }

  // Конец добавлено avis.
}