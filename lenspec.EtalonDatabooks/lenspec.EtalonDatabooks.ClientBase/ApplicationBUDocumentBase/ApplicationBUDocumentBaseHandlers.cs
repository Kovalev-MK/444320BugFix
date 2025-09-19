using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApplicationBUDocumentBase;

namespace lenspec.EtalonDatabooks
{
  partial class ApplicationBUDocumentBaseClientHandlers
  {

    //Добавлено Avis Expert
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      if (_obj.State.Properties.LifeCycleState.IsVisible != true && _obj.State.Properties.InternalApprovalState.IsVisible != true)
        _obj.State.Properties.LifeCycleState.IsVisible = _obj.State.Properties.InternalApprovalState.IsVisible = true;
      
      _obj.State.Properties.LifeCycleState.IsEnabled = false;
    }
    //конец Добавлено Avis Expert

  }
}