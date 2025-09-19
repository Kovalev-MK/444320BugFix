using System;
using Sungero.Core;

namespace avis.ApprovingCounterpartyDEB.Constants
{
  public static class Module
  {
    /// <summary>
    /// Гуид роли Уведомляемые по согласование контрагентов, банков и персон с ДБ
    /// </summary>
    [Public]
    public static readonly Guid NotifiedMembersApprovalDEB = Guid.Parse("B364A9C7-EEAA-4C11-A0FD-F40D86C72E68");

    /// <summary>
    /// Гуид роли Права на отчет «Контрагенты на согласовании»
    /// </summary>
    [Public]
    public static readonly Guid RightsReportOnApprovalCounterpartyRoleGuid = Guid.Parse("AA505E31-F975-4388-A744-FD6CD90CC307");

    /// <summary>
    /// Гуид роли Права на формирование отчёта по кнопке "Полный отчёт из КФ".
    /// </summary>
    [Public]
    public static readonly Guid DebReportRoleGuid = Guid.Parse("5394B9D8-664F-4601-9D23-2154736A9B65");

    /// <summary>
    /// Гуид роли Права на экспресс-отчет из КФ.
    /// </summary>
    [Public]
    public static readonly Guid ExpressReportKFRoleGuid = Guid.Parse("59D46AD7-A656-46E2-9A35-501B01565A12");
    
    /// <summary>
    /// Sid роли Ответственный экономист за определение лимита КА
    /// </summary>
    [Public]
    public static readonly Guid ResponsibleLimitEconomist = Guid.Parse("A508BAA3-9286-4023-9B82-914F699CB3E0");
    
  }
}