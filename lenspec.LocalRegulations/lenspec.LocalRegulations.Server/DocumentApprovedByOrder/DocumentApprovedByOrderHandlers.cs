using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.LocalRegulations.DocumentApprovedByOrder;

namespace lenspec.LocalRegulations
{
  partial class DocumentApprovedByOrderLeadingDocumentPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public override IQueryable<T> LeadingDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.LeadingDocumentFiltering(query, e);
      
      if (_obj.BusinessUnit != null)
      {
        query = query.Where(x => x.BusinessUnit != null && x.BusinessUnit.Equals(_obj.BusinessUnit));
      }
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class DocumentApprovedByOrderCreatingFromServerHandler
  {

    //Добавлено Avis Expert
    public override void CreatingFrom(Sungero.Domain.CreatingFromEventArgs e)
    {
      base.CreatingFrom(e);
      
      if (_source.LeadingDocument == null || !_source.LeadingDocument.AccessRights.CanRead())
        e.Without(_info.Properties.LeadingDocument);
    }
    //конец Добавлено Avis Expert
  }

  partial class DocumentApprovedByOrderServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      var isRemainObsoleteAfterTypeChange = e.Params.Contains(string.Format("doc{0}_ConvertingFrom", _obj.Id)) &&
        _obj.LifeCycleState == Sungero.Docflow.OfficialDocument.LifeCycleState.Obsolete;
      
      base.Created(e);
      
      if (_obj.State.IsInserted && _obj.LeadingDocument != null)
        _obj.Relations.AddFrom(Sungero.Docflow.PublicConstants.Module.AddendumRelationName, _obj.LeadingDocument);
      
      if (!isRemainObsoleteAfterTypeChange)
        _obj.LifeCycleState = Sungero.Docflow.OfficialDocument.LifeCycleState.Active;
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var leadingDocumentChanged = !Equals(_obj.LeadingDocument, _obj.State.Properties.LeadingDocument.OriginalValue);
      if (leadingDocumentChanged)
      {
        // Проверить, доступен ли для изменения ведущий документ.
        var isLeadingDocumentDisabled = Sungero.Docflow.PublicFunctions.OfficialDocument.NeedDisableLeadingDocument(_obj);
        if (isLeadingDocumentDisabled)
          e.AddError(Sungero.Docflow.OfficialDocuments.Resources.RelationPropertyDisabled);
        
        if (Sungero.Docflow.PublicFunctions.OfficialDocument.IsProjectDocument(_obj.LeadingDocument, new List<long>()))
          e.Params.AddOrUpdate(Sungero.Docflow.PublicConstants.OfficialDocument.GrantAccessRightsToProjectDocument, true);
      }
      
      base.BeforeSave(e);
      
      if (_obj.LeadingDocument != null && leadingDocumentChanged && _obj.AccessRights.StrictMode != AccessRightsStrictMode.Enhanced)
      {
        var accessRightsLimit = Etalon.PublicFunctions.OfficialDocument.GetAvailableAccessRights(_obj);
        if (accessRightsLimit != Guid.Empty)
          Etalon.PublicFunctions.OfficialDocument.CopyAccessRightsToDocument(Etalon.OfficialDocuments.As(_obj.LeadingDocument), Sungero.Docflow.OfficialDocuments.As(_obj), accessRightsLimit);
      }
      
      if (_obj.LeadingDocument != null && _obj.LeadingDocument.AccessRights.CanRead() &&
          !_obj.Relations.GetRelatedFromDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName).Contains(_obj.LeadingDocument))
        _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.AddendumRelationName, _obj.State.Properties.LeadingDocument.OriginalValue, _obj.LeadingDocument);
    }
    //конец Добавлено Avis Expert
  }
}