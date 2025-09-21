using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTask;

namespace lenspec.AutomatedSupportTickets
{
  partial class ExpirationOfGPHAnEmployeeTaskServerHandlers
  {
    // Добавлено avis.
    
    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Subject = lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTasks.Resources.SubjectAutotext;
    }

    /// <summary>
    /// До старта.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      _obj.Subject = lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTasks.Resources.ExpirationOfGPHAnEmployeeTaskSubjectFormat(_obj.Employee.Name);
      // Помещаем сотрудника во вложение.
      if (!_obj.Attachments.Contains(_obj.Employee))
        _obj.Attachments.Add(_obj.Employee);
    }
    
    // Конец добавлено avis.
  }
}