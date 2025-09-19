using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Company.Structures.Module
{
  //Добавлено Avis Expert
  
  #region Остальное.
  /// <summary>
  /// Структура для использования в LINQ-запросе для Сотрудника.
  /// </summary>
  [Public]
  partial class EmployeeLINQParams
  {
    public Sungero.Parties.IPerson Person { get; set; }
    
    public string Email { get; set; }
  }
  
  /// <summary>
  /// Параметры отправки уведомлений об истечении срока ГПД сотрудников.
  /// </summary>
  [Public]
  partial class ExpiringEmployeesNotificationParams
  {
    // Дата последнего уведомления.
    public DateTime LastNotification { get; set; }
    
    // Дата последнего уведомления с резервом.
    public DateTime LastNotificationReserve { get; set; }
    
    // Дата сегодня.
    public DateTime Today { get; set; }
    
    // Дата сегодня с резервом.
    public DateTime TodayReserve { get; set; }
    
    // Имя таблицы БД с информацией об уведомлениях.
    public string ExpiringEmployeesTableName { get; set; }
    
    // Имя параметра в Sungero_Docflow_Params с датой последнего уведомления.
    public string LastNotificationParamName { get; set; }
    
    // Параметры задачи об истечении срока ГПД сотрудника.
    public lenspec.Etalon.Module.Company.Structures.Module.IExpiringNotificationTaskParams TaskParams { get; set; }
  }
  
  /// <summary>
  /// Параметры отправки задачи об истечении срока ГПД сотрудника.
  /// </summary>
  [Public]
  partial class ExpiringNotificationTaskParams
  {
    // Тема.
    public string Subject { get; set; }
    
    // Текст.
    public string ActiveText { get; set; }
    
    // Исполнители.
    public List<IUser> Performers { get; set; }
    
    // Вложения.
    public List<lenspec.Etalon.IEmployee> Attachments { get; set; }
  }
  #endregion
  
  //конец Добавлено Avis Expert.
}