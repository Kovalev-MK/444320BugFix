using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters.Server
{
  partial class MassMailingTasksOfCurrentUserFolderHandlers
  {

    //Добавлено Avis Expert
    public virtual IQueryable<Sungero.Docflow.IApprovalTask> MassMailingTasksOfCurrentUserDataQuery(IQueryable<Sungero.Docflow.IApprovalTask> query)
    {
      var taskIds = PublicFunctions.Module.GetCurrentUserTaskWithMassMailingApplication();      
      query = query.Where(x => taskIds.Contains(x.Id));
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class OutgoingLettersOfCurrentUserFolderHandlers
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Получить исходящие письма текущего пользователя.
    /// </summary>
    /// <param name="_filter">Фильтр.</param>
    /// <returns>Список исходящих писем текущего пользователя.</returns>
    public virtual IQueryable<Sungero.RecordManagement.IOutgoingLetter> OutgoingLettersOfCurrentUserDataQuery(IQueryable<Sungero.RecordManagement.IOutgoingLetter> query)
    {
      query = query.Where(d => Equals(d.Author, Sungero.CoreEntities.Users.Current));
      
      if (_filter == null)
        return query;
      
      // Фильтр по журналу регистрации.
      if (_filter.DocumentRegister != null)
        query = query.Where(d => d.DocumentRegister == _filter.DocumentRegister);
      
      // Фильтр по виду документа.
      if (_filter.DocumentKind != null)
        query = query.Where(d => d.DocumentKind == _filter.DocumentKind);
      
      // Фильтр по статусу. Если все галочки включены, то нет смысла добавлять фильтр.
      if ((_filter.Registered || _filter.Reserved || _filter.NotRegistered) &&
          !(_filter.Registered && _filter.Reserved && _filter.NotRegistered))
        query = query.Where(l => _filter.Registered && l.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered ||
                            _filter.Reserved && l.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Reserved ||
                            _filter.NotRegistered && l.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.NotRegistered);
      
      // Фильтр по контрагенту.
      if (_filter.Counterparty != null)
        query = query.Where(d => d.Addressees.Select(x => x.Correspondent).Any(y => Equals(y, _filter.Counterparty)));
      
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
      
      var serverPeriodBegin = Equals(Calendar.SqlMinValue, periodBegin) ? periodBegin : Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(periodBegin);
      var serverPeriodEnd = Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd : periodEnd.EndOfDay().FromUserTime();
      var clientPeriodEnd = !Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd.AddDays(1) : Calendar.SqlMaxValue;
      query = query.Where(j => (j.DocumentDate.Between(serverPeriodBegin, serverPeriodEnd) ||
                                j.DocumentDate == periodBegin) && j.DocumentDate != clientPeriodEnd);
      
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class OutgoingLettersHandlers
  {
  }
}