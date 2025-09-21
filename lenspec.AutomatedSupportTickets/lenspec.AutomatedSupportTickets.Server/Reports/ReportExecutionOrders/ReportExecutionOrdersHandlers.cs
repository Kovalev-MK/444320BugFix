using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets
{
  partial class ReportExecutionOrdersServerHandlers
  {
    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      this.GetLetterAndTask();
    }

    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.ReportExecutionOrders.SourceTableName, ReportExecutionOrders.ReportSessionId);
    }
    
    /// <summary>
    /// Получить входящие письма и задачи по ним, удовлетворяющие заданным параметрам отчета.
    /// </summary>
    public virtual void GetLetterAndTask()
    {
      try
      {
        var lineNumber = 0;
        var reportSessionId = System.Guid.NewGuid().ToString();
        ReportExecutionOrders.ReportSessionId = reportSessionId;
        var executor = ReportExecutionOrders.executor;
        var controller = ReportExecutionOrders.controller;
        var term = ReportExecutionOrders.term;
        var ourCF = ReportExecutionOrders.ourCF;
        var correspondent = ReportExecutionOrders.correspondent;
        var businessUnitIds = ReportExecutionOrders.BusinessUnit.Select(x => x.Id).ToList();
        var status = ReportExecutionOrders.status;
        
        var documents = new List<Etalon.IIncomingLetter>();
        var letters = Etalon.IncomingLetters.GetAll();
        if (ReportExecutionOrders.beginDate != null)
        {
          letters = letters.Where(l => l.DocumentDate >= ReportExecutionOrders.beginDate);
        }
        if (ReportExecutionOrders.endDate != null)
        {
          letters = letters.Where(l => l.DocumentDate <= ReportExecutionOrders.endDate);
        }
        if (ourCF != null)
        {
          letters = letters.Where(l => l.OurCFlenspec == ourCF);
        }
        if (correspondent != null)
        {
          letters = letters.Where(l => l.Correspondent == correspondent);
        }
        if (businessUnitIds.Any())
        {
          letters = letters.Where(l => l.BusinessUnit != null && businessUnitIds.Contains(l.BusinessUnit.Id));
        }
        
        foreach (var letter in letters)
        {
          if (!letter.AccessRights.CanRead(ReportExecutionOrders.CurrentUser))
            continue;
          
          var tasks = Sungero.Docflow.PublicFunctions.OfficialDocument.Remote.GetCreatedActionItems(letter);
          // Проверим на Контролера
          if (controller != null)
          {
            tasks = tasks.Where(x => x.Supervisor == controller);
          }
          // Проверим на Исполнителя
          if (executor != null)
          {
            tasks = tasks.Where(x => x.Assignee == executor);
          }
          // Проверим по сроку
          if (term != null)
          {
            tasks = tasks.Where(x => x.Deadline == term);
          }
          //Проверим по статусу
          if (status == "В работе")
          {
            tasks = tasks.Where(x => x.ExecutionState == Sungero.RecordManagement.ActionItemExecutionTask.ExecutionState.OnExecution);
          }
          if (status == "На контроле")
          {
            tasks = tasks.Where(x => x.ExecutionState == Sungero.RecordManagement.ActionItemExecutionTask.ExecutionState.OnControl);
          }
          if (status == "Исполнено")
          {
            tasks = tasks.Where(x => x.ExecutionState == Sungero.RecordManagement.ActionItemExecutionTask.ExecutionState.Executed);
          }
          foreach (var task in tasks)
          {
            lineNumber++;
            this.WriteToIncomingLettersTable(letter.Id, task.Id, reportSessionId, lineNumber);
          }
        }
      }

      catch(Exception ex)
      {
        Logger.ErrorFormat("GetIncomingLetter - {0}", ex.Message);
      }
    }
    
    /// <summary>
    /// Заполнить отчет данными по входящим письмам и задачам.
    /// </summary>
    /// <param name="documentsId">ИД документа</param>
    /// <param name="taskId">ИД задачи</param>
    /// <param name="reportSessionId">ИД сессии отчета.</param>
    public virtual void WriteToIncomingLettersTable(long documentId, long taskId, string reportSessionId, int lineNumber)
    {
      try
      {
        var incomingLetters = new List<Structures.ReportExecutionOrders.TableLine>();
        var document = Etalon.IncomingLetters.GetAll().Where(x => x.Id == documentId).SingleOrDefault();
        var task = Sungero.RecordManagement.ActionItemExecutionTasks.GetAll().Where(x => x.Id == taskId).SingleOrDefault();
        long? parentsTaskID = null;
        var hyperlinkParentsTaskID = String.Empty;
        if (task.ParentAssignment != null)
        {
          parentsTaskID = task.ParentAssignment.Id;
          hyperlinkParentsTaskID = Hyperlinks.Get(Sungero.RecordManagement.ActionItemExecutionAssignments.GetAll().Where(x => x.Id == parentsTaskID).SingleOrDefault());
        }
        var documentDate = document.DocumentDate;
        var reg_Number = document.RegistrationNumber;
        var isp = String.Empty;
        if (document.OurCFlenspec != null)
        {
          isp = document.OurCFlenspec.Name;
        }
        var bussinesUnit = document.BusinessUnit.Name;
        var typeCorrespondent = document.Correspondent.Info.LocalizedName;
        var correspondent = document.Correspondent.DisplayValue;
        var content = document.Subject;
        var status = task.Info.Properties.Status.GetLocalizedValue(task.Status);
        var executors = task.Assignee.Name;
        var controllers = String.Empty;
        if (task.Supervisor != null)
        {
          controllers = task.Supervisor.Name;
        }
        var term = String.Empty;
        if (task.Deadline != null)
        {
          term = task.Deadline.Value.ToString("d");
        }
        var hyperlinkDocumentID = Hyperlinks.Get(document);
        var hyperlinkTaskID = Hyperlinks.Get(task);
        incomingLetters.Add(Structures.ReportExecutionOrders.TableLine.Create(reportSessionId,
                                                                              lineNumber,
                                                                              documentDate.Value.ToString("d"),
                                                                              reg_Number,
                                                                              isp,
                                                                              bussinesUnit,
                                                                              typeCorrespondent,
                                                                              correspondent,
                                                                              content,
                                                                              status,
                                                                              executors,
                                                                              controllers,
                                                                              term,
                                                                              documentId,
                                                                              taskId,
                                                                              parentsTaskID,
                                                                              hyperlinkDocumentID,
                                                                              hyperlinkTaskID,
                                                                              hyperlinkParentsTaskID));
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.ReportExecutionOrders.SourceTableName, incomingLetters);
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("ReportExecutionOrders - {0}", ex.Message);
      }
    }
  }
}

