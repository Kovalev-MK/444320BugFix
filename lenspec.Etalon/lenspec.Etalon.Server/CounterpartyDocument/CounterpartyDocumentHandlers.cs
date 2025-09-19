using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.CounterpartyDocument;

namespace lenspec.Etalon
{
  partial class CounterpartyDocumentCounterpartyPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> CounterpartyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.CounterpartyFiltering(query, e);
      
      // В поле для выбора доступны записи справочника Организации, Банки.
      if (Functions.CounterpartyDocument.IsDocumentKind(_obj, Module.Parties.Constants.Module.ExtractFromEGRULKind) ||
          Functions.CounterpartyDocument.IsDocumentKind(_obj, Module.Parties.Constants.Module.ConstituentDocumentKind) ||
          Functions.CounterpartyDocument.IsDocumentKind(_obj, Module.Parties.Constants.Module.CharterAndChangesKind))
        query = query.Where(c => Sungero.Parties.CompanyBases.Is(c));
      
      return query;
    }
  }

  partial class CounterpartyDocumentServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      var isExtractFromEGRULKind = Functions.CounterpartyDocument.IsDocumentKind(_obj, Module.Parties.Constants.Module.ExtractFromEGRULKind);
      var isCharterAndChangesKind = Functions.CounterpartyDocument.IsDocumentKind(_obj, Module.Parties.Constants.Module.CharterAndChangesKind);
      var isConstituentDocumentKind = Functions.CounterpartyDocument.IsDocumentKind(_obj, Module.Parties.Constants.Module.ConstituentDocumentKind);
      
      // Дубли проверем только при заполненном Контрагенте для нужных Видов документов.
      if (_obj.Counterparty != null && (isExtractFromEGRULKind || isCharterAndChangesKind || isConstituentDocumentKind) &&
          Functions.CounterpartyDocument.GetDuplicates(_obj).Any())
      {
        var duplicateFoundErrorMessage = string.Empty;
        // Сообщение валидации для вида "Выписка из ЕГРЮЛ/ЕГРИП".
        if (isExtractFromEGRULKind)
          duplicateFoundErrorMessage = lenspec.Etalon.CounterpartyDocuments.Resources.DuplicateFoundForExtractFromEGRUL;
        // Сообщение валидации для вида "Устав и изменения".
        if (isCharterAndChangesKind)
          duplicateFoundErrorMessage = lenspec.Etalon.CounterpartyDocuments.Resources.DuplicateFoundForCharterAndChanges;
        // Сообщение валидации для вида "Учредительный документ".
        if (isConstituentDocumentKind)
          duplicateFoundErrorMessage = lenspec.Etalon.CounterpartyDocuments.Resources.DuplicateFoundForConstituentDocument;
        
        e.AddError(duplicateFoundErrorMessage, _obj.Info.Actions.ShowDuplicateslenspec);
        return;
      }
      
      if ((isExtractFromEGRULKind || isCharterAndChangesKind || isConstituentDocumentKind) && !_obj.HasVersions)
        e.AddError(lenspec.Etalon.CounterpartyDocuments.Resources.VersionsNotFoundError);
    }
    
    // Добавлено avis.
    
    /// <summary>
    /// Создание документа.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      // Устанавливаем "нашу организацию".
      //_obj.BusinessUnitavis = Employees.Current.Department.BusinessUnit;
    }
    
    // Конец добавлено avis.
  }


}