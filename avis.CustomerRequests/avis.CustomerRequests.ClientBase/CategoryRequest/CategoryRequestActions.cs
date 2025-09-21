using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CategoryRequest;

namespace avis.CustomerRequests.Client
{
  partial class CategoryRequestActions
  {
    public virtual void ShowSettings(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      avis.CustomerRequests.CustReqSetups.GetAll(c => c.CustReqCategory == _obj).Show();
    }

    public virtual bool CanShowSettings(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && !_obj.State.IsChanged;
    }
  }
}