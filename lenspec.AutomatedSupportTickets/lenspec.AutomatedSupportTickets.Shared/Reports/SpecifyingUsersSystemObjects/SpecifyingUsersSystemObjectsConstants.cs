using System;
using Sungero.Core;

namespace lenspec.AutomatedSupportTickets.Constants
{
  public static class SpecifyingUsersSystemObjects
  {
    public const string SpecifyingUsersSystemObjectsTableName = "Sungero_Reports_SpecifyingReport";
    
    [Sungero.Core.PublicAttribute]
    public const int CompanyPriority = 1000;
    
    [Sungero.Core.PublicAttribute]
    public const int ExchangePriority = 1500;
    
    [Sungero.Core.PublicAttribute]
    public const int DocflowPriority = 2000;
    
    [Sungero.Core.PublicAttribute]
    public const int ProjectsPriority = 2500;
    
    [Sungero.Core.PublicAttribute]
    public const int MeetingsPriority = 3000;
    
    [Sungero.Core.PublicAttribute]
    public const int CounterpartyPriority = 4000;
  }
}