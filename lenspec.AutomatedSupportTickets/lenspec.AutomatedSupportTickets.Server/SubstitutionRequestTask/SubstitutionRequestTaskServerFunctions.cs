using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.SubstitutionRequestTask;

namespace lenspec.AutomatedSupportTickets.Server
{
  partial class SubstitutionRequestTaskFunctions
  {
    //Добавлено Avis Expert
    
    /// <summary>
    /// Выдать права на изменение карточки задачи.
    /// </summary>
    /// <param name="recipients">Исполнители.</param>
    public virtual void GrantAccessRightsForTask(List<IRecipient> recipients)
    {
      foreach(var recipient in recipients)
      {
        if (!_obj.AccessRights.IsGranted(DefaultAccessRightsTypes.Change, recipient))
        {
          _obj.AccessRights.Grant(recipient, DefaultAccessRightsTypes.Change);
        }
      }
    }
    
    /// <summary>
    /// Вычислить участников роли Администратор СЭД.
    /// </summary>
    /// <returns>Список пользователей-участников роли.</returns>
    public List<Sungero.CoreEntities.IUser> GetAdministratorEDMSRecipients()
    {
      var performers = new List<IUser>() { };
      
      var adminEDMS = Roles.GetAll(n => n.Sid == EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
      if (adminEDMS != null)
      {
        var recipients = Roles.GetAllUsersInGroup(adminEDMS);
        if (recipients.Any())
        {
          performers.AddRange(recipients);
        }
      }
      return performers;
    }
    
    /// <summary>
    /// Получить дубли замещения.
    /// </summary>
    /// <returns>Замещения, дублирующие текущее.</returns>
    [Remote(IsPure = true)]
    public IQueryable<Sungero.CoreEntities.ISubstitution> GetDublicatesInSubstitution()
    {
      var dublicate = Sungero.CoreEntities.Substitutions.GetAll().Where(x => _obj.Substitute.Equals(x.Substitute) &&
                                                                        _obj.SubstitutedUser.Equals(x.User) &&
                                                                        x.StartDate != null && _obj.StartDate.Value >= x.StartDate.Value &&
                                                                        x.EndDate != null && _obj.EndDate.Value <= x.EndDate.Value);
      
      return dublicate;
    }
    
    /// <summary>
    /// Удалить элемент очереди для этапа функции.
    /// </summary>
    public void DeleteFunctionQueueItem()
    {
      var queueItem = Sungero.Docflow.ApprovalFunctionQueueItems.GetAll(q => q.TaskId == _obj.Id && q.TaskStartId == _obj.StartId).FirstOrDefault();
      if (queueItem != null)
      {
        Sungero.Docflow.ApprovalFunctionQueueItems.Delete(queueItem);
        Logger.DebugFormat("Delete queue item. TaskId {0}, StartId {1}", _obj.Id, _obj.StartId);
      }
    }
    
    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public Sungero.CoreEntities.ISubstitution CreateSubstitution()
    {
      var substitution = Sungero.CoreEntities.Substitutions.Create();
      substitution.Substitute = _obj.Substitute;
      substitution.User = _obj.SubstitutedUser;
      substitution.StartDate = _obj.StartDate;
      substitution.EndDate = _obj.EndDate;
      substitution.Comment = _obj.Founding;
      substitution.Save();
      
      return substitution;
    }
    
    /// <summary>
    /// Создать заявку на формирование замещения.
    /// </summary>
    /// <returns>Заявка на формирование замещения.</returns>
    [Public]
    public static ISubstitutionRequestTask CreateSubstitutionRequestTask(Sungero.Company.IEmployee substitute, Sungero.Company.IEmployee substitutedUser,
                                                                         DateTime? startDate, DateTime? endDate, Sungero.Company.IEmployee managerOfSubstitutedUser,
                                                                         string founding, bool prolongation)
    {
      var task = AutomatedSupportTickets.SubstitutionRequestTasks.Create();
      
      task.Substitute = substitute;
      task.SubstitutedUser = substitutedUser;
      task.StartDate = startDate;
      task.EndDate = endDate;
      task.ManagerOfSubstitutedUser = managerOfSubstitutedUser;
      task.Founding = founding;
      task.Prolongation = prolongation;
      
      return task;
    }
    
    //конец Добавлено Avis Expert
  }
}