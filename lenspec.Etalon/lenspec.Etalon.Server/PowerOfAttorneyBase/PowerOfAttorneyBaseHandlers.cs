using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorneyBase;

namespace lenspec.Etalon
{

  partial class PowerOfAttorneyBaseRepresentativesEmployeelenspecPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> RepresentativesEmployeelenspecFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (Sungero.Parties.People.Is(_obj.IssuedTo))
        query = query.Where(x => Equals(x.Person, _obj.IssuedTo));
      return query;
    }
  }

}