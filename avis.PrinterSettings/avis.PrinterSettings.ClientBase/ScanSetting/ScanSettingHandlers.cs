using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PrinterSettings.ScanSetting;

namespace avis.PrinterSettings
{
  partial class ScanSettingClientHandlers
  {

    //Добавлено Avis Expert
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      _obj.State.Properties.User.IsEnabled = Users.Current.IncludedIn(Roles.Administrators);
    }
    //конец Добавлено Avis Expert

  }
}