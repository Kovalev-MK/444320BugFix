using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustReqSetup;

namespace avis.CustomerRequests
{

  partial class CustReqSetupExecutorPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ExecutorFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(x => x.Login != null && x.Status == Status.Active);
    }
  }

  partial class CustReqSetupControllerPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ControllerFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(x => x.Login != null && x.Status == Status.Active);
    }
  }


  partial class CustReqSetupServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      _obj.BUnames = string.Join("; ", _obj.BUCollection.Select(x => x.BusinessUnit.Name).ToList());
    }
  }

}