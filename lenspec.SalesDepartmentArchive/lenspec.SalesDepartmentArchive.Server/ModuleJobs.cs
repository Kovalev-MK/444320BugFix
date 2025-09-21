using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using Sungero.Metadata;

namespace lenspec.SalesDepartmentArchive.Server
{
  public class ModuleJobs
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Фоновый процесс для удаления выполненных Заявок на сдачу в архив
    /// </summary>
    public virtual void DeleteCompletedRequestsForSubmission()
    {
      var requestSubmission = SalesDepartmentArchive.SDARequestSubmissionToArchives.GetAll();
      foreach(var request in requestSubmission)
      {
        var tasks = GetTasksByDocument(request);
        
        if (tasks.Any(x => x.Status == Sungero.Workflow.Task.Status.Completed) &&
            !tasks.Any(x => x.Status == Sungero.Workflow.Task.Status.InProcess || x.Status == Sungero.Workflow.Task.Status.UnderReview))
        {
          var lastCompletedTask = tasks.Where(x => x.Status == Sungero.Workflow.Task.Status.Completed).OrderByDescending(x => x.Modified).FirstOrDefault();
          if (lastCompletedTask != null &&
              Calendar.Today.AddDays(-1 * Constants.Module.DeleteCompletedRequestsForSubmissionAfterDays) > lastCompletedTask.Modified)
          {
            foreach(var task in tasks)
            {
              try
              {
                Sungero.Workflow.Tasks.Delete(task);
              }
              catch(Exception ex)
              {
                Logger.ErrorFormat("Avis - DeleteCompletedRequestsForSubmission - не удалось удалить задачу ID={0} ", ex, task.Id);
              }
            }
            try
            {
              SalesDepartmentArchive.SDARequestSubmissionToArchives.Delete(request);
              Logger.DebugFormat("DeleteCompletedRequestsForSubmission - заявка ID={0} {1} удалена.", request.Id, request.Name);
            }
            catch(Exception ex)
            {
              Logger.ErrorFormat("Avis - DeleteCompletedRequestsForSubmission - не удалось удалить заявку ID={0} ", ex, request.Id);
            }
            //SendNotificationAboutDeletingRequestSubmission(request);
          }
        }
      }
    }
    
    private List<Sungero.Workflow.ITask> GetTasksByDocument(Sungero.Content.IElectronicDocument document)
    {
      var tasks = Sungero.Workflow.Tasks.GetAll()
        .Where(task => task.AttachmentDetails.Any(a => a.AttachmentTypeGuid == document.GetEntityMetadata().GetOriginal().NameGuid && a.AttachmentId == document.Id))
        .ToList();
      
      return tasks;
    }
    
    /// <summary>
    /// Метод для тестирования удаления документа.
    /// </summary>
    /// <param name="document">Документ, подлежащий удалению.</param>
    private void SendNotificationAboutDeletingRequestSubmission(Sungero.Docflow.IOfficialDocument document)
    {
      var notification = Sungero.Workflow.SimpleTasks.CreateWithNotices("Удаление Заявок на сдачу в архив", document.Author);
      notification.Attachments.Add(document);
      notification.NeedsReview = false;
      notification.ActiveText = string.Format("{0} будет удалена фоновым процессом, т.к. последняя задача по документу завершена более {1} дн. назад.",
                                              document,
                                              Constants.Module.DeleteCompletedRequestsForSubmissionAfterDays);
      notification.Start();
    }
    //конец Добавлено Avis Expert

  }
}