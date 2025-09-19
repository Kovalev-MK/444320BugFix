using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PrinterSettings.ScanSetting;

namespace avis.PrinterSettings
{
  partial class ScanSettingSharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void ScannerChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      this.FillName();
    }

    public virtual void UserChanged(avis.PrinterSettings.Shared.ScanSettingUserChangedEventArgs e)
    {
      this.FillName();
    }
    
    protected void FillName()
    {
      Functions.ScanSetting.FillName(_obj);
    }
    //конец Добавлено Avis Expert

  }
}