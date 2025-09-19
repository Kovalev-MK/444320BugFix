using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets.Structures.SpecifyingUsersSystemObjects
{
  /// <summary>
  /// Строка отчета.
  /// </summary>
  [Public]
  partial class SpecifyingReportTableLine
  {
    public string Employee { get; set; }
    
    public long EmployeeID { get; set; }
    
    public string JobTitle { get; set; }
    
    public string OurOrg { get; set; }
    
    public string Department { get; set; }
    
    public string AccountStatus { get; set; }
    
    public string EmployeeRecordStatus { get; set; }
    
    public string Roles { get; set; }
    
    public string AllrolesNor { get; set; }
      
    public string Substitutions { get; set; }
    
    public int Alltasks { get; set; }
    
    public int Alljobs { get; set; }
    
    public string ApprovalSettings { get; set; }
      
    public string ApprovalStages { get; set; }
    
    public string RegistrationGroups { get; set; }
      
    public string SignatureSettings { get; set; } 
      
    public string ResponsibleContractors { get; set; }
    
    public string ConditionBases { get; set; } 
      
    public string CustReqSetups { get; set; } 
    
    public string Certificates { get; set; }
    
    public string ManagersAssistants { get; set; }
    
    public string CollegialBodies { get; set; } 
    
    public string ReportSessionId { get; set; }
  }
}