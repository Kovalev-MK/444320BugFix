using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.CommitteeApprovalAssignment;

namespace lenspec.Tenders.Server
{
  partial class CommitteeApprovalAssignmentFunctions
  {
    
    /// <summary>
    /// Разрешить добавление согласующих.
    /// </summary>
    /// <param name="block">Блок.</param>
    /// <returns>True - разрешить, False - нет или block = null.</returns>
    [Remote]
    public virtual bool GetAllowAddApproversPropertyValue()
    {
      var block = Tenders.ApprovalCounterpartyRegisterTask.Blocks.CommitteeApprovalBlocks.Get(_obj.Task.Scheme, _obj.BlockUid);
      if (block == null)
        return false;
      
      return block.AllowAddApprovers.GetValueOrDefault();
    }
    
    /// <summary>
    /// Проверить, можно ли переадресовать согласование сотруднику.
    /// </summary>
    /// <param name="employee">Сотрудник.</param>
    /// <returns>True, если можно переадресовать, False - если нельзя.</returns>
    [Remote(IsPure = true)]
    public virtual bool CanForwardTo(Sungero.Company.IEmployee employee)
    {
      if (Equals(_obj.Performer, employee))
        return false;
      
      var assignments = CommitteeApprovalAssignments
        .GetAll(a => Equals(a.Task, _obj.Task) &&
                Equals(a.TaskStartId, _obj.TaskStartId) &&
                Equals(a.IterationId, _obj.IterationId))
        .ToList();
      
      if (assignments.Any(a => Equals(a.Performer, employee) && a.Status == Status.InProcess))
        return false;
      
      var lastEmployeeAssignment = assignments.OrderByDescending(a => a.Created).FirstOrDefault(a => Equals(a.Performer, employee));
      if (lastEmployeeAssignment == null)
      {
        // Если сотруднику еще не приходили задания, смотрим, есть ли он в списке исполнителей блока.
        // Если сотрудник в списке есть, значит задание сформируется позже, переадресовывать ему нельзя.
        var approverBlockPerformers = this.GetApprovalBlocksPerformers();
        return !approverBlockPerformers.Contains(employee);
      }
      else
      {
        // Если сотруднику ранее уже приходили задания, и они были завершены, учитываем только переадресацию сотрудника в последующих заданиях.
        var assignmentsAfterLastEmployeeAssignments = assignments.Where(a => a.Created > lastEmployeeAssignment.Created);
        foreach (var assignment in assignmentsAfterLastEmployeeAssignments)
        {
          if (assignment.ForwardedTo.Contains(employee))
            return false;
        }
      }
      
      return true;
    }
    
    /// <summary>
    /// Получить будущих, текущих и прошлых исполнителей блока согласования.
    /// </summary>
    /// <returns>Развернутые до сотрудников исполнители блока согласования.</returns>
    [Remote(IsPure = true)]
    public virtual List<Sungero.Company.IEmployee> GetApprovalBlocksPerformers()
    {
      var approvalBlock = Tenders.ApprovalCounterpartyRegisterTask.Blocks.CommitteeApprovalBlocks.Get(_obj.Task.Scheme, _obj.BlockUid);
      return Sungero.Company.PublicFunctions.Module.GetEmployeesFromRecipients(approvalBlock.Performers.ToList());
    }
    
    /// <summary>
    /// Получить активные задания на согласование в рамках текущей задачи и итерации.
    /// </summary>
    /// <returns>Активные задания на согласование.</returns>
    [Remote(IsPure = true)]
    public virtual IQueryable<ICommitteeApprovalAssignment> GetActiveAssignments()
    {
      return CommitteeApprovalAssignments.GetAll(a => Equals(a.Task, _obj.Task) &&
                                                 Equals(a.TaskStartId, _obj.TaskStartId) &&
                                                 Equals(a.IterationId, _obj.IterationId) &&
                                                 a.Status == Status.InProcess);
    }

    /// <summary>
    /// Получить исполнителей активных или будущих заданий на согласование.
    /// </summary>
    /// <returns>Исполнители заданий.</returns>
    [Public, Remote(IsPure = true)]
    public virtual IQueryable<IRecipient> GetActiveAndFutureAssignmentsPerformers()
    {
      var approvalBlock = Tenders.ApprovalCounterpartyRegisterTask.Blocks.CommitteeApprovalBlocks.Get(_obj.Task.Scheme, _obj.BlockUid);
      var blockPerformers = Sungero.Company.PublicFunctions.Module.GetEmployeesFromRecipients(approvalBlock.Performers.ToList());
      return Sungero.Docflow.PublicFunctions.Module.Remote.GetActiveAndFutureAssignmentsPerformers(_obj, blockPerformers);
    }
  }
}