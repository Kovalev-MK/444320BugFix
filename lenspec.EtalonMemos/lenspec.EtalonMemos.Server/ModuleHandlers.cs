using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.Company;
using Sungero.CoreEntities;
using Sungero.RecordManagement.ActionItemExecutionTask;
using Sungero;

namespace lenspec.EtalonMemos.Server
{
  partial class AssignmentsByMemoFromMeFolderHandlers
  {

    public virtual IQueryable<Sungero.RecordManagement.IActionItemExecutionTask> AssignmentsByMemoFromMeDataQuery(IQueryable<Sungero.RecordManagement.IActionItemExecutionTask> query)
    {        
      // Показать только те поручения, где Документ для исполнения = Служебная записка      
      query = query.Where(a => a.AttachmentDetails.Any(d => d.AttachmentTypeGuid == Constants.Module.MemoTypeGuid && d.GroupId == Constants.Module.MainDocumentGroupID));
     
      // Отфильтровать поручения, где Автор = текущий пользователь
      query = query.Where(b => Sungero.CoreEntities.Users.As(b.Author).Equals(Sungero.CoreEntities.Users.Current));      
     
      // Вернуть нефильтрованный список, если нет фильтра. Он будет использоваться во всех Get() и GetAll().
      var filter = _filter;
      if (_filter == null)
        return query;         
      
      // Не показывать не стартованные поручения.
      query = query.Where(l => l.Status != Sungero.Workflow.Task.Status.Draft);
      
      // Не показывать составные поручения (только подзадачи).
      query = query.Where(j => j.IsCompoundActionItem == false);
      
      // Фильтр по статусу.
      var statuses = new List<Enumeration>();
      if (filter.OnExecution)
      {
        statuses.Add(ExecutionState.OnExecution);
        statuses.Add(ExecutionState.OnControl);
        statuses.Add(ExecutionState.OnRework);
      }
      
      if (filter.Executed)
      {
        statuses.Add(ExecutionState.Executed);
        statuses.Add(ExecutionState.Aborted);
      }
      
      if (statuses.Any())
        query = query.Where(q => q.ExecutionState != null && statuses.Contains(q.ExecutionState.Value));
      
      // Фильтры "Поручения где я", "По сотруднику".
      var currentUser = Users.Current;
      
      // Сформировать списки пользователей для фильтрации.
      var authors = new List<IUser>();
      var assignees = new List<IUser>();
      var supervisors = new List<IUser>();
      
      if (filter.Author != null)
        authors.Add(filter.Author);
      if (filter.Assignee != null)
        assignees.Add(filter.Assignee);
      if (filter.Supervisor != null)
        supervisors.Add(filter.Supervisor);
      
      // Наложить фильтр по всем замещениям, если не указаны фильтры по текущему или выбранному сотруднику.
      if (Sungero.Docflow.PublicFunctions.Module.Remote.IsAdministratorOrAdvisor())
      {
        var allSubstitutes = Substitutions.ActiveSubstitutedUsers.ToList();
        allSubstitutes.Add(Users.Current);
        query = query.Where(j => allSubstitutes.Contains(j.AssignedBy) || allSubstitutes.Contains(j.Assignee) ||
                            j.CoAssignees.Any(p => allSubstitutes.Contains(p.Assignee)) ||
                            allSubstitutes.Contains(j.Supervisor) || allSubstitutes.Contains(j.StartedBy) ||
                            j.ActionItemObservers.Any(o => Recipients.AllRecipientIds.Contains(o.Observer.Id)));
      }
      
      query = query.Where(j => (!authors.Any() || authors.Contains(j.AssignedBy)) &&
                          (!assignees.Any() || assignees.Contains(j.Assignee) || j.CoAssignees.Any(p => assignees.Contains(p.Assignee))) &&
                          (!supervisors.Any() || supervisors.Contains(j.Supervisor)));
      
      // Фильтр по соблюдению сроков.
      var now = Calendar.Now;
      var today = Calendar.UserToday;
      var tomorrow = today.AddDays(1);
      if (filter.Overdue)
        query = query.Where(j => j.Status != Sungero.Workflow.Task.Status.Aborted && j.HasIndefiniteDeadline != true &&
                            ((j.ActualDate == null && j.Deadline < now && j.Deadline != today && j.Deadline != tomorrow) ||
                             (j.ActualDate != null && j.ActualDate > j.Deadline)));
      // Фильтр по плановому сроку.
      if (filter.LastMonth)
      {
        var lastMonthBeginDate = today.AddDays(-30);
        var lastMonthBeginDateNextDay = lastMonthBeginDate.AddDays(1);
        var lastMonthBeginDateWithTime = Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(lastMonthBeginDate);

        query = query.Where(j => ((j.Deadline >= lastMonthBeginDateWithTime && j.Deadline < now) ||
                                  j.Deadline == lastMonthBeginDate || j.Deadline == lastMonthBeginDateNextDay || j.Deadline == today) &&
                            j.Deadline != tomorrow);
      }

      if (filter.ManualPeriod)
      {
        if (filter.DateRangeFrom != null)
        {
          var dateRangeFromNextDay = filter.DateRangeFrom.Value.AddDays(1);
          var dateFromWithTime = Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(filter.DateRangeFrom.Value);
          query = query.Where(j => j.HasIndefiniteDeadline == true ||
                              j.Deadline >= dateFromWithTime ||
                              j.Deadline == filter.DateRangeFrom.Value ||
                              j.Deadline == dateRangeFromNextDay);
        }
        if (filter.DateRangeTo != null)
        {
          var dateRangeNextDay = filter.DateRangeTo.Value.AddDays(1);
          var dateTo = filter.DateRangeTo.Value.EndOfDay().FromUserTime();
          query = query.Where(j => j.HasIndefiniteDeadline != true &&
                              ((j.Deadline < dateTo || j.Deadline == filter.DateRangeTo.Value) &&
                               j.Deadline != dateRangeNextDay));
        }
      }
      
      return query;  
    }
  }

  partial class ControlIssuedAssignmentsFolderHandlers
  {

    public virtual IQueryable<Sungero.RecordManagement.IActionItemExecutionTask> ControlIssuedAssignmentsDataQuery(IQueryable<Sungero.RecordManagement.IActionItemExecutionTask> query)
    {
      // Отфильтровать поручения, где Документ для исполнения = Служебная записка      
      query = query.Where(a => a.AttachmentDetails.Any(d => d.AttachmentTypeGuid == Constants.Module.MemoTypeGuid && d.GroupId == Constants.Module.MainDocumentGroupID));
       
      // Отфильтровать поручения, где Контролёр = текущий пользователь
      query = query.Where(b => Sungero.CoreEntities.Users.As(b.Supervisor).Equals(Sungero.CoreEntities.Users.Current));
      
      // Вернуть нефильтрованный список, если нет фильтра. Он будет использоваться во всех Get() и GetAll().
      var filter = _filter;
      if (_filter == null)     
        return query;         
      
      // Не показывать не стартованные поручения.
      query = query.Where(l => l.Status != Sungero.Workflow.Task.Status.Draft);
      
      // Не показывать составные поручения (только подзадачи).
      query = query.Where(j => j.IsCompoundActionItem == false);
      
      // Фильтр по статусу.
      var statuses = new List<Enumeration>();
      if (filter.OnExecution)
      {
        statuses.Add(ExecutionState.OnExecution);
        statuses.Add(ExecutionState.OnControl);
        statuses.Add(ExecutionState.OnRework);
      }
      
      if (filter.Executed)
      {
        statuses.Add(ExecutionState.Executed);
        statuses.Add(ExecutionState.Aborted);
      }
      
      if (statuses.Any())
        query = query.Where(q => q.ExecutionState != null && statuses.Contains(q.ExecutionState.Value));
      
      // Фильтры "Поручения где я", "По сотруднику".
      var currentUser = Users.Current;
      
      // Сформировать списки пользователей для фильтрации.
      var authors = new List<IUser>();
      var assignees = new List<IUser>();
      var supervisors = new List<IUser>();
      
      if (filter.Author != null)
        authors.Add(filter.Author);
      if (filter.Assignee != null)
        assignees.Add(filter.Assignee);
      if (filter.Supervisor != null)
        supervisors.Add(filter.Supervisor);
      
      // Наложить фильтр по всем замещениям, если не указаны фильтры по текущему или выбранному сотруднику.
      if (Sungero.Docflow.PublicFunctions.Module.Remote.IsAdministratorOrAdvisor())
      {
        var allSubstitutes = Substitutions.ActiveSubstitutedUsers.ToList();
        allSubstitutes.Add(Users.Current);
        query = query.Where(j => allSubstitutes.Contains(j.AssignedBy) || allSubstitutes.Contains(j.Assignee) ||
                            j.CoAssignees.Any(p => allSubstitutes.Contains(p.Assignee)) ||
                            allSubstitutes.Contains(j.Supervisor) || allSubstitutes.Contains(j.StartedBy) ||
                            j.ActionItemObservers.Any(o => Recipients.AllRecipientIds.Contains(o.Observer.Id)));
      }
      
      query = query.Where(j => (!authors.Any() || authors.Contains(j.AssignedBy)) &&
                          (!assignees.Any() || assignees.Contains(j.Assignee) || j.CoAssignees.Any(p => assignees.Contains(p.Assignee))) &&
                          (!supervisors.Any() || supervisors.Contains(j.Supervisor)));
      
      // Фильтр по соблюдению сроков.
      var now = Calendar.Now;
      var today = Calendar.UserToday;
      var tomorrow = today.AddDays(1);
      if (filter.Overdue)
        query = query.Where(j => j.Status != Sungero.Workflow.Task.Status.Aborted && j.HasIndefiniteDeadline != true &&
                            ((j.ActualDate == null && j.Deadline < now && j.Deadline != today && j.Deadline != tomorrow) ||
                             (j.ActualDate != null && j.ActualDate > j.Deadline)));
      // Фильтр по плановому сроку.
      if (filter.LastMonth)
      {
        var lastMonthBeginDate = today.AddDays(-30);
        var lastMonthBeginDateNextDay = lastMonthBeginDate.AddDays(1);
        var lastMonthBeginDateWithTime = Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(lastMonthBeginDate);

        query = query.Where(j => ((j.Deadline >= lastMonthBeginDateWithTime && j.Deadline < now) ||
                                  j.Deadline == lastMonthBeginDate || j.Deadline == lastMonthBeginDateNextDay || j.Deadline == today) &&
                            j.Deadline != tomorrow);
      }

      if (filter.ManualPeriod)
      {
        if (filter.DateRangeFrom != null)
        {
          var dateRangeFromNextDay = filter.DateRangeFrom.Value.AddDays(1);
          var dateFromWithTime = Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(filter.DateRangeFrom.Value);
          query = query.Where(j => j.HasIndefiniteDeadline == true ||
                              j.Deadline >= dateFromWithTime ||
                              j.Deadline == filter.DateRangeFrom.Value ||
                              j.Deadline == dateRangeFromNextDay);
        }
        if (filter.DateRangeTo != null)
        {
          var dateRangeNextDay = filter.DateRangeTo.Value.AddDays(1);
          var dateTo = filter.DateRangeTo.Value.EndOfDay().FromUserTime();
          query = query.Where(j => j.HasIndefiniteDeadline != true &&
                              ((j.Deadline < dateTo || j.Deadline == filter.DateRangeTo.Value) &&
                               j.Deadline != dateRangeNextDay));
        }
      }
      
      return query;  
    }
  }

  partial class AssignmentsByMemoForMeFolderHandlers
  {

    public virtual IQueryable<Sungero.RecordManagement.IActionItemExecutionTask> AssignmentsByMemoForMeDataQuery(IQueryable<Sungero.RecordManagement.IActionItemExecutionTask> query)
    {
      // Показать только те поручения, где Документ для исполнения = Служебная записка
      query = query.Where(a => a.AttachmentDetails.Any(d => d.AttachmentTypeGuid == Constants.Module.MemoTypeGuid && d.GroupId == Constants.Module.MainDocumentGroupID));
      
      // Отфильтровать поручения, где Исполнитель = текущий пользователь
      query = query.Where(b => Sungero.CoreEntities.Users.As(b.Assignee).Equals(Sungero.CoreEntities.Users.Current));      
     
      // Вернуть нефильтрованный список, если нет фильтра. Он будет использоваться во всех Get() и GetAll().
      var filter = _filter;
      if (_filter == null)
        return query;         
      
      // Не показывать не стартованные поручения.
      query = query.Where(l => l.Status != Sungero.Workflow.Task.Status.Draft);
      
      // Не показывать составные поручения (только подзадачи).
      query = query.Where(j => j.IsCompoundActionItem == false);
      
      // Фильтр по статусу.
      var statuses = new List<Enumeration>();
      if (filter.OnExecution)
      {
        statuses.Add(ExecutionState.OnExecution);
        statuses.Add(ExecutionState.OnControl);
        statuses.Add(ExecutionState.OnRework);
      }
      
      if (filter.Executed)
      {
        statuses.Add(ExecutionState.Executed);
        statuses.Add(ExecutionState.Aborted);
      }
      
      if (statuses.Any())
        query = query.Where(q => q.ExecutionState != null && statuses.Contains(q.ExecutionState.Value));
      
      // Фильтры "Поручения где я", "По сотруднику".
      var currentUser = Users.Current;
      
      // Сформировать списки пользователей для фильтрации.
      var authors = new List<IUser>();
      var assignees = new List<IUser>();
      var supervisors = new List<IUser>();
      
      if (filter.Author != null)
        authors.Add(filter.Author);
      if (filter.Assignee != null)
        assignees.Add(filter.Assignee);
      if (filter.Supervisor != null)
        supervisors.Add(filter.Supervisor);
      
      // Наложить фильтр по всем замещениям, если не указаны фильтры по текущему или выбранному сотруднику.
      if (Sungero.Docflow.PublicFunctions.Module.Remote.IsAdministratorOrAdvisor())
      {
        var allSubstitutes = Substitutions.ActiveSubstitutedUsers.ToList();
        allSubstitutes.Add(Users.Current);
        query = query.Where(j => allSubstitutes.Contains(j.AssignedBy) || allSubstitutes.Contains(j.Assignee) ||
                            j.CoAssignees.Any(p => allSubstitutes.Contains(p.Assignee)) ||
                            allSubstitutes.Contains(j.Supervisor) || allSubstitutes.Contains(j.StartedBy) ||
                            j.ActionItemObservers.Any(o => Recipients.AllRecipientIds.Contains(o.Observer.Id)));
      }
      
      query = query.Where(j => (!authors.Any() || authors.Contains(j.AssignedBy)) &&
                          (!assignees.Any() || assignees.Contains(j.Assignee) || j.CoAssignees.Any(p => assignees.Contains(p.Assignee))) &&
                          (!supervisors.Any() || supervisors.Contains(j.Supervisor)));
      
      // Фильтр по соблюдению сроков.
      var now = Calendar.Now;
      var today = Calendar.UserToday;
      var tomorrow = today.AddDays(1);
      if (filter.Overdue)
        query = query.Where(j => j.Status != Sungero.Workflow.Task.Status.Aborted && j.HasIndefiniteDeadline != true &&
                            ((j.ActualDate == null && j.Deadline < now && j.Deadline != today && j.Deadline != tomorrow) ||
                             (j.ActualDate != null && j.ActualDate > j.Deadline)));
      // Фильтр по плановому сроку.
      if (filter.LastMonth)
      {
        var lastMonthBeginDate = today.AddDays(-30);
        var lastMonthBeginDateNextDay = lastMonthBeginDate.AddDays(1);
        var lastMonthBeginDateWithTime = Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(lastMonthBeginDate);

        query = query.Where(j => ((j.Deadline >= lastMonthBeginDateWithTime && j.Deadline < now) ||
                                  j.Deadline == lastMonthBeginDate || j.Deadline == lastMonthBeginDateNextDay || j.Deadline == today) &&
                            j.Deadline != tomorrow);
      }

      if (filter.ManualPeriod)
      {
        if (filter.DateRangeFrom != null)
        {
          var dateRangeFromNextDay = filter.DateRangeFrom.Value.AddDays(1);
          var dateFromWithTime = Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(filter.DateRangeFrom.Value);
          query = query.Where(j => j.HasIndefiniteDeadline == true ||
                              j.Deadline >= dateFromWithTime ||
                              j.Deadline == filter.DateRangeFrom.Value ||
                              j.Deadline == dateRangeFromNextDay);
        }
        if (filter.DateRangeTo != null)
        {
          var dateRangeNextDay = filter.DateRangeTo.Value.AddDays(1);
          var dateTo = filter.DateRangeTo.Value.EndOfDay().FromUserTime();
          query = query.Where(j => j.HasIndefiniteDeadline != true &&
                              ((j.Deadline < dateTo || j.Deadline == filter.DateRangeTo.Value) &&
                               j.Deadline != dateRangeNextDay));
        }
      }
      
      return query;        
    }
    
  }
}
