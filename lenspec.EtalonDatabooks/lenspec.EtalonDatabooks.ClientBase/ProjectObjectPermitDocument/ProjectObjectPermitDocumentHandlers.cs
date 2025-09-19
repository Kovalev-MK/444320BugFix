using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectObjectPermitDocument;

namespace lenspec.EtalonDatabooks
{
  partial class ProjectObjectPermitDocumentClientHandlers
  {

    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Доступность поля Объекты продаж, в зависимости от ИСП.
      PublicFunctions.ProjectObjectPermitDocument.EnableObjectAnSale(_obj);
      _obj.State.Properties.LifeCycleState.IsVisible = true;
      _obj.State.Properties.LifeCycleState.IsEnabled = true;
    }
  }
}