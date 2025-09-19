using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using lenspec.AutomatedSupportTickets.EditComponentRXRequestTask;

namespace lenspec.AutomatedSupportTickets.Server
{
  // Добавлено avis.
  partial class EditComponentRXRequestTaskRouteHandlers
  {

    /// <summary>
    /// Условие "Хотя бы один согласовал?"
    /// </summary>
    /// <returns></returns>
    public virtual bool Decision14Result()
    {
      var currentTaskStartId = _obj.StartId;
      var approved = false;
      var approvalManagerAssignments = AutomatedSupportTickets.ApprovalManagers.GetAll()
        .Where(a => Equals(a.Task, _obj) && a.TaskStartId == currentTaskStartId);
      if (approvalManagerAssignments.Any(a => a.Result == AutomatedSupportTickets.ApprovalManager.Result.Complete))
      {
        approved = true;
      }
      return approved;
    }

    /// <summary>
    /// Условие "Отклонено?"
    /// </summary>
    /// <returns></returns>
    public virtual bool Decision13Result()
    {
      var currentTaskStartId = _obj.StartId;
      var rejected = true;
      var approvalManagerAssignments = AutomatedSupportTickets.ApprovalManagers.GetAll()
        .Where(a => Equals(a.Task, _obj) && a.TaskStartId == currentTaskStartId);
      if (approvalManagerAssignments.Any(a => a.Result != AutomatedSupportTickets.ApprovalManager.Result.Reject))
      {
        rejected = false;
      }
      return rejected;
    }

    public virtual void StartBlock11(Sungero.Workflow.Server.NoticeArguments e)
    {
      e.Block.Subject = $"Руководитель Сотрудника не вычислен, отсутствует учетная запись, обратитесь к администратору.";
      e.Block.Performers.Add(_obj.Author);
    }
    
    /// <summary>
    /// Старт блока Дополнительный согласующий.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock10(lenspec.AutomatedSupportTickets.Server.ApprovalManagerArguments e)
    {
      e.Block.Subject = $"Согласуйте: {_obj.Subject}";
      //e.Block.Performers.Add(_obj.Forwarding);
      foreach(var item in _obj.AddApprovers)
      {
        e.Block.Performers.Add(item.Approver);
      }
      _obj.Forwarding = null;
    }
    
    /// <summary>
    /// Уведомление о выполнении.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock9(Sungero.Workflow.Server.NoticeArguments e)
    {
      e.Block.Subject = $"Ваша заявка была выполнена {_obj.Subject}.";
      e.Block.Performers.Add(_obj.Author);
    }
    
    /// <summary>
    /// Согласование с администраторо СЭД.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock8(lenspec.AutomatedSupportTickets.Server.ApprovalAdministratorArguments e)
    {
      var role = Roles.GetAll(r => r.Sid == EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
      e.Block.Subject = $"Выполнение заявки: {_obj.Subject}";
      e.Block.Performers.Add(role);
    }
    
    /// <summary>
    /// Уведомление об отклонении.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock7(Sungero.Workflow.Server.NoticeArguments e)
    {
      e.Block.Subject = "Ваша заявка была отклонена. Пожалуйста, ознакомьтесь с перепиской в задаче.";
      e.Block.Performers.Add(_obj.Author);
    }
    
    /// <summary>
    /// Согласование с руководителем.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock6(lenspec.AutomatedSupportTickets.Server.ApprovalManagerArguments e)
    {
      e.Block.Subject = $"Согласуйте: {_obj.Subject}";
      // Задаём руководителя.
      if (_obj.TypeRequest == lenspec.AutomatedSupportTickets.EditComponentRXRequestTask.TypeRequest.EditDoc && _obj.CollectionEmployees.Any())
      {
        foreach(var item in _obj.CollectionEmployees)
        {
          e.Block.Performers.Add(Etalon.PublicFunctions.Employee.GetDepartmentManagerOrCEO(item.Employee));
        }
      }
      else
      {
        var manager = Etalon.PublicFunctions.Employee.GetDepartmentManagerOrCEO(_obj.Author);
        e.Block.Performers.Add(manager);
      }
    }
  }
  // Конец добавлено avis.
}