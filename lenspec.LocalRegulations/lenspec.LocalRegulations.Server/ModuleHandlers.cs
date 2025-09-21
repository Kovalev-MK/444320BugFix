using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.LocalRegulations.Server
{
  partial class LocalRegulationOfCurrentUserFolderHandlers
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Получить локальные акты текущего пользователя.
    /// </summary>
    /// <param name="_filter">Фильтр.</param>
    /// <returns>Список локальных актов текущего пользователя.</returns>
    public virtual IQueryable<Sungero.Docflow.IOfficialDocument> LocalRegulationOfCurrentUserDataQuery(IQueryable<Sungero.Docflow.IOfficialDocument> query)
    {
      query = query.Where(d => lenspec.LocalRegulations.DocumentApprovedByOrders.Is(d) || Sungero.RecordManagement.OrderBases.Is(d));
      
      query = query.Where(d => Equals(d.Author, Sungero.CoreEntities.Users.Current));
      
      if (_filter == null)
        return query;
      
      // Фильтр по журналу регистрации.
      if (_filter.DocumentRegister != null)
        query = query.Where(d => Equals(d.DocumentRegister, _filter.DocumentRegister));
      
      // Фильтр по виду документа.
      if (_filter.DocumentKind != null)
        query = query.Where(d => Equals(d.DocumentKind, _filter.DocumentKind));
      
      // Фильтр по статусу. Если все галочки включены, то нет смысла добавлять фильтр.
      if ((_filter.Registered || _filter.Reserved || _filter.NotRegistered) &&
          !(_filter.Registered && _filter.Reserved && _filter.NotRegistered))
        query = query.Where(l => _filter.Registered && l.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered ||
                            _filter.Reserved && l.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Reserved ||
                            _filter.NotRegistered && l.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.NotRegistered);
      
      // Фильтр по нашей организации.
      if (_filter.BusinessUnit != null)
        query = query.Where(d => Equals(d.BusinessUnit, _filter.BusinessUnit));
      
      // Фильтр "Подразделение".
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
      
      var serverPeriodBegin = Equals(Calendar.SqlMinValue, periodBegin)
        ? periodBegin
        : Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(periodBegin);
      var serverPeriodEnd = Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd : periodEnd.EndOfDay().FromUserTime();
      var clientPeriodEnd = !Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd.AddDays(1) : Calendar.SqlMaxValue;
      query = query.Where(j => (j.DocumentDate.Between(serverPeriodBegin, serverPeriodEnd) ||
                                j.DocumentDate == periodBegin) && j.DocumentDate != clientPeriodEnd);
      
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class LocalRegulationsOfOrganizationFolderHandlers
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Получить локальные акты организации.
    /// </summary>
    /// <param name="_filter">Фильтр.</param>
    /// <returns>Список локальных актов организации.</returns>
    public virtual IQueryable<Sungero.Docflow.IOfficialDocument> LocalRegulationsOfOrganizationDataQuery(IQueryable<Sungero.Docflow.IOfficialDocument> query)
    {
      query = query.Where(d => lenspec.LocalRegulations.DocumentApprovedByOrders.Is(d) || Sungero.RecordManagement.OrderBases.Is(d));
      
      var currentEmployee = Sungero.Company.Employees.As(Users.Current);
      if (currentEmployee != null && currentEmployee.Department != null && currentEmployee.Department.BusinessUnit != null)
      {
        query = query.Where(d => d.BusinessUnit != null && d.BusinessUnit.Equals(currentEmployee.Department.BusinessUnit));
      }
      
      if (_filter == null)
        return query;
      
      // Фильтр по журналу регистрации.
      if (_filter.DocumentRegister != null)
        query = query.Where(d => Equals(d.DocumentRegister, _filter.DocumentRegister));
      
      // Фильтр по виду документа.
      if (_filter.DocumentKind != null)
        query = query.Where(d => Equals(d.DocumentKind, _filter.DocumentKind));
      
      // Фильтр по статусу. Если все галочки включены, то нет смысла добавлять фильтр.
      if ((_filter.Registered || _filter.Reserved || _filter.NotRegistered) &&
          !(_filter.Registered && _filter.Reserved && _filter.NotRegistered))
        query = query.Where(l => _filter.Registered && l.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered ||
                            _filter.Reserved && l.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Reserved ||
                            _filter.NotRegistered && l.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.NotRegistered);
      
      // Фильтр по нашей организации.
      if (_filter.BusinessUnit != null)
        query = query.Where(d => Equals(d.BusinessUnit, _filter.BusinessUnit));
      
      // Фильтр "Подразделение".
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
      
      var serverPeriodBegin = Equals(Calendar.SqlMinValue, periodBegin)
        ? periodBegin
        : Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(periodBegin);
      var serverPeriodEnd = Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd : periodEnd.EndOfDay().FromUserTime();
      var clientPeriodEnd = !Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd.AddDays(1) : Calendar.SqlMaxValue;
      query = query.Where(j => (j.DocumentDate.Between(serverPeriodBegin, serverPeriodEnd) ||
                                j.DocumentDate == periodBegin) && j.DocumentDate != clientPeriodEnd);
      
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class LocalRegulationsHandlers
  {
  }
}