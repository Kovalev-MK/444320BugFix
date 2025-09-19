using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonParties.Structures.DebReport
{
  /// <summary>
  /// Структура для таблицы "Виды деятельности".
  /// </summary>
  partial class DebActivity
  {
    public string SessionId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "Учредители".
  /// </summary>
  partial class DebFounder
  {
    public string SessionId { get; set; }
    public string Share { get; set; }
    public string ShareRub { get; set; }
    public string Name { get; set; }
    public string Date { get; set; }
    public string Comment { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "Учредители история".
  /// </summary>
  partial class DebHistoryFounder
  {
    public string SessionId { get; set; }
    public string ShareRub { get; set; }
    public string Name { get; set; }
    public string Date { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "Филиалы".
  /// </summary>
  partial class DebFilial
  {
    public string SessionId { get; set; }
    public string Address { get; set; }
    public string Date { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "Лицензии".
  /// </summary>
  partial class DebLicense
  {
    public string SessionId { get; set; }
    public string Source { get; set; }
    public string Number { get; set; }
    public string Date { get; set; }
    public string Name { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "Финансовое состояние".
  /// </summary>
  partial class DebFinanc
  {
    public string SessionId { get; set; }
    public string Code11501 { get; set; }
    public string Code11502 { get; set; }
    public string Code11503 { get; set; }
    
    public string Code12101 { get; set; }
    public string Code12102 { get; set; }
    public string Code12103 { get; set; }
    
    public string Code12301 { get; set; }
    public string Code12302 { get; set; }
    public string Code12303 { get; set; }
    
    public string Code12501 { get; set; }
    public string Code12502 { get; set; }
    public string Code12503 { get; set; }
    
    public string Code16001 { get; set; }
    public string Code16002 { get; set; }
    public string Code16003 { get; set; }
    
    public string Code13001 { get; set; }
    public string Code13002 { get; set; }
    public string Code13003 { get; set; }
    
    public string Code14101 { get; set; }
    public string Code14102 { get; set; }
    public string Code14103 { get; set; }
    
    public string Code15101 { get; set; }
    public string Code15102 { get; set; }
    public string Code15103 { get; set; }
    
    public string Code15201 { get; set; }
    public string Code15202 { get; set; }
    public string Code15203 { get; set; }
    
    public string Code17001 { get; set; }
    public string Code17002 { get; set; }
    public string Code17003 { get; set; }
    
    public string Code21101 { get; set; }
    public string Code21102 { get; set; }
    public string Code21103 { get; set; }
    
    public string Code21201 { get; set; }
    public string Code21202 { get; set; }
    public string Code21203 { get; set; }
    
    public string Code22101 { get; set; }
    public string Code22102 { get; set; }
    public string Code22103 { get; set; }
    
    public string Code23401 { get; set; }
    public string Code23402 { get; set; }
    public string Code23403 { get; set; }
    
    public string Code23501 { get; set; }
    public string Code23502 { get; set; }
    public string Code23503 { get; set; }
    
    public string Code24001 { get; set; }
    public string Code24002 { get; set; }
    public string Code24003 { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "Уплаченные налоги и сборы".
  /// </summary>
  partial class DebTaxe
  {
    public string SessionId { get; set; }
    public string Name { get; set; }
    public string Year1 { get; set; }
    public string Year2 { get; set; }
    public string Year3 { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "Сведения из ФНС об отсутствии задолженности".
  /// </summary>
  partial class DebFns
  {
    public string SessionId { get; set; }
    public string Bik { get; set; }
    public string Name { get; set; }
    public string Date { get; set; }
    public string Number { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "История ЮР.Адресов".
  /// </summary>
  partial class DebHistoryUL
  {
    public string SessionId { get; set; }
    public string Address { get; set; }
    public string Date { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "История ЮР.Адресов".
  /// </summary>
  partial class DebBanks
  {
    public string SessionId { get; set; }
    public string Number { get; set; }
    public string Bik { get; set; }
    public string Name { get; set; }
    public string Source { get; set; }
    public string Date { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "История руководителей".
  /// </summary>
  partial class DebDirector
  {
    public string SessionId { get; set; }
    public string Name { get; set; }
    public string Date { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "Информация об истцах".
  /// </summary>
  partial class DebArbitration
  {
    public string SessionId { get; set; }
    public string Number { get; set; }
    public string Name { get; set; }
    public string Inn { get; set; }
    public string MainActivity { get; set; }
    public string Count { get; set; }
    public string Summ { get; set; }
    public string LastDate { get; set; }
  }
  
  /// <summary>
  /// Структура для таблицы "Информация о контрактах".
  /// </summary>
  partial class DebContract
  {
    public string SessionId { get; set; }
    public string Number { get; set; }
    public string Type { get; set; }
    public string Customer { get; set; }
    public string Inn { get; set; }
    public string Description { get; set; }
    public string ContractNumber { get; set; }
    public string ContractDate { get; set; }
    public string Price { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; }
  }
}