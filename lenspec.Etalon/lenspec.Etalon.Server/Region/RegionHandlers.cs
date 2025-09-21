using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Region;

namespace lenspec.Etalon
{
  partial class RegionServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (Functions.Region.GetDuplicates(_obj).Any())
      {
        e.AddError(Regions.Resources.DuplicatesFound, _obj.Info.Actions.ShowDuplicateslenspec);
        return;
      }
      
      if (_obj.Country != null && _obj.Country.Status == Sungero.Commons.Country.Status.Closed)
        e.AddError(lenspec.Etalon.Regions.Resources.SelectActiveCountry);
    }
  }
}