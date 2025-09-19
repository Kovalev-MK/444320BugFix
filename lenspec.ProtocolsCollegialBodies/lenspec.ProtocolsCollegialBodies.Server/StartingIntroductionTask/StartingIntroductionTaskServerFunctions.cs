using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.StartingIntroductionTask;

namespace lenspec.ProtocolsCollegialBodies.Server
{
  partial class StartingIntroductionTaskFunctions
  {
    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на ознакомление.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = base.Execute(approvalTask);
      Logger.DebugFormat("Info - StartingIntroductionTask function started.");
      
      try
      {
        var document = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
        if (document == null)
        {
          return this.GetErrorResult("Не найден документ.");
        }
        
        var employees = GetDocumentExecutor(document, approvalTask);
        if (!employees.Any())
        {
          return this.GetErrorResult("Не найдены исполнители.");
        }
        
        var task = Sungero.RecordManagement.PublicFunctions.Module.Remote.CreateAcquaintanceTask(document);
        
        #region Список ознакомления
        
        // Сотрудники, которые не смогут получить задание на ознакомление.
        var notAutomatedEmployees = Sungero.Company.PublicFunctions.Module.Remote.GetNotAutomatedEmployees(employees);
        
        employees = employees.Except(notAutomatedEmployees).ToList();
        foreach (var employee in employees)
        {
          task.Performers.AddNew().Performer = employee;
        }
        
        #endregion
        
        // Заглушка для функции вычисления ответственного по виду роли
        var role = EtalonDatabooks.ComputedRoles.GetAll(x => x.Type == EtalonDatabooks.ComputedRole.Type.ResponsibleClerk).First();
        
        // Инициатор задачи - Ответственный делопроизводитель.
        task.Author = EtalonDatabooks.PublicFunctions.ComputedRole.Remote.GetResponsibleClerkPerformer(role, approvalTask);
        
        // Срок Задачи на ознакомление.
        task.Deadline = Calendar.Now.AddWorkingDays(_obj.TaskDeadline ?? 3);
        
        // Ознакомление в электронном виде.
        task.IsElectronicAcquaintance = _obj.IsElectronic ?? false;
        
        // Вложения группы Дополнительно.
        var others = approvalTask.OtherGroup.All;
        foreach(var attachment in others)
        {
          task.OtherGroup.All.Add(attachment);
        }
        
        task.Start();
        if (notAutomatedEmployees != null && notAutomatedEmployees.Any())
        {
          var notAutomatedEmployeesNames = string.Join(", ", notAutomatedEmployees.Select(x => x.Name).ToList());
          return Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult.Create(true, false, ProtocolsCollegialBodies.StartingIntroductionTasks.Resources.NonautomatedEmployeesDidNotReceiveAcquaintanceAssignmentFormat(notAutomatedEmployeesNames));
        }
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
        Logger.DebugFormat("Error - StartingAcquaintanceTask function: {0}. {1}", ex.Message, innerMessage);
        return this.GetErrorResult(ex.Message);
      }
      return result;
    }
    
    /// <summary>
    /// Получить исполнителей задачи.
    /// </summary>
    /// <returns>Список исполнитeлей.</returns>
    private List<Sungero.Company.IEmployee> GetDocumentExecutor(Sungero.Docflow.IOfficialDocument document, Sungero.Docflow.IApprovalTask approvalTask)
    {
      var employees = new List<Sungero.Company.IEmployee>();
      if (ProtocolsCollegialBodies.ProtocolCollegialBodies.Is(document))
      {
        var protocol = ProtocolsCollegialBodies.ProtocolCollegialBodies.As(document);
        
        //Добавим инициатора основной задачи
        var initiator = Sungero.Company.Employees.As(approvalTask.Author);
        if (initiator != null)
        {
          employees.Add(initiator);
          //Добавим руководителя инициатора основной задачи
          var manager = Sungero.Docflow.PublicFunctions.Module.Remote.GetManager(initiator);
          if (manager != null)
            employees.Add(manager);
        }
        
        //Добавим Членов Коллегиального органа
        var members = protocol.Members.Select(x => x.Member);
        if (members != null && members.Any())
          employees.AddRange(members);
        
        //Добавим Председателя коллегильного органа
        var chairman = protocol.Chairman;
        employees.Add(chairman);
        
        //Добавим доп. согласующих
        var dopmembers = approvalTask.AddApprovers.Select(x => Sungero.Company.Employees.As(x.Approver)).Where(x => x != null);
        if (dopmembers != null && dopmembers.Any())
          employees.AddRange(dopmembers);
      }
      
      return employees;
    }
  }
}