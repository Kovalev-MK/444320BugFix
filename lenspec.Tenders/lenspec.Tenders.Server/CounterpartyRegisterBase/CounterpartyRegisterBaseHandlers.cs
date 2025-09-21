using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.CounterpartyRegisterBase;

namespace lenspec.Tenders
{
  partial class CounterpartyRegisterBasePresenceRegionPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> PresenceRegionFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.IsUsedForQualification.HasValue && x.IsUsedForQualification.Value == true &&
                         x.Status == avis.EtalonContracts.PresenceRegion.Status.Active);
      
      return query;
    }
  }

  partial class CounterpartyRegisterBaseFilteringServerHandler<T>
  {

    public virtual IQueryable<avis.EtalonContracts.IPresenceRegion> PresenceRegionFiltering(IQueryable<avis.EtalonContracts.IPresenceRegion> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = query.Where(x => x.IsUsedForQualification.HasValue && x.IsUsedForQualification.Value == true &&
                         x.Status == avis.EtalonContracts.PresenceRegion.Status.Active);
      
      return query;
    }

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      if (_filter == null)
        return query;
      
      if (_filter.IsStrategicPartner || _filter.IsBlacklist)
        query = query.Where(l => _filter.IsStrategicPartner && l.IsStrategicPartner == _filter.IsStrategicPartner ||
                            _filter.IsBlacklist && l.IsBlacklist == _filter.IsBlacklist);
      
      if (_filter.Active || _filter.Closed)
        query = query.Where(r => _filter.Active && r.Status == Status.Active || _filter.Closed && r.Status == Status.Closed);

      if (_filter.PresenceRegion != null)
        query = query.Where(r => Equals(r.PresenceRegion, _filter.PresenceRegion));

      if (_filter.Counterparty != null)
        query = query.Where(r => Equals(r.Counterparty, _filter.Counterparty));
      
      if (_filter.LastMonth || _filter.Last90Days || _filter.Last180Days || _filter.ManualPeriod)
      {
        var periodBegin = Calendar.UserToday.AddDays(-30);
        var periodEnd = Calendar.UserToday.EndOfDay();
        
        if (_filter.LastMonth)
          periodBegin = Calendar.UserToday.AddDays(-30);
        
        if (_filter.Last90Days)
          periodBegin = Calendar.UserToday.AddDays(-90);
        
        if (_filter.Last180Days)
          periodBegin = Calendar.UserToday.AddDays(-180);
        
        if (_filter.ManualPeriod)
        {
          periodBegin = _filter.DateRangeFrom ?? Calendar.SqlMinValue;
          periodEnd = _filter.DateRangeTo ?? Calendar.SqlMaxValue;
        }
        
        query = Functions.Module.ApplyFilterByCreatedDate(query, periodBegin, periodEnd).Cast<T>();
      }
      
      return query;
    }
  }

  partial class CounterpartyRegisterBaseServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Created = Calendar.Now;
      _obj.Author = Sungero.Company.Employees.Current;
      if (!_obj.State.IsCopied)
      {
        _obj.IsBlacklist = false;
        _obj.IsStrategicPartner = false;
      }
    }
  }

  partial class CounterpartyRegisterBaseQCDecisionPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> QCDecisionFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var decisionOnInclusionKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DecisionOnInclusionOfCounterpartyKind);
      var decisionOnExclusionKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DecisionOnExclusionOfCounterpartyKind);
      query = query.Where(x => x.DocumentKind == decisionOnInclusionKind || x.DocumentKind == decisionOnExclusionKind);
      if (_obj.Counterparty != null)
        query = query.Where(x => x.Counterparties.Any(c => Equals(c.Counterparty, _obj.Counterparty)));
      
      return query;
    }
  }

}