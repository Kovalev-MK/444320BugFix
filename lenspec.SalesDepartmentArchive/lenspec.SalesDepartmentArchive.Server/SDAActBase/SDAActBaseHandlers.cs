using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAActBase;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAActBaseFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация типа документа.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual IQueryable<Sungero.Docflow.IDocumentType> DocumentTypeFiltering(IQueryable<Sungero.Docflow.IDocumentType> query, Sungero.Domain.FilteringEventArgs e)
    {
      var actAcceptanceOfApartmentTypeGuid = lenspec.SalesDepartmentArchive.PublicConstants.Module.ActAcceptanceOfApartmentTypeGuid;
      var actWarrantyPeriodTypeGuid = lenspec.SalesDepartmentArchive.PublicConstants.Module.ActWarrantyPeriodTypeGuid;
      query = query.Where(q => q.DocumentTypeGuid == actAcceptanceOfApartmentTypeGuid.ToString() || q.DocumentTypeGuid == actWarrantyPeriodTypeGuid.ToString());
      
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

  partial class SDAActBaseServerHandlers
  {
    
    /// <summary>
    ///  Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.Name = "<Имя будет сформировано автоматически>";
      _obj.IsNotDefect = false;
    }
  }

  partial class SDAActBaseClientContractPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> ClientContractFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Отображаем клиентские договоры, только с данным клиентом.
      if (_obj.Client != null)
      query = query.Where(q => q.CounterpartyClient.Any(c => c.ClientItem == _obj.Client));
      
      return query;
    }
  }

}