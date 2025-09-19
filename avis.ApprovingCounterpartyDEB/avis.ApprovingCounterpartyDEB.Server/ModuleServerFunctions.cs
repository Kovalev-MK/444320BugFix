using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.ApprovingCounterpartyDEB.Server
{
  public class ModuleFunctions
  {
    
    /// <summary>
    /// Получить ссылку на карточку задачи.
    /// </summary>
    /// <returns>Гиперссылка на задачу.</returns>
    [Public, Remote]
    public static string GetTaskHyperlink(long taskId)
    {
      var result = Hyperlinks.Get(avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Info, taskId);
      return result;
    }

    /// <summary>
    /// Получить параллельные задания в отношении указанного
    /// </summary>
    [Public]
    public List<Sungero.Workflow.IAssignment> GetParallelAssignments(Sungero.Workflow.IAssignment assignment)
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

    /// <summary>
    /// Создать задачу на согласование контрагента
    /// <param name="document">Документ (Согласование контрагента, банка или персоны)</param>
    /// </summary>
    [Remote]
    public IApprovalCounterpartyPersonDEB CreateApprovalCounterpartyTask(avis.ApprovingCounterpartyDEB.IApprovalCounterpartyBase document)
    {
      var task = ApprovalCounterpartyPersonDEBs.Create();
      task.ApprovalDocument.ApprovalCounterpartyBases.Add(document);
      return task;
    }
  }
}