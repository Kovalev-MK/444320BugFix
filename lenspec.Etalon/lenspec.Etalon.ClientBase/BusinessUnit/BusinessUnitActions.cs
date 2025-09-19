using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.BusinessUnit;

namespace lenspec.Etalon.Client
{
  internal static class BusinessUnitStaticActions
  {

    //Добавлено Avis Expert
    public static bool CanDownloadFrom1Clenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(Roles.Administrators);
    }

    public static void DownloadFrom1Clenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.BusinessUnit.DownloadBusinessUnitFrom1C();
    }

    public static bool CanUpdateAllBusinessUnitsavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(Roles.Administrators);
    }

    public static void UpdateAllBusinessUnitsavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var result = Functions.BusinessUnit.Remote.UpdateAllBusinessUnits();
      if (string.IsNullOrEmpty(result))
      {
        Dialogs.ShowMessage(lenspec.Etalon.BusinessUnits.Resources.BusinessUnitsHaveBeenChanged);
      }
      else
      {
        Dialogs.ShowMessage(result, MessageType.Error);
      }
    }
    //конец Добавлено Avis Expert
  }

  partial class BusinessUnitActions
  {
    /// <summary>
    /// Действие "Объекты проектов".
    /// </summary>
    /// <param name="e"></param>
    public virtual void ObjectAnProjectsShowavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Показать список объектов проекта со спец застройщиком.
      lenspec.EtalonDatabooks.ObjectAnProjects.GetAll(o => o.SpecDeveloper == _obj).Show();
    }

    public virtual bool CanObjectAnProjectsShowavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
    
    //Добавлено Avis Expert
    public virtual void DeleteHeaderLogoForDocslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.HeaderLogoForDocslenspec = null;
    }

    public virtual bool CanDeleteHeaderLogoForDocslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.HeaderLogoForDocslenspec != null;
    }

    public virtual void AddHeaderLogoForDocslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Выбор центральной шапки");
      var fileSelector = dialog.AddFileSelect("Выберите файл:", true);
      var import = dialog.Buttons.AddCustom("ОК");
      dialog.Buttons.AddCancel();
      dialog.SetOnButtonClick(b =>
                              {
                                _obj.HeaderLogoForDocslenspec = fileSelector.Value.Content;
                                _obj.State.Pages.Labelslenspec.Activate();
                              });
      dialog.Show();
    }

    public virtual bool CanAddHeaderLogoForDocslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void DeleteLabelWithoutAddrlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.LabelWithoutAddresslenspec = null;
    }

    public virtual bool CanDeleteLabelWithoutAddrlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.LabelWithoutAddresslenspec != null;
    }

    public virtual void AddLabelWithoutAddrlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Выбор угловой шапки");
      var fileSelector = dialog.AddFileSelect("Выберите файл:", true);
      var import = dialog.Buttons.AddCustom("ОК");
      dialog.Buttons.AddCancel();
      dialog.SetOnButtonClick(b =>
                              {
                                _obj.LabelWithoutAddresslenspec = fileSelector.Value.Content;
                                _obj.State.Pages.Labelslenspec.Activate();
                              });
      dialog.Show();
    }

    public virtual bool CanAddLabelWithoutAddrlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
    //конец Добавлено Avis Expert

  }

}