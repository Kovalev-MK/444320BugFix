using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets
{
  partial class SpecifyingUsersSystemObjectsClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Указание пользователей в объектах системы");
      var reportSessionId = System.Guid.NewGuid().ToString();
      var businessUnitsDefault = Sungero.Company.BusinessUnits.Null;
      var businessUnits = dialog.AddSelect("Наши организации", false, businessUnitsDefault);
      //      businessUnits.IsEnabled = true;
      //      businessUnits.IsVisible = true;
      //      var selectedBussinesUnitText = dialog.AddMultilineString("Наши организации", false, string.Empty).RowsCount(3);
      //      selectedBussinesUnitText.IsEnabled = false;
      //      var addBussinesUnitLink = dialog.AddHyperlink("Добавить наши организации");
      //      var deleteBussinesUnitLink = dialog.AddHyperlink("Исключить наши организации");
      var nor = Sungero.Company.BusinessUnits.GetAll();
      var employeesDefault = new List<Sungero.Company.IEmployee>();
      var employees = dialog.AddSelectMany("Сотрудники", true, employeesDefault.ToArray());
      employees.IsEnabled = false;
      employees.IsVisible = false;
      var selectedEmployeesText = dialog.AddMultilineString("Сотрудники", true, string.Empty).WithRowsCount(3);
      selectedEmployeesText.IsEnabled = false;
      var addEmployeesLink = dialog.AddHyperlink("Добавить сотрудников");
      var deleteEmployeesLink = dialog.AddHyperlink("Исключить сотрудников");
      var allemp = Sungero.Company.Employees.GetAll();
      
      #region Состояние
      
      var actionStatus = dialog.AddSelect("Состояние", true, 0).From("Действующая", "Закрытая");

      #endregion
      
      //      #region Наши организации
//
      //      addBussinesUnitLink.SetOnExecute(
      //        () =>
      //        {
      //          var selectedBussinesUnits = nor.ShowSelectMany().AsEnumerable();
      //          if (selectedBussinesUnits != null && selectedBussinesUnits.Any())
      //          {
      //            var sourceBussinesUnits = businessUnits.Value.ToList();
      //            sourceBussinesUnits.AddRange(selectedBussinesUnits);
      //            businessUnits.Value = sourceBussinesUnits.Distinct();
      ////            selectedBussinesUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
      //          }
      //        });
      //      deleteBussinesUnitLink.SetOnExecute(
      //        () =>
      //        {
      //          var selectedBussinesUnits = businessUnits.Value.ShowSelectMany("Выберите наши организации для исключения");
      //          if (selectedBussinesUnits != null && selectedBussinesUnits.Any())
      //          {
      //            var currentBussinesUnits = businessUnits.Value.ToList();
      //            foreach (var bussinesUnit in selectedBussinesUnits)
      //            {
      //              currentBussinesUnits.Remove(bussinesUnit);
      //            }
      //            businessUnits.Value = currentBussinesUnits;
      ////            selectedBussinesUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
      //            var employeesToRemove = employees.Value.Where(x => selectedBussinesUnits.Contains(x.Department.BusinessUnit)).ToList();
      //            var currentEmployees = employees.Value.ToList();
      //            foreach (var employee in employeesToRemove)
      //            {
      //              currentEmployees.Remove(employee);
      //            }
      //            employees.Value = currentEmployees;
      //            selectedEmployeesText.Value = string.Join("; ", employees.Value.Select(x => x.Name));
      //          }
      //        });
//
      //      #endregion
      
      #region Сотрудники
      
      addEmployeesLink.SetOnExecute(
        () =>
        {
          var filteredEmployees = allemp;
          if (businessUnits.Value != null)
          {
            filteredEmployees = filteredEmployees.Where(x => x.Department.BusinessUnit.Id == businessUnits.Value.Id);
          }
          if (actionStatus.Value != "Все")
          {
            if (actionStatus.Value == "Действующая")
            {
              filteredEmployees = filteredEmployees.Where(x => x.Status == Sungero.Company.Employee.Status.Active);
            }
            else filteredEmployees = filteredEmployees.Where(x => x.Status == Sungero.Company.Employee.Status.Closed);
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
      
      #region Проверка состояния
      actionStatus.SetOnValueChanged(x =>
                                     {
                                       businessUnits.Value = businessUnits.Value;
                                       if (x.NewValue != x.OldValue)
                                       {
                                         if (actionStatus.Value != null)
                                         {
                                           var employeesToRemove = employees.Value;
                                           if (actionStatus.Value == "Действующая")
                                           {
                                             employeesToRemove.Where(z => z.Status == Sungero.Company.Employee.Status.Active).ToList();
                                           }
                                           else employeesToRemove.Where(a => a.Status == Sungero.Company.Employee.Status.Closed).ToList();
                                           var currentEmployees = employees.Value.ToList();
                                           foreach (var employee in employeesToRemove)
                                           {
                                             currentEmployees.Remove(employee);
                                           }
                                           employees.Value = currentEmployees;
                                           selectedEmployeesText.Value = string.Join("; ", employees.Value.Select(y => y.Name).ToList());
                                         }
                                       }
                                     });
      #endregion
      
      #region Нажатие Ок в Диалоге
      if (dialog.Show() == DialogButtons.Ok)
      {
        if (employees.Value.Count() == 0)
        {
          if (businessUnits.Value != null)
          {
            allemp = allemp.Where(x => x.Department.BusinessUnit.Id == businessUnits.Value.Id);
          }
          if (actionStatus.Value == "Действующая")
          {
            allemp = allemp.Where(x => x.Status == Sungero.Company.Employee.Status.Active);
          }
          if (actionStatus.Value == "Закрытая")
          {
            allemp = allemp.Where(x => x.Status == Sungero.Company.Employee.Status.Closed);
          }
          employees.Value = allemp.Distinct();
        }
        SpecifyingUsersSystemObjects.employees.AddRange(employees.Value);
        SpecifyingUsersSystemObjects.sign = actionStatus.Value;
        SpecifyingUsersSystemObjects.ReportSessionId = reportSessionId;
      }
      else
        e.Cancel = true;
      #endregion
    }
  }
}
