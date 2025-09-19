using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.PersonnelDepartmentResponsibleAssignment;

namespace lenspec.AutomatedSupportTickets
{
  partial class PersonnelDepartmentResponsibleAssignmentSupervisorPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> SupervisorFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Фильтруем список руководителей, по наличию учетки.
      query = query.Where(q => q.Login != null);
      
      return query;
    }
  }

}