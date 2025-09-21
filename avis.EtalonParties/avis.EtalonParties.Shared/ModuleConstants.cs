using System;
using Sungero.Core;

namespace avis.EtalonParties.Constants
{
  public static class Module
  {
    #region Роли
    
    /// <summary>
    /// Guid роли "Права на создание всех видов контрагентов".
    /// </summary>
    [Public]
    public static readonly Guid RoleCreateCounterpartyGuid = Guid.Parse("ec311c82-a71f-4749-9dc5-8cef840d13d3");
    
    /// <summary>
    /// Guid роли "Права на работу с кнопкой Получить адрес из Dadata".
    /// </summary>
    [Public]
    public static readonly Guid RoleFillDadataGuid = Guid.Parse("0FD74403-88CC-4D70-9949-4714397AA10E");
    
    /// <summary>
    /// Guid роли "Ответственный за признак монополистов КА".
    /// </summary>
    [Public]
    public static readonly Guid RoleResponsibleForMonopolistsGuid = Guid.Parse("709CB47C-CAF3-4F78-9776-F25D68EF3E74");
    
    #endregion
    
    /// <summary>
    /// Количество календарных дней До окончания Срока согласования Контрагента.
    /// </summary>
    public const int SendNotificationAboutExpirationCounterpatyAfterDays = 30;
  }
}