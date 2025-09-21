using System;
using Sungero.Core;

namespace lenspec.AutomatedSupportTickets.Constants
{
  public static class ReconciliationSettings
  {
    public const string ReconciliationSettingsTableName = "Sungero_Reports_ReconciliationSettingsReport";
    
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