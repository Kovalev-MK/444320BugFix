using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTask;

namespace lenspec.AutomatedSupportTickets.Server
{
  partial class ExpirationOfGPHAnEmployeeTaskFunctions
  {
    /// <summary>
    /// Вывести информацию о сотруднике.
    /// </summary>      
    [Remote]
    public StateView GetExpirationOfGPHAnEmployeeTaskState()
    {
      // Проверяем наличие даты начала/окончания контракта.
      var dateContractStart = "Отсутствует дата начала ГПХ.";
      var dateContractEnd = "Отсутствует дата окончания ГПХ."; 
      if (_obj.Employee.ContractValidFromlenspec.HasValue)
        dateContractStart = _obj.Employee.ContractValidFromlenspec.Value.ToString("dd.MM.yyyy");
      if (_obj.Employee.ContractValidTilllenspec.HasValue)
        dateContractEnd = _obj.Employee.ContractValidTilllenspec.Value.ToString("dd.MM.yyyy");
      
      var jobTitle = string.Empty;
      if (_obj.Employee.JobTitle != null)
        jobTitle = _obj.Employee.JobTitle.Name;
      
      var department = string.Empty;
      if (_obj.Employee.Department != null)
        department = _obj.Employee.Department.Name;
      
      // Формируем текст для вывода в форму.
      var stateView = StateView.Create();
      var block = stateView.AddBlock();
      block.DockType = DockType.Bottom;
      
      block.AddLabel("ИНФОРМАЦИЯ О СОТРУДНИКЕ");
      block.AddLineBreak();
      
      block.AddLabel($"ФИО: {_obj.Employee.Name}");
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
    
    /// <summary>
    /// Получить ответственного отдела кадров.
    /// </summary>
    /// <param name="employee">Сотрудник по которому запущена задача.</param>
    /// <returns>Ответственный отдела кадров.</returns>
    [Public]
    public Sungero.Company.IEmployee GetResponsibleHR()
    {
      // Проверяем на наличие нашей организации у сотрудника.
      if (_obj.Employee.Department.BusinessUnit == null)
        return null;

      // Получаем роль 
      var roleKindEmployeeHR = Etalon.BusinessUnits.GetAll(x => x.Equals(_obj.Employee.Department.BusinessUnit)).Single().RoleKindEmployeelenspec;
      if (roleKindEmployeeHR.Any())
      {
        var performerHR = roleKindEmployeeHR.Where(x => x.RoleKind.Name.Equals(lenspec.EtalonDatabooks.ComputedRoles.Resources.RoleDescriptionResponsibleHR)).FirstOrDefault();
        if (performerHR != null)
        {
          return performerHR.Employee;
        }
      }
      
      return null;
    }
  }
}