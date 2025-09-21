using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PostalItems.Structures.PostalItem
{
  /// <summary>
  /// Результат запроса на получение потового индекса.
  /// </summary>
  [Public]
  partial class PostalCodeReceiveResult
  {
    /// Признак успешного выполнения.
    public bool IsSuccess { get; set; }
    /// Ошибка.
    public string Error { get; set; }
    /// Почтовый индекс.
    public string Code { get; set; }
  }
}