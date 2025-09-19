using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CategoryRequest;

namespace avis.CustomerRequests
{
  partial class CategoryRequestServerHandlers
  {

    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsClaim = false;
      _obj.IsObject = false;
      _obj.IsBankDetail = false;
      _obj.ClaimType = null;
    }
  }
}