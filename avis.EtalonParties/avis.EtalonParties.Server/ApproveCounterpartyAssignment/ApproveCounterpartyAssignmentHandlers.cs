using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.ApproveCounterpartyAssignment;

namespace avis.EtalonParties
{
  partial class ApproveCounterpartyAssignmentServerHandlers
  {
    // Добавлено avis.
    
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.CreateCompanyTask == null || _obj.CreateCompanyTask.TypeRequest != avis.EtalonParties.CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
        _obj.ActiveText = $"{_obj.Subject}\n{_obj.ActiveText}";
    }
    
    // Конец добавлено avis.
  }

}