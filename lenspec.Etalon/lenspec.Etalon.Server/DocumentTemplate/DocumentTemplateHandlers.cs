using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentTemplate;

namespace lenspec.Etalon
{
  partial class DocumentTemplateContractKindslenspecContractKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContractKindslenspecContractKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query;
    }
  }

  partial class DocumentTemplateFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = base.Filtering(query, e);

      // Фильтрация по агентам продаж для договоров.
      var contract = Sungero.Contracts.Contracts.As(_createFromTemplateContext);
      if (contract != null)
      {
        var counterparty = lenspec.Etalon.Counterparties.As(contract.Counterparty);
        var isSalesAgent = counterparty.SalesAgentlenspec == true;
        
        return query.Where(x =>
                           x.IsSalesAgentlenspec == null && !isSalesAgent ||
                           x.IsSalesAgentlenspec == isSalesAgent);
      }
      
      return query;
    }
  }

  partial class DocumentTemplateServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsSalesAgentlenspec = false;
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      bool isDocumentTypeRestrictionEnabled;
      e.Params.TryGetValue(lenspec.Etalon.DocumentTemplates.Resources.DocumentTypesRestrictionEnabled, out isDocumentTypeRestrictionEnabled);
      
      if (Functions.DocumentTemplate.IsDocumentTypeRestrictionError(_obj.DocumentType.ToString(), isDocumentTypeRestrictionEnabled))
        e.AddError(lenspec.Etalon.DocumentTemplates.Resources.DocumentTypesRestrictionError);
      
      var duplicates = Functions.DocumentTemplate.GetDuplicateDocumentTemplates(_obj.Name, _obj.Id);
      if (duplicates.Any())
      {
        e.AddError(lenspec.Etalon.DocumentTemplates.Resources.DocumentTemplateAlreadyExists, _obj.Info.Actions.ShowDuplicateslenspec);
        return;
      }
    }
  }

}