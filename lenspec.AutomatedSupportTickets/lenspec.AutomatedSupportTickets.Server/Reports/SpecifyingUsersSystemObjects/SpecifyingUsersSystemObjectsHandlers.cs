using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets
{
  partial class SpecifyingUsersSystemObjectsServerHandlers
  {
    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.SpecifyingUsersSystemObjects.SpecifyingUsersSystemObjectsTableName, SpecifyingUsersSystemObjects.ReportSessionId);
    }

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      SpecifyingUsersSystemObjects.CurrentDate = Calendar.Now;
      var reportData = lenspec.AutomatedSupportTickets.PublicFunctions.Module.GetAllDataSpecifyingUsersSystemObjects(SpecifyingUsersSystemObjects.employees);
      
      foreach (var element in reportData)
        element.ReportSessionId = SpecifyingUsersSystemObjects.ReportSessionId; 
      
      Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.SpecifyingUsersSystemObjects.SpecifyingUsersSystemObjectsTableName, reportData);
    }
  }
}