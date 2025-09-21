using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonContracts
{
  partial class UsingContractTemplatesReportServerHandlers
  {

    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(EtalonContracts.Constants.UsingContractTemplatesReport.UsingContractTemplatesReportTableName, UsingContractTemplatesReport.ReportSessionId);
    }

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      var reportSessionId = System.Guid.NewGuid().ToString();
      UsingContractTemplatesReport.ReportSessionId = reportSessionId;
      UsingContractTemplatesReport.CurrentDate = Calendar.Now;
      
      DateTime dateFrom;
      DateTime dateTo;
      Calendar.TryParseDateTime(UsingContractTemplatesReport.DateFrom, out dateFrom);
      Calendar.TryParseDateTime(UsingContractTemplatesReport.DateTo, out dateTo);
      
      var documents = Functions.Module.GetContractualDocs(dateFrom, dateTo);
      documents = documents.Where(x => x.TemplateIDlenspec.HasValue);
      
      var templateIds = documents.Select(x => x.TemplateIDlenspec).Distinct().ToList();
      var templates = lenspec.Etalon.DocumentTemplates.GetAll(x => templateIds.Contains(x.Id));
      var reportData = new List<Structures.UsingContractTemplatesReport.IUsingContractTemplatesReportTableLine>();
      
      foreach (var document in documents)
      {
        var template = templates.Where(x => x.Id == document.TemplateIDlenspec).FirstOrDefault();
        if (template == null)
          continue;
        
        var reportDataLine = Structures.UsingContractTemplatesReport.UsingContractTemplatesReportTableLine.Create();
        
        reportDataLine.ReportSessionId = reportSessionId;
        reportDataLine.DocumentId = document.Id;
        reportDataLine.DocumentHyperlink = Functions.Module.GetLinkToContractualDocument(document.Id);
        reportDataLine.Author = document.Author.Name;
        reportDataLine.CreatedDate = document.Created.Value;
        reportDataLine.BusinessUnit = document.BusinessUnit?.Name;
        reportDataLine.Counterparty = document.Counterparty?.Name;
        reportDataLine.TemplateName = template?.Name;
        reportDataLine.TemplateId = document.TemplateIDlenspec;
        reportDataLine.TemplateHyperlink = Hyperlinks.Get(template);

        reportData.Add(reportDataLine);
      }
      
      Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.UsingContractTemplatesReport.UsingContractTemplatesReportTableName, reportData);
    }

  }
}