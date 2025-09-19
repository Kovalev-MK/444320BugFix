using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientDocumentBase;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAClientDocumentBaseClientHandlers
  {

    //Добавлено Avis Expert
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Properties.LifeCycleState.IsVisible = true;
      _obj.State.Properties.InternalApprovalState.IsVisible = false;
    }
    //конец Добавлено Avis Expert

  }
}