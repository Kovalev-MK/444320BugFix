using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ApplicationsForDIRECTUM.Server
{
  partial class MyRequestsFolderHandlers
  {
    // Добавлено avis.
    
    /// <summary>
    /// Получение данных.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public virtual IQueryable<Sungero.Workflow.ITask> MyRequestsDataQuery(IQueryable<Sungero.Workflow.ITask> query)
    {
      var requestIds = PublicFunctions.Module.GetUrgentRequestToApprovalTasks();
      var taskIds = PublicFunctions.Module.GetTaskWithRequestApplicationOpeningRSBUandUUKind(requestIds);
      
      // Находим все задачи где автором является наш пользователь.
      query = query.Where(q => q.Author == Sungero.Company.Employees.Current);
     
      // Если задача является "Согласование по регламенту", то проверяем вложение на наличие документа "РСБУ и УУ".
      query = query.Where(q => lenspec.AutomatedSupportTickets.EditComponentRXRequestTasks.Is(q) ||
                          lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Is(q) ||
                          taskIds.Contains(q.Id)
                         );
      return query;
    }
    
    // Конец добавлено avis.
  }

  partial class ApplicationsForDIRECTUMHandlers
  {
  }
}