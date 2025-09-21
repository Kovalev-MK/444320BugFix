using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PostalItems.CategoryRPO;

namespace avis.PostalItems
{
  partial class CategoryRPOCreatingFromServerHandler
  {
    /// <summary>
    /// Копирование.
    /// </summary>
    /// <param name="e"></param>
    public override void CreatingFrom(Sungero.Domain.CreatingFromEventArgs e)
    {
      e.Without(_info.Properties.Sid);
    }
  }
}