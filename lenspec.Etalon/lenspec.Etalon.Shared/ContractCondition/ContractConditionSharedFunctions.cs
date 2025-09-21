using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractCondition;

namespace lenspec.Etalon.Shared
{
  partial class ContractConditionFunctions
  {
    /// <summary>
    /// Проверить условие "Рамочный".
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="task">Задача на согласование.</param>
    /// <returns>True, если рамочный договор.</returns>
    public override Sungero.Docflow.Structures.ConditionBase.ConditionResult CheckIsFrameworkContract(Sungero.Docflow.IOfficialDocument document, Sungero.Docflow.IApprovalTask task)
    {
      if (lenspec.Etalon.Contracts.Is(document))
      {
        var contract = lenspec.Etalon.Contracts.As(document);
        if (contract.IsFrameworkavis.HasValue)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(contract.IsFrameworkavis.Value, string.Empty);
        
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, ContractConditions.Resources.IsFrameworkContractPropertyIsNotFilled);
      }
      
      return base.CheckIsFrameworkContract(document, task);
    }
    
    /// <summary>
    /// Сменить доступность реквизитов.
    /// </summary>
    public override void ChangePropertiesAccess()
    {
      base.ChangePropertiesAccess();
      
      // Тип договора.
      var matchesCondition = _obj.ConditionType == lenspec.Etalon.ContractCondition.ConditionType.ContractType;
      _obj.State.Properties.ContractTypelenspec.IsVisible = matchesCondition;
      _obj.State.Properties.ContractTypelenspec.IsRequired = matchesCondition;
      
      // Группа вида.
      matchesCondition = _obj.ConditionType == lenspec.Etalon.ContractCondition.ConditionType.GroupKind;
      _obj.State.Properties.GroupKindlenspec.IsVisible = matchesCondition;
      _obj.State.Properties.GroupKindlenspec.IsRequired = matchesCondition;
      
      // Категория договора.
      matchesCondition = _obj.ConditionType == lenspec.Etalon.ContractCondition.ConditionType.ContractCategorylenspec;
      _obj.State.Properties.ContractCategorieslenspec.IsVisible = matchesCondition;
      _obj.State.Properties.ContractCategorieslenspec.IsRequired = matchesCondition;
      
      // Тип платежа.
      matchesCondition = _obj.ConditionType == lenspec.Etalon.ContractCondition.ConditionType.PaymentType;
      _obj.State.Properties.PaymentTypelenspec.IsVisible = matchesCondition;
      _obj.State.Properties.PaymentTypelenspec.IsRequired = matchesCondition;
      
      // Тип контрагента.
      matchesCondition = _obj.ConditionType == lenspec.Etalon.ContractCondition.ConditionType.CounterpartyTypelenspec;
      _obj.State.Properties.CounterpartyTypelenspec.IsVisible = matchesCondition;
      _obj.State.Properties.CounterpartyTypelenspec.IsRequired = matchesCondition;
      
      // Профиль НОР.
      matchesCondition = _obj.ConditionType == lenspec.Etalon.ContractCondition.ConditionType.CompanyProfile;
      _obj.State.Properties.BusinessUnitProfilelenspec.IsVisible = matchesCondition;
      _obj.State.Properties.BusinessUnitProfilelenspec.IsRequired = matchesCondition;
      
      // Профиль Контрагента - НОР.
      matchesCondition = _obj.ConditionType == lenspec.Etalon.ContractCondition.ConditionType.CompanyProfile;
      _obj.State.Properties.CounterpartyProfilelenspec.IsVisible = matchesCondition;
      _obj.State.Properties.CounterpartyProfilelenspec.IsRequired = matchesCondition;
    }
    
    /// <summary>
    /// Получить словарь поддерживаемых типов условий.
    /// </summary>
    /// <returns>
    /// Словарь.
    /// Ключ - GUID типа документа.
    /// Значение - список поддерживаемых условий.
    /// </returns>
    public override System.Collections.Generic.Dictionary<string, List<Enumeration?>> GetSupportedConditions()
    {
      var baseKindsSupport = base.GetSupportedConditions();
      
      // Привязка условий согласования к типам/видам документов.
      
      // Договоры.
      baseKindsSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.ContractType);
      baseKindsSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.GroupKind);
      baseKindsSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.ContractCategorylenspec);
      baseKindsSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.CounterpartyBU);
      baseKindsSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.CounterpartyTypelenspec);
      baseKindsSupport["f37c7e63-b134-4446-9b5b-f8811f6c9666"].Add(ConditionType.CompanyProfile);
      
      // Доп. соглашения.
      baseKindsSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.ContractType);
      baseKindsSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.GroupKind);
      baseKindsSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.ContractCategorylenspec);
      baseKindsSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.CounterpartyBU);
      baseKindsSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.CounterpartyTypelenspec);
      baseKindsSupport["265f2c57-6a8a-4a15-833b-ca00e8047fa5"].Add(ConditionType.CompanyProfile);
      
      // ЗНО.
      baseKindsSupport[lenspec.ApplicationsForPayment.PublicConstants.Module.ApplicationForPaymentType.ToString()].Add(ConditionType.GroupKind);
      baseKindsSupport[lenspec.ApplicationsForPayment.PublicConstants.Module.ApplicationForPaymentType.ToString()].Add(ConditionType.PaymentType);
      
      return baseKindsSupport;
    }
    
    public override Sungero.Docflow.Structures.ConditionBase.ConditionResult CheckCondition(Sungero.Docflow.IOfficialDocument document, Sungero.Docflow.IApprovalTask task)
    {
      var contractualDocument = lenspec.Etalon.ContractualDocuments.As(document);
      var emptyDocumentResult = Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, lenspec.Etalon.ContractConditions.Resources.EmptyDocumentResultMessage);
      var contract = lenspec.Etalon.ContractualDocuments.As(document);
      var applicationForPayment = lenspec.ApplicationsForPayment.ApplicationForPayments.As(document);
      
      if (_obj.ConditionType == ConditionType.ContractType)
      {
        if (contract != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(_obj.ContractTypelenspec.Equals(contract.ContractTypeavis), string.Empty);
        else
          return emptyDocumentResult;
      }
      
      if (_obj.ConditionType == ConditionType.GroupKind)
      {
        if (applicationForPayment != null)
        {
          if (applicationForPayment.Contract != null)
            contract = lenspec.Etalon.ContractualDocuments.As(applicationForPayment.Contract);
          else
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, lenspec.Etalon.ContractConditions.Resources.NeedFillContractInApplicationForPayment);
        }
        
        if (contract != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(_obj.GroupKindlenspec.Equals(contract.GroupContractTypeavis), string.Empty);
        else
          return emptyDocumentResult;
      }
      
      if (_obj.ConditionType == ConditionType.ContractCategorylenspec)
        return contractualDocument != null ?
          Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(_obj.ContractCategorieslenspec.Any(c => Equals(c.ContractCategory, contractualDocument.ContractCategoryavis)), string.Empty) :
          emptyDocumentResult;
      
      if (_obj.ConditionType == ConditionType.PaymentType)
      {
        if (applicationForPayment != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(_obj.PaymentTypelenspec == applicationForPayment.PaymentType, string.Empty);
        else
          return emptyDocumentResult;
      }
      
      // Контрагент – Наша организация.
      if (_obj.ConditionType == ConditionType.CounterpartyBU)
        return contractualDocument != null ?
          Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(Sungero.Company.BusinessUnits.GetAll().Any(u => Equals(u.Company, contractualDocument.Counterparty)), string.Empty) :
          emptyDocumentResult;
      
      // Тип контрагента.
      if (_obj.ConditionType == ConditionType.CounterpartyTypelenspec)
      {
        if (contractualDocument != null && contractualDocument.Counterparty != null)
        {
          var result = _obj.CounterpartyTypelenspec == lenspec.Etalon.ContractCondition.CounterpartyTypelenspec.Company && Sungero.Parties.Companies.Is(contractualDocument.Counterparty) ||
            _obj.CounterpartyTypelenspec == lenspec.Etalon.ContractCondition.CounterpartyTypelenspec.Bank && Sungero.Parties.Banks.Is(contractualDocument.Counterparty) ||
            _obj.CounterpartyTypelenspec == lenspec.Etalon.ContractCondition.CounterpartyTypelenspec.Person && Sungero.Parties.People.Is(contractualDocument.Counterparty);
          
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(result, string.Empty);
        }
        else
          return emptyDocumentResult;
      }

      // Профиль НОР.
      if (_obj.ConditionType == ConditionType.CompanyProfile)
      {
        // Проверка наличия договорного документа в задаче по регламенту и заполненность в нём поля Контрагент.
        if (contractualDocument != null && contractualDocument.Counterparty != null)
        {
          // Если в поле Контрагент не компания или не зеркало-НОР, то регламент идёт по ветке "нет".
          var businessunitsByCounterparty = lenspec.Etalon.BusinessUnits.GetAll(b => b.Company.Equals(contractualDocument.Counterparty));
          if (!Sungero.Parties.Companies.Is(contractualDocument.Counterparty) || !businessunitsByCounterparty.Any())
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(false, string.Empty);
          
          // Если поля Профиль НОР и Профиль Контаргента-НОР совпадают с полями, заданными в условии соответственно, то регламент идёт по ветке "да".
          var buByCounterparty = businessunitsByCounterparty.SingleOrDefault();
          var profileBU = lenspec.Etalon.BusinessUnits.As(contractualDocument.BusinessUnit).CompanyProfileavis;
          var profileCounterparty = buByCounterparty.CompanyProfileavis;
          var result = _obj.BusinessUnitProfilelenspec.Any(c => Equals(c.BUProfile, profileBU)) && _obj.CounterpartyProfilelenspec.Any(c => Equals(c.CompanyProfile, profileCounterparty));
          
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(result, string.Empty);
        }
        else
          return emptyDocumentResult;
      }
      
      return base.CheckCondition(document, task);
    }
    
    /// <summary>
    /// Очистка скрытых свойств.
    /// </summary>
    public override void ClearHiddenProperties()
    {
      base.ClearHiddenProperties();
      
      // Группа вида.
      if (!_obj.State.Properties.GroupKindlenspec.IsVisible)
        _obj.GroupKindlenspec = null;
      
      // Тип договора.
      if (!_obj.State.Properties.ContractTypelenspec.IsVisible)
        _obj.ContractTypelenspec = null;
      
      // Категория договора.
      if (!_obj.State.Properties.ContractCategorieslenspec.IsVisible)
        _obj.ContractCategorieslenspec.Clear();
      
      // Тип платежа.
      if (!_obj.State.Properties.PaymentTypelenspec.IsVisible)
        _obj.PaymentTypelenspec = null;
      
      // Тип контрагента.
      if (!_obj.State.Properties.CounterpartyTypelenspec.IsVisible)
        _obj.CounterpartyTypelenspec = null;
      
      // Профиль НОР.
      if (!_obj.State.Properties.BusinessUnitProfilelenspec.IsVisible)
        _obj.BusinessUnitProfilelenspec.Clear();
      
      // Профиль Контрагента - НОР.
      if (!_obj.State.Properties.CounterpartyProfilelenspec.IsVisible)
        _obj.CounterpartyProfilelenspec.Clear();
    }

  }
}