using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.ApprovalSubstitutionAssignment;

namespace lenspec.AutomatedSupportTickets
{
  partial class ApprovalSubstitutionAssignmentClientHandlers
  {

    //Добавлено Avis Expert
    public virtual void EndDateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue == null)
        return;
      
      if (_obj.StartDate != null && e.NewValue.Value < _obj.StartDate.Value)
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.IncorrectEndDate);
      }
      
      if (_obj.StartDate != null && e.NewValue.Value > _obj.StartDate.Value.AddDays(90))
      {
        e.AddError(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.IncorrectSubstitutionPeriod);
      }
    }
    //конец Добавлено Avis Expert

  }
}