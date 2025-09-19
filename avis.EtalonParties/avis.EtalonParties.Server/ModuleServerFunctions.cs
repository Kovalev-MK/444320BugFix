using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonParties.Server
{
  public class ModuleFunctions
  {
    
    /// <summary>
    /// Закрытие всех записей в справочнике Банковские реквизиты
    /// </summary>
    public virtual void CloseBankDetailsAsync()
    {
      var handler = avis.EtalonParties.AsyncHandlers.CloseBankDetails.Create();
      handler.ExecuteAsync();
    }
    
    /// <summary>
    /// Получить список всех заданий текущей задачи, параллельных указанному заданию.
    /// </summary>
    /// <param name="assignment">Задание.</param>
    /// <returns>Список заданий.</returns>
    public virtual List<Sungero.Workflow.IAssignment> GetParallelAssignments(Sungero.Workflow.IAssignment assignment)
    {
      var assignments = new List<Sungero.Workflow.IAssignment>();
      if (assignment == null)
        return assignments;
      
      assignments = Sungero.Workflow.Assignments.GetAll(x => Equals(x.Task, assignment.Task)).ToList();
      return assignments
        .Where(a => Equals(a.BlockUid, assignment.BlockUid))
        .Where(a => a.TaskStartId == assignment.TaskStartId && a.IterationId == assignment.IterationId)
        .ToList();
    }
  }
}