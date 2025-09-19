using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ActionItemExecutionTask;

namespace lenspec.Etalon.Client
{
  partial class ActionItemExecutionTaskFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Заполнить пункт составного поручения.
    /// </summary>
    /// <param name="actionItemPart">Пункт составного поручения.</param>
    public override void FillCompoundActionItemPart(Sungero.RecordManagement.IActionItemExecutionTaskActionItemParts actionItemPart)
    {
      //Добавлено Avis Expert
      var document = _obj.DocumentsGroup.OfficialDocuments.SingleOrDefault();
      if (Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment) ||
          document != null && avis.CustomerRequests.CustomerRequests.Is(document))
      {
        base.FillCompoundActionItemPart(actionItemPart);
        return;
      }
      
      // Текущая НОР - будет использоваться в фильтрации.
      var currentBusinessUnit = Sungero.Company.Employees.Current.Department.BusinessUnit;
      var employeesBU = Sungero.Company.Employees.GetAll(x => x.Department.BusinessUnit != null && x.Department.BusinessUnit.Equals(currentBusinessUnit)).ToList();
      // Список ИД неавтоматизированных сотрудников текущей НОР.
      var notAutomatedEmployeeIds = lenspec.EtalonDatabooks.PublicFunctions.Module.GetNotAutomatedEmployeesAvis(employeesBU).Select(x => x.Id).ToList();
      //конец Добавлено Avis Expert
      
      var settings = Sungero.RecordManagement.PublicFunctions.Module.GetSettings();
      var isSupervisorChanges = false;
      var isAssigneeChanges = false;
      var isDeadlineChanges = false;
      var isCoAssigneesChanges = false;
      var isCoAssigneesDeadlineChanges = false;
      var isActionItemTextChanges = false;
      var isAddItemPart = actionItemPart == null;
      var title = isAddItemPart ? Sungero.RecordManagement.ActionItemExecutionTasks.Resources.AddCompoundActionItemPart :
        Sungero.RecordManagement.ActionItemExecutionTasks.Resources.EditCompoundActionItemPartFormat(actionItemPart.Number);
      var supervisorDefault = isAddItemPart ? _obj.Supervisor : actionItemPart.Supervisor ?? _obj.Supervisor;
      isSupervisorChanges = !isAddItemPart && actionItemPart.Supervisor == null && _obj.Supervisor != null;
      var assigneeDefault = isAddItemPart ? Sungero.Company.Employees.Null : actionItemPart.Assignee;
      var deadlineDefault = isAddItemPart ? _obj.FinalDeadline : actionItemPart.Deadline ?? _obj.FinalDeadline;
      isDeadlineChanges = !isAddItemPart && actionItemPart.Deadline == null && _obj.FinalDeadline != null;
      var coAssigneesDefault = isAddItemPart
        ? new List<Sungero.Company.IEmployee>()
        : _obj.PartsCoAssignees.Where(p => p.PartGuid == actionItemPart.PartGuid).Select(p => p.CoAssignee).ToList();
      
      DateTime? coAssigneesDeadlineDefault = null;
      if (!isAddItemPart && coAssigneesDefault.Any())
      {
        isCoAssigneesDeadlineChanges = actionItemPart.CoAssigneesDeadline == null && deadlineDefault != null;
        coAssigneesDeadlineDefault = actionItemPart.CoAssigneesDeadline;
      }
      
      var actionItemPartDefault = isAddItemPart ? string.Empty : actionItemPart.ActionItemPart;
      var titleButton = isAddItemPart ? Sungero.RecordManagement.ActionItemExecutionTasks.Resources.AddButtonDialog :
        Sungero.RecordManagement.ActionItemExecutionTasks.Resources.EditButtonDialog;
      var dialog = Dialogs.CreateInputDialog(title);
      dialog.HelpCode = isAddItemPart
        ? Sungero.RecordManagement.Constants.ActionItemExecutionTask.AddActionItemHelpCode
        : Sungero.RecordManagement.Constants.ActionItemExecutionTask.EditActionItemHelpCode;
      
      var underControl = _obj.IsUnderControl == true;
      var supervisor = dialog.AddSelect(_obj.Info.Properties.Supervisor.LocalizedName, underControl, supervisorDefault)
        .Where(a => a.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
        .Where(a => a.Department.BusinessUnit != null && a.Department.BusinessUnit.Equals(currentBusinessUnit)) //Добавлено Avis Expert
        .Where(a => !notAutomatedEmployeeIds.Contains(a.Id)); //Добавлено Avis Expert
      supervisor.IsEnabled = underControl;

      //Добавлено Avis Expert
      #region Выбор из сотрудников подразделения текущего пользователя, за исключением руководителя, а также сотрудники нижестоящих подразделений.
      
      var currentDepartment = Sungero.Company.Employees.Current.Department;
      // Сотрудники подразделения текущего пользователя, кроме Руководителя.
      var currentDepartmentEmployees = currentDepartment.RecipientLinks.Select(x => Sungero.Company.Employees.As(x.Member)).Where(x => x != null);
      if (currentDepartment.Manager != null)
      {
        currentDepartmentEmployees = currentDepartmentEmployees.Where(x => !x.Equals(currentDepartment.Manager));
      }
      var currentDepartmentEmployeeIds = currentDepartmentEmployees.Select(x => x.Id);
      var subDepartmentIds = Etalon.Module.Company.PublicFunctions.Module.GetSubDepartments(currentDepartment).Select(x => x.Id);
      
      #endregion
      //конец Добавлено Avis Expert
      
      var assignee = dialog.AddSelect(_obj.Info.Properties.Assignee.LocalizedName, true, assigneeDefault)
        .Where(a => a.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
        .Where(a => currentDepartmentEmployeeIds.Contains(a.Id) || subDepartmentIds.Contains(a.Department.Id)) //Добавлено Avis Expert
        .Where(a => !notAutomatedEmployeeIds.Contains(a.Id)); //Добавлено Avis Expert
      
      var deadline = dialog.AddDate(_obj.Info.Properties.Deadline.LocalizedName, false, deadlineDefault).AsDateTime();
      deadline.IsEnabled = _obj.HasIndefiniteDeadline != true;
      deadline.IsRequired = deadline.IsEnabled;
      
      var coAssignees = dialog.AddSelectMany(_obj.Info.Properties.CoAssignees.LocalizedName, false, coAssigneesDefault.ToArray());
      coAssignees.IsEnabled = false;
      coAssignees.IsVisible = false;
      var coAssigneesText = dialog
        .AddMultilineString(_obj.Info.Properties.CoAssignees.LocalizedName, false, Sungero.Docflow.PublicFunctions.Module.GetCoAssigneesNames(coAssigneesDefault, false))
        .WithRowsCount(Sungero.RecordManagement.PublicConstants.Module.CoAssigneesTextRowsCount);
      coAssigneesText.IsEnabled = false;
      
      var addCoAssignees = dialog.AddHyperlink(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.AddCoAssignees);
      var deleteCoAssignees = dialog.AddHyperlink(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.RemoveCoAssignees);
      var coAssigneesDeadline = dialog.AddDate(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneesDeadlineDialog, false, coAssigneesDeadlineDefault).AsDateTime();
      coAssigneesDeadline.IsEnabled = coAssigneesDefault.Any();
      coAssigneesDeadline.IsRequired = coAssigneesDefault.Any() && _obj.HasIndefiniteDeadline != true;
      var actionItemPartText = dialog
        .AddMultilineString(_obj.Info.Properties.ActionItemParts.Properties.ActionItemPart.LocalizedName, false, actionItemPartDefault)
        .WithRowsCount(Sungero.RecordManagement.PublicConstants.Module.ActionItemPartTextRowsCount);
      
      var fillButton = dialog.Buttons.AddCustom(titleButton);
      dialog.Buttons.AddCancel();
      
      dialog.SetOnRefresh(
        (args) =>
        {
          if (deadline.Value.HasValue)
          {
            if (!Sungero.Docflow.PublicFunctions.Module.CheckDeadline(assignee.Value ?? Users.Current, deadline.Value, Calendar.Now))
              args.AddError(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.AssigneeDeadlineLessThanToday, deadline);
            else
            {
              var warnMessage = Sungero.Docflow.PublicFunctions.Module.CheckDeadlineByWorkCalendar(assignee.Value ?? Users.Current, deadline.Value);
              if (!string.IsNullOrEmpty(warnMessage))
                args.AddWarning(warnMessage);
            }
          }
          
          if (coAssigneesDeadline.Value.HasValue)
          {
            // Срок соисполнителей должен быть больше или равен текущей дате.
            if (!Sungero.Docflow.PublicFunctions.Module.CheckCoAssigneesDeadline(coAssignees.Value.ToList(), coAssigneesDeadline.Value))
              args.AddError(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneeDeadlineLessThanToday);

            // Срок выполнения соисполнителей выпадает на выходной день.
            foreach (IEmployee coAssignee in coAssignees.Value)
            {
              var warnMessage = Sungero.Docflow.PublicFunctions.Module.CheckDeadlineByWorkCalendar(coAssignee, coAssigneesDeadline.Value);
              if (!string.IsNullOrEmpty(warnMessage))
                args.AddWarning(warnMessage);
            }
          }
          var assigneeDeadline = deadline.Value.HasValue ? deadline.Value : _obj.FinalDeadline;
          
          if (assigneeDeadline != null && coAssigneesDeadline.Value.HasValue && !Sungero.Docflow.PublicFunctions.Module.CheckAssigneesDeadlines(assigneeDeadline, coAssigneesDeadline.Value))
            args.AddError(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneesDeadlineError);
          
          fillButton.IsEnabled = isSupervisorChanges || isAssigneeChanges || isDeadlineChanges || isCoAssigneesChanges || isCoAssigneesDeadlineChanges || isActionItemTextChanges;
        });
      
      // Контролер.
      supervisor.SetOnValueChanged(
        (args) =>
        {
          isSupervisorChanges = !Equals(args.NewValue, supervisorDefault);
        });
      
      // Исполнитель.
      assignee.SetOnValueChanged(
        (args) =>
        {
          isAssigneeChanges = !Equals(args.NewValue, assigneeDefault);
        });
      
      // Срок исполнителя.
      deadline.SetOnValueChanged(
        (args) =>
        {
          isDeadlineChanges = !Equals(args.NewValue, deadlineDefault);
          if (!coAssigneesDeadline.Value.HasValue && coAssignees.Value.Any())
          {
            coAssigneesDeadline.Value = Sungero.Docflow.PublicFunctions.Module.GetDefaultCoAssigneesDeadline(args.NewValue, -settings.ControlRelativeDeadlineInDays ?? 0, -settings.ControlRelativeDeadlineInHours ?? 0);
          }
        });
      
      // Соисполнители.
      coAssignees.SetOnValueChanged(
        (args) =>
        {
          coAssigneesText.Value = Sungero.Docflow.PublicFunctions.Module.GetCoAssigneesNames(coAssignees.Value.ToList(), false);
          isCoAssigneesChanges = !coAssigneesDefault.SequenceEqual(coAssignees.Value.ToList());
          
          var coAssigneesExist = coAssignees.Value.Any();
          coAssigneesDeadline.IsRequired = coAssigneesExist && _obj.HasIndefiniteDeadline != true;
          coAssigneesDeadline.IsEnabled = coAssigneesExist;
          
          if (!coAssigneesExist)
            coAssigneesDeadline.Value = null;
        });
      
      // Срок соисполнителей.
      coAssigneesDeadline.SetOnValueChanged(
        (args) =>
        {
          isCoAssigneesDeadlineChanges = !Equals(args.NewValue, coAssigneesDeadlineDefault);
        });
      
      // Текст поручения.
      actionItemPartText.SetOnValueChanged(
        (args) =>
        {
          isActionItemTextChanges = !Equals(args.NewValue, actionItemPartDefault ?? string.Empty);
        });
      
      #region Гиперссылки на добавление и удаление соисполнителей
      
      addCoAssignees.SetOnExecute(
        () =>
        {
          var selectedEmployees = Sungero.Company.PublicFunctions.Employee.Remote.GetEmployees()
            .Where(ca => ca.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
            .Where(ca => currentDepartmentEmployeeIds.Contains(ca.Id) || subDepartmentIds.Contains(ca.Department.Id)) //Добавлено Avis Expert
            .Where(ca => !notAutomatedEmployeeIds.Contains(ca.Id)) //Добавлено Avis Expert
            .ShowSelectMany(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.ChooseCoAssigneesForAdd).ToList();
          
          if (selectedEmployees != null && selectedEmployees.Any())
          {
            var sourceCoAssignees = coAssignees.Value.ToList();
            sourceCoAssignees.AddRange(selectedEmployees);
            coAssignees.Value = sourceCoAssignees.Distinct();
            
            if (!coAssigneesDeadline.Value.HasValue && coAssignees.Value.Any())
            {
              coAssigneesDeadline.Value = Sungero.Docflow.PublicFunctions.Module.GetDefaultCoAssigneesDeadline(deadline.Value, -settings.ControlRelativeDeadlineInDays ?? 0, -settings.ControlRelativeDeadlineInHours ?? 0);
            }
          }
        });
      
      deleteCoAssignees.SetOnExecute(
        () =>
        {
          var selectedEmployees = coAssignees.Value.ShowSelectMany(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.ChooseCoAssigneesForDelete);
          if (selectedEmployees != null && selectedEmployees.Any())
          {
            var currentCoAssignees = coAssignees.Value.ToList();
            
            foreach (var employee in selectedEmployees)
            {
              currentCoAssignees.Remove(employee);
            }
            
            coAssignees.Value = currentCoAssignees;
          }
        });
      
      #endregion
      
      dialog.SetOnButtonClick(
        args =>
        {
          if (args.Button == fillButton)
          {
            if (deadline.Value.HasValue && !Sungero.Docflow.PublicFunctions.Module.CheckDeadline(assignee.Value ?? Users.Current, deadline.Value, Calendar.Now))
            {
              args.AddError(ActionItemExecutionTasks.Resources.AssigneeDeadlineLessThanToday, deadline);
              return;
            }

            if (coAssigneesDeadline.Value.HasValue && !Sungero.Docflow.PublicFunctions.Module.CheckCoAssigneesDeadline(coAssignees.Value.ToList(), coAssigneesDeadline.Value))
            {
              args.AddError(ActionItemExecutionTasks.Resources.CoAssigneeDeadlineLessThanToday);
              return;
            }
            
            var assigneeDeadline = deadline.Value.HasValue ? deadline.Value : _obj.FinalDeadline;
            
            if (assigneeDeadline != null && coAssigneesDeadline.Value.HasValue && !Sungero.Docflow.PublicFunctions.Module.CheckAssigneesDeadlines(assigneeDeadline, coAssigneesDeadline.Value))
            {
              args.AddError(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneesDeadlineError);
              return;
            }
            
            if (args.IsValid)
            {
              if (isAddItemPart)
                Sungero.RecordManagement.PublicFunctions.ActionItemExecutionTask.AddActionItemPart(_obj, assignee.Value, deadline.Value, actionItemPartText.Value, coAssignees.Value.ToList(), coAssigneesDeadline.Value, supervisor.Value);
              else
                Sungero.RecordManagement.PublicFunctions.ActionItemExecutionTask.EditActionItemPart(_obj, actionItemPart, assignee.Value, deadline.Value, actionItemPartText.Value, coAssignees.Value.ToList(), coAssigneesDeadline.Value, supervisor.Value);
            }
          }
        });
      
      dialog.Show();
    }
    
    /// <summary>
    /// Изменить пункт составного поручения.
    /// </summary>
    /// <param name="actionItemPart">Пункт составного поручения.</param>
    public override void ChangeCompoundActionItemPart(Sungero.RecordManagement.IActionItemExecutionTaskActionItemParts actionItemPart)
    {
      //Добавлено Avis Expert
      var document = _obj.DocumentsGroup.OfficialDocuments.SingleOrDefault();
      if (Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment) ||
          document != null && avis.CustomerRequests.CustomerRequests.Is(document))
      {
        base.ChangeCompoundActionItemPart(actionItemPart);
        return;
      }
      
      // Текущая НОР - будет использоваться в фильтрации.
      var currentBusinessUnit = Sungero.Company.Employees.Current.Department.BusinessUnit;
      var employeesBU = Sungero.Company.Employees.GetAll(x => x.Department.BusinessUnit != null && x.Department.BusinessUnit.Equals(currentBusinessUnit)).ToList();
      // Список ИД неавтоматизированных сотрудников текущей НОР.
      var notAutomatedEmployeeIds = Sungero.Company.PublicFunctions.Module.Remote.GetNotAutomatedEmployees(employeesBU).Select(x => x.Id).ToList();
      //конец Добавлено Avis Expert
      
      // Получить настройки модуля Делопроизводство для вычисления срока соисполнителей по умолчанию.
      var settings = Sungero.RecordManagement.PublicFunctions.Module.GetSettings();
      var deadlineShiftInDays = -settings.ControlRelativeDeadlineInDays ?? 0;
      var deadlineShiftInHours = -settings.ControlRelativeDeadlineInHours ?? 0;
      
      // Инициализировать изменения поручения.
      var changes = Sungero.RecordManagement.Structures.ActionItemExecutionTask.ActionItemChanges.Create();
      var actionItemPartExecutionTask = actionItemPart.ActionItemPartExecutionTask;
      changes.InitiatorOfChange = Users.Current;
      changes.OldAssignee = actionItemPartExecutionTask.Assignee;
      changes.NewAssignee = changes.OldAssignee;
      changes.OldDeadline = actionItemPartExecutionTask.Deadline;
      changes.NewDeadline = changes.OldDeadline;
      changes.OldCoAssignees = actionItemPartExecutionTask.CoAssignees.Select(a => a.Assignee).ToList();
      changes.NewCoAssignees = changes.OldCoAssignees;
      changes.OldSupervisor = actionItemPartExecutionTask.Supervisor;
      changes.NewSupervisor = changes.OldSupervisor;
      changes.CoAssigneesOldDeadline = actionItemPartExecutionTask.CoAssigneesDeadline;
      changes.CoAssigneesNewDeadline = changes.CoAssigneesOldDeadline;
      changes.ChangeContext = Sungero.RecordManagement.Constants.ActionItemExecutionTask.ChangeContext.Part;

      // Получить заголовок и текст диалога корректировки
      var changeDialogInfo = this.GetDialogTitleAndText(actionItemPartExecutionTask);
      var dialogTitle = changeDialogInfo.DialogTitle;
      var dialogText = changeDialogInfo.DialogText;
      
      var dialogOpenDate = Calendar.Now;
      var helpCode = Sungero.RecordManagement.Constants.ActionItemExecutionTask.ActionItemHelpCode;
      var dialog = Dialogs.CreateInputDialog(dialogTitle, dialogText);
      dialog.HelpCode = helpCode;
      
      // Контролер.
      var supervisor = dialog.AddSelect(actionItemPart.Info.Properties.Supervisor.LocalizedName, false, changes.OldSupervisor)
        .Where(s => s.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
        .Where(a => a.Department.BusinessUnit != null && a.Department.BusinessUnit.Equals(currentBusinessUnit)) //Добавлено Avis Expert
        .Where(a => !notAutomatedEmployeeIds.Contains(a.Id)); //Добавлено Avis Expert
      supervisor.IsRequired = actionItemPartExecutionTask.Supervisor != null;
      
      //Добавлено Avis Expert
      #region Выбор из сотрудников подразделения текущего пользователя, за исключением руководителя, а также сотрудники нижестоящих подразделений.
      
      var currentDepartment = Sungero.Company.Employees.Current.Department;
      // Сотрудники подразделения текущего пользователя, кроме Руководителя.
      var currentDepartmentEmployees = currentDepartment.RecipientLinks.Select(x => Sungero.Company.Employees.As(x.Member)).Where(x => x != null);
      if (currentDepartment.Manager != null)
      {
        currentDepartmentEmployees = currentDepartmentEmployees.Where(x => !x.Equals(currentDepartment.Manager));
      }
      var currentDepartmentEmployeeIds = currentDepartmentEmployees.Select(x => x.Id);
      var subDepartmentIds = Etalon.Module.Company.PublicFunctions.Module.GetSubDepartments(currentDepartment).Select(x => x.Id);
      
      #endregion
      //конец Добавлено Avis Expert
      
      // Исполнитель.
      var assignee = dialog.AddSelect(actionItemPart.Info.Properties.Assignee.LocalizedName, true, actionItemPartExecutionTask.Assignee)
        .Where(a => a.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
        .Where(a => currentDepartmentEmployeeIds.Contains(a.Id) || subDepartmentIds.Contains(a.Department.Id)) //Добавлено Avis Expert
        .Where(a => !notAutomatedEmployeeIds.Contains(a.Id)); //Добавлено Avis Expert
      assignee.IsEnabled = actionItemPartExecutionTask.ExecutionState != ExecutionState.OnControl;
      
      // Срок исполнителя.
      var deadline = dialog.AddDate(actionItemPart.Info.Properties.Deadline.LocalizedName, false, changes.OldDeadline).AsDateTime();
      deadline.IsEnabled = actionItemPartExecutionTask.ExecutionState != ExecutionState.OnControl;
      deadline.IsRequired = actionItemPartExecutionTask.HasIndefiniteDeadline != true;
      
      // Соисполнители.
      var coAssignees = dialog.AddSelectMany(actionItemPart.Info.Properties.CoAssignees.LocalizedName, false, changes.OldCoAssignees.ToArray());
      coAssignees.IsEnabled = false;
      coAssignees.IsVisible = false;
      var coAssigneesText = dialog
        .AddMultilineString(actionItemPart.Info.Properties.CoAssignees.LocalizedName, false, string.Join("; ", coAssignees.Value.Select(x => x.Person.ShortName)))
        .WithRowsCount(3);
      coAssigneesText.IsEnabled = false;
      var addCoAssignees = dialog.AddHyperlink(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.AddCoAssignees);
      addCoAssignees.IsEnabled = actionItemPartExecutionTask.ExecutionState != Sungero.RecordManagement.ActionItemExecutionTask.ExecutionState.OnControl;
      var deleteCoAssignees = dialog.AddHyperlink(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.RemoveCoAssignees);
      deleteCoAssignees.IsEnabled = actionItemPartExecutionTask.ExecutionState != Sungero.RecordManagement.ActionItemExecutionTask.ExecutionState.OnControl;
      
      // Срок соисполнителей.
      var coAssigneesDeadline = dialog.AddDate(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.CoAssigneesDeadlineDialog, false, changes.CoAssigneesOldDeadline).AsDateTime();
      var coAssigneesExist = coAssignees.Value.Any();
      var coAssigneesHaveOldDeadline = changes.CoAssigneesOldDeadline.HasValue;
      coAssigneesDeadline.IsEnabled = coAssigneesExist && actionItemPartExecutionTask.ExecutionState != Sungero.RecordManagement.ActionItemExecutionTask.ExecutionState.OnControl;
      coAssigneesDeadline.IsRequired = coAssigneesExist && coAssigneesHaveOldDeadline && actionItemPartExecutionTask.Deadline != null;
      
      // Обоснование.
      var editingReason = dialog.AddMultilineString(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.EditingReason, true).WithRowsCount(2);
      
      var changeButton = dialog.Buttons.AddCustom(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.Change);
      dialog.Buttons.AddCancel();
      
      dialog.SetOnRefresh(
        args =>
        {
          var supervisorChanged = !Equals(changes.OldSupervisor, changes.NewSupervisor);
          var deadlineChanged = changes.OldDeadline != changes.NewDeadline;
          var assigneeChanged = !Equals(changes.OldAssignee, changes.NewAssignee);
          var coAssigneesChanged = !changes.OldCoAssignees.SequenceEqual(changes.NewCoAssignees);
          var coAssigneeDeadlineChanged = changes.CoAssigneesOldDeadline != changes.CoAssigneesNewDeadline;
          
          CheckDeadlines(changes, deadline, coAssigneesDeadline, args);
          changeButton.IsEnabled = supervisorChanged || deadlineChanged || assigneeChanged || coAssigneeDeadlineChanged || coAssigneesChanged;

          var coAssigneesExistNow = changes.NewCoAssignees.Any();
          coAssigneesDeadline.IsEnabled = coAssigneesExistNow && actionItemPartExecutionTask.ExecutionState != Sungero.RecordManagement.ActionItemExecutionTask.ExecutionState.OnControl;
          coAssigneesDeadline.IsRequired = coAssigneesExistNow && (coAssigneesHaveOldDeadline || changes.OldDeadline.HasValue || changes.NewDeadline.HasValue);
        });
      
      // Контролер.
      supervisor.SetOnValueChanged(
        (args) =>
        {
          changes.NewSupervisor = args.NewValue;
        });
      
      // Исполнитель.
      assignee.SetOnValueChanged(
        (args) =>
        {
          changes.NewAssignee = args.NewValue;
        });
      
      // Срок.
      deadline.SetOnValueChanged(
        (args) =>
        {
          changes.NewDeadline = args.NewValue;
          if (!coAssigneesDeadline.Value.HasValue && coAssignees.Value.Any())
          {
            coAssigneesDeadline.Value = Sungero.Docflow.PublicFunctions.Module.GetDefaultCoAssigneesDeadline(args.NewValue,
                                                                                                             deadlineShiftInDays,
                                                                                                             deadlineShiftInHours);
          }
        });
      
      // Соисполнители.
      coAssignees.SetOnValueChanged(
        (args) =>
        {
          coAssigneesText.Value = string.Join("; ", coAssignees.Value.Select(x => x.Person.ShortName));
          changes.NewCoAssignees = args.NewValue.ToList();
          
          var coAssigneesExistNow = coAssignees.Value.Any();
          DateTime? newCoAssigneesDeadline;
          
          if (!coAssigneesExistNow)
          {
            newCoAssigneesDeadline = null;
          }
          else
          {
            newCoAssigneesDeadline = coAssigneesDeadline.Value ?? actionItemPartExecutionTask.CoAssigneesDeadline;
            
            // Вычислить срок соисполнителей по умолчанию.
            if (newCoAssigneesDeadline == null)
            {
              newCoAssigneesDeadline = Sungero.Docflow.PublicFunctions.Module.GetDefaultCoAssigneesDeadline(deadline.Value,
                                                                                                            deadlineShiftInDays,
                                                                                                            deadlineShiftInHours);
            }
          }
          
          coAssigneesDeadline.IsEnabled = coAssigneesExistNow;
          coAssigneesDeadline.IsRequired = coAssigneesExistNow && coAssigneesHaveOldDeadline && actionItemPartExecutionTask.Deadline != null;
          coAssigneesDeadline.Value = newCoAssigneesDeadline;
        });
      
      // Добавление соисполнителей.
      addCoAssignees.SetOnExecute(
        () =>
        {
          var selectedEmployees = Sungero.Company.PublicFunctions.Employee.Remote.GetEmployees()
            .Where(ca => ca.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
            .Where(ca => currentDepartmentEmployeeIds.Contains(ca.Id) || subDepartmentIds.Contains(ca.Department.Id)) //Добавлено Avis Expert
            .Where(ca => !notAutomatedEmployeeIds.Contains(ca.Id)) //Добавлено Avis Expert
            .ShowSelectMany(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.ChooseCoAssigneesForAdd).ToList();
          
          if (selectedEmployees != null && selectedEmployees.Any())
          {
            selectedEmployees.AddRange(coAssignees.Value);
            coAssignees.Value = selectedEmployees.Distinct();
          }
        });
      
      // Удаление соисполнителей.
      deleteCoAssignees.SetOnExecute(
        () =>
        {
          var selectedEmployees = coAssignees.Value.ShowSelectMany(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.ChooseCoAssigneesForDelete);
          if (selectedEmployees != null && selectedEmployees.Any())
          {
            var currentCoAssignees = coAssignees.Value.ToList();
            
            foreach (var employee in selectedEmployees)
            {
              currentCoAssignees.Remove(employee);
            }
            
            coAssignees.Value = currentCoAssignees;
          }
        });
      
      // Срок соисполнителя.
      coAssigneesDeadline.SetOnValueChanged(
        (args) =>
        {
          changes.CoAssigneesNewDeadline = args.NewValue;
        });
      
      // Причина корректировки.
      editingReason.SetOnValueChanged(
        (args) =>
        {
          changes.EditingReason = args.NewValue;
        });
      
      // Нажатие любой кнопки диалога.
      dialog.SetOnButtonClick(
        (args) =>
        {
          if (!Equals(args.Button, changeButton))
            return;
          
          CheckDeadlines(changes, deadline, coAssigneesDeadline, args);
          
          var errorMessage = Functions.ActionItemExecutionTask.Remote.CheckActionItemPartEditInDialog(Etalon.ActionItemExecutionTasks.As(actionItemPart.ActionItemExecutionTask),
                                                                                                      Sungero.RecordManagement.ActionItemExecutionTasks.As(actionItemPart.ActionItemPartExecutionTask),
                                                                                                      assignee.Value, deadline.Value,
                                                                                                      dialogOpenDate);
          if (!string.IsNullOrWhiteSpace(errorMessage))
            args.AddError(errorMessage);
          
          if (string.IsNullOrWhiteSpace(changes.EditingReason) && !string.IsNullOrEmpty(changes.EditingReason))
            args.AddError(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.EmptyEditingReason, editingReason);
        });
      
      // Показ диалога.
      if (dialog.Show() == changeButton)
      {
        // Показать диалог выдачи прав на вложения из группы "Дополнительно",
        // если у кого-то из участников нет на них прав.
        var accessRightGranted = this.ShowDialogGrantAccessRights(actionItemPartExecutionTask,
                                                                  actionItemPartExecutionTask.OtherGroup.All.ToList(), changes);
        if (accessRightGranted == false)
          return;
        
        // Протащить изменения в грид.
        // Контролера и срок обновить, только если они менялись, т.к. могли изначально браться из общих полей.
        if (!Equals(changes.OldSupervisor, changes.NewSupervisor))
          actionItemPart.Supervisor = supervisor.Value;
        if (!Equals(changes.OldDeadline, changes.NewDeadline))
          actionItemPart.Deadline = deadline.Value;
        actionItemPart.Assignee = assignee.Value;
        actionItemPart.CoAssigneesDeadline = coAssigneesDeadline.Value;
        Sungero.RecordManagement.PublicFunctions.ActionItemExecutionTask.DeletePartsCoAssignees(_obj, actionItemPart);
        Sungero.RecordManagement.PublicFunctions.ActionItemExecutionTask.AddPartsCoAssignees(_obj, actionItemPart, changes.NewCoAssignees);
        Sungero.RecordManagement.PublicFunctions.ActionItemExecutionTask.Remote.SetActionItemChangeDeadlinesParams(_obj, changes);
        _obj.Save();
        
        // Обработать изменения пункта поручения.
        Sungero.RecordManagement.PublicFunctions.ActionItemExecutionTask.Remote.ChangeSimpleActionItem(actionItemPartExecutionTask, changes);
        
        // Создание подзадачек соисполнителям и пунктов поручений.
        // Асинхронное событие вызываем после выполнения ChangeSimpleActionItem, чтобы сохранились все изменения actionItemPartExecutionTask.
        Sungero.RecordManagement.PublicFunctions.Module.Remote.ExecuteApplyActionItemLockIndependentChanges(changes, actionItemPartExecutionTask.Id, actionItemPartExecutionTask.OnEditGuid);
        
        // Показать уведомление об успешной корректировке.
        Dialogs.NotifyMessage(Sungero.RecordManagement.ActionItemExecutionTasks.Resources.ChangeActionItemSuccess);
      }
    }
    //конец Добавлено Avis Expert
  }
}