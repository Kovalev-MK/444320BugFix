using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ApplicationsForPayment.Server
{
  partial class MyApplicationsForPaymentFolderHandlers
  {

    public virtual IQueryable<lenspec.ApplicationsForPayment.IApplicationForPayment> MyApplicationsForPaymentPreFiltering(IQueryable<lenspec.ApplicationsForPayment.IApplicationForPayment> query)
    {
      query = query.Where(x => x.Author == Users.Current);
      return query;
    }
  }

  partial class ApplicationsForPaymentHandlers
  {
  }
}