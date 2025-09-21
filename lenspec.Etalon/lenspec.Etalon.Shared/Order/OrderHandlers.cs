using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Order;

namespace lenspec.Etalon
{
  partial class OrderSharedHandlers
  {

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      
      // Если сменили или очистили НОР, очистить поле "Отменяет приказ". DIRRXMIGR-37
      if (e.NewValue == null)
        _obj.CanceledOrderslenspec.Clear();

      if (e.OldValue != e.NewValue)
        _obj.CanceledOrderslenspec.Clear();
    }

  }
}