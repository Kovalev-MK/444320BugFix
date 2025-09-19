using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderDocumentBase;

namespace lenspec.Tenders
{
  partial class TenderDocumentBaseServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      // Очистить поля оргструктуры, которые автоматически заполнились при создании внутреннего документа.
      Functions.TenderDocumentBase.UpdateOrganizationStructure(_obj);
    }
    //конец Добавлено Avis Expert
  }

  partial class TenderDocumentBaseBusinessUnitPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public override IQueryable<T> BusinessUnitFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.BusinessUnitFiltering(query, e);
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class TenderDocumentBaseObjectAnProjectsObjectAnProjectPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public virtual IQueryable<T> ObjectAnProjectsObjectAnProjectFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.IsLinkToInvest != true);
      
      if (_root.OurCF.Any(x => x.OurCF != null))
      {
        var ourCFIds = _root.OurCF.Where(x => x.OurCF != null).Select(x => x.OurCF.Id).ToList();
        query = query.Where(x => x.OurCF != null && ourCFIds.Contains(x.OurCF.Id));
      }
      return query;
    }
    //конец Добавлено Avis Expert
  }

}