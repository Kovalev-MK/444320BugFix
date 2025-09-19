using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Employee;

namespace lenspec.Etalon.Server
{
  partial class EmployeeFunctions
  {
    /// <summary>
    /// Получить руководителя сотрудника с учетом состояния записи сотрудника, состояние УЗ и совпадения с самим сотрудником.
    /// </summary>
    /// <param name="employee">Сотрудник.</param>
    /// <returns>Руководитель подразделения, либо руководитель НОР.</returns>
    [Public]
    public static Sungero.Company.IEmployee GetDepartmentManagerOrCEO(Sungero.CoreEntities.IUser user)
    {
      var employee = Sungero.Company.Employees.As(user);
      if (employee == null)
        return null;
      
      var department = employee.Department;
      do
      {
        // Если руководитель автоматизирован и не является проверяемым сотрудником.
        if (department.Manager != null && lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(department.Manager) == true && department.Manager != employee && department.Manager.Status == Status.Active)
          return department.Manager;
        
        // Если головного подразделения нету.
        if (department.HeadOffice == null)
          return department.BusinessUnit.CEO;
        
        // Переходим на следующее подразделение.
        department = department.HeadOffice;
      } while (department != null);
      
      return employee;
    }
    
    /// <summary>
    /// Получить руководителя сотрудника с учетом состояния записи сотрудника и совпадения с самим сотрудником, без учета состояние УЗ.
    /// </summary>
    /// <param name="author">Сотрудник.</param>
    /// <returns>Руководитель подразделения, либо руководитель НОР, либо сам сотрудник.</returns>
    [Public]
    public static Sungero.Company.IEmployee GetManagerOrEmployee(Sungero.Company.IEmployee employee)
    {
      var department = employee.Department;
      do
      {
        // Если руководитель автоматизирован и не является проверяемым сотрудником.
        if (department.Manager != null && lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(department.Manager) == true && department.Manager != employee && department.Manager.Status == Status.Active)
          return department.Manager;
        
        // Если головного подразделения нету.
        if (department.HeadOffice == null && department.BusinessUnit != null && department.BusinessUnit.CEO != null)
          return department.BusinessUnit.CEO;
        
        // Переходим на следующее подразделение.
        department = department.HeadOffice;
      } while (department != null);
      
      return employee;
    }

    /// <summary>
    /// Сформировать всплывающую подсказку о сотруднике в виде модели всплывающего окна.
    /// </summary>
    /// <returns>Всплывающая подсказка о сотруднике в виде модели всплывающего окна.</returns>
    /// <remarks>Используется в подсказке о сотруднике.</remarks>
    public override Sungero.Core.IDigestModel GetEmployeePopup()
    {
      if (_obj.IsSystem == true)
        return null;
      
      var digest = Sungero.Core.UserDigest.Create(_obj);
      if (_obj.Department != null)
        digest.AddEntity(_obj.Department);
      
      // Вывод должноти.
      if (_obj.JobTitle != null && _obj.BusinessUnitlenspec != null)
        digest.AddLabel($"{_obj.JobTitle.Name}, {_obj.BusinessUnitlenspec.Name}");
      else if (_obj.JobTitle != null)
        digest.AddLabel($"{_obj.JobTitle.Name}");
      else if (_obj.BusinessUnitlenspec != null)
        digest.AddLabel($"{_obj.BusinessUnitlenspec.Name}");
      
      //вычислим руководителя
      if (!string.IsNullOrWhiteSpace(_obj.Phone))
        digest.AddLabel(string.Format("{0} {1}", Sungero.Company.Employees.Resources.PopupPhoneDescription, _obj.Phone));
      {
        var manager = GetManagerOrEmployee(_obj);
        
        if (manager != null)
          digest.AddEntity(manager, Sungero.Company.Employees.Resources.PopupManagerDescription);
      }
      return digest;
    }
    
    /// <summary>
    /// Получить пользователей AD по ФИО.
    /// </summary>
    /// <param name="lastName">Фамилия.</param>
    /// <param name="firstName">Имя.</param>
    /// <param name="middleName">Отчество.</param>
    /// <returns>Список загруженных пользователей.</returns>
    [Public]
    public static List<string> GetEmployeeADFromIntegraDB(string lastName, string firstName, string middleName)
    {
      var settings = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.Constants.Module.ActiveDirectoryEmployeeRecordCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      
      // инициализируем helper для получения данных из интеграционной базы
      var settingsString = Encryption.Decrypt(settings.ConnectionParams);
      var connectionString = settingsString.Split('|');
      if (string.IsNullOrEmpty(connectionString[0]))
        throw AppliedCodeException.Create("Не правильная строка подключения к БД.");
      
      var employeeADRepository = new AvisIntegrationHelper.Repositories.EmployeeADRepository(connectionString[0]); // new AvisExpert.EtalonIntegraionHelper.Repositories.EmployeeADRepository(connectionString[0]);
      
      // Генерируем список пользователей.
      var employees = employeeADRepository.GetList(lastName, firstName, middleName, true);
      
      if (employees.Count == 0)
        throw AppliedCodeException.Create(lenspec.Etalon.Employees.Resources.EmployeeNotFoundErrorMessage);
      
      var peoples = new List<string>();
      
      foreach (var employee in employees)
        peoples.Add($"{employee.Id}-{employee.LastName} {employee.FirstName} {employee.MiddleName}-{employee.JobTitle}-{employee.Company}");
      
      return peoples;
    }

    /// <summary>
    /// Получить пользователей AD по ФИО.
    /// </summary>
    /// <param name="people">выбранный пользователь.</param>
    [Public]
    public static void CreateLogin(string people, IEmployee employee)
    {
      var settings = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.Constants.Module.ActiveDirectoryEmployeeRecordCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      
      // инициализируем helper для получения данных из интеграционной базы
      var settingsString = Encryption.Decrypt(settings.ConnectionParams);
      var connectionString = settingsString.Split('|');
      if (string.IsNullOrEmpty(connectionString[0]))
        return;// lenspec.Etalon.BusinessUnits.Resources.FailedToGetConnectionString;
      
      var employeeADRepositor = new AvisIntegrationHelper.Repositories.EmployeeADRepository(connectionString[0]); // new AvisExpert.EtalonIntegraionHelper.Repositories.EmployeeADRepository(connectionString[0]);
      
      // Получаем логин выбранного пользователя
      var employeeAdId = people.Split('-')[0];
      var employeeAd = employeeADRepositor.GetById(Convert.ToInt32(employeeAdId), true);
      
      // Создаём новый логин в директуме
      var login = Sungero.CoreEntities.Logins.Create();
      login.LoginName = employeeAd.Login;
      login.Save();
      
      // Заполняем логин у пользователя
      employee.Login = login;
    }
    
    /// <summary>
    /// Синхронизировать сотрудника в роль по Виду должности.
    /// </summary>
    public void SynchronizeEmployeeInRole()
    {
      var originalRole = _obj.State.Properties.JobTitleKindlenspec.OriginalValue != null
        ? _obj.State.Properties.JobTitleKindlenspec.OriginalValue.Role
        : null;
      var newRole = _obj.JobTitleKindlenspec != null
        ? Sungero.CoreEntities.Roles.Get(_obj.JobTitleKindlenspec.Role.Id)
        : null;
      // Завершить, если состояние записи не изменилось и (не поменялся Вид должности или запись так и осталась закрытой).
      if (_obj.State.Properties.Status.OriginalValue == _obj.Status
          && (Equals(originalRole, newRole) || _obj.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed))
        return;
      
      // Добавить сотрудника в роль, соответствующую Виду должности, если запись действующая и сотрудник не включен в новую роль.
      // Если запись закрыта, пропускать любое включение в роль.
      if (_obj.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed && newRole != null && !_obj.IncludedIn(newRole))
      {
        var newRoleRecipients = newRole.RecipientLinks;
        newRoleRecipients.AddNew().Member = _obj;
      }
      
      // Удалить сотрудника из участников роли, соответствующей прошлому Виду должности.
      if(originalRole != null && _obj.IncludedIn(originalRole))
      {
        //cесли запись стала закрытой, и сотрудник входит в роль, сохраненную в БД.
        if(_obj.State.Properties.Status.OriginalValue != _obj.Status && _obj.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed
           || _obj.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed && !Equals(originalRole, newRole))
        {
          var originalRoleRecipients = originalRole.RecipientLinks;
          while (originalRoleRecipients.Any(r => Equals(r.Member, _obj)))
            originalRoleRecipients.Remove(originalRoleRecipients.First(r => Equals(r.Member, _obj)));
        }
      }
    }
    
    /// <summary>
    /// Получить всех активных сотрудников в подразделении с указанным видом должности
    /// </summary>
    /// <param name="department">Подразделение</param>
    /// <param name="fobTitleKindName">Наименование вида должности</param>
    /// <returns>Список сотрудников</returns>
    [Remote, Public]
    public static List<Sungero.Company.IEmployee> GetEmployeesFromDepartmentByJobTitleKind(Sungero.Company.IDepartment department, string jobTitleKindName)
    {
      var employees = new List<Sungero.Company.IEmployee>();
      if (department == null || string.IsNullOrEmpty(jobTitleKindName))
        return employees;
      
      foreach (var line in department.RecipientLinks)
      {
        var employeeInDepartment = lenspec.Etalon.Employees.As(line.Member);
        if (employeeInDepartment != null && employeeInDepartment.Status == Etalon.Employee.Status.Active &&
            employeeInDepartment.JobTitleKindlenspec != null && employeeInDepartment.JobTitleKindlenspec.Name.Equals(jobTitleKindName))
        {
          employees.Add(employeeInDepartment);
        }
      }
      return employees;
    }
    
    //конец Добавлено Avis Expert
  }
}