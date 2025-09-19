using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PrinterSettings.ScanSetting;

namespace avis.PrinterSettings
{
  partial class ScanSettingUiFilteringServerHandler<T>
  {
    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.UiFilteringEventArgs e)
    {
      query = base.Filtering(query, e);
      
      var visibleSettingIds = Functions.Module.GetVisibleScanSettingIds();
      query = query.Where(c => visibleSettingIds.Contains(c.Id));
      
      return query;
    }
  }

  partial class ScanSettingServerHandlers
  {
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var dublicates = Functions.ScanSetting.GetDublicates(_obj);
      if (dublicates.Any())
      {
        e.AddError(avis.PrinterSettings.ScanSettings.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
        return;
      }
    }
  }
}