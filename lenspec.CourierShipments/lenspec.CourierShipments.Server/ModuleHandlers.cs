using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.CourierShipments.Server
{
  partial class CourierShipmentsApplicationListFolderHandlers
  {

    //Добавлено Avis Expert
    public virtual IQueryable<lenspec.CourierShipments.ICourierShipmentsApplication> CourierShipmentsApplicationListDataQuery(IQueryable<lenspec.CourierShipments.ICourierShipmentsApplication> query)
    {
      if (_filter == null)
        return query;
      
      // Фильтр по журналу регистрации.
      if (_filter.DocumentRegister != null)
        query = query.Where(d => Equals(d.DocumentRegister, _filter.DocumentRegister));
      
      // Фильтр по нашей организации.
      if (_filter.BusinessUnit != null)
        query = query.Where(d => Equals(d.BusinessUnit, _filter.BusinessUnit));
      
      // Фильтр по подразделению.
      if (_filter.Department != null)
        query = query.Where(c => Equals(c.Department, _filter.Department));
      
      // Фильтр по инициатору.
      if (_filter.Initiator != null)
        query = query.Where(c => Equals(c.Author, Users.As(_filter.Initiator)));
      
      // Фильтр по интервалу времени.
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
    //конец Добавлено Avis Expert
  }

  partial class CourierShipmentsHandlers
  {
  }
}