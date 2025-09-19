using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Tenders.Structures.ApprovalCounterpartyRegisterTask
{

  /// <summary>
  /// Результаты согласования.
  /// </summary>
  [Public]
  partial class ApprovalResults
  {
    /// Количество согласовавших.
    public int Approved { get; set; }
    /// Количество отказавших.
    public int Rejected { get; set; }
  }

}