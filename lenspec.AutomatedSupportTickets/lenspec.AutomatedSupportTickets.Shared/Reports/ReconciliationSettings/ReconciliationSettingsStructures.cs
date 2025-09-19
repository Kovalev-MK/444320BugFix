using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets.Structures.ReconciliationSettings
{
  /// <summary>
  /// Строка отчета.
  /// </summary>
  [Public]
  partial class SpecifyingReportTableLine
  {
    public long Id { get; set; }
    
    public string BusinessUnits { get; set; }
    
    public string ApprovalSettings { get; set; }
    
    public string RoleKinds { get; set; }
    
    public string Employee { get; set; }
    
    public string Note { get; set; }
    
    public string ReportSessionId { get; set; }
    
    public string HyperlinkID { get; set; }
    
    public string Status { get; set; }
    
  }
}