using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Company;
using lenspec.AutomatedSupportTickets;
using Sungero.Domain.Shared;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace lenspec.AutomatedSupportTickets.Server
{
  public class ModuleFunctions
  {

    /// <summary>
    ///  Получаем данные из Excel
    /// </summary>
    /// <param name="body">Содержимое файла</param>
    /// <param name="path">Имя файла</param>
    /// <param name="roleid">Ид группы куда добвляются пользователи</param>
    /// <returns>Данные из Excel</returns>
    [Public]
    public string TestParseExcel(byte[] body, string path, long roleid)
    {
      var role = Sungero.CoreEntities.Roles.GetAll(x => x.Id == roleid).FirstOrDefault();
      // Получение тела документа
      var stream = new System.IO.MemoryStream(body);
      var document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Open(stream, false);
      
      // Получение первого листа
      var wbPart = document.WorkbookPart;
      var sheet = wbPart.Workbook.Descendants<DocumentFormat.OpenXml.Spreadsheet.Sheet>().First();

      
      // Получение диапазона заполненного данными
      var wsPart = (DocumentFormat.OpenXml.Packaging.WorksheetPart)(wbPart.GetPartById(sheet.Id));
      var rows = wsPart.Worksheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Row>();
      
      // Если данных нет - закончить обработку
      if (rows.Count() == 0)
        return "Нет данных для заполнения";
      
      // Получить и обработать данные
      foreach (var row in rows)
      {
        //Получаем первую ячейку
        Cell firstCell = row.Descendants().FirstOrDefault() as Cell;
        if (firstCell != null)
        {
          string cellValue = GetCellValue(wbPart, firstCell);
          if (cellValue != null)
          {
            var employe = Etalon.Employees.GetAll(x => cellValue == x.Name && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).FirstOrDefault();
            if (employe != null)
            {
              var recipient = role.RecipientLinks.AddNew();
              recipient.Member = employe;
              role.Save();
            }
          }
        }
      }
      var result = "Вроде работает";
      
      return result;
    }
    
    private static string GetCellValue(WorkbookPart workbookPart, Cell cell)
    {
      string value = cell.InnerText;
      if (cell.DataType != null)
      {
        int sharedStringIndex;
        if (int.TryParse(value, out sharedStringIndex))
        {
          var sharedStringTablePart = workbookPart.SharedStringTablePart;
          if (sharedStringTablePart == null)
          {
            return value;
          }
          var sharedStringTable = sharedStringTablePart.SharedStringTable;
          if (sharedStringTable == null)
          {
            return value;
          }
          if (sharedStringIndex >= 0 && sharedStringIndex < sharedStringTable.Count)
          {
            string sharedStringValue = sharedStringTable.ElementAt(sharedStringIndex).InnerText;
            value = sharedStringValue;
            return value;
          }
          else
          {
            return value;
          }
        }
        else
        {
          return value;
        }
      }
      else
      {
        return value;
      }
    }
    
    /// <summary>
    /// Создать заявку на изменение компонентов Directum RX.
    /// </summary>
    /// <returns>Заявка на формирование замещения.</returns>
    [Remote, Public]
    public static AutomatedSupportTickets.IEditComponentRXRequestTask CreateEditComponentRXRequestTask()
    {
      return AutomatedSupportTickets.EditComponentRXRequestTasks.Create();
    }

    /// <summary>
    /// Функция для получения данных для отчета Указание пользователей в объектах системы
    /// </summary>
    /// <param name="businessUnits">Наши организации</param>
    /// <param name="approvalRules">Регламенты</param>
    /// <param name="roleKinds">Роли</param>
    /// <returns>Данные для отчета.</returns>
    [Public]
    public virtual List<lenspec.AutomatedSupportTickets.Structures.ReconciliationSettings.SpecifyingReportTableLine> GetAllDataReconciliationSettings(System.Collections.Generic.IEnumerable<Sungero.Company.IBusinessUnit> businessUnits,
                                                                                                                                                      System.Collections.Generic.IEnumerable<Sungero.Docflow.IApprovalRuleBase> approvalRules,
                                                                                                                                                      System.Collections.Generic.IEnumerable<lenspec.EtalonDatabooks.IRoleKind> roleKinds,
                                                                                                                                                      string status)
    {
      var result = new List<lenspec.AutomatedSupportTickets.Structures.ReconciliationSettings.SpecifyingReportTableLine>();
      var rolesFiltered  = new List<lenspec.EtalonDatabooks.IRoleKind>();
      var filteredApprovalSettings = lenspec.EtalonDatabooks.ApprovalSettings.GetAll();
      if (status != null)
      {
        //Отфильтруем настройки по состоянию
        if (status == "Действующая")
        {
          filteredApprovalSettings = filteredApprovalSettings.Where(z => z.Status == lenspec.EtalonDatabooks.ApprovalSetting.Status.Active);
        }
        else filteredApprovalSettings = filteredApprovalSettings.Where(a => a.Status == lenspec.EtalonDatabooks.ApprovalSetting.Status.Closed);
      }
      if (businessUnits.Any())
      {
        var businessUnitsIds = businessUnits.Select(x => x.Id).ToList();
        filteredApprovalSettings = filteredApprovalSettings.Where(x => x.BusinessUnit != null && businessUnitsIds.Contains(x.BusinessUnit.Id));
      }
      if (approvalRules.Any())
      {
        filteredApprovalSettings = filteredApprovalSettings.Where(x => approvalRules.Contains(x.ApprovalRule));
      }
      if (roleKinds.Any())
      {
        filteredApprovalSettings = filteredApprovalSettings.Where(x => x.RoleKindEmployee.Any(y => roleKinds.Contains(y.RoleKind)));
      }
      foreach (var approvalSetting in filteredApprovalSettings)
      {
        var approvalSettingId = approvalSetting.Id;
        var businessUnit      = approvalSetting.BusinessUnit != null ? approvalSetting.BusinessUnit.Name : string.Empty;
        var reglament         = approvalSetting.ApprovalRule.Name;
        var hyperlinkId       = Sungero.Core.Hyperlinks.Get(lenspec.EtalonDatabooks.ApprovalSettings.GetAll(x => x.Id == approvalSettingId).SingleOrDefault());
        var sost              = approvalSetting.Info.Properties.Status.GetLocalizedValue(approvalSetting.Status);
        foreach (var roles in approvalSetting.RoleKindEmployee)
        {
          var role      = roles.RoleKind.Name;
          var employee  = roles.Employee.Name;
          var note      = string.Empty;
          if (approvalSetting.BusinessUnit != null)
          { if (approvalSetting.BusinessUnit.RoleKindEmployeelenspec.Any(x => x.RoleKind.Id == roles.RoleKind.Id))
            {
              note = lenspec.AutomatedSupportTickets.Resources.AddedFromBusinessUnit;
            }
          }
          //Сформируем строку данных
          var check = CreateTableLineReconciliationSettings(approvalSettingId, businessUnit, reglament, role, employee, note, hyperlinkId, sost);
          result.Add(check);
        }
      }
      return result;
    }

    /// <summary>
    /// Функция для получения данных для отчета Указание пользователей в объектах системы
    /// </summary>
    /// <param name="employee">Сотрудник для обработки.</param>
    /// <param name="sign">Состояние сотрудника</param>
    /// <returns>Данные для отчета.</returns>
    [Public]
    public virtual List<lenspec.AutomatedSupportTickets.Structures.SpecifyingUsersSystemObjects.SpecifyingReportTableLine> GetAllDataSpecifyingUsersSystemObjects(System.Collections.Generic.IEnumerable<Sungero.Company.IEmployee> employees)
    {
      var result = new List<lenspec.AutomatedSupportTickets.Structures.SpecifyingUsersSystemObjects.SpecifyingReportTableLine>();
      
      if (!employees.Any())
        return result;
      
      foreach(var employee in employees)
      {
        //ФИО пользователя
        var employeeFIO = employee.Name;
        
        //ИД пользователя
        var employeeID  = employee.Id;
        
        //Должность
        var employeeJobTitle = string.Empty;
        if (employee.JobTitle != null)
        {
          employeeJobTitle = employee.JobTitle.Name;
        }
        
        //Подразделения и Наша организация
        var employeeDep = string.Empty;
        var ourOrg = string.Empty;
        if (employee.Department != null)
        {
          employeeDep = employee.Department.ShortName;
          if (employee.Department.BusinessUnit != null)
          {
            ourOrg = employee.Department.BusinessUnit.Name;
          }
        }
        
        //Состояние учетной записи
        var accountStatus = string.Empty;
        if (employee.Login != null)
        {
          accountStatus = employee.Login.Info.Properties.Status.GetLocalizedValue(employee.Login.Status);
        }
        
        //Состояние записи Сотрудника
        var employeeRecordStatus = employee.Info.Properties.Status.GetLocalizedValue(employee.Status);
        
        //Роли
        var recipients = Roles.GetAll().Where(r => r.RecipientLinks.Any(e => Equals(e.Member, employee)));
        var roles = string.Join(",", recipients.Select(n => n.Name).ToList());
        
        //Роли в НОР
        var allroles = lenspec.Etalon.BusinessUnits.GetAll().Where(x => x.RoleKindEmployeelenspec.Any(y => y.Employee.Equals(employee)));
        var allrolesNor = string.Empty;
        foreach (var ar in allroles)
        {
          var roleKinds = ar.RoleKindEmployeelenspec.Where(y => y.Employee.Equals(employee));
          foreach (var rk in roleKinds)
          {
            allrolesNor += ar.Name + " - " + rk.RoleKind.Name + ";" + Environment.NewLine;
          }
        }
        
        //Замещения
        var substitution = Substitutions.GetAll()
          .Where(s => s.IsSystem != true)
          .Where(s => (Equals(s.Substitute, employee) || Equals(s.User, employee)) && (!s.EndDate.HasValue || s.EndDate >= Calendar.UserToday));
        var substitutions = string.Join(Environment.NewLine, substitution.Select(x => CreateSubstitutionPresentation(x)).ToList());
        
        //Количество задач в работе
        var allTasks = Sungero.Workflow.Tasks.GetAll(x => x.Status == Sungero.Workflow.Task.Status.InProcess && x.Author.Equals(employee)).Count();
        
        //Количество заданий в работе
        var allJobs  = Sungero.Workflow.Assignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee)).Count();
        
        //Настройки согласования
        var approvalSetting = lenspec.EtalonDatabooks.ApprovalSettings.GetAll().Where(x => x.RoleKindEmployee.Any(y => y.Employee.Equals(employee)));
        var approvalSettings = string.Join(Environment.NewLine, approvalSetting.ToList());
        
        //Этапы согласования
        var approvalStage = lenspec.Etalon.ApprovalStages.GetAll().Where(x => x.Recipients.Any(y => y.Recipient.Equals(employee)));
        var approvalStages = string.Join(",", approvalStage.ToList());
        
        //Группы регистрации
        var registrationGroup = lenspec.Etalon.RegistrationGroups.GetAll().Where(x => x.RecipientLinks.Any(y => y.Member.Equals(employee)));
        var registrationGroups = string.Join(",", registrationGroup.ToList());
        
        //Право подписи
        var signatureSetting = lenspec.Etalon.SignatureSettings.GetAll().Where(x => x.Recipient.Equals(employee));
        var signatureSettings = string.Empty;
        foreach (var ss in signatureSetting)
        {
          signatureSettings += lenspec.AutomatedSupportTickets.Resources.SpecifyingUsersSignatureSettingsFormat(ss.Id, ss.Note) + Environment.NewLine;
        }
        
        //Ответственный за контрагентов
        var responsibleContractor = lenspec.Etalon.CompanyBases.GetAll().Where(x => x.Responsible.Equals(employee));
        var responsibleContractors = string.Join(",", responsibleContractor.ToList());
        
        //Условия в правилах согласования
        var conditionBases = lenspec.Etalon.Conditions.GetAll().Where(x => employee.Equals(x.RecipientForComparison));
        var conditionBase = string.Empty;
        foreach(var cb in conditionBases)
        {
          var approvalRules = Sungero.Docflow.ApprovalRuleBases.GetAll().Where(x => x.Conditions.Any(y => y.Condition.Id == cb.Id));
          foreach (var approvalRule in approvalRules)
          {
            conditionBase += lenspec.AutomatedSupportTickets.Resources.SpecifyingUsersApprovalConditionFormat(approvalRule.Name, cb) + Environment.NewLine;
          }
        }

        //Настройка исполнения по обращениям клиентов
        var custReqSetup = avis.CustomerRequests.CustReqSetups.GetAll().Where(x => employee.Equals(x.Controller) || employee.Equals(x.Executor));
        var custReqSetups = string.Join(",", custReqSetup.ToList());
        
        //Цифровые сертификаты
        var certificate = GetCertificatesOfEmployee(employee);
        var certificates = string.Join(",", certificate.ToList());
        
        //Ассисент руководителя
        var managersAssistant = Sungero.Company.ManagersAssistants.GetAll().Where(x => employee.Equals(x.Assistant));
        var managersAssistants = string.Join(",", managersAssistant.ToList());
        
        //Коллегиальные органы
        var collegialBody = lenspec.ProtocolsCollegialBodies.CollegialBodies.GetAll().Where(x => x.CollectionProperty.Any(y => y.Member.Equals(employee)) || x.Chairman.Equals(employee));
        var collegialBodies = string.Join(",", collegialBody.ToList());
        
        //Сформируем строку данных
        var check = CreateAllDataTableLine(employeeFIO, employeeID, employeeJobTitle,
                                           employeeDep, ourOrg, accountStatus,
                                           employeeRecordStatus, roles, allrolesNor, substitutions,
                                           allTasks, allJobs, approvalSettings, approvalStages, registrationGroups,
                                           signatureSettings, responsibleContractors, conditionBase, custReqSetups,
                                           certificates, managersAssistants, collegialBodies);
        result.Add(check);
      }
      return result;
    }
    
    /// <summary>
    /// Строит строку данных для отчета.
    /// </summary>
    /// <returns>Строка данных отчета.</returns>
    [Public]
    public static Structures.SpecifyingUsersSystemObjects.SpecifyingReportTableLine CreateAllDataTableLine(string employeeFIO,
                                                                                                           long employeeID,
                                                                                                           string employeeJobTitle,
                                                                                                           string employeeDep,
                                                                                                           string ourOrg,
                                                                                                           string accountStatus,
                                                                                                           string employeeRecordStatus,
                                                                                                           string roles,
                                                                                                           string allrolesNor,
                                                                                                           string substitutions,
                                                                                                           int alltasks,
                                                                                                           int alljobs,
                                                                                                           string approvalSettings,
                                                                                                           string approvalStages,
                                                                                                           string registrationGroups,
                                                                                                           string signatureSettings,
                                                                                                           string responsibleContractors,
                                                                                                           string conditionBase,
                                                                                                           string custReqSetups,
                                                                                                           string certificates,
                                                                                                           string managersAssistants,
                                                                                                           string collegialBodies)
    {
      var newTableLine = new Structures.SpecifyingUsersSystemObjects.SpecifyingReportTableLine();
      newTableLine.Employee = employeeFIO;
      newTableLine.EmployeeID = employeeID;
      newTableLine.JobTitle = employeeJobTitle;
      newTableLine.Department = employeeDep;
      newTableLine.OurOrg = ourOrg;
      newTableLine.AccountStatus = accountStatus;
      newTableLine.EmployeeRecordStatus = employeeRecordStatus;
      newTableLine.Roles = roles;
      newTableLine.AllrolesNor = allrolesNor;
      newTableLine.Substitutions = substitutions;
      newTableLine.Alltasks = alltasks;
      newTableLine.Alljobs = alljobs;
      newTableLine.ApprovalSettings = approvalSettings;
      newTableLine.ApprovalStages = approvalStages;
      newTableLine.RegistrationGroups = registrationGroups;
      newTableLine.SignatureSettings = signatureSettings;
      newTableLine.ResponsibleContractors = responsibleContractors;
      newTableLine.ConditionBases = conditionBase;
      newTableLine.CustReqSetups = custReqSetups;
      newTableLine.Certificates = certificates;
      newTableLine.ManagersAssistants = managersAssistants;
      newTableLine.CollegialBodies = collegialBodies;
      return newTableLine;
    }
    
    /// <summary>
    /// Строит строку данных для отчета по настройкам согласования.
    /// </summary>
    /// <returns>Строка данных отчета.</returns>
    [Public]
    public static Structures.ReconciliationSettings.SpecifyingReportTableLine CreateTableLineReconciliationSettings(long id,
                                                                                                                    string businessUnits,
                                                                                                                    string approvalSettings,
                                                                                                                    string roleKinds,
                                                                                                                    string employee,
                                                                                                                    string note,
                                                                                                                    string hyperlinkID,
                                                                                                                    string sost)
    {
      var newTableLine = new Structures.ReconciliationSettings.SpecifyingReportTableLine();
      newTableLine.Id               = id;
      newTableLine.BusinessUnits    = businessUnits;
      newTableLine.ApprovalSettings = approvalSettings;
      newTableLine.RoleKinds        = roleKinds;
      newTableLine.Employee         = employee;
      newTableLine.Note             = note;
      newTableLine.HyperlinkID      = hyperlinkID;
      newTableLine.Status           = sost;
      return newTableLine;
    }
    
    /// <summary>
    /// Сформировать информацию о замещении.
    /// </summary>
    /// <param name="substitution">Замещение.</param>
    /// <returns>Информация о замещении.</returns>
    public static string CreateSubstitutionPresentation(ISubstitution substitution)
    {
      var startDate = substitution.StartDate.HasValue ? string.Format("{0} {1}", Resources.From, substitution.StartDate.Value.ToShortDateString()) : string.Empty;
      var endDate = substitution.EndDate.HasValue ? string.Format("{0} {1}", Resources.To, substitution.EndDate.Value.ToShortDateString()) : string.Empty;
      var period = string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate) ? Resources.Permanently : string.Format("{0} {1}", startDate, endDate).Trim();
      return string.Format("{0}: {1}{2}{3}: {4}{2}{5}: {6}",
                           Resources.Substitute,
                           Sungero.Company.PublicFunctions.Employee.GetShortName(GetEmployeeById(substitution.Substitute.Id), false),
                           Environment.NewLine,
                           Resources.Employee,
                           Sungero.Company.PublicFunctions.Employee.GetShortName(GetEmployeeById(substitution.User.Id), false),
                           Resources.Period,
                           period,
                           Environment.NewLine);
    }
    
    /// <summary>
    /// Получить сотрудника по id.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <returns>Сотрудник.</returns>
    public static IEmployee GetEmployeeById(long id)
    {
      return Employees.Get(id);
    }
    
    /// <summary>
    /// Получить сертификаты сотрудника.
    /// </summary>
    /// <param name="employee">Сотрудник.</param>
    /// <returns>Список сертификатов.</returns>
    public static IQueryable<ICertificate> GetCertificatesOfEmployee(IEmployee employee)
    {
      return Certificates.GetAll(x => Equals(x.Owner, employee));
    }

    //Добавлено Avis Expert
    /// <summary>
    /// Создать заявку на формирование замещения.
    /// </summary>
    /// <returns>Заявка на формирование замещения.</returns>
    [Remote, Public]
    public static AutomatedSupportTickets.ISubstitutionRequestTask CreateSubstitutionRequest()
    {
      return AutomatedSupportTickets.SubstitutionRequestTasks.Create();
    }
    
    //Добавлено Avis Expert
    /// <summary>
    /// Проверить, автоматизирован ли сотрудник.
    /// </summary>
    /// <returns>True, если сотрудник автоматизирован, иначе false.</returns>
    [Remote, Public]
    public bool CheckAutomatedUser(Sungero.Company.IEmployee employee)
    {
      var notAutomatedEmployeesIds = Sungero.Company.PublicFunctions.Module.Remote.GetNotAutomatedEmployees(new List<Sungero.Company.IEmployee> () { employee })
        .Select(x => x.Id)
        .ToList();
      if (notAutomatedEmployeesIds.Any() && notAutomatedEmployeesIds.Contains(employee.Id))
      {
        return false;
      }
      else
      {
        return true;
      }
    }
    //конец Добавлено Avis Expert
  }
}