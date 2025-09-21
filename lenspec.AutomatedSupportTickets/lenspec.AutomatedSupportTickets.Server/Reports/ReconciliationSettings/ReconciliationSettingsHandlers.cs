using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets
{
  partial class ReconciliationSettingsServerHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      ReconciliationSettings.currentData = Calendar.Now;
      var reportData = lenspec.AutomatedSupportTickets.PublicFunctions.Module.GetAllDataReconciliationSettings(ReconciliationSettings.businessUnits,
                                                                                                               ReconciliationSettings.approvalRules,
                                                                                                               ReconciliationSettings.roleKinds,
                                                                                                               ReconciliationSettings.status);
      
      foreach (var element in reportData)
        element.ReportSessionId = ReconciliationSettings.reportSessionId;
      
      Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.ReconciliationSettings.ReconciliationSettingsTableName, reportData);
    }

    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.ReconciliationSettings.ReconciliationSettingsTableName, ReconciliationSettings.reportSessionId);
    }
  }

}