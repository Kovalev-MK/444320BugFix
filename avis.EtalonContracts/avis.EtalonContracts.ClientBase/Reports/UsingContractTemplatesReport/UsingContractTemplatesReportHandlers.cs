using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonContracts
{
  partial class UsingContractTemplatesReportClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      var dialog = Dialogs.CreateInputDialog(avis.EtalonContracts.Resources.UsingContractTemplateReportDialogTitle);
      var dateFrom = dialog.AddDate(avis.EtalonContracts.Resources.UsingContractTemplateReportDialogDateFrom, true, Calendar.Today.BeginningOfMonth());
      var dateTo = dialog.AddDate(avis.EtalonContracts.Resources.UsingContractTemplateReportDialogDateTo, true, Calendar.Today);
      dialog.SetOnRefresh(er =>
                          {
                            if (dateFrom.Value > dateTo.Value)
                              er.AddError(avis.EtalonContracts.Resources.ContractualDocumentDatesValidationMessage,
                                          dateFrom,
                                          dateTo);
                          });
      
      if (dialog.Show() != DialogButtons.Ok)
        e.Cancel = true;
      
      UsingContractTemplatesReport.DateFrom = dateFrom.Value.Value.ToString();
      UsingContractTemplatesReport.DateTo = dateTo.Value.Value.ToString();
    }

  }
}