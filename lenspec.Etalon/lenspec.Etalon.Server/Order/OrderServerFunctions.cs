using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Order;

namespace lenspec.Etalon.Server
{
  partial class OrderFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Создать новый документ, утверждаемый приказом.
    /// </summary>
    /// <returns>Документ, утвердаемый приказом.</returns>
    [Remote]
    public lenspec.LocalRegulations.IDocumentApprovedByOrder CreateDocumentApprovedByOrder()
    {
      var document = lenspec.LocalRegulations.DocumentApprovedByOrders.Create();
      document.LeadingDocument = _obj;
      document.BusinessUnit = _obj.BusinessUnit;
      document.Department = _obj.Department;
      document.OurSignatory = _obj.OurSignatory;
      document.PreparedBy = _obj.PreparedBy;
      document.Assignee = _obj.Assignee;
      return document;
    }
    //конец Добавлено Avis Expert
  }
}