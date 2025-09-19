using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters
{
  partial class UnsentMassEmailsReportServerHandlers
  {

    //Добавлено Avis Expert
    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.UnsentMassEmailsReport.SourceTableName, UnsentMassEmailsReport.ReportSessionId);
    }

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      var reportSessionId = System.Guid.NewGuid().ToString();
      UnsentMassEmailsReport.ReportSessionId = reportSessionId;
      var dataTable = new List<Structures.UnsentMassEmailsReport.TableLine>();
      
      var unsentEmails = UnsentMassEmailsReport.MassMailingNotification.UnsentEmails;
      var lineNumber = 0;
      foreach(var email in unsentEmails)
      {
        lineNumber++;
        var tableLine = Structures.UnsentMassEmailsReport.TableLine.Create();
        tableLine.ReportSessionId = reportSessionId;
        tableLine.LineNumber = lineNumber;
        tableLine.DocumentName = email.UnsentOutgoingLetter.Name;
        tableLine.RegistrationNumber = email.UnsentOutgoingLetter.RegistrationNumber;
        tableLine.Correspondent = email.UnsentOutgoingLetter.Correspondent != null ? email.UnsentOutgoingLetter.Correspondent.Name : string.Empty;
        tableLine.Error = email.ErrorMessage;
        tableLine.DocumentId = email.UnsentOutgoingLetter.Id.ToString();
        tableLine.DocumentHyperlink = Hyperlinks.Get(email.UnsentOutgoingLetter);
        
        dataTable.Add(tableLine);
      }
      
      Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.UnsentMassEmailsReport.SourceTableName, dataTable);
    }
    //конец Добавлено Avis Expert

  }
}