using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ExecutionLawyerAssignment;

namespace avis.PowerOfAttorneyModule
{
  partial class ExecutionLawyerAssignmentAttorneysAttorneyPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AttorneysAttorneyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(i => Sungero.Parties.Companies.Is(i) || Sungero.Parties.People.Is(i));
      return query;
    }
  }

}