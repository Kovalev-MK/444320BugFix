using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.EDSApplication;

namespace lenspec.ElectronicDigitalSignatures
{
  partial class EDSApplicationFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      if (_filter == null)
        return base.Filtering(query, e);
      
      query = base.Filtering(query, e);
      
      // Контрол "Ссылка на сущность". Фильтр по полю Сотрудник.
      if (_filter.PreparedBy != null)
        query = query.Where(d => d.PreparedBy == _filter.PreparedBy);
      
      // Контрол "Набор флажков". Фильтр по Категории заявки.
      if (_filter.InitialIssue || _filter.Renewal || _filter.Cancellation)
        query = query.Where(d => (_filter.InitialIssue && d.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.InitialIssue) ||
                            (_filter.Renewal && d.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Renewal) ||
                            (_filter.Cancellation && d.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Cancellation));
      
      return query;
    }
  }

  partial class EDSApplicationServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      // Выдать получателю УКЭП права на просмотр документа.
      var preparedBy = _obj.PreparedBy;
      if (preparedBy != null && !Equals(_obj.State.Properties.PreparedBy.OriginalValue, preparedBy) &&
          !Equals(preparedBy, Sungero.Company.Employees.Current) &&
          !_obj.AccessRights.IsGrantedDirectly(DefaultAccessRightsTypes.Read, preparedBy) &&
          _obj.AccessRights.StrictMode != AccessRightsStrictMode.Enhanced)
        _obj.AccessRights.Grant(preparedBy, DefaultAccessRightsTypes.Read);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsUsedInRX = false;
      _obj.IsSelfCancellation = false;
      _obj.ApplicationState = lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationState.Draft;
    }
  }

}