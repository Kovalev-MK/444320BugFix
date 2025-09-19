using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.ActAcceptanceOfApartment;

namespace lenspec.SalesDepartmentArchive
{

  partial class ActAcceptanceOfApartmentFilteringServerHandler<T>
  {
    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      //query = base.Filtering(query, e);
      
      if (_filter == null)
        return query;
      
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

  partial class ActAcceptanceOfApartmentClientContractPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> ClientContractFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Отображаем клиентские договоры, только с данным клиентом.
      if (_obj.Client != null)
        query = query.Where(q => q.CounterpartyClient.Any(c => c.ClientItem == _obj.Client));
      
      return query;
    }
  }

  partial class ActAcceptanceOfApartmentDocumentKindPropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация выбора из спика "Вид документа."
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override IQueryable<T> DocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var actAcceptanceOfApartmentTypeGuid = lenspec.SalesDepartmentArchive.PublicConstants.Module.ActAcceptanceOfApartmentTypeGuid;
      query = query.Where(q => q.DocumentType.DocumentTypeGuid == actAcceptanceOfApartmentTypeGuid.ToString());
      
      return query;
    }
  }

  partial class ActAcceptanceOfApartmentServerHandlers
  {
    
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Формируем имя документа.
      //      if (_obj.ActDate == null)
      //        _obj.Name = $"{_obj.DocumentKind.Name} к {_obj.ClientContract.Name} клиент {_obj.Client.Name}";
      //      else
      //        _obj.Name = $"{_obj.DocumentKind.Name} от {_obj.ActDate.Value.ToString("dd.MM.yyyy")} к {_obj.ClientContract.Name} клиент {_obj.Client.Name}";
      
      Functions.ActAcceptanceOfApartment.FillName(_obj);
    }
    
    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      //base.Created(e);
      
      // Заполняем название.
      _obj.Name = "<Имя будет сформировано автоматически по содержанию и другим реквизитам документа>";
      
      _obj.IsNotDefect = false;
    }
  }
}