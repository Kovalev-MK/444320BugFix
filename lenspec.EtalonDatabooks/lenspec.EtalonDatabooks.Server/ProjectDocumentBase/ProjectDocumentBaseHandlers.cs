using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectDocumentBase;

namespace lenspec.EtalonDatabooks
{
  partial class ProjectDocumentBaseCollectionObjectAnProjectObjectAnProjectPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CollectionObjectAnProjectObjectAnProjectFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Фильтрация активных записей.
      query = query.Where(q => q.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      
      // Фильтрация по ИСП.
      if (_root.OurCF != null)
        query = query.Where(q => q.OurCF == _root.OurCF);
        
      return query;
    }
  }



}