using System;
using Sungero.Core;

namespace lenspec.AutomatedSupportTickets.Constants
{
  public static class Module
  {
    #region Роли
    
    /// <summary>
    ///  GUID роли "Завершение Задач и Заданий".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid CompleteAssignmentsResponsible = Guid.Parse("AEAA46DC-AD3D-4451-BD74-84A24EA7D076");
    
    /// <summary>
    ///  GUID роли "Отключению учетных записей и Указание пользователей в объектах системы".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid AccessAccountDisconnectionAndUserReport = Guid.Parse("f7b1e725-61ef-4e81-965d-cdf60f5769ea");

    #endregion
  }
}