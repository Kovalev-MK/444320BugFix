using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTask;

namespace lenspec.AutomatedSupportTickets.Server
{
  partial class ExpirationOfGPHAnEmployeeTaskRouteHandlers
  {

    /// <summary>
    /// Выполнение задания руководителем.
    /// </summary>
    /// <param name="e"></param>
    public virtual void CompleteAssignment9(lenspec.AutomatedSupportTickets.ICoordinationHeadDepartmentOfInitiator assignment, lenspec.AutomatedSupportTickets.Server.CoordinationHeadDepartmentOfInitiatorArguments e)
    {
      if (assignment.Result == AutomatedSupportTickets.CoordinationHeadDepartmentOfInitiator.Result.Forward)
      {
        assignment.Forward(assignment.SelectedPerformer, ForwardingLocation.Next);
      }
      if (assignment.Result == AutomatedSupportTickets.CoordinationHeadDepartmentOfInitiator.Result.Extend)
      {
        // Прекратить задание на проверку закрытых задач.
        var assignmentsToAbort = AutomatedSupportTickets.PersonnelDepartmentResponsibleAssignments.GetAll(x => x.Task.Id == _obj.Id && x.TaskStartId == _obj.StartId && x.BlockUid == "14").AsEnumerable();
        foreach (var item in assignmentsToAbort)
        {
          item.Abort();
        }
      }
    }

    /// <summary>
    /// У сотрудника есть действующая учетная запись?
    /// </summary>
    /// <returns>True, если есть действующая УЗ, иначе false.</returns>
    public virtual bool Decision27Result()
    {
      return _obj.Employee.Login != null && _obj.Employee.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Active;
    }
    
    /// <summary>
    /// Мониторинг, запуск последнего задания администратору сэд за 1 день до окончания срока гпх.
    /// </summary>
    /// <returns></returns>
    public virtual bool Monitoring23Result()
    {
      if (_obj.Employee.ContractValidTilllenspec <= Calendar.Now.AddWorkingDays(1))
        return true;
      
      return false;
      //throw new System.NotImplementedException();
    }
    
    /// <summary>
    /// Согласование с администратором сэд.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock14(lenspec.AutomatedSupportTickets.Server.PersonnelDepartmentResponsibleAssignmentArguments e)
    {
      var role = Roles.GetAll(r => r.Sid == lenspec.AutomatedSupportTickets.Constants.Module.CompleteAssignmentsResponsible).FirstOrDefault();
      e.Block.Subject = $"Проверка задач и заданий увольняемого сотрудника ГПХ {_obj.Employee}";
      e.Block.Performers.Add(role);
    }
    
    /// <summary>
    /// Уведомление об отсутствии руководителя.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock17(Sungero.Workflow.Server.NoticeArguments e)
    {
      e.Block.Subject = $"Остановлена задача на согласование договора сотрудника ГПХ для {_obj.Employee.Name}: отсутствует руководитель.";
      // Отправляем администраторам СЭД.
      var role = Roles.GetAll(r => r.Sid == EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
      e.Block.Performers.Add(role);
    }
    
    /// <summary>
    /// Задание сотруднику ГПХ.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock13(lenspec.AutomatedSupportTickets.Server.PersonnelDepartmentResponsibleAssignmentArguments e)
    {
      // Формируем тему задания.
      e.Block.Subject = lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTasks.Resources.EmployeeGPHAssigmentName;
      if (_obj.Employee.ContractValidTilllenspec.HasValue)
        e.Block.AbsoluteDeadline = _obj.Employee.ContractValidTilllenspec.Value.AddDays(-1);
      // Устанавливаем количество дней когда завершится задание, -1 день. (Работает кривовато)
      //e.Block.RelativeStopDeadlineDays = _obj.Employee.ContractValidTilllenspec.Value.Subtract(Calendar.Now).Days;
      //e.Block.Subject = $"Test {e.Block.RelativeStopDeadlineDays}";
      
      // Задаём исполнителя.
      e.Block.Performers.Add(_obj.Employee);
    }
    
    /// <summary>
    /// У сотрудника есть действующая учетная запись?
    /// </summary>
    /// <returns>True, если есть действующая УЗ, иначе false.</returns>
    public virtual bool Decision11Result()
    {
      return _obj.Employee.Login != null && _obj.Employee.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Active;
    }

    /// <summary>
    /// Старт. Укажите новую дату окончания договора ГПХ.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock9(lenspec.AutomatedSupportTickets.Server.CoordinationHeadDepartmentOfInitiatorArguments e)
    {
      // Формируем тему задания.
      e.Block.Subject = $"Укажите новый срок действия договора сотрудника ГПХ: {_obj.Employee.Name}.";
      // Срок (+8 рабочих часов от текущей даты и времени).
      e.Block.AbsoluteDeadline = Calendar.Now.AddWorkingHours(8);
      // Указываем исполнителем руководителя.
      e.Block.Performers.Add(_obj.Supervisor);
    }
    
    /// <summary>
    /// Руководитель инициатора определен?
    /// </summary>
    /// <returns></returns>
    public virtual bool Decision5Result()
    {
      var author = _obj.Employee;
      var department = author.Department;
      //оператор итерации
      do
      { //Если руководитель подразделение не пустой и статус активен
        if (department.Manager != null && department.Manager.Status == Sungero.Company.Employee.Status.Active &&
            department.Manager.Login != null && department.Manager.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
        { //Если Руководитель равен Автору
          if (department.Manager.Equals(author))
          { //Если головное подразделение пустое
            if (department.HeadOffice == null)
            { //Руководителем ставим автора
              _obj.Supervisor = author;
              break;
            }
            else
            { //Если головное подразделение не пустое, то берем его и продолжаем еще раз
              department = department.HeadOffice;
              continue;
            }
          }
          else
          { //Если руководитель не равен автору, то ставим его руководителем
            _obj.Supervisor = lenspec.Etalon.Employees.As(department.Manager);
            break;
          }
        }
        else
        { //Если головное подразделение пустое или у руководителя статус закрыт
          if (department.HeadOffice == null)
          {
            _obj.Supervisor = author.Department.BusinessUnit != null ? lenspec.Etalon.Employees.As(author.Department.BusinessUnit.CEO) : null;
            break;
          }
          else
          {
            department = department.HeadOffice;
            continue;
          }
        }
      } while (department != null);

      // Возвращаем результат
      if (_obj.Supervisor == null)
        return false;
      
      _obj.Save();
      return true;
    }
  }
}