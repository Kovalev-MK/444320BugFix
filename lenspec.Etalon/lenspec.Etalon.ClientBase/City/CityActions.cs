using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.City;

namespace lenspec.Etalon.Client
{
  partial class CityActions
  {
    public virtual void ShowDuplicateslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.City.Remote.GetDuplicates(_obj);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(Sungero.Commons.Resources.DuplicateNotFound);
    }

    public virtual bool CanShowDuplicateslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}