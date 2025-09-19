using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.LocalRegulations.Server
{
  public class ModuleAsyncHandlers
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Отправить уведомление об ошибке.
    /// </summary>
    /// <param name="args">Аргументы обработчика.</param>
    public virtual void AsyncFunctionStageErrorNotification(lenspec.LocalRegulations.Server.AsyncHandlerInvokeArgs.AsyncFunctionStageErrorNotificationInvokeArgs args)
    {
      try
      {
        var performer = Sungero.CoreEntities.Recipients.Get(args.PerformerId);
        if (performer == null)
          throw new Exception("Не найден исполнитель");
        
        var task = Sungero.Workflow.SimpleTasks.CreateWithNotices(args.Subject, performer);
        task.NeedsReview = false;
        task.ActiveText = args.ActiveText;
        if (args.AttachmentId != 0)
        {
          var document = Sungero.Docflow.OfficialDocuments.Get(args.AttachmentId);
          task.Attachments.Add(document);
        }
        task.Start();
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - AsyncFunctionStageErrorNotification - ", ex);
        args.Retry = false;
      }
    }
    //конец Добавлено Avis Expert

  }
}