using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ConstantDatabook;

namespace lenspec.EtalonDatabooks
{
  partial class ConstantDatabookServerHandlers
  {
    // Добавлено avis.

    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsNumeration = false;
    }
    
    // Конец добавлено avis.
  }

}