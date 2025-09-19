using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets
{
  partial class TimeLastUserConnectionClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      TimeLastUserConnection.beginDate  = Calendar.Today.BeginningOfMonth();
      TimeLastUserConnection.endDate  = Calendar.Now;
      var dialog = Dialogs.CreateInputDialog("Время последнего подключения пользователей");
      var beginDate = dialog.AddDate("Дата, время с", true, TimeLastUserConnection.beginDate);
      var endDate = dialog.AddDate("Дата, время по", false, TimeLastUserConnection.endDate);
      
      var businessUnitsDefault = new List<Sungero.Company.IBusinessUnit>();
      var businessUnits = dialog.AddSelectMany("Наши организации", false, businessUnitsDefault.ToArray());
      businessUnits.IsEnabled = false;
      businessUnits.IsVisible = false;
      var selectedBussinesUnitText = dialog.AddMultilineString("Наши организации", false, string.Empty).WithRowsCount(3);
      selectedBussinesUnitText.IsEnabled = false;
      var addBussinesUnitLink = dialog.AddHyperlink("Добавить наши организации");
      var deleteBussinesUnitLink = dialog.AddHyperlink("Исключить наши организации");
      
      var departmentsDefault = new List<Sungero.Company.IDepartment>();
      var departments = dialog.AddSelectMany("Подразделения", false, departmentsDefault.ToArray());
      departments.IsEnabled = false;
      departments.IsVisible = false;
      var selectedDepartmentsText = dialog.AddMultilineString("Подразделения", false, string.Empty).WithRowsCount(3);
      selectedDepartmentsText.IsEnabled = false;
      var addDepartmentsLink = dialog.AddHyperlink("Добавить подразделения");
      var deleteDepartmentsLink = dialog.AddHyperlink("Исключить подразделения");
      
      var employeesDefault = new List<Sungero.Company.IEmployee>();
      var employees = dialog.AddSelectMany("Сотрудники", true, employeesDefault.ToArray());
      employees.IsEnabled = false;
      employees.IsVisible = false;
      var selectedEmployeesText = dialog.AddMultilineString("Сотрудники", false, string.Empty).WithRowsCount(3);
      selectedEmployeesText.IsEnabled = false;
      var addEmployeesLink = dialog.AddHyperlink("Добавить сотрудников");
      var deleteEmployeesLink = dialog.AddHyperlink("Исключить сотрудников");
      
      #region Наши организации
      
      addBussinesUnitLink.SetOnExecute(
        () =>
        {
          var selectedBussinesUnits = Sungero.Company.BusinessUnits.GetAll().ShowSelectMany().AsEnumerable();
          if (selectedBussinesUnits != null && selectedBussinesUnits.Any())
          {
            var sourceBussinesUnits = businessUnits.Value.ToList();
            sourceBussinesUnits.AddRange(selectedBussinesUnits);
            businessUnits.Value = sourceBussinesUnits.Distinct();
            
            selectedBussinesUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
          }
        });
      deleteBussinesUnitLink.SetOnExecute(
        () =>
        {
          var selectedBussinesUnits = businessUnits.Value.ShowSelectMany("Выберите наши организации для исключения");
          if (selectedBussinesUnits != null && selectedBussinesUnits.Any())
          {
            var currentBussinesUnits = businessUnits.Value.ToList();
            foreach (var bussinesUnit in selectedBussinesUnits)
            {
              currentBussinesUnits.Remove(bussinesUnit);
            }
            businessUnits.Value = currentBussinesUnits;
            selectedBussinesUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
            
            var departmentToRemove = departments.Value.Where(x => selectedBussinesUnits.Contains(x.BusinessUnit)).ToList();
            var currentDepartments = departments.Value.ToList();
            foreach (var department in departmentToRemove)
            {
              currentDepartments.Remove(department);
            }
            departments.Value = currentDepartments;
            selectedDepartmentsText.Value = string.Join("; ", departments.Value.Select(x => x.Name));
            
            var employeesToRemove = employees.Value.Where(x => selectedBussinesUnits.Contains(x.Department.BusinessUnit)).ToList();
            var currentEmployees = employees.Value.ToList();
            foreach (var employee in employeesToRemove)
            {
              currentEmployees.Remove(employee);
            }
            employees.Value = currentEmployees;
            selectedEmployeesText.Value = string.Join("; ", employees.Value.Select(x => x.Name));
          }
        });
      
      #endregion
      
      #region Подразделения
      
      addDepartmentsLink.SetOnExecute(
        () =>
        {
          var filteredDepartments = Sungero.Company.Departments.GetAll();
          if (businessUnits.Value.Any())
          {
            filteredDepartments = filteredDepartments.Where(x => businessUnits.Value.Contains(x.BusinessUnit));
          }
          var selectedDepartments = filteredDepartments.ShowSelectMany().AsEnumerable();
          if (selectedDepartments != null && selectedDepartments.Any())
          {
            var sourceDepartments = departments.Value.ToList();
            sourceDepartments.AddRange(selectedDepartments);
            departments.Value = sourceDepartments.Distinct();
            
            selectedDepartmentsText.Value = string.Join("; ", departments.Value.Select(x => x.Name));
          }
        });
      deleteDepartmentsLink.SetOnExecute(
        () =>
        {
          var selectedDepartments = departments.Value.ShowSelectMany("Выберите подразделения для исключения");
          if (selectedDepartments != null && selectedDepartments.Any())
          {
            var currentDepartments = departments.Value.ToList();
            foreach (var department in selectedDepartments)
            {
              currentDepartments.Remove(department);
            }
            departments.Value = currentDepartments;
            selectedDepartmentsText.Value = string.Join("; ", departments.Value.Select(x => x.Name));
            
            var employeesToRemove = employees.Value.Where(x => selectedDepartments.Contains(x.Department)).ToList();
            var currentEmployees = employees.Value.ToList();
            foreach (var employee in employeesToRemove)
            {
              currentEmployees.Remove(employee);
            }
            employees.Value = currentEmployees;
            selectedEmployeesText.Value = string.Join("; ", employees.Value.Select(x => x.Name));
          }
        });
      
      #endregion
      
      #region Сотрудники
      
      addEmployeesLink.SetOnExecute(
        () =>
        {
          var filteredEmployees = Sungero.Company.Employees.GetAll(x => x.Login != null);
          if (businessUnits.Value.Any())
          {
            filteredEmployees = filteredEmployees.Where(x => businessUnits.Value.Contains(x.Department.BusinessUnit));
          }
          if (departments.Value.Any())
          {
            filteredEmployees = filteredEmployees.Where(x => departments.Value.Contains(x.Department));
          }
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
      
      dialog.SetOnButtonClick((args) =>
                              {
                                if (endDate.Value.HasValue)
                                {
                                  TimeLastUserConnection.endDate = Calendar.Today.EndOfDay();
                                }
                                Sungero.Docflow.PublicFunctions.Module.CheckDialogPeriod(args, beginDate, endDate);
                                lenspec.AutomatedSupportTickets.PublicFunctions.Module.CheckDialogPeriodBetween(args, beginDate, endDate);
                                if(employees.Value.Count() == 0)
                                {
                                  args.AddError("Заполните хотя бы одного сотрудника!");
                                }
                              });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        //        if (beginDate.Value.HasValue)
        //        {
        //          TimeLastUserConnection.beginDate = beginDate.Value.Value;
        //        }
        //        else
        //        {
        //          TimeLastUserConnection.beginDate = new DateTime(1991, 12, 31);
        //        }
        //        if (endDate.Value.HasValue)
        //        {
        //          TimeLastUserConnection.endDate = endDate.Value.Value;
        //        }
        //        else
        //        {
        //          TimeLastUserConnection.endDate = Calendar.Today.EndOfDay();
        //        }
        if (employees.Value.Count() == 0)
        {
          var filteredEmployees = Sungero.Company.Employees.GetAll(x => x.Login != null);
          if (businessUnits.Value.Any())
          {
            filteredEmployees = filteredEmployees.Where(x => businessUnits.Value.Contains(x.Department.BusinessUnit));
          }
          if (departments.Value.Any())
          {
            filteredEmployees = filteredEmployees.Where(x => departments.Value.Contains(x.Department));
          }
          TimeLastUserConnection.employeeIds = string.Join(",", filteredEmployees.Select(x => x.Id).ToList());
        }
        else
          TimeLastUserConnection.employeeIds = string.Join(",", employees.Value.Select(x => x.Id).ToList());
      }
      else
      {
        e.Cancel = true;
      }
    }
    
    private List<Sungero.Company.IDepartment> GetFilteredDepartments(Sungero.Company.IBusinessUnit businessUnit, bool? filterDepartmentsForBusinessUnits)
    {
      var departments = Sungero.Company.PublicFunctions.Department.Remote.GetDepartments()
        .Where(d => d.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      
      // Подразделения фильтруются по НОР.
      if (filterDepartmentsForBusinessUnits == true && businessUnit != null)
        return departments
          .Where(d => Equals(d.BusinessUnit, businessUnit))
          .ToList();

      // Подразделения не фильтруются по НОР.
      return departments.ToList();
    }
  }
}