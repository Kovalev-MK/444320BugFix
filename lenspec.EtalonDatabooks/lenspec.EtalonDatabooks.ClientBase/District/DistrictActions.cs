using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.District;

namespace lenspec.EtalonDatabooks.Client
{
  partial class DistrictActions
  {
    public virtual void ShowCities(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var cities = Functions.District.Remote.GetRelatedCities(_obj);
      if (cities.Any())
        cities.Show();
      else
        Dialogs.ShowMessage("Населенные пункты данного округа не найдены.");
    }

    public virtual bool CanShowCities(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowAreas(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var areas = Functions.District.Remote.GetRelatedAreas(_obj);
      if (areas.Any())
        areas.Show();
      else
        Dialogs.ShowMessage("Районы данного округа не найдены.");
    }

    public virtual bool CanShowAreas(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowRegions(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var regions = Functions.District.Remote.GetRelatedRegions(_obj);
      if (regions.Any())
        regions.Show();
      else
        Dialogs.ShowMessage("Районы данного округа не найдены.");
    }

    public virtual bool CanShowRegions(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void CopyEntity(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CopyEntity(e);
    }

    public override bool CanCopyEntity(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
    }

    /// <summary>
    /// Формирование списка дублей округа.
    /// </summary>
    public virtual void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.District.Remote.GetDuplicates(_obj);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(Districts.Resources.DuplicateNotFound);
    }

    public virtual bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}