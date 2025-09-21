using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectObjectPermitDocument;

namespace lenspec.EtalonDatabooks
{
  partial class ProjectObjectPermitDocumentCollectionAccountingProjectAccountingProjectPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CollectionAccountingProjectAccountingProjectFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Фильтрация активных записей.
      query = query.Where(q => q.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      
      // Фильтрация по ИСП.
      if (_root.OurCF != null)
        query = query.Where(q => q.OurCF == _root.OurCF);
      
      // Фильтрация по Связано с Инвест
      query = query.Where(x => x.IsLinkToInvest == null || x.IsLinkToInvest == false);
      return query;
    }
  }

  partial class ProjectObjectPermitDocumentCollectionObjectAnProjectObjectAnProjectPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> CollectionObjectAnProjectObjectAnProjectFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.CollectionObjectAnProjectObjectAnProjectFiltering(query, e);
      query = query.Where(x => x.IsLinkToInvest == true);
      return query;
    }
  }



  partial class ProjectObjectPermitDocumentOurCFPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> OurCFFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.OurCFFiltering(query, e);
      query = query.Where(q => q.Status == lenspec.EtalonDatabooks.OurCF.Status.Active);
      return query;
    }
  }

  partial class ProjectObjectPermitDocumentServerHandlers
  {
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Проверяем на дубли.
      var projectObjectPermitDoc = lenspec.EtalonDatabooks.ProjectObjectPermitDocuments.GetAll(p => p.Id != _obj.Id && p.NumberRNV == _obj.NumberRNV && p.DateRNV == _obj.DateRNV);
      if (projectObjectPermitDoc.Count() > 0)
      {
        e.AddError(lenspec.EtalonDatabooks.ProjectObjectPermitDocuments.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
        return;
      }
      
      // Проверяем на наличие версии.
      if (!_obj.HasVersions)
      {
        e.AddError(lenspec.EtalonDatabooks.ProjectObjectPermitDocuments.Resources.NeedCreateDocumentVersion);
        return;
      }
      _obj.Subject = _obj.Name;
      
      base.BeforeSave(e);
    }
  }
}