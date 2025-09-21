using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingLetter;

namespace lenspec.Etalon
{
  partial class IncomingLetterManagementContractsMKDlenspecManagementContractMKDPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ManagementContractsMKDlenspecManagementContractMKDFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.LifeCycleState == Sungero.Docflow.OfficialDocument.LifeCycleState.Active);
      return query;
    }
  }

  partial class IncomingLetterClientContractslenspecClientContractPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ClientContractslenspecClientContractFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.LifeCycleState == Sungero.Docflow.OfficialDocument.LifeCycleState.Active);
      return query;
    }
  }

  partial class IncomingLetterServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.IsCorrespondenceWithinGrouplenspec = false;
    }
  }



}