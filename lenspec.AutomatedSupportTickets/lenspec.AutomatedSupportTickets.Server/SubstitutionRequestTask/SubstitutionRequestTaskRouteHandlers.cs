using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using lenspec.AutomatedSupportTickets.SubstitutionRequestTask;

namespace lenspec.AutomatedSupportTickets.Server
{
  partial class SubstitutionRequestTaskRouteHandlers
  {
    
    #region Уведомление об отказе в согласовании замещения
    
    public virtual void StartNotice7(lenspec.AutomatedSupportTickets.ISubstitutionRequestNotification notice, lenspec.AutomatedSupportTickets.Server.SubstitutionRequestNotificationArguments e)
    {
      notice.ThreadSubject = lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.RefusedToApproveSubstitutionFormat(_obj.Substitute,
                                                                                                                                   _obj.SubstitutedUser);
      notice.Author = _obj.ManagerOfSubstitutedUser;
    }

    public virtual void StartBlock7(lenspec.AutomatedSupportTickets.Server.SubstitutionRequestNotificationArguments e)
    {
      e.Block.Subject = lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.RefusedToApproveSubstitutionFormat(_obj.Substitute,
                                                                                                                              _obj.SubstitutedUser);
      try
      {
        if (_obj.Author.IsSystem != true)
        {
          e.Block.Performers.Add(_obj.Author);
        }
      }
      catch(Exception ex)
      {
        Logger.DebugFormat("SubstitutionRequestTask - block 7 - {0}", ex.Message);
      }
    }
    
    #endregion

    
    #region Уведомление о создании записи замещения
    
    public virtual void StartNotice6(lenspec.AutomatedSupportTickets.ISubstitutionRequestNotification notice, lenspec.AutomatedSupportTickets.Server.SubstitutionRequestNotificationArguments e)
    {
      notice.ThreadSubject = lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.CreatedSubstitutionFormat(_obj.Substitute,
                                                                                                                          _obj.SubstitutedUser);
    }

    public virtual void StartBlock6(lenspec.AutomatedSupportTickets.Server.SubstitutionRequestNotificationArguments e)
    {
      e.Block.Subject = lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.CreatedSubstitutionFormat(_obj.Substitute, _obj.SubstitutedUser);
      
      var performers = Functions.SubstitutionRequestTask.GetAdministratorEDMSRecipients(_obj);
      foreach (var performer in performers)
      {
        e.Block.Performers.Add(performer);
      }
      
      if (_obj.Prolongation != true && !e.Block.Performers.Contains(_obj.Author))
      {
        e.Block.Performers.Add(_obj.Author);
      }
    }
    
    #endregion

    
    public virtual void Script16Execute()
    {
      Functions.SubstitutionRequestTask.DeleteFunctionQueueItem(_obj);
    }

    #region Заполнение сведений об ошибке
    
    public virtual void Script15Execute()
    {
      var queueItem = Sungero.Docflow.ApprovalFunctionQueueItems.GetAll(q => q.TaskId == _obj.Id && q.TaskStartId == _obj.StartId).FirstOrDefault();
      if (queueItem != null)
      {
        if (_obj.Author.IsSystem == false)
        {
          Sungero.Docflow.PublicFunctions.Module.Remote.SendNoticesAsSubtask(lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.ErrorCreatingSubstitution,
                                                                             new List<IUser>() { _obj.Author },
                                                                             _obj,
                                                                             queueItem.ErrorMessage,
                                                                             null,
                                                                             _obj.Subject);
        }
      }
      else
      {
        Logger.DebugFormat("RequestToArchive - Function execution failed. Queue item not found in Script15. TaskId {0}, StartId {1}", _obj.Id, _obj.StartId);
      }
    }
    
    #endregion

    public virtual void Script19Execute()
    {
      Functions.SubstitutionRequestTask.DeleteFunctionQueueItem(_obj);
    }

    public virtual bool Decision18Result()
    {
      var queueItem = Sungero.Docflow.ApprovalFunctionQueueItems.GetAll(q => q.TaskId == _obj.Id && q.TaskStartId == _obj.StartId).FirstOrDefault();
      if (queueItem != null)
      {
        return queueItem.ProcessingStatus == Sungero.Docflow.ApprovalFunctionQueueItem.ProcessingStatus.Completed;
      }
      else
      {
        Logger.DebugFormat("RequestToArchive - Queue item not found in Decision53. TaskId {0}, StartId {1}", _obj.Id, _obj.StartId);
      }
      
      return false;
    }

    #region Проверка завершения сценария
    
    public virtual bool Monitoring17Result()
    {
      return _obj.SubstitutionGroup.Substitutions.Any();
    }
    
    #endregion
    

    #region Проверка на успешное выполнение сценария
    
    public virtual bool Decision13Result()
    {
      var queueItem = Sungero.Docflow.ApprovalFunctionQueueItems.GetAll(q => q.TaskId == _obj.Id && q.TaskStartId == _obj.StartId).FirstOrDefault();
      if (queueItem != null)
      {
        return queueItem.ProcessingStatus == Sungero.Docflow.ApprovalFunctionQueueItem.ProcessingStatus.Completed;
      }
      else
      {
        Logger.DebugFormat("RequestToArchive - Queue item not found in Decision13. TaskId {0}, StartId {1}", _obj.Id, _obj.StartId);
      }
      return false;
    }
    
    #endregion
    

    #region Уведомление об истечении срока выполнения сценария
    
    public virtual void Script24Execute()
    {
      var queueItem = Sungero.Docflow.ApprovalFunctionQueueItems.GetAll(q => q.TaskId == _obj.Id && q.TaskStartId == _obj.StartId).FirstOrDefault();
      if (queueItem == null)
      {
        Logger.DebugFormat("RequestToArchive - Queue item not found in Block524. TaskId {0}, StartId {1}", _obj.Id, _obj.StartId);
        return;
      }
      
      if (queueItem.IsNoticeSended == true)
      {
        Logger.DebugFormat("RequestToArchive - Notice already sended in Block24. TaskId {0}, StartId {1}", _obj.Id, _obj.StartId);
        return;
      }
      
      var performers = Functions.SubstitutionRequestTask.GetAdministratorEDMSRecipients(_obj);
      if (performers.Any())
      {
        Sungero.Docflow.PublicFunctions.Module.Remote.SendNoticesAsSubtask("Тема уведомления об истечении срока", performers, _obj, queueItem.ErrorMessage, null, _obj.Subject);
      }
      
      queueItem.IsNoticeSended = true;
      queueItem.Save();
    }
    
    #endregion
    

    #region Ожидание выполнения сценария
    
    public virtual bool Monitoring12Result()
    {
      var queueItem = Sungero.Docflow.ApprovalFunctionQueueItems.GetAll(q => q.TaskId == _obj.Id && q.TaskStartId == _obj.StartId).FirstOrDefault();
      if (queueItem == null)
      {
        Logger.DebugFormat("RequestToArchive - Queue item not found in Monitoring12. TaskId {0}, StartId {1}", _obj.Id, _obj.StartId);
        return true;
      }
      
      Logger.DebugFormat("RequestToArchive - Queue item has status {2} in Monitoring12. TaskId {0}, StartId {1}", _obj.Id, _obj.StartId, queueItem.ProcessingStatus);
      
      return queueItem.ProcessingStatus == Sungero.Docflow.ApprovalFunctionQueueItem.ProcessingStatus.Completed ||
        queueItem.ProcessingStatus == Sungero.Docflow.ApprovalFunctionQueueItem.ProcessingStatus.Error;
    }
    
    #endregion
    

    #region Выполнение сценария
    
    public virtual void Script11Execute()
    {
      var queueItem = Sungero.Docflow.ApprovalFunctionQueueItems.GetAll()
        .Where(x => x.TaskId == _obj.Id && x.TaskStartId == (_obj.StartId ?? 0) && x.ProcessingStatus == Sungero.Docflow.ApprovalFunctionQueueItem.ProcessingStatus.NotProcessed)
        .FirstOrDefault();
      if (queueItem == null)
      {
        Logger.DebugFormat("RequestToArchive - Queue item not found, task: {0}, StartId {1}, Block11", _obj.Id, _obj.StartId);
        return;
      }
      // Если не будет отрабатывать, то перенести в асинхронный обработчик.
      var substitution = Functions.SubstitutionRequestTask.CreateSubstitution(_obj);
      if (substitution == null)
      {
        Logger.DebugFormat("RequestToArchive - Substitution has not been created, task: {0}, StartId {1}, Block11", _obj.Id, _obj.StartId);
        queueItem.ProcessingStatus = Sungero.Docflow.ApprovalFunctionQueueItem.ProcessingStatus.Error;
        return;
      }
      else
      {
        _obj.SubstitutionGroup.Substitutions.Add(substitution);
        queueItem.ProcessingStatus = Sungero.Docflow.ApprovalFunctionQueueItem.ProcessingStatus.Completed;
        queueItem.Save();
        Logger.DebugFormat("RequestToArchive - Substitution has been added as attachment, task: {0}, StartId {1}, Block11", _obj.Id, _obj.StartId);
      }
    }
    
    #endregion
    

    #region Добавление сценария в очередь
    
    public virtual void Script10Execute()
    {
      var queueItem = Sungero.Docflow.ApprovalFunctionQueueItems.Create();
      queueItem.ProcessingStatus = Sungero.Docflow.ApprovalFunctionQueueItem.ProcessingStatus.NotProcessed;
      queueItem.TaskId = _obj.Id;
      queueItem.TaskStartId = _obj.StartId ?? 0;
      queueItem.IsNoticeSended = false;
      queueItem.Save();
      Logger.DebugFormat("RequestToArchive - Create queue item. Id {0}, TaskId {1}, StartId {2}", queueItem.Id, queueItem.TaskId, queueItem.TaskStartId);
    }
    #endregion
    
    
    #region Согласование
    
    public virtual void StartBlock3(lenspec.AutomatedSupportTickets.Server.ApprovalSubstitutionAssignmentArguments e)
    {
      if (_obj.Prolongation == true)
      {
        e.Block.Subject = lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.ApproveProlongationOfSubstitutionFormat(_obj.Substitute.Name, _obj.SubstitutedUser.Name);
      }
      else
      {
        e.Block.Subject = lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Resources.ApproveCreationOfSubstitutionFormat(_obj.Substitute.Name, _obj.SubstitutedUser.Name);
      }
      e.Block.RelativeDeadlineDays = Constants.SubstitutionRequestTask.ApprovalRelativeDeadlineDays;
      e.Block.Performers.Add(_obj.ManagerOfSubstitutedUser);
      
      Functions.SubstitutionRequestTask.GrantAccessRightsForTask(_obj, new List<IRecipient>() { _obj.ManagerOfSubstitutedUser });
    }
    
    public virtual void StartAssignment3(lenspec.AutomatedSupportTickets.IApprovalSubstitutionAssignment assignment, lenspec.AutomatedSupportTickets.Server.ApprovalSubstitutionAssignmentArguments e)
    {
      assignment.Substitute = _obj.Substitute;
      assignment.SubstitutedUser = _obj.SubstitutedUser;
      assignment.StartDate = _obj.StartDate;
      assignment.EndDate = _obj.EndDate;
      assignment.ManagerOfSubstitutedUser = _obj.ManagerOfSubstitutedUser;
      assignment.Founding = _obj.Founding;
    }
    
    #endregion
    

    #region Условие Инициатор = Сотрудник?
    
    public virtual bool Decision5Result()
    {
      return _obj.Author.Equals(Sungero.CoreEntities.Users.As(_obj.SubstitutedUser));
    }
    
    #endregion

  }
}