using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Country;

namespace lenspec.Etalon.Client
{
  partial class CountryActions
  {
    public virtual void ShowCitieslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var cities = Functions.Country.Remote.GetRelatedCities(_obj);
      if (cities.Any())
        cities.Show();
      else
         Dialogs.ShowMessage("Населенные пункты данной страны не найдены.");
    }

    public virtual bool CanShowCitieslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowAreaslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var areas = Functions.Country.Remote.GetRelatedAreas(_obj);
      if (areas.Any())
        areas.Show();
      else
         Dialogs.ShowMessage("Районы данной страны не найдены.");
    }

    public virtual bool CanShowAreaslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowRegionslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var regions = Functions.Country.Remote.GetRelatedRegions(_obj);
      if (regions.Any())
        regions.Show();
      else
         Dialogs.ShowMessage("Регионы данной страны не найдены.");
    }

    public virtual bool CanShowRegionslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowDstrictslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var districts = Functions.Country.Remote.GetRelatedDistricts(_obj);
      if (districts.Any())
        districts.Show();
      else
        Dialogs.ShowMessage("Округа данной страны не найдены.");
    }

    public virtual bool CanShowDstrictslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.Country.Remote.GetDuplicates(_obj);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(Sungero.Commons.Resources.DuplicateNotFound);
    }

    public override bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }



}