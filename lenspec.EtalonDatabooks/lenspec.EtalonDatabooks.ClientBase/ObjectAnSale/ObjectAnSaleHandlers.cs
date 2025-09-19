using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnSale;

namespace lenspec.EtalonDatabooks
{
  // Добавлено avis.
  partial class ObjectAnSaleClientHandlers
  {
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      if (string.IsNullOrEmpty(_obj.Name))
      {
        _obj.Name = "<Поле будет заполнено автоматически>";
      }
    }
  }

  // Конец добавлено avis.
}