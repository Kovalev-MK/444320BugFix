using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Region;

namespace lenspec.Etalon.Client
{
  partial class RegionActions
  {
    public virtual void ShowCitieslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var cities = Functions.Region.Remote.GetRelatedCities(_obj);
      if (cities.Any())
        cities.Show();
      else
        Dialogs.ShowMessage("Населенные пункты данного региона не найдены.");
    }

    public virtual bool CanShowCitieslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowDuplicateslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.Region.Remote.GetDuplicates(_obj);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(Regions.Resources.DuplicatesNotFound);
    }

    public virtual bool CanShowDuplicateslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}