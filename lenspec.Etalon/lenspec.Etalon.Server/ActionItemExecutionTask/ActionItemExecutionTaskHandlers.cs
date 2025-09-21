using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ActionItemExecutionTask;



namespace lenspec.Etalon
{

  partial class ActionItemExecutionTaskAssignedByPropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация выбора из списка "Выдал".
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override IQueryable<T> AssignedByFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      //TODO: нужен ли коммент коробочной фильтрации?
      //query = base.AssignedByFiltering(query, e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
        return query;
      
      // Фильтруем по персонам.
      if (Users.Current.IsSystem != true && Employees.Current != null)
        query = query.Where(q => q.Person.Equals(Employees.Current.Person));
      
      return query;
    }
  }

  // Добавлено avis.
  
  partial class ActionItemExecutionTaskActionItemPartsSupervisorPropertyFilteringServerHandler<T>
  {
    
    /// <summary>
    /// Контроллер, табличной части.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override IQueryable<T> ActionItemPartsSupervisorFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.ActionItemPartsSupervisorFiltering(query, e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
        return query;
      
      // Фильтруем по НОР, текущего пользователя, кроме Обращения клиента.
      var document = _root.DocumentsGroup.OfficialDocuments.SingleOrDefault();
      if ((document == null || !avis.CustomerRequests.CustomerRequests.Is(document)) &&
          Employees.Current.Department != null && Employees.Current.Department.BusinessUnit != null)
      {
        query = query.Where(q => q.Department.BusinessUnit != null && q.Department.BusinessUnit.Equals(Employees.Current.Department.BusinessUnit));
      }
      
      return query;
    }
  }
  
  partial class ActionItemExecutionTaskAssigneePropertyFilteringServerHandler<T>
  {

    /// <summary>
    /// Фильтрация исполнителя.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override IQueryable<T> AssigneeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.AssigneeFiltering(query, e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
        return query;
      
      var document = _obj.DocumentsGroup.OfficialDocuments.SingleOrDefault();
      if (document != null && avis.CustomerRequests.CustomerRequests.Is(document))
      {
        query = query.Where(x => x.Login != null && x.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      }
      else if (_obj.AssignedBy != null && _obj.AssignedBy.Department != null)
      {
        var currentDepartment = _obj.AssignedBy.Department;
        // Сотрудники подразделения текущего пользователя, кроме Руководителя.
        var currentDepartmentEmployees = currentDepartment.RecipientLinks.Select(x => Sungero.Company.Employees.As(x.Member)).Where(x => x != null);
        if (currentDepartment.Manager != null)
          currentDepartmentEmployees = currentDepartmentEmployees.Where(x => !x.Equals(currentDepartment.Manager));
        
        var currentDepartmentEmployeeIds = currentDepartmentEmployees.Select(x => x.Id);
        // Список ИД подчиненных подразделений.
        var subDepartmentIds = Etalon.Module.Company.PublicFunctions.Module.GetSubDepartments(currentDepartment).Select(x => x.Id);
        query = query.Where(q => currentDepartmentEmployeeIds.Contains(q.Id) || subDepartmentIds.Contains(q.Department.Id));
        
        var employees = query.Select(x => Sungero.Company.Employees.As(x)).ToList();
        var notAuthomated = lenspec.EtalonDatabooks.PublicFunctions.Module.GetNotAutomatedEmployeesAvis(employees).Select(x => x.Id).ToList();
        if (notAuthomated != null && notAuthomated.Any())
          query = query.Where(x => !notAuthomated.Contains(x.Id));
      }
      
      return query;
    }
  }

  partial class ActionItemExecutionTaskActionItemPartsAssigneePropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Исполнитель табличной части.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override IQueryable<T> ActionItemPartsAssigneeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.ActionItemPartsAssigneeFiltering(query, e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
        return query;
      
      var document = _root.DocumentsGroup.OfficialDocuments.SingleOrDefault();
      if ((document == null || !avis.CustomerRequests.CustomerRequests.Is(document)) &&
          Employees.Current != null && Employees.Current.Department != null)
      {
        var currentDepartment = Employees.Current.Department;
        // Сотрудники подразделения текущего пользователя, кроме Руководителя.
        var currentDepartmentEmployees = currentDepartment.RecipientLinks.Select(x => Sungero.Company.Employees.As(x.Member)).Where(x => x != null);
        if (currentDepartment.Manager != null)
        {
          currentDepartmentEmployees = currentDepartmentEmployees.Where(x => !x.Equals(currentDepartment.Manager));
        }
        var currentDepartmentEmployeeIds = currentDepartmentEmployees.Select(x => x.Id);
        // Список ИД подчиненных подразделений.
        var subDepartmentIds = Etalon.Module.Company.PublicFunctions.Module.GetSubDepartments(currentDepartment).Select(x => x.Id);
        query = query.Where(q => currentDepartmentEmployeeIds.Contains(q.Id) || subDepartmentIds.Contains(q.Department.Id));
      }
      
      return query;
    }
  }
  
  partial class ActionItemExecutionTaskCoAssigneesAssigneePropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация свойства "Соисполнители".
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override IQueryable<T> CoAssigneesAssigneeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.CoAssigneesAssigneeFiltering(query, e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
        return query;
      
      var document = _root.DocumentsGroup.OfficialDocuments.SingleOrDefault();
      if (document != null && avis.CustomerRequests.CustomerRequests.Is(document))
      {
        query = query.Where(x => x.Login != null && x.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      }
      else if (Employees.Current != null && Employees.Current.Department != null)
      {
        var currentDepartment = Employees.Current.Department;
        // Сотрудники подразделения текущего пользователя, кроме Руководителя.
        var currentDepartmentEmployees = currentDepartment.RecipientLinks.Select(x => Sungero.Company.Employees.As(x.Member)).Where(x => x != null);
        if (currentDepartment.Manager != null)
        {
          currentDepartmentEmployees = currentDepartmentEmployees.Where(x => !x.Equals(currentDepartment.Manager));
        }
        // Неавтоматизированные сотрудники.
        var notAutomatedEmployees = Sungero.Company.PublicFunctions.Module.Remote.GetNotAutomatedEmployees(currentDepartmentEmployees.ToList()).Select(x => x.Id).ToList();
        
        var currentDepartmentEmployeeIds = currentDepartmentEmployees.Select(x => x.Id);
        // Список ИД подчиненных подразделений.
        var subDepartmentIds = Etalon.Module.Company.PublicFunctions.Module.GetSubDepartments(currentDepartment).Select(x => x.Id);
        query = query.Where(q => currentDepartmentEmployeeIds.Contains(q.Id) || subDepartmentIds.Contains(q.Department.Id)).Where(q => !notAutomatedEmployees.Contains(q.Id));
      }
      
      return query;
    }
  }

  partial class ActionItemExecutionTaskSupervisorPropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация свойства "Контролер".
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override IQueryable<T> SupervisorFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.SupervisorFiltering(query, e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
        return query;
      
      var document = _obj.DocumentsGroup.OfficialDocuments.SingleOrDefault();
      if (document != null && avis.CustomerRequests.CustomerRequests.Is(document))
      {
        query = query.Where(x => x.Login != null && x.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      }
      else if (Employees.Current.Department != null && Employees.Current.Department.BusinessUnit != null)
      {
        // Фильтруем по НОР, текущего пользователя.
        query = query.Where(q => q.Department.BusinessUnit != null && q.Department.BusinessUnit.Equals(Employees.Current.Department.BusinessUnit));
      }
      
      var employees = query.Select(x => Sungero.Company.Employees.As(x)).ToList();
      var notAuthomated = lenspec.EtalonDatabooks.PublicFunctions.Module.GetNotAutomatedEmployeesAvis(employees).Select(x => x.Id).ToList();
      if (notAuthomated != null && notAuthomated.Any())
        query = query.Where(x => !notAuthomated.Contains(x.Id));
      
      return query;
    }
  }

  partial class ActionItemExecutionTaskServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var document = _obj.DocumentsGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && avis.CustomerRequests.CustomerRequests.Is(document))
      {
        var roleSeeRequests = Roles.GetAll().SingleOrDefault(n => n.Sid == avis.CustomerRequests.PublicConstants.Module.RoleAccessToRequests);
        _obj.AccessRights.Grant(roleSeeRequests, DefaultAccessRightsTypes.Read);
      }
      
      base.BeforeSave(e);
    }

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      base.BeforeStart(e);
      
      var od = _obj.DocumentsGroup.OfficialDocuments.FirstOrDefault();
      
      if (_obj.OtherGroup.All.Any())
      {
        foreach (var att in _obj.OtherGroup.All)
        {
          var ed = Sungero.Content.ElectronicDocuments.As(att);
          if (ed != null)
          {
            if (!ed.Relations.GetRelatedDocuments().Any(x => x.Id == od.Id))
            {
              try
              {
                od.Relations.Add(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, ed);
              }
              catch (Exception ex)
              {
                e.AddError("Не создалась связь обращения с доп. документами: " + ex.Message);
              }
              
            }
          }
        }
        od.Relations.Save();
      }
    }
    
    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
        return;
      
      if (Users.Current.IsSystem != true && Employees.Current != null)
      {
        if (!Employees.Current.Equals(_obj.AssignedBy))
          _obj.AssignedBy = Employees.Current;
        
        if (_obj.State.Properties.Supervisor.IsEnabled)
          _obj.Supervisor = Employees.Current;
      }
    }
  }
  
  // Конец добавлено avis.
}