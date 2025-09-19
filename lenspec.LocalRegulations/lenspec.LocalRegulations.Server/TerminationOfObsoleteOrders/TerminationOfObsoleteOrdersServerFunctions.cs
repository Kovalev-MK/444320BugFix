using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.LocalRegulations.TerminationOfObsoleteOrders;

namespace lenspec.LocalRegulations.Server
{
  partial class TerminationOfObsoleteOrdersFunctions
  {

    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = base.Execute(approvalTask);
      Logger.DebugFormat("Etalon - TerminationOfObsoleteOrders - старт сценария.");
      
      try
      {
        var document = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
        if (document == null)
        {
          return this.GetErrorResult("Не вложен приказ!");
        }
        
        if (lenspec.Etalon.Orders.Is(document))
        {
          var canceledOrders = lenspec.Etalon.Orders.As(document).CanceledOrderslenspec.Where(x => x.CanceledOrder.LifeCycleState != lenspec.Etalon.Order.LifeCycleState.Obsolete).ToList();
          if (canceledOrders != null && canceledOrders.Any())
          {
            var errorExist = false;
            foreach(var doc in canceledOrders)
            {
              // Установить отменённым приказам стадию ЖЦ - Устаревший
              try
              {
                var order = lenspec.Etalon.Orders.As(doc.CanceledOrder);
                order.LifeCycleState = lenspec.Etalon.Order.LifeCycleState.Obsolete;
                order.Save();
                
                // Установить связь между приказом и отменённым приказом
                document.Relations.AddFrom(Sungero.Docflow.Constants.Module.CancelRelationName, order);
              }
              catch(Exception)
              {
                errorExist = true;
              }
              if (errorExist)
              {
                return this.GetRetryResult(string.Format("Не удалось поменять состояние приказов на прекращение в приказе {0}.", document.Id));
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Etalon - TerminationOfObsoleteOrders: ", ex);
        return this.GetErrorResult(ex.Message);
      }
      Logger.DebugFormat("Etalon - TerminationOfObsoleteOrders - окончание сценария.");
      return result;
    }
  }
}