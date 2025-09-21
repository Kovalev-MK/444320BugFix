using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.CoordinationHeadDepartmentOfInitiator;

namespace lenspec.AutomatedSupportTickets
{

  partial class CoordinationHeadDepartmentOfInitiatorSelectedPerformerPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public virtual IQueryable<T> SelectedPerformerFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var employee = lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTasks.As(_obj.Task).Employee;
      var bu = employee.Department.BusinessUnit;
      if (employee != null && bu != null)
      {
        query = query.Where(x => bu.Equals(x.Department.BusinessUnit));
      }
      
      var employees = query.Select(x => Sungero.Company.Employees.As(x)).ToList();
      var notAuthomated = lenspec.EtalonDatabooks.PublicFunctions.Module.GetNotAutomatedEmployeesAvis(employees).Select(x => x.Id).ToList();
      if (notAuthomated != null && notAuthomated.Any())
        query = query.Where(x => !notAuthomated.Contains(x.Id));
      
      return query;
    }
    //конец Добавлено Avis Expert
  }

}