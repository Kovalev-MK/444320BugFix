using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.CommitteeApprovalAssignment;

namespace lenspec.Tenders.Shared
{
  partial class CommitteeApprovalAssignmentFunctions
  {
    /// <summary>
    /// Разрешено ли добавление согласующих.
    /// </summary>
    /// <returns>True - разрешено, False - нет.</returns>
    public virtual bool CanAddApprovers()
    {
      return Functions.CommitteeApprovalAssignment.Remote.GetAllowAddApproversPropertyValue(_obj);
    }
  }
}