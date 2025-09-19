using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.ActsOfManagementCompanyBase;

namespace lenspec.SalesDepartmentArchive
{
  partial class ActsOfManagementCompanyBaseFilteringServerHandler<T>
  {

    public virtual IQueryable<Sungero.Docflow.IDocumentType> DocumentTypeFiltering(IQueryable<Sungero.Docflow.IDocumentType> query, Sungero.Domain.FilteringEventArgs e)
    {
      var actsOfManagementCompany = lenspec.SalesDepartmentArchive.PublicConstants.Module.ActOfManagementCompanyGuid;
      query = query.Where(q => q.DocumentTypeGuid == actsOfManagementCompany);
      
      return query;
    }

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      //query = base.Filtering(query, e);
      
      if (_filter == null)
        return query;
      
      // Фильтр по типу документа.
      if (_filter.DocumentType != null)
        query = query.Where(d => Equals(d.DocumentKind.DocumentType, _filter.DocumentType));
      
      // Фильтр по виду документа.
      if (_filter.DocumentKind != null)
        query = query.Where(d => Equals(d.DocumentKind, _filter.DocumentKind));
      
      // Фильтруем по автору.
      if (_filter.Author != null)
        query = query.Where(q => Equals(q.Author, _filter.Author));
      
      // Фильтр по подразделению.
      if (_filter.Department != null)
        query = query.Where(c => Equals(c.Department, _filter.Department));
      
      // Фильтр по интервалу времени
      var periodBegin = Calendar.UserToday.AddDays(-7);
      var periodEnd = Calendar.UserToday.EndOfDay();
      
      if (_filter.LastWeek)
        periodBegin = Calendar.UserToday.AddDays(-7);
      
      if (_filter.LastMonth)
        periodBegin = Calendar.UserToday.AddDays(-30);
      
      if (_filter.Last90Days)
        periodBegin = Calendar.UserToday.AddDays(-90);
      
      if (_filter.ManualPeriod)
      {
        periodBegin = _filter.DateRangeFrom ?? Calendar.SqlMinValue;
        periodEnd = _filter.DateRangeTo ?? Calendar.SqlMaxValue;
      }
      
      var serverPeriodBegin = Equals(Calendar.SqlMinValue, periodBegin) ? periodBegin : Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(periodBegin);
      var serverPeriodEnd = Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd : periodEnd.EndOfDay().FromUserTime();
      var clientPeriodEnd = !Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd.AddDays(1) : Calendar.SqlMaxValue;
      
      query = query.Where(j => (j.Created.Between(serverPeriodBegin, serverPeriodEnd) ||
                                j.Created == periodBegin) && j.Created != clientPeriodEnd);
      
      return query;
    }
  }

  partial class ActsOfManagementCompanyBaseRoomPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> RoomFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.OurCF != null)
        query = query.Where(q => q.OurCF == _obj.OurCF);
      
      return query;
    }
  }

  partial class ActsOfManagementCompanyBaseOurCFPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> OurCFFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Room != null)
        query = query.Where(q => q == _obj.Room.OurCF);
      
      return query;
    }
  }

  partial class ActsOfManagementCompanyBaseManagementContractMKDPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ManagementContractMKDFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Owner != null)
        query = query.Where(q => q.Client == _obj.Owner);
      
      return query;
    }
  }

}