using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractualDocument;

namespace lenspec.Etalon.Shared
{
  partial class ContractualDocumentFunctions
  {
    /// <summary>
    /// Получить строку создания версии "Из шаблона" или "Импорт" из истории документа.
    /// </summary>
    public Sungero.Content.IDocumentHistory GetImportOrFromTemplateHistoryRow()
    {
      return _obj.History.GetAll()
        .Where(x => x.OperationDetailed.HasValue &&
               (x.OperationDetailed.Value == Sungero.Content.DocumentHistory.OperationDetailed.FromTemplate ||
                x.OperationDetailed.Value == Sungero.Content.DocumentHistory.OperationDetailed.Import ||
                x.Operation.Value == Sungero.Content.DocumentHistory.Operation.Import) &&
               x.VersionNumber == _obj.LastVersion.Number)
        .OrderByDescending(x => x.Id)
        .FirstOrDefault();
    }
    
    /// <summary>
    /// Проверить коллекцию Объекты строительства на пустоту.
    /// </summary>
    /// <returns>Признак возможности сохранения.</returns>
    public bool CheckConstructionObjects()
    {
      if (_obj.IsICPlenspec == false)
        return true;
      
      if (Users.Current.IsSystem == true || Users.Current.IncludedIn(EtalonDatabooks.PublicConstants.Module.RightsToSaveWithoutConstructionObjects))
        return true;
      
      if (!_obj.ConstructionObjectsavis.Any())
        return false;
      
      return true;
    }
    
    /// <summary>
    /// Изменить параметр Типовой в зависимости от типа действия.
    /// </summary>
    public void IsStandard(Sungero.Domain.Shared.BaseEventArgs e)
    {
      if (!e.Params.Contains(Etalon.Constants.Contracts.ContractualDocument.Params.CreateFromAction))
        return;
      
      // Параметр удаляется за пределами функции
      var isCreatedFromTemplate = false;
      e.Params.TryGetValue(Sungero.Docflow.Constants.Module.CreateFromTemplate, out isCreatedFromTemplate);
      
      if (isCreatedFromTemplate)
      {
        _obj.IsStandard = true;
      }
      else if (_obj.InternalApprovalState != InternalApprovalState.Signed)
      {
        _obj.IsStandard = false;
        _obj.TemplateIDlenspec = null;
      }
    }
    
    /// <summary>
    /// Проверить, что сумма НДС совпадает с авторасчетом.
    /// </summary>
    /// <param name="vatAmount">Сумма НДС.</param>
    /// <returns>True - если значение суммы НДС совпадает с авторасчетом, иначе false.</returns>
    [Public]
    public override bool CheckVatAmount(double? vatAmount)
    {
      return true;
    }
    
    /// <summary>
    /// Доступность полей в зависимости от выбранного Контрагента.
    /// </summary>
    [Public]
    public void CounterpartyIsRequired()
    {
      /*
      // Если поле пустое.
      if (_obj.Counterparty == null)
      {
        _obj.State.Properties.EmployeeSignatoryavis.IsEnabled = false;
        _obj.State.Properties.ContactEmployeeavis.IsEnabled = false;
        _obj.State.Properties.EmployeeSignatoryavis.IsVisible = true;
        _obj.State.Properties.ContactEmployeeavis.IsVisible = true;
        
        _obj.State.Properties.CounterpartySignatory.IsEnabled = false;
        _obj.State.Properties.Contact.IsEnabled = false;
        _obj.State.Properties.CounterpartySignatory.IsVisible = false;
        _obj.State.Properties.Contact.IsVisible = false;
        
        return;
      }
      
      var company = lenspec.Etalon.Companies.As(_obj.Counterparty);
      if (company != null && company.GroupCounterpartyavis.IdDirectum5 == 6)
      {
        _obj.State.Properties.EmployeeSignatoryavis.IsEnabled = true;
        _obj.State.Properties.ContactEmployeeavis.IsEnabled = true;
        _obj.State.Properties.EmployeeSignatoryavis.IsVisible = true;
        _obj.State.Properties.ContactEmployeeavis.IsVisible = true;
        
        _obj.State.Properties.CounterpartySignatory.IsEnabled = false;
        _obj.State.Properties.Contact.IsEnabled = false;
        _obj.State.Properties.CounterpartySignatory.IsVisible = false;
        _obj.State.Properties.Contact.IsVisible = false;
        
        return;
      }
      
      _obj.State.Properties.EmployeeSignatoryavis.IsEnabled = false;
      _obj.State.Properties.ContactEmployeeavis.IsEnabled = false;
      _obj.State.Properties.EmployeeSignatoryavis.IsVisible = false;
      _obj.State.Properties.ContactEmployeeavis.IsVisible = false;
      
      _obj.State.Properties.CounterpartySignatory.IsEnabled = true;
      _obj.State.Properties.Contact.IsEnabled = true;
      _obj.State.Properties.CounterpartySignatory.IsVisible = true;
      _obj.State.Properties.Contact.IsVisible = true;
       */
    }
  }
}