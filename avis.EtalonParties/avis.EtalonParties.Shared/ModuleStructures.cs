using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonParties.Structures.Module
{
  /// <summary>
  /// Структура групп контрагентов, для инициализации.
  /// </summary>
  partial class GroupCounterpartyModel
  {
    public int DirectumId { get; set; }
    public string Name { get; set; }
  }
  
  /// <summary>
  /// Структура категорий контрагентов, для инициализации.
  /// </summary>
  partial class CategoryCounterpartyModel
  {
    public int? DirectumId { get; set; }
    public string Name { get; set; }
    public List<int> GroupCounterpartyId { get; set; }
  }
  
  /// <summary>
  /// Даты итераций задачи.
  /// </summary>
  [Public]
  partial class TaskIterations
  {
    public DateTime Date { get; set; }
    
    public bool IsRework { get; set; }
    
    public bool IsRestart { get; set; }
  }

  #region Deb Report.
  partial class TableLine
  {
    public string ReportSessionId { get; set; }   
    public string ToName { get; set; }   
    public string PostAddress { get; set; }
  }
  #endregion
}