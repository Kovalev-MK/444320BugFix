using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Module.Company.Structures.Module;

namespace lenspec.Etalon.Module.Company.Server
{
  partial class ModuleFunctions
  {

    /// <summary>
    /// Вызываем асинхронный обработчик AsyncChangingApprovalsInISP.
    /// </summary>
    /// <param name="employeeCurrentId">ИД текущего исполнителя.</param>
    /// <param name="employeeNewId">ИД нового исполнителя.</param>
    /// <param name="roleId">Тип роли.</param>
    /// <param name="ourCFIds">ИД выбранных ИСП.</param>
    [Public]
    public static void AsyncChangingApprovalsInISPAsyns(string employeeCurrentId, string employeeNewId, string roleIds, string ourCFIds)
    {
      // Создать асинхронный обработчик .
      var asyncRightsHandler = avis.OurCFModule.AsyncHandlers.AsyncChangingApprovalsInISP.Create();
      // Заполнить параметры асинхронного обработчика.
      asyncRightsHandler.employeeCurrentId  = employeeCurrentId;
      asyncRightsHandler.employeeNewId      = employeeNewId;
      asyncRightsHandler.roleIds            = roleIds;
      asyncRightsHandler.ourCFIds           = ourCFIds;
      // Вызвать асинхронный обработчик.
      var startedMessage = "Изменения согласующих в ИСП началось.";
      var completeMessage = "Изменения согласующих в ИСП завершено";
      var errorMessage =" Изменения согласующих в ИСП завершено с ошибкой";
      asyncRightsHandler.ExecuteAsync(startedMessage, completeMessage, errorMessage, Users.Current);
    }

    /// <summary>
    /// Вызываем асинхронный обработчик DisablingAccounts.
    /// </summary>
    /// <param name="employee">Сотрудник которому отключают УЗ.</param>
    [Public]
    public static void DisablingAccountsAsync(Sungero.Company.IEmployee employee, string textTasks, string textAssignment, string taskAbort, string assignmentAbort, string substitutionsAbort)
    {
      // Создать асинхронный обработчик DisablingAccounts.
      var asyncRightsHandler = lenspec.AutomatedSupportTickets.AsyncHandlers.DisablingAccounts.Create();
      // Заполнить параметры асинхронного обработчика.
      asyncRightsHandler.userId = Sungero.CoreEntities.Users.Current.Id;
      asyncRightsHandler.employeeId = employee.Id;
      asyncRightsHandler.textTasks = textTasks;
      asyncRightsHandler.textAssignment = textAssignment;
      asyncRightsHandler.taskAbort = taskAbort;
      asyncRightsHandler.assignmentAbort = assignmentAbort;
      asyncRightsHandler.substitutionsAbort = substitutionsAbort;
      // Вызвать асинхронный обработчик.
      asyncRightsHandler.ExecuteAsync();
    }
    
    //Добавлено Avis Expert
    
    /// <summary>
    /// Вызов асинхронного обработчика для изменения настроек согласования.
    /// </summary>
    /// <param name="businessUnitIds">ИД выбранных в диалоге НОР.</param>
    /// <param name="regulationIds">ИД выбранных в диалоге регламентов.</param>
    /// <param name="actionType">Тип операции над настройками согласования.</param>
    /// <param name="status">Статус.</param>
    /// <param name="roleId">ИД выбранных в диалоге видов ролей.</param>
    /// <param name="currentPerfId">ИД текущего исполнителя.</param>
    /// <param name="newPerfId">ИД нового исполнителя.</param>
    [Remote(IsPure = true)]
    public static void ChangeApprovalSettingsExecuteAsync(string businessUnitIds, string regulationIds, string actionType, string status, string roleIds, long currentPerfId, long newPerfId)
    {
      var asyncHandler = lenspec.Etalon.Module.Company.AsyncHandlers.ChangeApprovalSettingslenspec.Create();
      asyncHandler.BusinessUnitIds = businessUnitIds;
      asyncHandler.RegulationIds = regulationIds;
      asyncHandler.ActionType = actionType;
      asyncHandler.Status = status;
      asyncHandler.RoleIds = roleIds;
      asyncHandler.CurrentPerformerId = currentPerfId;
      asyncHandler.NewPerformerId = newPerfId;
      asyncHandler.ExecuteAsync();
    }
    
    /// <summary>
    /// Удаляем все выполненные задания и прочитанные уведомления из папки входящие.
    /// </summary>
    /// <param name="employee">Сотрудник которому очищается папка.</param>
    [Public]
    public static void ClearInboxFolder(lenspec.Etalon.IEmployee employee)
    {
      // Получаем папку входящие.
      var inboxFolder = Sungero.Workflow.SpecialFolders.GetInbox(employee);
      
      // Проверяем все итемы.
      var deleteITems = new List<Sungero.Domain.Shared.IEntity>();
      foreach (var item in inboxFolder.Items)
      {
        // удаляем итем если он подходит под одно из...
        var assignment = Sungero.Workflow.Assignments.As(item);
        if (assignment != null && assignment.Status != Sungero.Workflow.Assignment.Status.InProcess)
          deleteITems.Add(item);
        
        var nitic = Sungero.Workflow.Notices.As(item);
        if (nitic != null && nitic.Status != Sungero.Workflow.Notice.Status.InProcess)
          deleteITems.Add(item);
        
        var reviewAssignment = Sungero.Workflow.ReviewAssignments.As(item);
        if (reviewAssignment != null && reviewAssignment.Status != Sungero.Workflow.ReviewAssignment.Status.InProcess)
          deleteITems.Add(item);
      }
      
      // Удаляем все ненужные итемы.
      foreach (var item in deleteITems)
        inboxFolder.Items.Remove(item);
      
      // Сохраняем.
      inboxFolder.Save();
    }
    
    /// <summary>
    /// Удаляем все выполненные задания и прочитанные уведомления из папки исходящие.
    /// </summary>
    /// <param name="employee">Сотрудник которому очищается папка.</param>
    [Public]
    public static void ClearOutboxFolder(lenspec.Etalon.IEmployee employee)
    {
      // Получаем папку исходящие.
      var outboxFolder = Sungero.Workflow.SpecialFolders.GetOutbox(employee);
      
      // Проверяем все итемы.
      var deleteITems = new List<Sungero.Domain.Shared.IEntity>();
      foreach (var item in outboxFolder.Items)
      {
        // удаляем итем если он подходит под одно из...
        var assignment = Sungero.Workflow.Assignments.As(item);
        if (assignment != null && assignment.Status != Sungero.Workflow.Assignment.Status.InProcess)
          deleteITems.Add(item);
        
        var nitic = Sungero.Workflow.Notices.As(item);
        if (nitic != null && nitic.Status != Sungero.Workflow.Notice.Status.InProcess)
          deleteITems.Add(item);
        
        var reviewAssignment = Sungero.Workflow.ReviewAssignments.As(item);
        if (reviewAssignment != null && reviewAssignment.Status != Sungero.Workflow.ReviewAssignment.Status.InProcess)
          deleteITems.Add(item);
      }
      
      // Удаляем все ненужные итемы.
      foreach (var item in deleteITems)
        outboxFolder.Items.Remove(item);
      
      // Сохраняем.
      outboxFolder.Save();
    }
    
    /// <summary>
    /// Создать новую учетную запись в системе.
    /// </summary>
    /// <param name="employeeAD">Выбранный пользователь из AD.</param>
    [Public]
    public static string CreateLogin(string employeeAD)
    {
      var settings = Etalon.Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.Constants.Module.ActiveDirectoryEmployeeRecordCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения.");
      
      // Инициализируем helper для получения данных из интеграционной базы.
      var settingsString = Encryption.Decrypt(settings.ConnectionParams);
      var connectionString = settingsString.Split('|');
      if (string.IsNullOrEmpty(connectionString[0]))
        throw AppliedCodeException.Create("Ошибка подключения к базе данных.");
      
      var employeeADRepositor = new AvisIntegrationHelper.Repositories.EmployeeADRepository(connectionString[0]);
      
      // Получаем логин выбранного пользователя.
      var employeeAdId = employeeAD.Split('-')[0];
      var employeeAd = employeeADRepositor.GetById(Convert.ToInt32(employeeAdId), true);
      
      // Создаём новую УЗ.
      var login = Sungero.CoreEntities.Logins.Create();
      login.LoginName = employeeAd.Login;
      login.Save();
      return string.Format("Учетная запись {0} создана.", employeeAd.Login);
    }

    /// <summary>
    /// Создать Заявку на создание НОР.
    /// </summary>
    /// <returns>Новая Заявка на создание НОР.</returns>
    [Remote]
    public static EtalonDatabooks.IApplicationBUCreationDocument CreateApplictionBUCreation()
    {
      return EtalonDatabooks.ApplicationBUCreationDocuments.Create();
    }
    
    /// <summary>
    /// Создать Заявку на изменение НОР.
    /// </summary>
    /// <returns>Новая Заявка на изменение НОР.</returns>
    [Remote]
    public static EtalonDatabooks.IApplicationBUEditingDocument CreateApplictionBUEditing()
    {
      return EtalonDatabooks.ApplicationBUEditingDocuments.Create();
    }

    #region Рассылка уведомлений
    
    /// <summary>
    /// Формирование данных для уведомлений, попытка отправить их.
    /// </summary>
    /// <param name="employeeIds">Список идентификаторов сотрудников, которые должны быть отправлены уведомлением.</param>
    /// <param name="notifyParams">Параметры уведомления в таблице в БД.</param>
    [Public]
    public void SendExpiringEmployeesNotifications(List<long> employeeIds,
                                                   Etalon.Module.Company.Structures.Module.IExpiringEmployeesNotificationParams notifyParams,
                                                   List<IUser> performers)
    {
      var employeePart = Etalon.Employees.GetAll(e => employeeIds.Contains(e.Id)).ToList();
      if (!employeePart.Any())
        return;
      PublicFunctions.Module.ClearIdsFromExpiringEmployeesTable(notifyParams.ExpiringEmployeesTableName, employeePart.Select(e => e.Id).ToList());
      PublicFunctions.Module.AddExpiringEmployeesToTable(notifyParams.ExpiringEmployeesTableName, employeePart.Select(e => e.Id).ToList());
      
      var subject = Sungero.Docflow.PublicFunctions.Module.TrimQuotes(lenspec.Etalon.Module.Company.Resources.CivilLawContractExpires);
      if (subject.Length > Sungero.Workflow.SimpleTasks.Info.Properties.Subject.Length)
        subject = subject.Substring(0, Sungero.Workflow.SimpleTasks.Info.Properties.Subject.Length);
      
      var employeeNames = string.Empty;
      for(int i = 1; i <= employeePart.Count; i++)
      {
        employeeNames += "\r\n" + i + ". " + employeePart[i - 1].Name;
      }
      var activeText = Sungero.Docflow.PublicFunctions.Module.TrimQuotes(string.Format(lenspec.Etalon.Module.Company.Resources.CivilLawContractIsExpiringForEmployeeList, employeeNames));
      
      var attachments = new List<Etalon.IEmployee>() { };
      attachments.AddRange(employeePart);
      
      notifyParams.TaskParams.Subject = subject;
      notifyParams.TaskParams.ActiveText = activeText;
      notifyParams.TaskParams.Performers = performers;
      notifyParams.TaskParams.Attachments = attachments;
      PublicFunctions.Module.TrySendExpiringEmployeesNotifications(notifyParams);
    }
    
    /// <summary>
    /// Получить параметры по умолчанию для рассылки уведомлений по сотрудникам.
    /// </summary>
    /// <param name="lastNotificationParamName">Имя параметра в Sungero_Docflow_Params с датой последнего уведомления.</param>
    /// <param name="noticesTableName">Имя таблицы, в которой содержится информация об уведомлениях.</param>
    /// <returns>Параметры для рассылки уведомлений по сотрудникам.</returns>
    [Public]
    public IExpiringEmployeesNotificationParams GetDefaultExpiringCivilContractsNotificationParams(string lastNotificationParamName, string noticesTableName)
    {
      var param = ExpiringEmployeesNotificationParams.Create();
      param.LastNotification = Sungero.Docflow.PublicFunctions.Module.GetLastNotificationDate(lastNotificationParamName, null);
      param.LastNotificationReserve = param.LastNotification.AddDays(-Constants.Module.ExpiringEmployeesDaysBeforeClosing);
      param.Today = Calendar.Today;
      param.TodayReserve = param.Today.AddDays(Constants.Module.ExpiringEmployeesDaysBeforeClosing);
      param.ExpiringEmployeesTableName = noticesTableName;
      param.LastNotificationParamName = lastNotificationParamName;
      param.TaskParams = ExpiringNotificationTaskParams.Create();
      return param;
    }
    
    /// <summary>
    /// Получить Id сотрудников, по которым уже отправлены уведомления.
    /// </summary>
    /// <param name="expiringDocumentTableName">Имя таблицы, в которой хранятся Id сотрудников для отправки уведомлений.</param>
    /// <returns>Список Id сотрудников, по которым задачи уже отправлены.</returns>
    [Public]
    public static List<int> GetEmployeesWithSendedTask(string expiringEmployeesTableName)
    {
      var result = new List<int>();
      var commandText = string.Format(Queries.Module.SelectEmployeesWithSendedTask, expiringEmployeesTableName);
      using (var command = SQL.GetCurrentConnection().CreateCommand())
      {
        try
        {
          command.CommandText = commandText;
          using (var rdr = command.ExecuteReader())
          {
            while (rdr.Read())
              result.Add(rdr.GetInt32(0));
          }
          return result;
        }
        catch (Exception ex)
        {
          Logger.Error("Error while getting array of employees with sent tasks", ex);
          return result;
        }
      }
    }
    
    /// <summary>
    /// Убрать из таблицы для отправки Id сотрудников.
    /// </summary>
    /// <param name="expiringDocsTableName">Имя таблицы для отправки уведомлений.</param>
    /// <param name="ids">Id сотрудников.</param>
    [Public]
    public static void ClearIdsFromExpiringEmployeesTable(string expiringEmployeesTableName, List<long> ids)
    {
      if (string.IsNullOrWhiteSpace(expiringEmployeesTableName))
        return;
      if (!ids.Any())
        return;
      string command = string.Empty;
      command = string.Format(Queries.Module.DeleteEmployeeIdsWithoutTask, expiringEmployeesTableName, string.Join(", ", ids));
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand(command);
    }
    
    /// <summary>
    /// Записать Id сотрудников в таблицу для отправки.
    /// </summary>
    /// <param name="expiringDocsTableName">Имя таблицы для отправки уведомлений.</param>
    /// <param name="ids">Id сотрудников.</param>
    [Public]
    public static void AddExpiringEmployeesToTable(string expiringEmployeesTableName, List<long> ids)
    {
      if (string.IsNullOrWhiteSpace(expiringEmployeesTableName))
        return;
      if (!ids.Any())
        return;
      var command = string.Format(Queries.Module.AddExpiringEmployeesToTable, expiringEmployeesTableName, string.Join("), (", ids));
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand(command);
    }
    
    /// <summary>
    /// Добавить в таблицу для отправки задачу, с указанием сотрудника.
    /// </summary>
    /// <param name="expiringDocsTableName">Имя таблицы для отправки уведомлений.</param>
    /// <param name="employee">Сотрудник.</param>
    /// <param name="task">Задача, которая была запущена.</param>
    [Public]
    public static void AddTaskToExpiringTable(string expiringEmployeesTableName, long employee, long task)
    {
      if (string.IsNullOrWhiteSpace(expiringEmployeesTableName))
        return;
      var command = string.Format(Queries.Module.AddTaskExpiringEmployeesTable, expiringEmployeesTableName, employee, task);
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand(command);
      Logger.DebugFormat("Task {0} for employee {1} started and marked in db.", task, employee);
    }
    
    /// <summary>
    /// Обновить дату последней рассылки уведомлений.
    /// </summary>
    /// <param name="notificationParams">Параметры уведомлений.</param>
    [Public]
    public static void UpdateLastNotificationDate(IExpiringEmployeesNotificationParams notificationParams)
    {
      var newDate = notificationParams.Today.ToString("yyyy-MM-dd HH:mm:ss");
      var command = string.Format(Queries.Module.UpdateLastExpiringNotificationDate,
                                  notificationParams.LastNotificationParamName,
                                  newDate);
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommand(command);
      Logger.DebugFormat("Last notification date is set to {0}", newDate);
    }
    
    /// <summary>
    /// Попытаться отправить уведомления по сотруднику, срок ГПД которого истекает.
    /// </summary>
    /// <param name="notificationParams">Параметры уведомления.</param>
    [Public]
    public static void TrySendExpiringEmployeesNotifications(IExpiringEmployeesNotificationParams notificationParams)
    {
      if (notificationParams.TaskParams.Attachments == null)
      {
        Logger.DebugFormat("Has no employees to notify.");
        return;
      }
      
      var performers = notificationParams.TaskParams.Performers;
      var subject = notificationParams.TaskParams.Subject;
      var activeText = notificationParams.TaskParams.ActiveText;
      var attachments = notificationParams.TaskParams.Attachments;
      var employeeIds = attachments.Select(a => a.Id).ToList();
      
      var logEmployeeIds = string.Join(", ", attachments.Select(a => a.Id.ToString()).ToList());
      var employeeLogView = string.Format("Employee Ids: {0}", logEmployeeIds);
      
      if (performers == null || !performers.Any())
      {
        foreach(var employeeId in employeeIds)
        {
          PublicFunctions.Module.AddTaskToExpiringTable(notificationParams.ExpiringEmployeesTableName, employeeId, 0);
          Logger.DebugFormat("{0} has no performers to notify.", employeeLogView);
        }
      }
      
      try
      {
        var newTask = Sungero.Workflow.SimpleTasks.CreateWithNotices(subject, performers, attachments.ToArray());
        newTask.NeedsReview = false;
        newTask.ActiveText = activeText;
        
        var logPerformersIds = string.Join(", ", performers.Select(p => p.Id.ToString()).ToList());
        Logger.DebugFormat("Notice prepared to start with parameters: Employee Ids '{0}', subject length {1}, performers Ids '{2}', active text length {3}",
                           logEmployeeIds, subject.Length, logPerformersIds, activeText.Length);
        
        var users = new List<Sungero.CoreEntities.IUser>() { newTask.Author,  newTask.StartedBy };
        
        foreach (var user in users)
        {
          if (user == null)
          {
            Logger.Debug("User is null");
            continue;
          }
          
          Logger.DebugFormat("Access rights check to change the outbox for user with id {0}", user.Id);
          
          var outbox = Sungero.Workflow.SpecialFolders.GetOutbox(user);
          if (outbox != null)
            Logger.DebugFormat("Outbox for user with id {0} exists", user.Id);
          
          if (outbox.AccessRights.CanChangeFolderContent())
            Logger.DebugFormat("User with id {0} has access rights to change his outbox", user.Id);
        }
        
        newTask.Start();
        Logger.DebugFormat("Notice with Id '{0}' has been started", newTask.Id);
        
        foreach(var employeeId in employeeIds)
        {
          PublicFunctions.Module.AddTaskToExpiringTable(notificationParams.ExpiringEmployeesTableName, employeeId, newTask.Id);
        }
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("{0} notification failed. {1}", employeeLogView, ex.Message);
      }
    }
    
    #endregion
    
    #region [Замена категории персон]
    
    /// <summary>
    /// Замена категории персон с "Клиенты (покупатели, дольщики)" на "Клиенты (собственники)".
    /// </summary>
    /// <param name="objectsAnProjects">Объекты проектов.</param>
    /// <param name="ourCFs">ИСП.</param>
    [Remote]
    public static void ChangingPersonsRequisites(
      List<lenspec.EtalonDatabooks.IObjectAnProject> objectsAnProjects,
      List<lenspec.EtalonDatabooks.IOurCF> ourCFs)
    {
      // Отфильтрованные по ИСП/Объектам проектов клиентские договоры.
      var contracts = lenspec.SalesDepartmentArchive.PublicFunctions.SDAClientContract
        .GetContracts(objectsAnProjects, ourCFs);
      
      // Отбираем персон из полей договоров для смены категории.
      foreach (var contract in contracts)
        foreach (Sungero.Parties.ICounterparty client in contract.CounterpartyClient.Select(c => c.ClientItem))
          if (lenspec.Etalon.People.Is(client))
            ChangingPersonRequisites(objectsAnProjects, ourCFs, lenspec.Etalon.People.As(client));
    }
    
    /// <summary>
    /// Замена категории персоны с "Клиенты (покупатели, дольщики)" на "Клиенты (собственники)".
    /// </summary>
    /// <param name="objectsAnProjects">Объекты проектов.</param>
    /// <param name="ourCFs">ИСП.</param>
    /// <param name="person">Персона.</param>
    private static void ChangingPersonRequisites(
      List<lenspec.EtalonDatabooks.IObjectAnProject> objectsAnProjects,
      List<lenspec.EtalonDatabooks.IOurCF> ourCFs,
      lenspec.Etalon.IPerson person)
    {
      // Отбор персон с признаком "Клиенты (покупатели, дольщики)".
      if (person.IsClientBuyersavis != true)
        return;
      
      /* Если есть хотя бы 1 документ, у которого
       * ИСП/Группа объекта != ИСП/Группа объекта
       * из диалогового окна, то признак
       * Клиенты(покупатели, дольщики) не изменять.
       */
      var isClientBuyer = false;
      var personContracts = lenspec.SalesDepartmentArchive.PublicFunctions.SDAClientContract
        .GetContracts(person);
      foreach (var contract in personContracts)
      {
        var objectAnProject = contract.ObjectAnProject;
        if (objectAnProject == null)
          break;
        
        if (
          !objectsAnProjects.Contains(objectAnProject) &&
          (objectAnProject.OurCF != null && !ourCFs.Contains(objectAnProject.OurCF)))
        {
          // Не изменяем признак.
          isClientBuyer = person.IsClientBuyersavis.Value;
          break;
        }
      }
      
      var asyncHandler = AsyncHandlers.AsyncChangingPersonsRequisiteslenspec.Create();
      asyncHandler.IsClientBuyer = isClientBuyer;
      asyncHandler.IsClientOwners = true;
      asyncHandler.PersonId = person.Id;
      asyncHandler.InitiatorId = Users.Current.Id;
      
      asyncHandler.ExecuteAsync();
    }
    
    #endregion [Замена категории персон]
    
    //конец Добавлено Avis Expert.
    
    [Public]
    public static List<Sungero.Company.IDepartment> GetSubDepartments(Sungero.Company.IDepartment department)
    {
      var departments = new List<Sungero.Company.IDepartment>();
      
      var subDepartments = Sungero.Company.Departments.GetAll().Where(x => x.HeadOffice != null && x.HeadOffice.Equals(department)).ToList();
      departments.AddRange(subDepartments);
      foreach(var subDep in subDepartments)
      {
        departments.AddRange(GetSubDepartments(subDep));
      }
      
      return departments;
    }
  }
}