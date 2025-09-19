using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.CreateCompanyTask;

namespace avis.EtalonParties
{
  partial class CreateCompanyTaskResponsibleByCounterpartyPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> ResponsibleByCounterpartyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.ResponsibleByCounterparty != null)
      {
        var substitutedBUIds = Functions.ResponsibleByCounterparty.GetSubstitutedBusinessUnitIds(_obj.ResponsibleByCounterparty);
        query = query.Where(x => substitutedBUIds.Contains(x.BusinessUnit.Id));
      }
      return query;
    }
  }

  partial class CreateCompanyTaskBusinessUnitPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> BusinessUnitFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.ResponsibleByCounterparty != null)
      {
        var substitutedBUIds = Functions.ResponsibleByCounterparty.GetSubstitutedBusinessUnitIds(_obj.ResponsibleByCounterparty);
        query = query.Where(x => substitutedBUIds.Contains(x.Id));
      }
      return query;
    }
  }

  partial class CreateCompanyTaskFilteringServerHandler<T>
  {
    
    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      if (_filter == null)
        return query;
      
      query = query.Where(x => (_filter.Initiator == null || x.Author == _filter.Initiator) &&
                          (_filter.DateRangeFrom == null || x.Created == _filter.DateRangeFrom || x.Created > _filter.DateRangeFrom) &&
                          (_filter.DateRangeTo == null || x.Created == _filter.DateRangeTo || x.Created < _filter.DateRangeTo));
      return query;
    }
  }
  
  partial class CreateCompanyTaskServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      // Если отсутствует автор, заполняем.
      if (_obj.Author == null)
        _obj.Author = Users.Current;
      
      // Генерируем тему задачи.
      PublicFunctions.CreateCompanyTask.NameGenerator(_obj);
    }
    
    /// <summary>
    /// До старта.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      PublicFunctions.CreateCompanyTask.NameGenerator(_obj);
      
      // Если поле инн заполнено, то проверяем правильность заполненности ИНН.
      if (!string.IsNullOrEmpty(_obj.TIN))
      {
        // Проверяем что инн нужного формата.
        var errorMessage = Sungero.Parties.PublicFunctions.Counterparty.CheckTin(_obj.TIN, true);
        if (!string.IsNullOrEmpty(errorMessage))
        {
          e.AddError(errorMessage);
          return;
        }
      }
      
      if (_obj.TypeRequest == CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
      {
        if (_obj.DatabookActionType == CreateCompanyTask.DatabookActionType.EditEntry &&
            !_obj.AttachmentGroup.All.Contains(_obj.ResponsibleByCounterparty))
          e.AddError("Добавьте в задачу запись справочника «Ответственные по контрагентам»");
        
        return;
      }
      
      // Если заявка на контрагента и прикреплено 0 докумнетов, то выводим ошибку.
      if (_obj.Attachments.Count == 0 && _obj.TypeObject == CreateCompanyTask.TypeObject.Counterparty)
      {
        e.AddError("Добавьте в задачу документ вида «Сведения о контрагенте»");
        return;
      }
      
      if (_obj.TypeObject == CreateCompanyTask.TypeObject.Counterparty)
      {
        // TODO: Вынести guid в константы. Docflow.
        var counterpartyDocument = Guid.Parse("49d0c5e7-7069-44d2-8eb6-6e3098fc8b10");
        // Получаем список id документов сведения о контрагенте.
        var counterpartyDocumentIds = _obj.AttachmentDetails.Where(a=> a.AttachmentTypeGuid == counterpartyDocument)
          .Select(a => a.AttachmentId);
        // Получаем все документы "Сведения о контрагентах".
        var counterpartyDocuments = Sungero.Docflow.CounterpartyDocuments.GetAll(c => counterpartyDocumentIds.Contains(c.Id));
        // Проверяем наличие версий документа. (null - не у одного документа нету версии).
        var version = counterpartyDocuments.Where(c => c.Versions.FirstOrDefault(v => v != null) != null).FirstOrDefault();
        
        if (version == null)
        {
          e.AddError("Добавьте в задачу документ вида «Сведения о контрагенте» и создайте хотя бы одну версию.");
          return;
        }
      }
    }
  }
}