using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Employee;

namespace lenspec.Etalon.Client
{
  partial class EmployeeFunctions
  {
    //Добавлено Avis Expert
    
    /// <summary>
    /// Загрузить пользователей из AD.
    /// </summary>
    public static void DownloadEmployeeADFromIntegraDB(IEmployee employee)
    {
      // Создать диалог ввода.
      var dialog = Dialogs.CreateInputDialog(lenspec.Etalon.Employees.Resources.FindToAD);
      
      // Добавить поле для ввода ФИО.
      var lastName = dialog.AddString(lenspec.Etalon.Employees.Resources.LastName, false);
      var firstName = dialog.AddString(lenspec.Etalon.Employees.Resources.FirstName, false);
      var middleName = dialog.AddString(lenspec.Etalon.Employees.Resources.MiddleName, false);

      // Заполняем поля, ФИО если они уже заполнены в учетной записи
      if (employee.Person != null && employee.Person.LastName != null)
        lastName.Value = employee.Person.LastName;
      
      if (employee.Person != null && employee.Person.FirstName != null)
        firstName.Value = employee.Person.FirstName;
      
      if (employee.Person != null && employee.Person.MiddleName != null)
        middleName.Value = employee.Person.MiddleName;
      
      // кнопка "Поиск"
      var btnFind = dialog.Buttons.AddOk();
      btnFind.Name = lenspec.Etalon.Employees.Resources.FindButton;
      // кнопка "Отмена"
      var btnCancel = dialog.Buttons.AddCancel();
      btnCancel.Name = lenspec.Etalon.Employees.Resources.CancelButton;
      
      // Кнопка "поиск"
      if (dialog.Show() == btnFind)
      {
        if (string.IsNullOrWhiteSpace(lastName.Value) && 
            string.IsNullOrWhiteSpace(firstName.Value) && 
            string.IsNullOrWhiteSpace(middleName.Value))
        {
          ShowDialogErrorMessage("Все поля пустые.","Заполните хотя бы одно поле для поиска сотрудника.");
          return;
        }
        
        try
        {
           var peoples = PublicFunctions.Employee.GetEmployeeADFromIntegraDB(lastName.Value, firstName.Value, middleName.Value);
           SelectEmployee(peoples, employee);
        }
        catch (Exception ex)
        {
          ShowDialogErrorMessage("Сотрудник не найден.", ex.Message);
        }
      }
    }
    
    /// <summary>
    /// Выбор определенного пользователя
    /// </summary>
    public static void SelectEmployee(List<string> peoples, IEmployee employee)
    {
      // Создаём диалоговое окно
      var dialog = Dialogs.CreateInputDialog("Найденные сотрудники в Active Directory");
      var employeeSelect = dialog.AddSelect("Сотрудник", true, peoples[0]).From(peoples.ToArray());
      
      // кнопка "Поиск"
      var btnSelect = dialog.Buttons.AddOk();
      btnSelect.Name = "Выбрать";
      // кнопка "Отмена"
      var btnCancel = dialog.Buttons.AddCancel();
      btnCancel.Name = "Отмена";
      
      if (dialog.Show() == btnSelect)
        PublicFunctions.Employee.CreateLogin(employeeSelect.Value, employee);
    }
    
    private static void ShowDialogErrorMessage(string title, string text)
    {
      var dialog = Dialogs.CreateTaskDialog(title, text, MessageType.Information, "Уведомление");
      
      var buttonCancel = dialog.Buttons.AddCancel();
      buttonCancel.Name = "Закрыть";
      dialog.Show();
    }
    //конец Добавлено Avis Expert
  }
}