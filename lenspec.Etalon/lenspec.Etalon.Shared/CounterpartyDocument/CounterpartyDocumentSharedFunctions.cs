using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.CounterpartyDocument;

namespace lenspec.Etalon.Shared
{
  partial class CounterpartyDocumentFunctions
  {

    /// <summary>
    /// Обновление состояния полей.
    /// </summary>
    public void UpdateFields(Sungero.Docflow.IDocumentKind documentKind)
    {
      var isExtractFromEGRULKind = Functions.CounterpartyDocument.Remote.IsDocumentKind(documentKind, Module.Parties.Constants.Module.ExtractFromEGRULKind);
      
      _obj.State.Properties.DocumentKind.IsEnabled = !_obj.HasVersions;
      _obj.State.Properties.CustomDocumentDatelenspec.IsVisible = isExtractFromEGRULKind;
      _obj.State.Properties.CustomDocumentDatelenspec.IsEnabled = !(isExtractFromEGRULKind && _obj.HasVersions);
      _obj.State.Properties.Counterparty.IsEnabled = !(isExtractFromEGRULKind && _obj.HasVersions);
      
      _obj.State.Properties.Subject.IsRequired = !isExtractFromEGRULKind;
    }
    
    public override void FillName()
    {
      var documentKind = _obj.DocumentKind;
      if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      
      if (documentKind == null || !documentKind.GenerateDocumentName.Value)
        return;
      
      var name = string.Empty;
      if (Functions.CounterpartyDocument.Remote.IsDocumentKind(_obj, Module.Parties.Constants.Module.ExtractFromEGRULKind))
        name = GetExtractFromEGRULDocName();
      else if (Functions.CounterpartyDocument.Remote.IsDocumentKind(_obj, Module.Parties.Constants.Module.CharterAndChangesKind))
        name = GetCharterAndChangesDocName();
      else
        name = GetDefaultDocName();
      
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      _obj.Name = name;
    }
    
    /// <summary>
    /// Наименование для вида документа "Выписка из ЕГРЮЛ/ЕГРИП".
    /// </summary>
    /// <returns>Имя документа в заданном формате.</returns>
    private string GetExtractFromEGRULDocName()
    {
      // Имя в формате: <Вид документа> от <Дата документа> контрагента: <Наименование из поля Контрагент>.
      using (TenantInfo.Culture.SwitchTo())
      {
        return string.Format(
          "{0} от {1} контрагента: {2}",
          _obj.DocumentKind == null ?               "<Вид документа>" :             _obj.DocumentKind.Name,
          _obj.CustomDocumentDatelenspec == null ?  "<Дата документа>" :            _obj.CustomDocumentDatelenspec.Value.ToShortDateString(),
          _obj.Counterparty == null ?               "<Наименование контрагента>" :  _obj.Counterparty.Name
         );
      }
    }
    
    /// <summary>
    /// Наименование для вида документа "Устав и изменения".
    /// </summary>
    /// <returns>Имя документа в заданном формате.</returns>
    private string GetCharterAndChangesDocName()
    {
      // Имя в формате: <Вид документа> контрагента: <Наименование из поля Контрагент>.
      using (TenantInfo.Culture.SwitchTo())
      {
        return string.Format(
          "{0} контрагента: {1}",
          _obj.DocumentKind == null ? "<Вид документа>" :             _obj.DocumentKind.Name,
          _obj.Counterparty == null ? "<Наименование контрагента>" :  _obj.Counterparty.Name
         );
      }
    }
    
    /// <summary>
    /// Наименование документа по умолчанию.
    /// </summary>
    /// <returns>Имя документа в заданном формате.</returns>
    private string GetDefaultDocName()
    {
      // Имя в формате: <Вид документа>: <Наименование> контрагента: <Наименование из поля Контрагент>.
      using (TenantInfo.Culture.SwitchTo())
      {
        return string.Format(
          "{0}: {1}, контрагента: {2}",
          _obj.DocumentKind == null ? "<Вид документа>" :             _obj.DocumentKind.Name,
          _obj.Subject == null ?      "<Наименование>" :              _obj.Subject,
          _obj.Counterparty == null ? "<Наименование контрагента>" :  _obj.Counterparty.Name
         );
      }
    }
  }
}