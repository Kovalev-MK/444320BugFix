using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.ApproveRevision;

namespace avis.EtalonParties
{
  partial class ApproveRevisionServerHandlers
  {
    // Добавлено avis.
    
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      _obj.ActiveText = $"{_obj.Subject}\n {_obj.ActiveText}";
    }
    
    // Конец добавлено avis.
  }

}