using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalSimpleAssignment;

namespace lenspec.Etalon.Client
{
  partial class ApprovalSimpleAssignmentFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Показывать сводку по документу.
    /// </summary>
    /// <returns>True, если в задании нужно показывать сводку по документу.</returns>
    [Public]
    public bool NeedViewDocumentSummary()
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document == null)
        return false;
      
      return Sungero.Docflow.PublicFunctions.OfficialDocument.NeedViewDocumentSummary(document);
    }
    
    /// <summary>
    /// Показывать сводку по документу.
    /// </summary>
    /// <returns>True, если в задании нужно показывать сводку по документу.</returns>
    [Public]
    public bool NeedViewInstruction()
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document == null)
        return false;
      
      if (CourierShipments.CourierShipmentsApplications.Is(document))
      {
        return CourierShipments.PublicFunctions.CourierShipmentsApplication.NeedViewInstruction(CourierShipments.CourierShipmentsApplications.As(document));
      }
      
      return false;
    }
    //конец Добавлено Avis Expert
  }
}