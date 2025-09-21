using System;
using Sungero.Core;

namespace lenspec.Etalon.Module.Company.Constants
{
  public static class Module
  {

    /// <summary>
    /// Действия с табличной частью в настройках согласования
    /// </summary>
    public static class ApprovalSettingsActions
    {
      public const string Addition = "Добавление";
      public const string Modification = "Изменение";
      public const string Removing = "Удаление";
    }
    
    public static class ApprovalSettingsStatus
    {
       public const string Active = "Действующая";
       public const string Closed = "Закрытая";
       public const string All = "Все";
    }
    
    /// <summary>
    /// Значение для незаполненного вида роли в диалоге для изменения настроек согласования
    /// </summary>
    public const int EmptyRoleKind = -1;

    //Добавлено Avis Expert
    // Уведомления об окончании срока действия ГПД сотрудника.
    public const string NotificationDatabaseKey = "LastNotificationOfExpiringEmployees";
    public const string ExpiringEmployeesTableName = "lenspec_Company_ExpiringEmployees";
    
    //Количество дней до закрытия учетной записи Сотрудника при отправке уведомлений.
    public const int ExpiringEmployeesDaysBeforeClosing = 5;
    //конец Добавлено Avis Expert
  }
}