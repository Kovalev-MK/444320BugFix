using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ManagementCompanyJKHArhive.ArhiveJKHDocumentBase;

namespace avis.ManagementCompanyJKHArhive
{
  partial class ArhiveJKHDocumentBaseClientHandlers
  {
    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.LifeCycleState.IsVisible = true;
    }
  }
}