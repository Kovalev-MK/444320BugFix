using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Company;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow.Task;

namespace lenspec.LocalRegulations
{
  partial class AcquaintanceFormReportServerHandlers
  {

    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.AcquaintanceFormReport.SourceTableName, AcquaintanceFormReport.ReportSessionId);
    }

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      var task = AcquaintanceFormReport.Task;
      var isElectronicAcquaintance = task.IsElectronicAcquaintance == true;
      var sourceDocument = task.DocumentGroup.OfficialDocuments.First();
      // Вернуть номер версии только если у документа есть версии, и статус задачи не "Черновик", иначе - 0.
      var versionNumber = 0;
      var acquaintanceVersion = task.AcquaintanceVersions.FirstOrDefault(v => v.IsMainDocument == true);
      if (acquaintanceVersion != null &&
          (task.Status == Status.InProcess ||
           task.Status == Status.Suspended ||
           task.Status == Status.Completed ||
           task.Status == Status.Aborted))
      {
        versionNumber = acquaintanceVersion.Number.Value;
      }
      else
      {
        var document = task.DocumentGroup.OfficialDocuments.First();
        versionNumber = document.HasVersions ? document.LastVersion.Number.Value : 0;
      }
      var nonBreakingSpace = Convert.ToChar(160);
      
      // Шапка.
      AcquaintanceFormReport.DocumentName = Sungero.Docflow.PublicFunctions.Module.FormatDocumentNameForReport(sourceDocument, versionNumber, false);
      
      // Приложения.
      var documentAddenda = new List<Sungero.Content.IElectronicDocument>();
      var addendaIds = task.AcquaintanceVersions
        .Where(x => x.IsMainDocument != true)
        .Select(x => x.DocumentId);
      
      var addenda = task.AddendaGroup.OfficialDocuments
        .Where(x => addendaIds.Contains(x.Id))
        .Distinct()
        .ToList();
      documentAddenda.AddRange(addenda);
      
      if (documentAddenda.Any())
      {
        AcquaintanceFormReport.AddendaDescription = Sungero.RecordManagement.Reports.Resources.AcquaintanceReport.Addendas;
        foreach (var addendum in documentAddenda)
        {
          var addendumInfo = string.Format("\n - {0} ({1}:{2}{3}).", addendum.DisplayValue.Trim(),
                                           Sungero.Docflow.Resources.Id, nonBreakingSpace, addendum.Id);
          AcquaintanceFormReport.AddendaDescription += addendumInfo;
        }
      }
      
      // Данные.
      var reportSessionId = System.Guid.NewGuid().ToString();
      AcquaintanceFormReport.ReportSessionId = reportSessionId;
      var dataTable = new List<Structures.AcquaintanceFormReport.TableLine>();
      var employees = GetEmployeesFromParticipants(task);
      
      #region Вычисление неавтоматизированных сотрудников
      
      var notAutomatedEmployeesIds = lenspec.EtalonDatabooks.PublicFunctions.Module.GetNotAutomatedEmployeesAvis(employees.ToList()).Select(x => x.Id).ToList();
      
      #endregion
      
      foreach (var employee in employees)
      {
        var newLine = Structures.AcquaintanceFormReport.TableLine.Create();
        newLine.ShortName = employee.Person.ShortName;
        newLine.LastName = employee.Person.LastName;
        if (employee.JobTitle != null)
          newLine.JobTitle = employee.JobTitle.DisplayValue;
        newLine.Department = employee.Department.DisplayValue;
        newLine.RowNumber = 0;
        newLine.ReportSessionId = reportSessionId;
        // Автоматизирован.
        newLine.IsAutomated = notAutomatedEmployeesIds.Any(x => x.Equals(employee.Id))
          ? newLine.IsAutomated = "Нет"
          : newLine.IsAutomated = "Да";
        dataTable.Add(newLine);
      }
      Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.AcquaintanceFormReport.SourceTableName, dataTable);
      
      // Подвал.
      var currentUser = Users.Current;
      var printedByName = Sungero.Company.Employees.Is(currentUser)
        ? Sungero.Company.Employees.As(currentUser).Person.ShortName
        : currentUser.Name;
      AcquaintanceFormReport.Printed = Sungero.RecordManagement.Reports.Resources.AcquaintanceReport.PrintedByFormat(printedByName, Calendar.UserNow);
    }
    
    /// <summary>
    /// Получить список конечных исполнителей ознакомления на момент отправки.
    /// </summary>
    /// <param name="task">Ознакомление.</param>
    /// <returns>Список сотрудников на момент отправки задачи.</returns>
    public static IEnumerable<IEmployee> GetEmployeesFromParticipants(Sungero.RecordManagement.IAcquaintanceTask task)
    {
      // Заполнение AcquaintanceTaskParticipants происходит в схеме.
      // От старта задачи до начала обработки схемы там ничего не будет - взять из исполнителей задачи.
      var storedParticipants = Sungero.RecordManagement.AcquaintanceTaskParticipants.GetAll().FirstOrDefault(x => x.TaskId == task.Id);
      if (storedParticipants != null)
        return storedParticipants.Employees.Select(p => p.Employee).ToList();
      
      return Sungero.RecordManagement.PublicFunctions.AcquaintanceTask.Remote.GetParticipants(task);
    }
  }
}