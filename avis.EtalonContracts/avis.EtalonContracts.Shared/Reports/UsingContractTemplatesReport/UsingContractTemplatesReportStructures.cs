using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonContracts.Structures.UsingContractTemplatesReport
{
  /// <summary>
  /// Строка отчета.
  /// </summary>
  [Public]
  partial class UsingContractTemplatesReportTableLine
  {
    public string ReportSessionId { get; set; }
    
    public long DocumentId { get; set; }
    
    public string DocumentHyperlink { get; set; }
    
    public string Author { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public string BusinessUnit { get; set; }
    
    public string Counterparty { get; set; }
    
    public string TemplateName { get; set; }
    
    public long? TemplateId { get; set; }
    
    public string TemplateHyperlink { get; set; }
  }

}