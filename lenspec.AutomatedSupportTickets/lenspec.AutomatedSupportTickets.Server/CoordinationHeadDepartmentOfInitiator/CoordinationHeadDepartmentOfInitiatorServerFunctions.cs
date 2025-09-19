using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.CoordinationHeadDepartmentOfInitiator;

namespace lenspec.AutomatedSupportTickets.Server
{
  partial class CoordinationHeadDepartmentOfInitiatorFunctions
  {
    /// <summary>
    /// Отображение инструкции.
    /// </summary>       
    [Remote]
    public StateView GetCoordinationHeadDepartmentOfInitiatorState2()
    {
      // Проверяем наличие даты начала/окончания контракта.
      var instruction = string.Empty;
      
      // Формируем текст для вывода.
      var stateView = StateView.Create();
      var block = stateView.AddBlock();
      block.DockType = DockType.Bottom;
      
      block.AddLabel("ИНСТРУКЦИЯ");
      block.AddLineBreak();
      
      if (_obj.BlockUid == "9")
        instruction = "Заканчивается срок действия договора сотрудника ГПХ. Выберите будет ли продлен договор с сотрудником ГПХ или укажите ответственного за договор сотрудника ГПХ.";
      
      if (_obj.BlockUid == "14")
        instruction = "Проверьте все невыполненные задания и задачи сотрудника ГПХ, активные замещения. При необходимости закройте.";
      
      block.AddLabel(instruction);
      return stateView;
    }
    
    /// <summary>
    /// Вывести информацию о сотруднике.
    /// </summary>       
    [Remote]
    public StateView GetCoordinationHeadDepartmentOfInitiatorState()
    {
      var task = lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTasks.As(_obj.Task);
      
      // Проверяем наличие даты начала/окончания контракта.
      var dateContractStart = "Отсутствует дата начала ГПХ.";
      var dateContractEnd = "Отсутствует дата окончания ГПХ."; 
      if (task.Employee.ContractValidFromlenspec.HasValue)
        dateContractStart = task.Employee.ContractValidFromlenspec.Value.ToString("dd.MM.yyyy");
      if (task.Employee.ContractValidTilllenspec.HasValue)
        dateContractEnd = task.Employee.ContractValidTilllenspec.Value.ToString("dd.MM.yyyy");
      
      var jobTitle = string.Empty;
      if (task.Employee.JobTitle != null)
        jobTitle = task.Employee.JobTitle.Name;
      
      var department = string.Empty;
      if (task.Employee.Department != null)
        department = task.Employee.Department.Name;
      
      // Формируем текст для вывода.
      var stateView = StateView.Create();
      var block = stateView.AddBlock();
      block.DockType = DockType.Bottom;
      
      block.AddLabel("ИНФОРМАЦИЯ О СОТРУДНИКЕ");
      block.AddLineBreak();
      block.AddLabel($"ФИО: {task.Employee.Name}");
      block.AddLineBreak();
      block.AddLabel($"Должность: {jobTitle}");
      block.AddLineBreak();
      block.AddLabel($"Подразделение: {department}");
      block.AddLineBreak();
      block.AddLabel($"Дата начала ГПХ: {dateContractStart}");
      block.AddLineBreak();
      block.AddLabel($"Дата окончания ГПХ: {dateContractEnd}");
      
      return stateView;
    }
  }
}