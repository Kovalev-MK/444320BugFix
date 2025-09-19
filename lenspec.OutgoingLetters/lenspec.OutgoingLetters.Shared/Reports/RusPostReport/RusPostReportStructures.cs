using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters.Structures.RusPostReport
{
  //Добавлено Avis Expert
  /// <summary>
  /// Данные по документу для отчета.
  /// </summary>
  partial class TableLine
  {
    public string ReportSessionId { get; set; }
    
    public int LineNumber { get; set; }
    
    public string CorrespondentAndContractNumber { get; set; }
    
    public string RegistrationDataAndSubject { get; set; }
    
    public string AddressIndex { get; set; }
    
    public string Region { get; set; }
    
    public string City { get; set; }
    
    public string Address { get; set; }
    
    public string BussinesUnit { get; set; }
    
    public string DeliveryMethod { get; set; }
    
    public string ReturnAddress { get; set; }
    
    public string Weight { get; set; }
    
    public string Rating { get; set; }
    
    public string Phone { get; set; }
  }
  //конец Добавлено Avis Expert
}