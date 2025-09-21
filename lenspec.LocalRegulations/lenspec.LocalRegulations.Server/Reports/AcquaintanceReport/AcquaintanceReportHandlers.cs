using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Company;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow.Task;

namespace lenspec.LocalRegulations
{
  partial class AcquaintanceReportServerHandlers
  {

    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.AcquaintanceReport.SourceTableName, AcquaintanceReport.ReportSessionId);
    }

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      var sourceDocument = AcquaintanceReport.Document;
      var sourceTask = AcquaintanceReport.Task;
      var calledFromDocument = sourceDocument != null;
      var selectedVersionNumber = this.AcquaintanceReport.DocumentVersion;
      
      // Если у документа нет тела, но есть задачи ознакомления, номер версии берем 0, иначе выбранный.
      var versionNumber = 0;
      if (selectedVersionNumber != null)
        versionNumber = Convert.ToInt32(selectedVersionNumber);
      
      var tasks = new List<Sungero.RecordManagement.IAcquaintanceTask>();
      
      if (calledFromDocument)
      {
        // Получить задачи на ознакомление по документу.
        tasks = Sungero.Docflow.PublicFunctions.OfficialDocument.Remote.GetAcquaintanceTasks(sourceDocument);
        // Исключить задачи, которые делились на подзадачи по количеству участников.
        var mainTasks = tasks.Select(x => lenspec.Etalon.AcquaintanceTasks.As(x).TaskWithAllParticipantsavis).Where(x => x != null);
        if (mainTasks.Any())
        {
          tasks = tasks.Where(x => !mainTasks.Contains(x)).ToList();
        }
        // Фильтр по номеру версии.
        tasks = tasks
          .Where(t => t.AcquaintanceVersions.First(v => v.IsMainDocument == true).Number == versionNumber)
          .ToList();
      }
      else
      {
        tasks.Add(sourceTask);
        // Вернуть номер версии только если у документа есть версии, и статус задачи не "Черновик", иначе - 0.
        var acquaintanceVersion = sourceTask.AcquaintanceVersions.FirstOrDefault(v => v.IsMainDocument == true);
        if (acquaintanceVersion != null &&
            (sourceTask.Status == Status.InProcess ||
             sourceTask.Status == Status.Suspended ||
             sourceTask.Status == Status.Completed ||
             sourceTask.Status == Status.Aborted))
        {
          versionNumber = acquaintanceVersion.Number.Value;
        }
        else
        {
          var document = sourceTask.DocumentGroup.OfficialDocuments.First();
          versionNumber = document.HasVersions ? document.LastVersion.Number.Value : 0;
        }
        sourceDocument = sourceTask.DocumentGroup.OfficialDocuments.First();
      }
      
      // Провалидировать подписи версии.
      Sungero.Domain.Shared.IEntity version = null;
      if (versionNumber > 0 && sourceDocument.Versions.Any(v => v.Number == versionNumber))
        version = sourceDocument.Versions.First(v => v.Number == versionNumber).ElectronicDocument;
      var validationMessages = Sungero.RecordManagement.PublicFunctions.Module.GetDocumentSignatureValidationErrors(version, true);
      if (validationMessages.Any())
      {
        validationMessages.Insert(0, Sungero.RecordManagement.Resources.SignatureValidationErrorMessage);
        AcquaintanceReport.SignValidationErrors = string.Join(System.Environment.NewLine, validationMessages);
      }
      
      // Шапка.
      var nonBreakingSpace = Convert.ToChar(160);
      AcquaintanceReport.DocumentHyperlink = Hyperlinks.Get(sourceDocument);
      AcquaintanceReport.DocumentName = Sungero.Docflow.PublicFunctions.Module.FormatDocumentNameForReport(sourceDocument, versionNumber, true);
      
      // Приложения.
      var documentAddenda = new List<Sungero.Content.IElectronicDocument>();
      var addendaIds = tasks.SelectMany(x => x.AcquaintanceVersions)
        .Where(x => x.IsMainDocument != true)
        .Select(x => x.DocumentId);
      var addenda = tasks.SelectMany(x => x.AddendaGroup.OfficialDocuments)
        .Where(x => addendaIds.Contains(x.Id))
        .Distinct()
        .ToList();
      documentAddenda.AddRange(addenda);
      
      if (documentAddenda.Any())
      {
        AcquaintanceReport.AddendaDescription = Reports.Resources.AcquaintanceReport.Addendas;
        foreach (var addendum in documentAddenda)
        {
          var addendumInfo = string.Format("\n - {0} ({1}:{2}{3}).", addendum.DisplayValue.Trim(),
                                           Sungero.Docflow.Resources.Id, nonBreakingSpace, addendum.Id);
          AcquaintanceReport.AddendaDescription += addendumInfo;
        }
      }
      
      // Данные.
      var reportSessionId = System.Guid.NewGuid().ToString();
      AcquaintanceReport.ReportSessionId = reportSessionId;
      var dataTable = new List<Structures.AcquaintanceReport.TableLine>();
      var department = this.AcquaintanceReport.Department;
      
      foreach (var task in tasks)
      {
        var createdDate = Sungero.Docflow.PublicFunctions.Module.ToShortDateShortTime(task.Created.Value.ToUserTime());
        var taskId = task.Id;
        var taskHyperlink = Hyperlinks.Get(task);
        var isElectronicAcquaintance = task.IsElectronicAcquaintance == true;
        var taskDisplayName = isElectronicAcquaintance
          ? Reports.Resources.AcquaintanceReport.ElectronicAcquaintanceTaskDisplayNameFormat(createdDate)
          : Reports.Resources.AcquaintanceReport.SelfSignAcquaintanceTaskDisplayNameFormat(createdDate);
        
        // Фильтрация сотрудников по подразделениям.
        var acquainters = new List<Sungero.Company.IEmployee>();
        // Заполнение AcquaintanceTaskParticipants происходит в схеме.
        // От старта задачи до начала обработки схемы там ничего не будет - взять из исполнителей задачи.
        var storedParticipants = Sungero.RecordManagement.AcquaintanceTaskParticipants.GetAll().FirstOrDefault(x => x.TaskId == task.Id);
        if (storedParticipants != null)
        {
          acquainters = storedParticipants.Employees.Select(p => p.Employee).ToList();
        }
        else
        {
          acquainters = Sungero.RecordManagement.PublicFunctions.AcquaintanceTask.Remote.GetParticipants(task);
        }
        if (AcquaintanceReport.Department != null)
          acquainters = AcquaintanceReport.IncludeSubDepartments == true
            ? acquainters.Where(x => x.IncludedIn(AcquaintanceReport.Department)).ToList()
            : acquainters.Where(x => Equals(x.Department, AcquaintanceReport.Department)).ToList();
        
        #region Вычисление неавтоматизированных сотрудников
        
        var notAutomatedEmployeesIds = lenspec.EtalonDatabooks.PublicFunctions.Module.GetNotAutomatedEmployeesAvis(acquainters).Select(x => x.Id).ToList();
        
        #endregion
        
        foreach (var employee in acquainters)
        {
          // Задание.
          var assignment = Sungero.RecordManagement.AcquaintanceAssignments.GetAll()
            .Where(a => Equals(a.Task, task) && Equals(a.Performer, employee) && a.Created >= task.Started)
            .FirstOrDefault();
          
          // Не включать сотрудника в отчёт, если его задание было снято.
          var isAborted = assignment == null ? false : assignment.Status == Sungero.Workflow.Assignment.Status.Aborted;
          if (isAborted)
            continue;
          
          var newLine = Structures.AcquaintanceReport.TableLine.Create();
          newLine.RowNumber = 0;
          newLine.ReportSessionId = reportSessionId;
          
          // Задача.
          newLine.TaskDisplayName = taskDisplayName;
          newLine.TaskId = taskId;
          newLine.TaskHyperlink = taskHyperlink;
          
          // Сотрудник.
          newLine.ShortName = employee.Person.ShortName;
          newLine.LastName = employee.Person.LastName;
          if (employee.JobTitle != null)
            newLine.JobTitle = employee.JobTitle.DisplayValue;
          newLine.Department = employee.Department.DisplayValue;
          
          if (task.Status != Status.InProcess &&
              task.Status != Status.Suspended &&
              task.Status != Status.Completed)
          {
            if (employee.Status != Sungero.Company.Employee.Status.Closed)
            {
              // Автоматизирован.
              newLine.IsAutomated = notAutomatedEmployeesIds.Any(x => x.Equals(employee.Id))
                ? newLine.IsAutomated = "Нет"
                : newLine.IsAutomated = "Да";
              dataTable.Add(newLine);
            }
            continue;
          }
          
          if (assignment == null)
          {
            if (employee.Status != Sungero.Company.Employee.Status.Closed)
            {
              // Автоматизирован.
              newLine.IsAutomated = notAutomatedEmployeesIds.Any(x => x.Equals(employee.Id))
                ? newLine.IsAutomated = "Нет"
                : newLine.IsAutomated = "Да";
              dataTable.Add(newLine);
            }
            continue;
          }
          
          newLine.AssignmentId = assignment.Id.ToString();
          newLine.AssignmentHyperlink = Hyperlinks.Get(assignment);
          
          var isCompleted = assignment.Status == Sungero.Workflow.Task.Status.Completed;
          if (isCompleted)
          {
            // Дата ознакомления.
            var completed = Calendar.ToUserTime(assignment.Completed.Value);
            newLine.AcquaintanceDate = Sungero.Docflow.PublicFunctions.Module.ToShortDateShortTime(completed);
            
            // Примечание.
            if (!Equals(assignment.CompletedBy, assignment.Performer))
            {
              var completedByShortName = Sungero.Company.Employees.Is(assignment.CompletedBy)
                ? Employees.As(assignment.CompletedBy).Person.ShortName
                : assignment.CompletedBy.Name;
              newLine.Note += string.Format("{0}\n", completedByShortName);
              newLine.Note += string.Format("\"{0}\"", assignment.ActiveText);
            }
            else if (!Equals(assignment.ActiveText, Reports.Resources.AcquaintanceReport.AcquaintedDefaultResult.ToString()))
            {
              newLine.Note += string.Format("\"{0}\"", assignment.ActiveText);
            }
          }
          
          // Статус.
          if (!isCompleted)
          {
            newLine.State = string.Empty;
          }
          else if (Equals(assignment.CompletedBy, assignment.Performer) || !isElectronicAcquaintance)
          {
            newLine.State = Reports.Resources.AcquaintanceReport.AcquaintedState;
          }
          else
          {
            newLine.State = Reports.Resources.AcquaintanceReport.CompletedState;
          }
          
          // Автоматизирован.
          newLine.IsAutomated = notAutomatedEmployeesIds.Any(x => x.Equals(employee.Id))
            ? newLine.IsAutomated = "Нет"
            : newLine.IsAutomated = "Да";
          dataTable.Add(newLine);
        }
      }
      
      // Фильтр по статусу выполнения.
      if (AcquaintanceReport.EmployeesAcquaintanceStatus.Equals(Reports.Resources.AcquaintanceReport.ForAcquaintedPerformers))
        dataTable = dataTable.Where(d => d.State == Reports.Resources.AcquaintanceReport.AcquaintedState).ToList();
      else if (AcquaintanceReport.EmployeesAcquaintanceStatus.Equals(Reports.Resources.AcquaintanceReport.ForNotAcquaintedPerformers))
        dataTable = dataTable.Where(d => d.State != Reports.Resources.AcquaintanceReport.AcquaintedState).ToList();
      
      Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.AcquaintanceReport.SourceTableName, dataTable);
      
      // Подвал.
      var currentUser = Users.Current;
      var printedByName = Sungero.Company.Employees.Is(currentUser)
        ? Sungero.Company.Employees.As(currentUser).Person.ShortName
        : currentUser.Name;
      AcquaintanceReport.Printed = Reports.Resources.AcquaintanceReport.PrintedByFormat(printedByName, Calendar.UserNow);
    }
  }
}