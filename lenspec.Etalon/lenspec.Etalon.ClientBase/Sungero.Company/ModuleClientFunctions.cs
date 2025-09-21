using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Company.Client
{
  partial class ModuleFunctions
  {
    
    [LocalizeFunction("ChangesConsonantsISPDialog_ResourceKey", "ChangesConsonantsISPDialog_DescriptionResourceKey")]
    public virtual void ChangesConsonantsISPDialog()
    {
      var employeeCurrentId = string.Empty;
      var employeeNewId     = string.Empty;
      var roleIds           = string.Empty;
      var ourCFIds          = string.Empty;
      var dialog = Dialogs.CreateInputDialog("Укажите параметры для изменения состава согласующих");
      var ok = dialog.Buttons.AddOk();
      var noOk = dialog.Buttons.AddCancel();
      ok.Name = "Ок";
      noOk.Name = "Отмена";
      var employeeCurrent = dialog.AddSelect("Текущий исполнитель", false, Sungero.Company.Employees.Null);
      var employeeNew = dialog.AddSelect("Новый исполнитель", true, Sungero.Company.Employees.Null);
      
      var roleDefault = new List<lenspec.EtalonDatabooks.IRoleKind>();
      var role = dialog.AddSelectMany("Роль", true, roleDefault.ToArray());
      var selectedRoleText = dialog.AddMultilineString("Роль", true, string.Empty).WithRowsCount(3);
      selectedRoleText.IsEnabled = false;
      role.IsEnabled = false;
      role.IsVisible = false;
      var addRoleLink = dialog.AddHyperlink("Добавить Роль");
      var deleteRoleLink = dialog.AddHyperlink("Исключить Роль");
      
      var ourCFDefault = new List<lenspec.EtalonDatabooks.IOurCF>();
      var ourCF = dialog.AddSelectMany("ИСП", true, ourCFDefault.ToArray());
      ourCF.IsEnabled = false;
      ourCF.IsVisible = false;
      var selectedOurCFText = dialog.AddMultilineString("ИСП", true, string.Empty).WithRowsCount(3);
      selectedOurCFText.IsEnabled = false;
      var addOurCFLink = dialog.AddHyperlink("Добавить ИСП");
      var deleteOurCFLink = dialog.AddHyperlink("Исключить ИСП");
      
      #region Роль
      
      addRoleLink.SetOnExecute(
        () =>
        {
          var roleFiltered  = new List<lenspec.EtalonDatabooks.IRoleKind>();
          var selectedRoleFiltered = lenspec.EtalonDatabooks.RoleKinds.GetAll();
          var selectedOurCFs = ourCF.Value.ToList();
          if (ourCF.Value.Any())
          {
            foreach (var oFs in selectedOurCFs)
            {
              foreach (var of in oFs.CollectionCoordinators )
              {
                roleFiltered.Add(of.Role);
              }
            }
            if (roleFiltered.Count() > 0)
            {
              selectedRoleFiltered = selectedRoleFiltered.Where(x => roleFiltered.Contains(x));
            }
          }
          
          var selectedRole = selectedRoleFiltered.ShowSelectMany().AsEnumerable();
          if (selectedRole != null && selectedRole.Any())
          {
            var sourceRole = role.Value.ToList();
            sourceRole.AddRange(selectedRole);
            role.Value = sourceRole.Distinct();
            selectedRoleText.Value = string.Join("; ", role.Value.Select(x => x.Name));
          }
        });
      deleteRoleLink.SetOnExecute(
        () =>
        {
          var selectedRole = role.Value.ShowSelectMany("Выберите Роли для исключения");
          if (selectedRole != null && selectedRole.Any())
          {
            var currentRole = role.Value.ToList();
            foreach (var r in selectedRole)
            {
              currentRole.Remove(r);
            }
            role.Value = currentRole;
            selectedRoleText.Value = string.Join("; ", role.Value.Select(x => x.Name));
            
            var selectedOurCFFiltered = lenspec.EtalonDatabooks.OurCFs.GetAll();
            var rolesToRemove  = new List<lenspec.EtalonDatabooks.IOurCF>();
            foreach (var ruls in selectedOurCFFiltered)
            {
              if (ruls.CollectionCoordinators.Any(x => selectedRole.Contains(x.Role)))
              {
                rolesToRemove.Add(ruls);
              }
            }
            var currentOurCF = ourCF.Value.ToList();
            foreach (var removeRole in rolesToRemove)
            {
              currentOurCF.Remove(removeRole);
            }
            ourCF.Value = currentOurCF;
          }
        });
      
      #endregion
      
      #region ИСП
      
      addOurCFLink.SetOnExecute(
        () =>
        {
          var ourCFFiltered  = new List<lenspec.EtalonDatabooks.IOurCF>();
          var selectedOurCFFiltered = lenspec.EtalonDatabooks.OurCFs.GetAll().Where(x => x.Status == lenspec.EtalonDatabooks.OurCF.Status.Active);
          if (employeeCurrent.Value != null)
          {
            selectedOurCFFiltered = selectedOurCFFiltered.Where(x => x.CollectionCoordinators.FirstOrDefault(e => e.Employee == employeeCurrent.Value) != null);
            if (role.Value.Any())
            {
              foreach (var ruls in selectedOurCFFiltered)
              {
                if (ruls.CollectionCoordinators.Any(x => role.Value.Contains(x.Role)))
                {
                  ourCFFiltered.Add(ruls);
                }
              }
              selectedOurCFFiltered = selectedOurCFFiltered.Where(x => ourCFFiltered.Contains(x));
            }
          }
          var selectedOurCF = selectedOurCFFiltered.ShowSelectMany().AsEnumerable();
          if (selectedOurCF != null && selectedOurCF.Any())
          {
            var sourceOurCF = ourCF.Value.ToList();
            sourceOurCF.AddRange(selectedOurCF);
            ourCF.Value = sourceOurCF.Distinct();
            selectedOurCFText.Value = string.Join("; ", ourCF.Value.Select(x => x.CommercialName));
          }
        });
      deleteOurCFLink.SetOnExecute(
        () =>
        {
          var selectedOurCF = ourCF.Value.ShowSelectMany("Выберите ИСП для исключения");
          if (selectedOurCF != null && selectedOurCF.Any())
          {
            var currentOurCF = ourCF.Value.ToList();
            foreach (var i in selectedOurCF)
            {
              currentOurCF.Remove(i);
            }
            ourCF.Value = currentOurCF;
            var allOurCF = ourCF.Value.ToList();
            selectedOurCFText.Value = string.Join("; ", ourCF.Value.Select(x => x.CommercialName));
          }
        });
      
      #endregion
      
      //   if (dialog.Show() == DialogButtons.Ok)btnFind
      if (dialog.Show() == ok)
      {
        //Вызов асинхронного обработчика
        if (employeeCurrent.Value != null)
          employeeCurrentId = employeeCurrent.Value.Id.ToString();
        if (employeeNew.Value != null)
          employeeNewId = employeeNew.Value.Id.ToString();
        if (role.Value.Any())
          roleIds  = string.Join(" ", role.Value.Select(x => x.Id.ToString()));
        if (ourCF.Value.Any())
          ourCFIds = string.Join(" ", ourCF.Value.Select(x => x.Id.ToString()));
        Etalon.Module.Company.PublicFunctions.Module.AsyncChangingApprovalsInISPAsyns(employeeCurrentId, employeeNewId, roleIds, ourCFIds);
      }
    }

    [LocalizeFunction("DisableAccountsDialog_ResourceKey", "DisableAccountsDialog_DescriptionResourceKey")]
    public virtual void DisableAccountsDialog()
    {
      if (Users.Current.IncludedIn(lenspec.AutomatedSupportTickets.PublicConstants.Module.AccessAccountDisconnectionAndUserReport))
      {
        var dialog = Dialogs.CreateInputDialog("Отключение учетных записей");
        var employeesDefault = new List<Sungero.Company.IEmployee>();
        var employees = dialog.AddSelectMany("Пользователи", true, employeesDefault.ToArray());
        employees.IsEnabled = false;
        employees.IsVisible = false;
        var selectedEmployeesText = dialog.AddMultilineString("Пользователи", true, string.Empty).WithRowsCount(3);
        selectedEmployeesText.IsEnabled = false;
        var addEmployeesLink = dialog.AddHyperlink("Добавить пользователей");
        var deleteEmployeesLink = dialog.AddHyperlink("Исключить пользователей");
        
        #region Сотрудники
        
        addEmployeesLink.SetOnExecute(
          () =>
          {
            //var filteredEmployees = Sungero.Company.Employees.GetAll().Where(x => x.Login != null);
            var filteredEmployees = Sungero.Company.Employees.GetAll();
            var selectedEmployees = filteredEmployees.ShowSelectMany().AsEnumerable();
            if (selectedEmployees != null && selectedEmployees.Any())
            {
              var sourceEmployees = employees.Value.ToList();
              sourceEmployees.AddRange(selectedEmployees);
              employees.Value = sourceEmployees.Distinct();
              
              selectedEmployeesText.Value = string.Join("; ", employees.Value.Select(x => x.Name));
            }
          });
        deleteEmployeesLink.SetOnExecute(
          () =>
          {
            var selectedEmployees = employees.Value.ShowSelectMany("Выберите сотрудников для исключения");
            if (selectedEmployees != null && selectedEmployees.Any())
            {
              var currentEmployees = employees.Value.ToList();
              foreach (var employee in selectedEmployees)
              {
                currentEmployees.Remove(employee);
              }
              employees.Value = currentEmployees;
              selectedEmployeesText.Value = string.Join("; ", employees.Value.Select(x => x.Name));
            }
          });
        
        #endregion
        
        var taskAbort = dialog.AddSelect("Прекращение задач", true).From("Да","Нет");
        var assignmentAbort = dialog.AddSelect("Выполнение заданий", true).From("Да","Нет");
        var substitutionsAbort = dialog.AddSelect("Закрытие замещений", true).From("Да","Нет");
        var textTasks = dialog.AddMultilineString("Текст для задач", false) ;
        textTasks.MaxLength(500);
        textTasks.Value = "Прекращено в связи с увольнением сотрудника.";
        var textAssignment = dialog.AddMultilineString("Текст для задания", false) ;
        textAssignment.MaxLength(500);
        textAssignment.Value = "Автоматическое выполнение в связи с увольнением сотрудника. Если задача актуальна, рестартуйте её и укажите другого исполнителя.";
        dialog.SetOnButtonClick((args) =>
                                {
                                  if (!employees.Value.Any())
                                  {
                                    args.AddError("Укажите хотя бы одного сотрудника", selectedEmployeesText);
                                  }
                                });
        if (dialog.Show() == DialogButtons.Ok)
        {
          //Вызов асинхронного обработчика
          foreach (var employee in employees.Value)
          {
            Etalon.Module.Company.PublicFunctions.Module.DisablingAccountsAsync(employee, textTasks.Value, textAssignment.Value, taskAbort.Value, assignmentAbort.Value, substitutionsAbort.Value);
          }
        }
      }
      else
      {
        Dialogs.ShowMessage("Нет прав на выполнение действия.", MessageType.Error);
      }
    }

    /// <summary>
    /// Изменение настроек согласования.
    /// </summary>
    [LocalizeFunction("ChangeApprovalSettings_ResourceKey", "ChangeApprovalSettings_DescriptionResourceKey")]
    public virtual void ChangeApprovalSettings()
    {
      if (!Sungero.Company.Employees.Current.IncludedIn(Roles.Administrators))
      {
        Dialogs.ShowMessage("Недостаточно прав доступа");
        return;
      }
      var dialog = Dialogs.CreateInputDialog("Укажите параметры для изменения настроек согласования");
      
      var newPerformerList = Sungero.Company.Employees.GetAll(x => x.Status == Sungero.Company.Employee.Status.Active &&
                                                              x.Person.Status == Sungero.Parties.Person.Status.Active &&
                                                              x.Login != null && x.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      var regulationsList = Sungero.Docflow.ApprovalRules.GetAll(x => x.Status == Sungero.Docflow.ApprovalRule.Status.Active);
      
      var action = dialog.AddSelect("Действие", true, string.Empty).From(Constants.Module.ApprovalSettingsActions.Addition,
                                                                         Constants.Module.ApprovalSettingsActions.Modification,
                                                                         Constants.Module.ApprovalSettingsActions.Removing);
      
      var businessUnits = dialog.AddSelectMany("Наша организация", false, Sungero.Company.BusinessUnits.Null);
      var regulations = dialog.AddSelectMany("Регламент", false, Sungero.Docflow.ApprovalRules.Null).From(regulationsList);
      var roles = dialog.AddSelectMany("Роль", false, lenspec.EtalonDatabooks.RoleKinds.Null);
      var currentPerformer = dialog.AddSelect("Текущий исполнитель", false, Sungero.Company.Employees.Null);
      var newPerformer = dialog.AddSelect("Новый исполнитель", false, Sungero.Company.Employees.Null).From(newPerformerList);
      
      var status = dialog.AddSelect("Статус", false, Constants.Module.ApprovalSettingsStatus.All).From(Constants.Module.ApprovalSettingsStatus.Active,
                                                                                                       Constants.Module.ApprovalSettingsStatus.Closed,
                                                                                                       Constants.Module.ApprovalSettingsStatus.All);
      
      action.SetOnValueChanged((x) =>
                               {
                                 if (x.NewValue == Constants.Module.ApprovalSettingsActions.Addition)
                                 {
                                   roles.IsRequired = true;
                                   newPerformer.IsEnabled = true;
                                   newPerformer.IsRequired = true;
                                   currentPerformer.Value = null;
                                   currentPerformer.IsEnabled = false;
                                   currentPerformer.IsRequired = false;
                                   regulations.IsRequired = false;
                                 }
                                 
                                 if (x.NewValue == Constants.Module.ApprovalSettingsActions.Modification)
                                 {
                                   roles.IsRequired = false;
                                   currentPerformer.IsEnabled = true;
                                   currentPerformer.IsRequired = true;
                                   newPerformer.IsEnabled = true;
                                   newPerformer.IsRequired = true;
                                   regulations.IsRequired = false;
                                 }
                                 
                                 if (x.NewValue == Constants.Module.ApprovalSettingsActions.Removing)
                                 {
                                   roles.IsRequired = true;
                                   currentPerformer.IsEnabled = true;
                                   currentPerformer.IsRequired = true;
                                   newPerformer.Value = null;
                                   newPerformer.IsEnabled = false;
                                   newPerformer.IsRequired = false;
                                   regulations.IsRequired = true;
                                 }
                               });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        var businessUnitIds = string.Empty;
        var regulationIds = string.Empty;
        var roleIds = string.Empty;
        var currentPerformerId = currentPerformer.Value != null ? currentPerformer.Value.Id : 0;
        var newPerformerId = newPerformer.Value != null ? newPerformer.Value.Id : 0;
        
        if (businessUnits.Value.Any())
          businessUnitIds = string.Join(" ", businessUnits.Value.Select(x => x.Id.ToString()));
        if (regulations.Value.Any())
          regulationIds = string.Join(" ", regulations.Value.Select(x => x.Id.ToString()));
        if (roles.Value.Any())
          roleIds = string.Join(" ", roles.Value.Select(x => x.Id.ToString()));
        
        Dialogs.NotifyMessage("Изменения будут внесены в ближайшее время.");
        Functions.Module.Remote.ChangeApprovalSettingsExecuteAsync(businessUnitIds, regulationIds, action.Value, status.Value, roleIds, currentPerformerId, newPerformerId);
      }
    }

    #region Создание УЗ с обложки.

    /// <summary>
    /// Создать новую УЗ, доступно только Администраторам.
    /// </summary>
    [LocalizeFunction("CreateAccount_ResourceKey", "CreateAccount_DescriptionResourceKey")]
    public virtual void CreateAccount()
    {
      if (Sungero.CoreEntities.Logins.AccessRights.CanCreate())
      {
        Functions.Module.DownloadEmployeeADFromIntegraDB();
      }
      else
      {
        Dialogs.ShowMessage("Нет прав на создание учетной записи.", MessageType.Error);
      }
    }

    /// <summary>
    /// Загрузить пользователей из AD.
    /// </summary>
    [LocalizeFunction("DownloadEmployeeADFromIntegraDB_ResourceKey", "DownloadEmployeeADFromIntegraDB_DescriptionResourceKey")]
    public static void DownloadEmployeeADFromIntegraDB()
    {
      var dialog = Dialogs.CreateInputDialog("Поиск сотрудников в Active Directory");
      
      var lastName = dialog.AddString("Фамилия", false);
      var firstName = dialog.AddString("Имя", false);
      var middleName = dialog.AddString("Отчество", false);
      
      var btnFind = dialog.Buttons.AddOk();
      btnFind.Name = "Поиск";
      var btnCancel = dialog.Buttons.AddCancel();
      btnCancel.Name = "Отмена";
      
      // Кнопка "поиск"
      if (dialog.Show() == btnFind)
      {
        if (string.IsNullOrWhiteSpace(lastName.Value) &&
            string.IsNullOrWhiteSpace(firstName.Value) &&
            string.IsNullOrWhiteSpace(middleName.Value))
        {
          Dialogs.ShowMessage("Все поля пустые. Заполните хотя бы одно поле для поиска сотрудника.", MessageType.Error);
          return;
        }
        
        try
        {
          var people = Etalon.PublicFunctions.Employee.GetEmployeeADFromIntegraDB(lastName.Value, firstName.Value, middleName.Value);
          CreateLoginFromAD(people);
        }
        catch (Exception ex)
        {
          Dialogs.ShowMessage(ex.Message, MessageType.Error);
        }
      }
    }

    /// <summary>
    /// Выбор определенного пользователя из AD для создания УЗ.
    /// </summary>
    /// <param name="people">Список найденных сотрудников в AD</param>
    [LocalizeFunction("CreateLoginFromAD_ResourceKey", "CreateLoginFromAD_DescriptionResourceKey")]
    public static void CreateLoginFromAD(List<string> people)
    {
      // Создаём диалоговое окно
      var dialog = Dialogs.CreateInputDialog("Найденные сотрудники в Active Directory");
      var employeeSelect = dialog.AddSelect("Сотрудник", true, people[0]).From(people.ToArray());
      
      // кнопка "Поиск"
      var btnSelect = dialog.Buttons.AddOk();
      btnSelect.Name = "Выбрать";
      // кнопка "Отмена"
      var btnCancel = dialog.Buttons.AddCancel();
      btnCancel.Name = "Отмена";
      
      if (dialog.Show() == btnSelect)
      {
        var result = PublicFunctions.Module.CreateLogin(employeeSelect.Value);
        Dialogs.ShowMessage(result, MessageType.Information);
      }
    }

    #endregion

    /// <summary>
    /// Создать и показать карточку Заявки на создание НОР.
    /// </summary>
    [LocalizeFunction("CreateApplictionBUCreation_ResourceKey", "CreateApplictionBUCreation_DescriptionResourceKey")]
    public virtual void CreateApplictionBUCreation()
    {
      Functions.Module.Remote.CreateApplictionBUCreation().Show();
    }

    /// <summary>
    /// Создать и показать карточку Заявки на изменение НОР.
    /// </summary>
    [LocalizeFunction("CreateApplictionBUEditing_ResourceKey", "CreateApplictionBUEditing_DescriptionResourceKey")]
    public virtual void CreateApplictionBUEditing()
    {
      Functions.Module.Remote.CreateApplictionBUEditing().Show();
    }
    
    /// <summary>
    /// Замена категории персон с "Клиенты (покупатели, дольщики)" на "Клиенты (собственники)".
    /// </summary>
    [LocalizeFunction("ChangingPersonsRequisites_ResourceKey", "ChangingPersonsRequisites_DescriptionResourceKey")]
    public virtual void ChangingPersonsRequisites()
    {
      var dialog = Dialogs.CreateInputDialog("Выберите объекты проектов или ИСП");
      
      #region Объекты проектов
      
      var objectsAnProjectDefault = new List<lenspec.EtalonDatabooks.IObjectAnProject>();
      var objectsAnProject = dialog.AddSelectMany("Объекты проектов", false, objectsAnProjectDefault.ToArray());
      objectsAnProject.IsEnabled = false;
      objectsAnProject.IsVisible = false;
      
      // Видимое представление списка.
      var objectsAnProjectsSelected = dialog.AddMultilineString("Объекты проектов", true);
      objectsAnProjectsSelected.IsEnabled = false;
      
      #region Действия
      
      var addObjectsAnProject = dialog.AddHyperlink("Добавить объекты проектов");
      var removeObjectsAnProject = dialog.AddHyperlink("Исключить объекты проектов");
      
      addObjectsAnProject.SetOnExecute(
        () =>
        {
          var selected = lenspec.EtalonDatabooks.ObjectAnProjects
            .GetAll(x => !objectsAnProject.Value.Contains(x))
            .ShowSelectMany("Выберите объекты для добавления");
          
          if (selected == null || !selected.Any())
            return;
          
          var updated = objectsAnProject.Value.ToList();
          updated.AddRange(selected);
          objectsAnProject.Value = updated;
          objectsAnProjectsSelected.Value = GetNamesListFormat(objectsAnProject.Value.Select(x => x.Name));
        });
      
      removeObjectsAnProject.SetOnExecute(
        () =>
        {
          var selected = objectsAnProject.Value.ShowSelectMany("Выберите объекты для исключения");
          
          if (selected == null || !selected.Any())
            return;

          objectsAnProject.Value = objectsAnProject.Value.Except(selected);
          objectsAnProjectsSelected.Value = GetNamesListFormat(objectsAnProject.Value.Select(x => x.Name));
        });
      
      #endregion Действия
      
      #endregion Объекты проектов
      
      #region ИСП
      
      var ourCFsDefault = new List<lenspec.EtalonDatabooks.IOurCF>();
      var ourCFs = dialog.AddSelectMany("ИСП", false, ourCFsDefault.ToArray());
      ourCFs.IsEnabled = false;
      ourCFs.IsVisible = false;
      
      // Видимое представление списка.
      var ourCFsSelected = dialog.AddMultilineString("ИСП", true);
      ourCFsSelected.IsEnabled = false;
      
      #region Действия
      
      var addOurCFs = dialog.AddHyperlink("Добавить ИСП");
      var removeOurCFs = dialog.AddHyperlink("Исключить ИСП");
      
      addOurCFs.SetOnExecute(
        () =>
        {
          var selected = lenspec.EtalonDatabooks.OurCFs
            .GetAll(x => !ourCFs.Value.Contains(x))
            .ShowSelectMany("Выберите ИСП для добавления");
          
          if (selected == null || !selected.Any())
            return;
          
          var updated = ourCFs.Value.ToList();
          updated.AddRange(selected);
          ourCFs.Value = updated;
          ourCFsSelected.Value = GetNamesListFormat(ourCFs.Value.Select(x => x.CommercialName));
        });
      
      removeOurCFs.SetOnExecute(
        () =>
        {
          var selected = ourCFs.Value.ShowSelectMany("Выберите ИСП для исключения");
          
          if (selected == null || !selected.Any())
            return;

          ourCFs.Value = ourCFs.Value.Except(selected);
          ourCFsSelected.Value = GetNamesListFormat(ourCFs.Value.Select(x => x.CommercialName));
        });
      
      #endregion Действия

      #endregion ИСП
      
      #region Видимость полей
      
      objectsAnProject.SetOnValueChanged(
        (x) => {
          var isEmpty = !x.NewValue.Any();
          ourCFsSelected.IsRequired = isEmpty;
          addOurCFs.IsEnabled = isEmpty;
          removeOurCFs.IsEnabled = isEmpty; 
        });
      
      ourCFs.SetOnValueChanged(
        (x) => {
          var isEmpty = !x.NewValue.Any();
          objectsAnProjectsSelected.IsRequired = isEmpty;
          addObjectsAnProject.IsEnabled = isEmpty;
          removeObjectsAnProject.IsEnabled = isEmpty; 
        });
      
      #endregion Видимость полей
      
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok;
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        // Уведомление о начале выполнения.
        var functionName = Resources.ChangingPersonsRequisites_ResourceKey.ToString().TrimEnd('.');
        var message = Resources.ProcessStartedFormat(functionName);
        Dialogs.NotifyMessage(message);
        
        Functions.Module.Remote.ChangingPersonsRequisites(
          objectsAnProject.Value.ToList(), 
          ourCFs.Value.ToList());
      }
    }
    
    /// <summary>
    /// Формирование списка наименований в заданном виде.
    /// </summary>
    /// <param name="names">Наименования.</param>
    /// <returns>Строка с форматированными наименованиями.</returns>
    private static string GetNamesListFormat(IEnumerable<string> names)
    {
      return names.Any() ?
        string.Join("; ", names) + "." :
        string.Empty;
    }
  }
}