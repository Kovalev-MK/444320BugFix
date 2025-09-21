using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters.Structures.UnsentMassEmailsReport
{
  
  //Добавлено Avis Expert
  /// <summary>
  /// Данные по документу для отчета.
  /// </summary>
  partial class TableLine
  {
    public string ReportSessionId { get; set; }
    
    public int LineNumber { get; set; }
    
    public string DocumentName { get; set; }
    
    public string RegistrationNumber { get; set; }
    
    public string Correspondent { get; set; }
    
    public string Error { get; set; }
    
    public string DocumentId { get; set; }
    
    public string DocumentHyperlink { get; set; }
  }
  //конец Добавлено Avis Expert
}