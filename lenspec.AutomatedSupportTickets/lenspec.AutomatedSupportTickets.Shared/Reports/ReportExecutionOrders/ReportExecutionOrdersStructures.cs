using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets.Structures.ReportExecutionOrders
{
  /// <summary>
  /// Данные для отчета.
  /// </summary>
  partial class TableLine
  {
    public string ReportSessionId { get; set; }
    
    public int LineNumber { get; set; }
    
    public string DocumentDate { get; set; }
    
    public string RegNumber { get; set; }
    
    public string Isp { get; set; }
    
    public string BussinesUnit { get; set; }
    
    public string TypeCorrespondent { get; set; }
    
    public string Correspondent { get; set; }
    
    public string Content { get; set; }
    
    public string Status { get; set; }
    
    public string Executors { get; set; }
    
    public string Controllers { get; set; }
    
    public string Term { get; set; }
    
    public long DocumentID { get; set; }
    
    public long TaskID { get; set; }
    
    public long? ParentsTaskID { get; set; }
    
    public string HyperlinkDocumentID { get; set; }
    
    public string HyperlinkTaskID { get; set; }
    
    public string HyperlinkParentsTaskID { get; set; }
  }
}