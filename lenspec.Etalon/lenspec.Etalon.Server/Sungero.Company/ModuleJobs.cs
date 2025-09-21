using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Company.Server
{
  partial class ModuleJobs
  {

    /// <summary>
    /// Эталон. Компания. Удаление дублей сотрудников из подразделений.
    /// </summary>
    public virtual void RemovingDuplicateEmployeeslenspec()
    {
      var departments = lenspec.Etalon.Departments.GetAll(d => Equals(d.Status, Sungero.Company.Department.Status.Active));
      foreach (var department in departments)
      {
        var isForcedLocked = false;
        try
        {
          isForcedLocked = Locks.TryLock(department);
          if (isForcedLocked)
            RemoveDuplicateEmployees(department);
          else
            Logger.DebugFormat("avis – RemovingDuplicateEmployeeslenspec - Подразделение '{0}' заблокировано.", department.Name);
        }
        finally
        {
          if (isForcedLocked)
            Locks.Unlock(department);
        }
      }
    }

    /// <summary>
    /// Удаление дублей сотрудников из подразделения.
    /// </summary>
    /// <param name="department">Подразделение.</param>
    private static void RemoveDuplicateEmployees(Sungero.Company.IDepartment department)
    {
      var employees = department.RecipientLinks.Select(x => Sungero.Company.Employees.As(x.Member));
      var uniqueEmployees = new HashSet<Sungero.Company.IEmployee>();
      var count = 0;
      
      // Подсчитываем всех сотрудников против уникальных сотрудников.
      foreach (var employee in employees)
      {
        count++;
        uniqueEmployees.Add(employee);
      }
      
      // Если все сотрудники уникальны, пропускаем итерацию.
      if (uniqueEmployees.Count == count)
        return;
      
      // Перезаписываем состав подразделения.
      department.RecipientLinks.Clear();
      foreach (var employee in uniqueEmployees)
      {
        var row = department.RecipientLinks.AddNew();
        row.Member = employee;
      }
      department.Save();
    }
    
    /// <summary>
    /// Эталон. Компания. Снятие галочек о Сотрудниках ГК.
    /// </summary>
    public virtual void RemovingCheckboxIsEmployeeGKlenspec()
    {
      try
      {
        var employeesGK = lenspec.Etalon.People.GetAll(x => x.IsEmployeeGKavis == true).AsEnumerable();
        var activeEmployees = Sungero.Company.Employees.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
        foreach (var person in employeesGK)
        {
          if (activeEmployees.Any(x => x.Person.Id == person.Id))
            continue;
          
          var asyncHandler = lenspec.Etalon.Module.Company.AsyncHandlers.AsyncUpdatingPersonOfFiredEmployeelenspec.Create();
          asyncHandler.PersonId = person.Id;
          asyncHandler.IsFillNote = false;
          asyncHandler.ExecuteAsync();
        }
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - RemovingCheckboxIsEmployeeGKlenspec - ", ex);
      }
    }

    //Добавлено Avis Expert
    /// <summary>
    /// Эталон. Компания. Перенос закрытых записей сотрудников в «Закрытые сотрудники».
    /// </summary>
    public virtual void MovingClosedEmployeeslenspec()
    {
      Logger.Debug("Avis - MovingClosedEmployeeslenspec запущен.");
      try
      {
        var constIdDepartment = lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.DepartmentForClosedEmployees).FirstOrDefault();
        if (constIdDepartment == null || string.IsNullOrEmpty(constIdDepartment.Value))
          throw new Exception("заполните константу для ИД подразделения.");
        
        var idDepartment = Convert.ToInt32(constIdDepartment.Value);
        var departmentForClosedEmployees = Sungero.Company.Departments.GetAll(x => x.Id == idDepartment).SingleOrDefault();
        if (departmentForClosedEmployees == null)
          throw new Exception($"не удалось найти подразделение {idDepartment}.");
        
        var closedEmployees = Sungero.Company.Employees
          .GetAll(x =>
                  x.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed &&
                  x.Department != departmentForClosedEmployees)
          .AsEnumerable();
        
        foreach (var employee in closedEmployees)
        {
          var asyncHandler = Company.AsyncHandlers.AsyncMovingClosedEmployeeslenspec.Create();
          asyncHandler.DepartmentId = idDepartment;
          asyncHandler.EmployeeId = employee.Id;
          asyncHandler.ExecuteAsync();
        }
        Logger.Debug("Avis - MovingClosedEmployeeslenspec завершен.");
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - MovingClosedEmployeeslenspec - ", ex);
      }
    }

    /// <summary>
    /// Эталон. Компания. Привязка учетной записи к актуальной записи сотрудника
    /// </summary>
    public virtual void LinkingAccountToCurrentEmployeelenspec()
    {
      // Создать асинхронный обработчик GrantAccessRightsToDocument.
      var asyncLinkingAccountToCurrentEmpl = lenspec.Etalon.Module.Company.AsyncHandlers.LinkingAccountToCurrentEmployeelenspec.Create();
      
      // Вызвать асинхронный обработчик.
      asyncLinkingAccountToCurrentEmpl.ExecuteAsync();
    }

    /// <summary>
    /// Эталон. Очистка папки Входящие/Исходящие от выполненных заданий и прочитанных уведомлений.
    /// </summary>
    public virtual void ClearEmployeeFoldersavis()
    {
      Logger.Debug("Avis - ClearEmployeeFoldersavis запущен.");
      var employees = lenspec.Etalon.Employees.GetAll();
      
      foreach (var employee in employees)
      {
        // Удаляем все выполненные задания и прочитанные уведомления из папки входящие.
        Etalon.Module.Company.PublicFunctions.Module.ClearInboxFolder(employee);
        // Удаляем все выполненные задания и прочитанные уведомления из папки исходящие.
        Etalon.Module.Company.PublicFunctions.Module.ClearOutboxFolder(employee);
      }
      Logger.Debug("Avis - ClearEmployeeFoldersavis завершен.");
    }

    /// <summary>
    /// Закрытие замещений, в которых замещающий сотрудник имеет закрытую УЗ.
    /// </summary>
    public virtual void CloseSubstitutionByLoginavis()
    {
      Logger.Debug("Avis - CloseSubstitutionByLoginavis запущен.");
      try
      {
        var employeesWithClosedLogin = Sungero.Company.Employees.GetAll(x => x.Login != null && x.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed);
        foreach(var employee in employeesWithClosedLogin)
        {
          var substitutions = Sungero.CoreEntities.Substitutions.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active &&
                                                                        x.Substitute.Equals(employee) &&
                                                                        (!x.EndDate.HasValue || x.EndDate.Value > Calendar.Today));
          foreach(var substitutionToClose in substitutions)
          {
            try
            {
              var substitution = Sungero.CoreEntities.Substitutions.Get(substitutionToClose.Id);
              substitution.EndDate = Calendar.Today;
              substitution.Save();
            }
            catch(Exception ex)
            {
              Logger.ErrorFormat("Avis - CloseSubstitutionByLoginavis - не удалось сохранить изменения в замещении {0} - ", ex, substitutionToClose.Id);
            }
          }
          
          #region Логика для автоматического выполнения задания по истечении срока
          
          // Создать асинхронный обработчик CompleteCollegialBodiesMembersTasks.
          var asyncRightsHandler          = lenspec.AutomatedSupportTickets.AsyncHandlers.CompleteCollegialBodiesMembersTasks.Create();
          // Заполнить параметры асинхронного обработчика.
          asyncRightsHandler.ClosingDate  = Calendar.Today;
          asyncRightsHandler.EmployeeId   = employee.Id;
          // Вызвать асинхронный обработчик.
          asyncRightsHandler.ExecuteAsync();
          
          #endregion
        }
        Logger.Debug("Avis - CloseSubstitutionByLoginavis завершен.");
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - CloseSubstitutionByLoginavis - ", ex);
      }
    }

    /// <summary>
    /// Закрытие учетных записей, которые не привязаны к действующим сотрудникам.
    /// </summary>
    public virtual void CloseLoginsWithoutEmployeeavis()
    {
      Logger.Debug("Avis - CloseLoginsWithoutEmployeeavis запущен.");
      try
      {
        // Отобрать УЗ системных пользователей.
        var systemLoginIds = Sungero.CoreEntities.Users.GetAll(x => x.IsSystem == true && x.Login != null).Select(x => x.Login.Id).ToList();
        var activeEmployees = Sungero.Company.Employees.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && x.Login != null);
        var loginsToClose = Sungero.CoreEntities.Logins.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active &&
                                                               !activeEmployees.Any(emp => emp.Login.Equals(x)));
        if (systemLoginIds != null && systemLoginIds.Any())
        {
          loginsToClose = loginsToClose.Where(x => !systemLoginIds.Contains(x.Id));
        }
        foreach(var login in loginsToClose)
        {
          var loginToClose = Sungero.CoreEntities.Logins.Get(login.Id);
          loginToClose.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
          loginToClose.Save();
        }
        Logger.Debug("Avis - CloseLoginsWithoutEmployeeavis завершен.");
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - CloseLoginsWithoutEmployeeavis - ", ex);
      }
    }

    /// <summary>
    /// Создание замещений для сотрудников, работающих по совместительству.
    /// </summary>
    public virtual void AutomaticCreationOfSubstitutionsavis()
    {
      Logger.Debug("Avis - AutomaticCreationOfSubstitutionsavis запущен.");
      try
      {
        var activeEmployees = Etalon.Employees
          .GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && x.Person != null && x.Email != null && x.Email != string.Empty)
          .ToList()
          .GroupBy(x => new Company.Structures.Module.EmployeeLINQParams { Person = x.Person, Email = x.Email });
        var activeSubstitutions = Sungero.CoreEntities.Substitutions.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active &&
                                                                            (!x.EndDate.HasValue || x.EndDate.Value >= Calendar.Today) &&
                                                                            x.IsSystem != true);
        foreach(var employees in activeEmployees)
        {
          var employeeWithMainPlaceOfWork = employees.Where(x => x.Login != null && x.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Active &&
                                                            x.PlaceOfWorkavis != null && x.PlaceOfWorkavis.Code1C == 0);
          var withOtherPlaceOfWork = employees.Where(x => x.PlaceOfWorkavis != null && x.PlaceOfWorkavis.Code1C != 0 ||
                                                     (x.Login == null || x.Login != null && x.Login.Status != Sungero.CoreEntities.DatabookEntry.Status.Active) &&
                                                     x.PlaceOfWorkavis != null && x.PlaceOfWorkavis.Code1C == 0);
          // Для каждой записи с основным местом работы проверить замещение записи по совместительству.
          foreach(var withMainPlace in employeeWithMainPlaceOfWork)
          {
            foreach(var substitutedUser in withOtherPlaceOfWork)
            {
              var substitutionIsExists = activeSubstitutions.Where(x => x.Substitute.Equals(withMainPlace) && x.User.Equals(substitutedUser)).Any();
              if (!substitutionIsExists)
              {
                CreateSubstitutionByPlaceOfWork(withMainPlace, substitutedUser);
              }
            }
          }
          // Для каждой записи с основным местом работы проверить замещение другой записи с основным местом работы (каждый с каждым).
          foreach(var substitute in employeeWithMainPlaceOfWork)
          {
            foreach(var substitutedUser in employeeWithMainPlaceOfWork)
            {
              // Исключить замещения записи сотрудника на самого себя.
              if (substitute.Equals(substitutedUser))
              {
                continue;
              }
              var substitutionIsExists = activeSubstitutions.Where(x => x.Substitute.Equals(substitute) && x.User.Equals(substitutedUser)).Any();
              if (!substitutionIsExists)
              {
                CreateSubstitutionByPlaceOfWork(substitute, substitutedUser);
              }
            }
          }
        }
        Logger.Debug("Avis - AutomaticCreationOfSubstitutionsavis завершен.");
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - AutomaticCreationOfSubstitutionsavis - ", ex);
      }
    }
    
    private void CreateSubstitutionByPlaceOfWork(Etalon.IEmployee substitute, Etalon.IEmployee substitutedUser)
    {
      try
      {
        var substitution = Sungero.CoreEntities.Substitutions.Create();
        substitution.Substitute = substitute;
        substitution.User = substitutedUser;
        substitution.Save();
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - AutomaticCreationOfSubstitutionsavis - не удалось создать замещение: {0} за {1}", ex, substitute.Id, substitutedUser.Id);
      }
    }

    /// <summary>
    /// Агент для закрытия учетных записей сотрудников, у которых истек срок срок действия ГПД.
    /// </summary>
    public virtual void CloseExpiringLoginslenspec()
    {
      Logger.Debug("Avis - CloseExpiringLoginslenspec запущен.");
      // Вычислить учетные записи сотрудников, которые необходимо закрыть.
      List<Sungero.CoreEntities.ILogin> logins = Etalon.Employees.GetAll()
        .Where(e => e.Status == Etalon.Employee.Status.Active)
        .Where(e => e.ContractValidTilllenspec.HasValue && e.ContractValidTilllenspec < Calendar.Today)
        .Select(e => e.Login)
        .Where(l => l != null && l.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
        .ToList();
      Logger.DebugFormat("Powers of Logins to closing count = {0}.", logins.Count());
      
      foreach(var login in logins)
      {
        try
        {
          // Перевести запись в статус Закрытая.
          login.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
          login.Save();
        }
        catch(Exception ex)
        {
          Logger.ErrorFormat("Error closing login with Id={0} - ", ex, login.Id);
        }
      }
      Logger.Debug("Avis - CloseExpiringLoginslenspec завершен.");
    }

    /// <summary>
    /// Агент рассылки уведомления об окончании срока действия ГПД сотрудника.
    /// </summary>
    public virtual void SendNotificationForExpiringEmployeeslenspec()
    {
      Logger.Debug("Avis - SendNotificationForExpiringEmployeeslenspec запущен.");
      try
      {
        // Вычисляем учетные записи сотрудников, которым надо отправить уведомление об окочании срока ГПХ.
        var employeesNotification = Etalon.Employees.GetAll(e =>
                                                            e.Status == Employee.Status.Active &&
                                                            e.CivilLawContractlenspec == true &&
                                                            e.IsGPHavis == false &&
                                                            e.ContractValidTilllenspec <= Calendar.Now.AddDays(14));
        
        foreach(var employee in employeesNotification)
        {
          // Отправляем задачу.
          var task = lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTasks.Create();
          task.Employee = employee;
          //task.Attachments.Add(employee);
          task.Start();
          
          // Устанавливаем признак отправлено уведомление ГПХ и сохраняем.
          employee.IsGPHavis = true;
          employee.Save();
        }
        
        // Завершение задачи сотрудников у кого срок гпх меньше 1 дня.
        // Получаем список заданий сотруднику ГПХ.
        var assigments = lenspec.AutomatedSupportTickets.PersonnelDepartmentResponsibleAssignments.GetAll(p => p.Subject == "Закройте все невыполненные задания и задачи в них." && p.Status == Sungero.Workflow.Assignment.Status.InProcess);
        
        foreach (var assigment in assigments)
        {
          // Получаем основное задание и проверяем срок гпх у сотрудника.
          var task = lenspec.AutomatedSupportTickets.ExpirationOfGPHAnEmployeeTasks.As(assigment.Task);
          if (task == null)
            continue;
          
          // Завершаем все задания у сотрудников, если срок меньше 1 дня.
          var checkDate = Calendar.Now.AddDays(1);
          if (task.Employee.ContractValidTilllenspec <= checkDate)
          {
            assigment.ActiveText = "Закончился срок выполнения задания";
            assigment.Abort();
          }
        }
        /*
         * Отсылка задания администратору СЭД за 3 дня до конца гпх сотрудника.
         * 
        // Создать таблицу соответствия Сотрудник-Задача.
        var createTableCommand = Queries.Module.CreateTableForExpiringEmployees;
        Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand(createTableCommand);
        
        // Заполнить параметры для рассылки уведомлений.
        var notifyParams = PublicFunctions.Module.GetDefaultExpiringCivilContractsNotificationParams(Constants.Module.NotificationDatabaseKey,
                                                                                                     Constants.Module.ExpiringEmployeesTableName);
        // Вычислить сотрудников, карточки которых уже были отправлены уведомлением.
        var alreadySentEmployees = PublicFunctions.Module.GetEmployeesWithSendedTask(notifyParams.ExpiringEmployeesTableName);
        
        // Вычислить сотрудников, карточки которых необходимо отправить уведомлением.
        var employeeIds = Etalon.Employees.GetAll()
          .Where(e => !alreadySentEmployees.Contains(e.Id))
          .Where(e => e.Status == Etalon.Employee.Status.Active && e.ContractValidTilllenspec.HasValue && notifyParams.Today.AddDays(Constants.Module.ExpiringEmployeesDaysBeforeClosing) == e.ContractValidTilllenspec)
          .Select(e => e.Id)
          .ToList();
        Logger.DebugFormat("Info - SendNotificationForExpiringEmployeeslenspec - Powers of Employee to send notification count = {0}.", employeeIds.Count());
        
        // Вычислить получателей уведомлений (участников роли Администратор СЭД).
        var performers = new List<IUser>() { };
        var adminEDMS = Roles.GetAll().Where(n => n.Sid == EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
        if (adminEDMS != null)
        {
          var recipients = Roles.GetAllUsersInGroup(adminEDMS);
          if (recipients.Any())
          {
            performers.AddRange(recipients);
          }
        }

        var result = Sungero.Core.Transactions.Execute(() => PublicFunctions.Module.SendExpiringEmployeesNotifications(employeeIds, notifyParams, performers));
        
        if (Sungero.Docflow.PublicFunctions.Module.IsAllNotificationsStarted(notifyParams.ExpiringEmployeesTableName))
        {
          PublicFunctions.Module.UpdateLastNotificationDate(notifyParams);
          Sungero.Docflow.PublicFunctions.Module.ClearExpiringTable(notifyParams.ExpiringEmployeesTableName, false);
        }
         */
        Logger.Debug("Avis - SendNotificationForExpiringEmployeeslenspec завершен.");
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - SendNotificationForExpiringEmployeeslenspec - ", ex);
      }
    }
    //конец Добавлено Avis Expert

  }
}