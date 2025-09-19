using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.ResponsibleByCounterparty;

namespace avis.EtalonParties
{
  partial class ResponsibleByCounterpartyCounterpartyPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> CounterpartyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => !Sungero.Parties.People.Is(x));
      return query;
    }
  }

  partial class ResponsibleByCounterpartyServerHandlers
  {
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if ((_obj.State.IsInserted || _obj.State.Properties.Counterparty.IsChanged || _obj.State.Properties.BusinessUnit.IsChanged) &&
          Functions.ResponsibleByCounterparty.GetDuplicates(_obj).Any())
      {
        e.AddError(avis.EtalonParties.ResponsibleByCounterparties.Resources.DetectedDuplicates, _obj.Info.Actions.ShowDuplicates);
        return;
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      if (Sungero.Company.Employees.Current != null)
        _obj.BusinessUnit = Sungero.Company.Employees.Current.Department.BusinessUnit;
      _obj.CreatedAt = Calendar.Today;
    }
  }

  partial class ResponsibleByCounterpartyBusinessUnitPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> BusinessUnitFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var substitutedBUIds = Functions.ResponsibleByCounterparty.GetSubstitutedBusinessUnitIds(_obj);
      query = query.Where(x => substitutedBUIds.Contains(x.Id));
      
      return query;
    }
  }

}