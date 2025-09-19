using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ApplicationsForDIRECTUM.Server
{
  public class ModuleFunctions
  {
    
    /// <summary>
    /// Получить ИД My ApprovalTasks
    /// </summary>
    /// <returns>Список ИД Заявок.</returns>
    [Public]
    public List<long> GetUrgentRequestToApprovalTasks()
    {
      var requestIds = Sungero.Docflow.ApprovalTasks.GetAll(x => x.Author == Users.Current)
        .Select(x => x.Id)
        .ToList();
      
      return requestIds;
    }
      
    /// <summary>
    /// Получить ИД задач, в которые вложены «Заявка на открытие периодов РСБУ и УУ».
    /// </summary>
    /// <returns>Список ИД задач.</returns>
    [Public] // SubmissionToArchive
    public List<long> GetTaskWithRequestApplicationOpeningRSBUandUUKind(List<long> requestIds)
    {
      // Индификатор ApplicationOpeningRSBUandUU.
      var guidApplicationOpeningRSBUanUUDoc = ApplicationsForDIRECTUM.Server.ApplicationOpeningRSBUandUU.ClassTypeGuid;
      
      var tasks = Sungero.Workflow.Tasks.GetAll()
        .Where(task => task.AttachmentDetails.Any(a => a.AttachmentTypeGuid == guidApplicationOpeningRSBUanUUDoc))
        .Select(task => task.Id)
        .ToList();
      return tasks;
    }

     /// <summary>
    /// Создать клиентский договор.
    /// </summary>
    /// <returns>Клиентский договор.</returns>
    [Remote, Public]
    public static ApplicationsForDIRECTUM.IApplicationOpeningRSBUandUU CreateDoc()
    {
      return ApplicationsForDIRECTUM.ApplicationOpeningRSBUandUUs.Create();
    }
  }
}