using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Company.Server
{
  partial class ModuleAsyncHandlers
  {

    public virtual void AsyncUpdatingPersonOfFiredEmployeelenspec(lenspec.Etalon.Module.Company.Server.AsyncHandlerInvokeArgs.AsyncUpdatingPersonOfFiredEmployeelenspecInvokeArgs args)
    {
      Logger.DebugFormat("Avis - AsyncUpdatingPersonOfFiredEmployeelenspec обработка персоны {0}.", args.PersonId);
      
      var person = lenspec.Etalon.People.GetAll(x => x.Id == args.PersonId).SingleOrDefault();
      if (person == null)
      {
        Logger.ErrorFormat("Avis - AsyncUpdatingPersonOfFiredEmployeelenspec - не удалось найти персону {0}.", args.PersonId);
        args.Retry = false;
        return;
      }
      
      var activeEmployees = Sungero.Company.Employees.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && 
                                                             args.PersonId == x.Person.Id);
      if (activeEmployees.Any())
      {
        Logger.ErrorFormat("Avis - AsyncUpdatingPersonOfFiredEmployeelenspec - есть действующие записи сотрудников с персоной {0}.", args.PersonId);
        args.Retry = false;
        return;
      }
      
      var persLockInfo = Locks.GetLockInfo(person);
      if (persLockInfo.IsLocked)
      {
        args.Retry = true;
        return;
      }
      
      var persIsLocked = false;
      try
      {
        persIsLocked = Locks.TryLock(person);
        if (persIsLocked)
        {
          person.IsEmployeeGKavis = false;
          if (person.IsLawyeravis != true && person.IsOtheravis != true && person.IsClientBuyersavis != true && person.IsClientOwnersavis != true)
          {
            person.IsOtheravis = true;
            if (args.IsFillNote)
              person.Note += lenspec.Etalon.Module.Company.Resources.EmployeeIsAutomaticallyFired;
          }
          person.Save();
        }
        else
        {
          Logger.DebugFormat("Avis - AsyncUpdatingPersonOfFiredEmployeelenspec - не удалось заблокировать персону {0}.", person.Id);
          args.Retry = true;
          return;
        }
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - AsyncUpdatingPersonOfFiredEmployeelenspec - ", ex);
      }
      finally
      {
        if (persIsLocked)
          Locks.Unlock(person);
      }
    }

    /// <summary>
    /// Замена категории персон с "Клиенты (покупатели, дольщики)" на "Клиенты (собственники)".
    /// </summary>
    public virtual void AsyncChangingPersonsRequisiteslenspec(lenspec.Etalon.Module.Company.Server.AsyncHandlerInvokeArgs.AsyncChangingPersonsRequisiteslenspecInvokeArgs args)
    {
      const string LogPrefix = "Avis - AsyncChangingPersonsRequisiteslenspec -";
      Logger.DebugFormat("{0} замена категории персоны {1}.", LogPrefix, args.PersonId);
      
      // Проверка наличия персоны.
      var person = lenspec.Etalon.People.GetAll(p => p.Id == args.PersonId).FirstOrDefault();
      if (person == null)
      {
        var message = string.Format("{0} не удалось найти персону {1}.", LogPrefix, args.PersonId);
        ProceedError(args, false, message, person);
        return;
      }
      
      // Проверка блокировки сущности.
      if (Locks.GetLockInfo(person).IsLocked)
      {
        var message = string.Format("{0} запись персоны {1} заблокирована.", LogPrefix, args.PersonId);
        ProceedError(args, true, message);
        return;
      }
      
      // Попытка изменения информации о персоне.
      var isForcedLocked = false;
      try
      {
        isForcedLocked = Locks.TryLock(person);
        if (isForcedLocked)
        {
          person.IsClientBuyersavis = args.IsClientBuyer;
          person.IsClientOwnersavis = args.IsClientOwners;
          person.Save();
        }
        else
        {
          var message = string.Format("{0} не удалось установить блокировку сущности: {1}", LogPrefix, args.PersonId);
          ProceedError(args, true, message);
        }
      }
      catch (Exception ex)
      {
        var message = string.Format("{0} ошибка изменения реквизитов сущности: {1}", LogPrefix, ex.Message);
        ProceedError(args, false, message, person);
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(person);
      }
    }
    
    /// <summary>
    /// Обработка ошибок выполнения.
    /// </summary>
    /// <param name="retry">Признак необходимости повторного выполнения.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="person">Персона, при обработке которой возникла ошибка.</param>
    private void ProceedError(
      lenspec.Etalon.Module.Company.Server.AsyncHandlerInvokeArgs.AsyncChangingPersonsRequisiteslenspecInvokeArgs args, 
      bool retry, 
      string message,
      Sungero.Parties.IPerson person = null)
    {
      if (!retry)
      {
        Logger.Error(message);
        
        // Формирование уведомления об ошибке.
        var functionName = Resources.ChangingPersonsRequisites_ResourceKey.ToString().TrimEnd('.');
        var subject = Resources.ChangingPersonsRequisites_ErrorNotificationSubjectFormat(functionName);
        var initiator = Users.GetAll(u => u.Id == args.InitiatorId).SingleOrDefault();
        var task = Sungero.Workflow.SimpleTasks.CreateWithNotices(subject, initiator);
        task.ActiveText = message;
        if (person != null) 
          task.Attachments.Add(person);
        task.Start();
      }
      else
        Logger.Debug(message);

      args.Retry = retry;
    }

    public virtual void AsyncMovingClosedEmployeeslenspec(lenspec.Etalon.Module.Company.Server.AsyncHandlerInvokeArgs.AsyncMovingClosedEmployeeslenspecInvokeArgs args)
    {
      Logger.DebugFormat("Avis - AsyncMovingClosedEmployeeslenspec перенос сотрудника {0}.", args.EmployeeId);
      
      var department = Sungero.Company.Departments.GetAll(x => x.Id == args.DepartmentId).SingleOrDefault();
      if (department == null)
      {
        Logger.ErrorFormat("Avis - AsyncMovingClosedEmployeeslenspec - не удалось найти подразделение {0}.", args.DepartmentId);
        args.Retry = false;
        return;
      }
      var depLockInfo = Locks.GetLockInfo(department);
      if (depLockInfo.IsLocked)
      {
        args.Retry = true;
        return;
      }
      
      var employee = Sungero.Company.Employees.GetAll(x => x.Id == args.EmployeeId).SingleOrDefault();
      if (employee == null)
      {
        Logger.ErrorFormat("Avis - AsyncMovingClosedEmployeeslenspec - не удалось найти сотрудника {0}.", args.EmployeeId);
        args.Retry = false;
        return;
      }
      var empLockInfo = Locks.GetLockInfo(employee);
      if (empLockInfo.IsLocked)
      {
        args.Retry = true;
        return;
      }
      
      var empIsLocked = false;
      var depIsLocked = false;
      try
      {  
        // Перенести сотрудника в спец. подразделение, только если удалось заблокировать обе карточки. У подразделения сотрудник добавляется в коллекцию Сотрудники.
        depIsLocked = Locks.TryLock(department);
        if (depIsLocked)
        {
          empIsLocked = Locks.TryLock(employee);
          if (empIsLocked)
          {
            AddNoteToClosedEmployee(employee);
            employee.Department = department;
            employee.Save();
          }
          else
          {
            Logger.DebugFormat("Avis - AsyncMovingClosedEmployeeslenspec - не удалось заблокировать сотрудника {0}.", args.EmployeeId);
            args.Retry = true;
            return;
          }
        }
        else
        {
          Logger.DebugFormat("Avis - AsyncMovingClosedEmployeeslenspec - не удалось заблокировать подразделение {0}.", args.DepartmentId);
          args.Retry = true;
          return;
        }
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - AsyncMovingClosedEmployeeslenspec - ", ex);
      }
      finally
      {
        if (empIsLocked)
          Locks.Unlock(employee);
        if (depIsLocked)
          Locks.Unlock(department);
      }
    }
    
    private void AddNoteToClosedEmployee(Sungero.Company.IEmployee employee)
    {
      if (employee == null)
        return;
      
      var note = employee.Note;
      if (string.IsNullOrWhiteSpace(note))
        note = string.Empty;
      else
        note += Environment.NewLine;
      
      var manager = Sungero.Docflow.PublicFunctions.Module.Remote.GetManager(employee);
      var businessUnit = employee.Department?.BusinessUnit;
      
      note += lenspec.Etalon.Module.Company.Resources.ClosedEmployeeNoteFormat(
        Calendar.Now.ToString(),
        employee.Department.Name,
        manager == null ? "-" : manager.Name,
        businessUnit == null ? "-" : businessUnit.Name
       );
      
      var maxLength = employee.Info.Properties.Note.Length;
      if (note.Length > maxLength)
        note = note.Substring(0, maxLength);
      
      employee.Note = note;
    }

    public virtual void ChangeApprovalSettingslenspec(lenspec.Etalon.Module.Company.Server.AsyncHandlerInvokeArgs.ChangeApprovalSettingslenspecInvokeArgs args)
    {
      //Выборка настроек согласования
      var approvalSettings = lenspec.EtalonDatabooks.ApprovalSettings.GetAll(x => args.Status == Constants.Module.ApprovalSettingsStatus.All ||
                                                                             (args.Status == Constants.Module.ApprovalSettingsStatus.Active && x.Status == lenspec.EtalonDatabooks.ApprovalSetting.Status.Active) ||
                                                                             (args.Status == Constants.Module.ApprovalSettingsStatus.Closed && x.Status == lenspec.EtalonDatabooks.ApprovalSetting.Status.Closed));
      if (!string.IsNullOrEmpty(args.BusinessUnitIds))
      {
        var ids = args.BusinessUnitIds.Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToArray();
        approvalSettings = approvalSettings.Where(x => ids.Contains(x.BusinessUnit.Id));
      }
      if (!string.IsNullOrEmpty(args.RegulationIds))
      {
        var ids = args.RegulationIds.Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToArray();
        var approvalRuleAllVersions = new List<Sungero.Docflow.IApprovalRuleBase>();
        foreach (var id in ids)
        {
          var approvalRule = Sungero.Docflow.ApprovalRuleBases.Get(id);
          approvalRuleAllVersions.AddRange(Etalon.PublicFunctions.ApprovalTask.Remote.GetAllRuleVersions(approvalRule));
        }
        approvalSettings = approvalSettings.Where(x => approvalRuleAllVersions.Contains(x.ApprovalRule));
      }
      var approvalSettingIds = approvalSettings.Select(x => x.Id);
      
      //Выполнение соответсвующих операций с настройками согласования
      var roleIds = args.RoleIds.Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToList<long>();
      
      foreach (var settingId in approvalSettingIds)
      {
        switch (args.ActionType)
        {
          case Constants.Module.ApprovalSettingsActions.Addition:
            foreach (var roleId in roleIds)
            {
              var asyncHandler = Company.AsyncHandlers.ChangeApprovalSettingsAdditionavis.Create();
              asyncHandler.ApprovalSettingId = settingId;
              asyncHandler.NewPerformerId = args.NewPerformerId;
              asyncHandler.RoleId = roleId;
              asyncHandler.ExecuteAsync();
            }
            break;
          case Constants.Module.ApprovalSettingsActions.Modification:
            if (roleIds != null && roleIds.Any())
            {
              foreach (var roleId in roleIds)
              {
                var asyncHandler = Company.AsyncHandlers.ChangeApprovalSettingsModificationavis.Create();
                asyncHandler.ApprovalSettingId = settingId;
                asyncHandler.CurrentPerformerId = args.CurrentPerformerId;
                asyncHandler.NewPerformerId = args.NewPerformerId;
                asyncHandler.RoleId = roleId;
                asyncHandler.ExecuteAsync();
              }
            }
            else
            {
              var asyncHandler = Company.AsyncHandlers.ChangeApprovalSettingsModificationavis.Create();
              asyncHandler.ApprovalSettingId = settingId;
              asyncHandler.CurrentPerformerId = args.CurrentPerformerId;
              asyncHandler.NewPerformerId = args.NewPerformerId;
              asyncHandler.RoleId = Constants.Module.EmptyRoleKind;
              asyncHandler.ExecuteAsync();
            }
            break;
          case Constants.Module.ApprovalSettingsActions.Removing:
            foreach (var roleId in roleIds)
            {
              var asyncHandler = Company.AsyncHandlers.ChangeApprovalSettingsRemovingavis.Create();
              asyncHandler.ApprovalSettingId = settingId;
              asyncHandler.CurrentPerformerId = args.CurrentPerformerId;
              asyncHandler.RoleId = roleId;
              asyncHandler.ExecuteAsync();
            }
            break;
        }
      }
    }

    public virtual void ChangeApprovalSettingsRemovingavis(lenspec.Etalon.Module.Company.Server.AsyncHandlerInvokeArgs.ChangeApprovalSettingsRemovingavisInvokeArgs args)
    {
      try
      {
        var approvalSetting = lenspec.EtalonDatabooks.ApprovalSettings.Get(args.ApprovalSettingId);
        var deletedLines = approvalSetting.RoleKindEmployee.Where(x => x.Employee.Id == args.CurrentPerformerId && x.RoleKind.Id == args.RoleId);
        foreach (var line in deletedLines.ToList())
        {
          approvalSetting.RoleKindEmployee.Remove(line);
        }
        approvalSetting.Save();
      }
      catch (Exception ex)
      {
        args.Retry = true;
        Logger.ErrorFormat("Изменение настроек согласования (Удаление). Ошибка обработки записи с ИД={0} - {1}", args.ApprovalSettingId, ex.Message);
      }
    }

    public virtual void ChangeApprovalSettingsModificationavis(lenspec.Etalon.Module.Company.Server.AsyncHandlerInvokeArgs.ChangeApprovalSettingsModificationavisInvokeArgs args)
    {
      try
      {
        var approvalSetting = lenspec.EtalonDatabooks.ApprovalSettings.Get(args.ApprovalSettingId);
        var newPerformer = lenspec.Etalon.Employees.Get(args.NewPerformerId);
        if (args.RoleId == Constants.Module.EmptyRoleKind)
        {
          var changingLines = approvalSetting.RoleKindEmployee.Where(x => x.Employee.Id == args.CurrentPerformerId);
          foreach (var line in changingLines)
          {
            line.Employee = newPerformer;
          }
        }
        else
        {
          var changingLines = approvalSetting.RoleKindEmployee.Where(x => x.Employee.Id == args.CurrentPerformerId && x.RoleKind.Id == args.RoleId);
          foreach (var line in changingLines)
          {
            line.Employee = newPerformer;
          }
        }
        approvalSetting.Save();
      }
      catch (Exception ex)
      {
        args.Retry = true;
        Logger.ErrorFormat("Изменение настроек согласования (Изменение). Ошибка обработки записи с ИД={0} - {1}", args.ApprovalSettingId, ex.Message);
      }
    }

    public virtual void ChangeApprovalSettingsAdditionavis(lenspec.Etalon.Module.Company.Server.AsyncHandlerInvokeArgs.ChangeApprovalSettingsAdditionavisInvokeArgs args)
    {
      try
      {
        var approvalSetting = lenspec.EtalonDatabooks.ApprovalSettings.Get(args.ApprovalSettingId);
        var employee = lenspec.Etalon.Employees.Get(args.NewPerformerId);
        var roleKind = lenspec.EtalonDatabooks.RoleKinds.Get(args.RoleId);
        var newLine = approvalSetting.RoleKindEmployee.AddNew();
        newLine.RoleKind = roleKind;
        newLine.Employee = employee;
        approvalSetting.Save();
      }
      catch (Exception ex)
      {
        args.Retry = true;
        Logger.ErrorFormat("Изменение настроек согласования (Добавление). Ошибка обработки записи с ИД={0} - {1}", args.ApprovalSettingId, ex.Message);
      }
    }

    public virtual void LinkingAccountToCurrentEmployeelenspec(lenspec.Etalon.Module.Company.Server.AsyncHandlerInvokeArgs.LinkingAccountToCurrentEmployeelenspecInvokeArgs args)
    {
      Logger.Debug("Info - LinkingAccountToCurrentEmployeelenspec started.");
      
      //все УЗ которые не привязанны к активному сотруднику
      var activeEmployees = lenspec.Etalon.Employees.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && x.Login != null);
      var logins = Sungero.CoreEntities.Logins.GetAll().Where(x => !activeEmployees.Any(emp => emp.Login.Equals(x))
                                                              && x.LoginName != null
                                                              && x.LoginName != string.Empty
                                                              && x.TypeAuthentication == Sungero.CoreEntities.Login.TypeAuthentication.Windows);
      
      var systemLoginIds = Sungero.CoreEntities.Users.GetAll().Where(x => x.IsSystem == true && x.Login != null).Select(x => x.Login.Id).ToList();
      if (systemLoginIds != null && systemLoginIds.Any())
      {
        logins = logins.Where(x => !systemLoginIds.Contains(x.Id));
      }
      
      //сотрудники без УЗ
      var employeesWithoutLogin = lenspec.Etalon.Employees.GetAll().Where(x => x.Login == null
                                                                          && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active
                                                                          && x.Email != null
                                                                          && x.Email != string.Empty);
      //закрытые сотрудники с УЗ
      var closedEmployees = lenspec.Etalon.Employees.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed && x.Login != null);
      
      foreach(var login in logins)
      {
        var loginName = @login.LoginName.Substring(7, @login.LoginName.Length - 7);
        Logger.DebugFormat("Info - Взята в работу УЗ с логином {0}.", loginName);
        
        //активация и отвязка УЗ от закрытого сотрудника
        var closedEmployeesThisLogin = closedEmployees.Where(x => login.Equals(x.Login));
        if (login.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed || closedEmployeesThisLogin.Any())
        {
          login.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
          login.Save();
          foreach (var closedEmployee in closedEmployeesThisLogin)
          {
            closedEmployee.Login = null;
            closedEmployee.Save();
          }
        }
        
        //сотрудники подходящие под данную УЗ
        var matchingEmployees = employeesWithoutLogin.Where(x => string.Equals(x.Email.Substring(0, x.Email.IndexOf("@")), loginName));
        var matchingEmployeesCount = matchingEmployees.ToList().Count();
        if (matchingEmployeesCount > 1)
        {
          var employeeWithMainPlaceOfWork = matchingEmployees.Where(x => x.PlaceOfWorkavis != null && x.PlaceOfWorkavis.Code1C == 0).FirstOrDefault();
          if (employeeWithMainPlaceOfWork != null)
          {
            Logger.DebugFormat("Info - Найдено более 1 сотрудника подходящего под логин {0}. Первому найденному сотруднику с основным местом работы назначена данная УЗ", loginName);
            
            employeeWithMainPlaceOfWork.Login = login;
            employeeWithMainPlaceOfWork.Save();
          }
        }
        if (matchingEmployeesCount == 1)
        {
          var employee = matchingEmployees.FirstOrDefault();
          Logger.DebugFormat("Info - Найден 1 сотрудник для логина {0}.", loginName);
          employee.Login = login;
          employee.Save();
          
        }
        if (matchingEmployeesCount == 0)
        {
          Logger.DebugFormat("Info - Не найдено подходящих сотрудников для логина {0}.", loginName);
        }
      }
      Logger.Debug("Info - LinkingAccountToCurrentEmployeelenspec finished.");
    }
    
    //Добавлено Avis Expert
    /// <summary>
    /// Асинхронный обработчик для синхронизации сотрудника с ролью по Виду должности.
    /// </summary>
    /// <param name="args">Аргументы асинхронного обработчика.</param>
    public virtual void SynchronizeJobTitleKindWithRolelenspec(lenspec.Etalon.Module.Company.Server.AsyncHandlerInvokeArgs.SynchronizeJobTitleKindWithRolelenspecInvokeArgs args)
    {
      Logger.DebugFormat("Avis - SynchronizeJobTitleKindWithRolelenspec started for employee {0}.", args.EmployeeId);
      try
      {
        var originalRole = args.OldJobTitleId != 0
          ? EtalonDatabooks.JobTitleKinds.Get(args.OldJobTitleId).Role
          : null;
        var newRole = args.NewJobTitleId != 0
          ? EtalonDatabooks.JobTitleKinds.Get(args.NewJobTitleId).Role
          : null;
        
        var employee = Sungero.Company.Employees.GetAll(x => x.Id == args.EmployeeId).SingleOrDefault();
        if (employee == null)
        {
          Logger.ErrorFormat("Avis - SynchronizeJobTitleKindWithRolelenspec - сотрудник {0} не найден.", args.EmployeeId);
          return;
        }
        
        // Добавить сотрудника в роль, соответствующую Виду должности, если запись действующая и сотрудник не включен в новую роль.
        // Если запись закрыта, пропускать любое включение в роль.
        if (args.NewStatus == true && newRole != null && !employee.IncludedIn(newRole))
        {
          var lockInfoCard = Locks.GetLockInfo(newRole);
          if (lockInfoCard != null && lockInfoCard.IsLocked)
          {
            args.Retry = true;
            Logger.DebugFormat("Avis - SynchronizeJobTitleKindWithRolelenspec - карточка роли {0} заблокирована пользователем {1}.", newRole.Id, lockInfoCard.OwnerName);
            return;
          }
          var newRoleRecipients = newRole.RecipientLinks;
          newRoleRecipients.AddNew().Member = employee;
          newRole.Save();
        }
        
        // Удалить сотрудника из участников роли, соответствующей прошлому Виду должности.
        if(originalRole != null && employee.IncludedIn(originalRole))
        {
          // если запись стала закрытой и сотрудник входит в роль, сохраненную в БД,
          // или запись действующая и поменялся Вид должности.
          if(args.OldStatus != args.NewStatus && args.NewStatus == false || args.NewStatus == true && !Equals(originalRole, newRole))
          {
            var lockInfoCard = Locks.GetLockInfo(originalRole);
            if (lockInfoCard != null && lockInfoCard.IsLocked)
            {
              args.Retry = true;
              Logger.DebugFormat("Avis - SynchronizeJobTitleKindWithRolelenspec - карточка роли {0} заблокирована пользователем {1}.", originalRole.Id, lockInfoCard.OwnerName);
              return;
            }
            var originalRoleRecipients = originalRole.RecipientLinks;
            while (originalRoleRecipients.Any(r => Equals(r.Member, employee)))
              originalRoleRecipients.Remove(originalRoleRecipients.First(r => Equals(r.Member, employee)));
            if (originalRole.State.IsChanged)
            {
              originalRole.Save();
            }
          }
        }
        Logger.DebugFormat("Avis - SynchronizeJobTitleKindWithRolelenspec finished for employee {0}.", args.EmployeeId);
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - SynchronizeJobTitleKindWithRolelenspec - сотрудник {0} - ", ex, args.EmployeeId);
      }
    }
    //конец Добавлено Avis Expert

  }
}