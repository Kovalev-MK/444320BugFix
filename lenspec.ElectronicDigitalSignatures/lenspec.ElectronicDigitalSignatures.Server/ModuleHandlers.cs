using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ElectronicDigitalSignatures.Server
{
  partial class MyEDSApplicationFolderHandlers
  {

    public virtual IQueryable<lenspec.ElectronicDigitalSignatures.IEDSApplication> MyEDSApplicationPreFiltering(IQueryable<lenspec.ElectronicDigitalSignatures.IEDSApplication> query)
    {
      if (_filter == null)
        return query;
      
      if (_filter.LastMonth)
      {
        var beginDate = Calendar.UserToday.AddDays(-30);
        var endDate = Calendar.UserToday;

        query = Sungero.Docflow.PublicFunctions.Module.OfficialDocumentsApplyFilterByDate(query, beginDate, endDate)
          .Cast<lenspec.ElectronicDigitalSignatures.IEDSApplication>();
      }
      
      return query;
    }

    public virtual IQueryable<lenspec.ElectronicDigitalSignatures.IEDSApplication> MyEDSApplicationDataQuery(IQueryable<lenspec.ElectronicDigitalSignatures.IEDSApplication> query)
    {
      query = query.Where(x => Users.Current == x.PreparedBy || Users.Current == x.Author);
      
      if (_filter == null)
        return query;
      
      // Контрол "Набор флажков". Фильтр по Категории заявки.
      if (_filter.InitialIssue || _filter.Renewal || _filter.Cancellation)
        query = query.Where(d => (_filter.InitialIssue && d.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.InitialIssue) ||
                            (_filter.Renewal && d.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Renewal) ||
                            (_filter.Cancellation && d.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Cancellation));
      
      // Фильтр "Интервал времени".
      if (_filter.LastWeek || _filter.LastMonth || _filter.Last90Days || _filter.ManualPeriod)
      {
        var beginDate = Calendar.UserToday.AddDays(-7);
        var endDate = Calendar.UserToday;
        
        if (_filter.LastMonth)
          beginDate = Calendar.UserToday.AddDays(-30);
        if (_filter.Last90Days)
          beginDate = Calendar.UserToday.AddDays(-90);
        
        if (_filter.ManualPeriod)
        {
          beginDate = _filter.DateRangeFrom ?? Calendar.SqlMinValue;
          endDate = _filter.DateRangeTo ?? Calendar.SqlMaxValue;
        }
        
        query = Sungero.Docflow.PublicFunctions.Module.OfficialDocumentsApplyFilterByDate(query, beginDate, endDate)
          .Cast<lenspec.ElectronicDigitalSignatures.IEDSApplication>();
      }
      
      return query;
    }
  }

  partial class ElectronicDigitalSignaturesHandlers
  {
  }
}