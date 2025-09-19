using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractCondition;

namespace lenspec.Etalon.Server
{
  partial class ContractConditionFunctions
  {
    /// <summary>
    /// Заполнение отображаемого имени условия в схеме.
    /// </summary>
    /// <returns>Наименование условия.</returns>
    public override string GetConditionName()
    {
      using (TenantInfo.Culture.SwitchTo())
      {
        // "Вид договора – {0}?"
        if (_obj.ConditionType == ConditionType.ContractCategorylenspec)
        {
          var categories = string.Join("/", _obj.ContractCategorieslenspec.Select(c => c.ContractCategory?.Name));
          return ContractConditions.Resources.ContractCategoryNameFormat(categories);
        }
        
        // "Тип договора – {0}?"
        if (_obj.ConditionType == ConditionType.ContractType)
        {
          var contractType = _obj.ContractTypelenspec?.Name;
          return ContractConditions.Resources.ContractTypeNameFormat(contractType);
        }
        
        // "Группа категорий – {0}?"
        if (_obj.ConditionType == ConditionType.GroupKind)
        {
          var groupKind = _obj.GroupKindlenspec?.Name;
          return ContractConditions.Resources.GroupKindNameFormat(groupKind);
        }
        
        // "Тип платежа - {0}?"
        if (_obj.ConditionType == ConditionType.PaymentType)
        {
          return lenspec.Etalon.ContractConditions.Resources.PaymentTypeNameFormat(ContractConditions.Info.Properties.PaymentTypelenspec.GetLocalizedValue(_obj.PaymentTypelenspec));
        }
        
        // "Контрагент – Наша организация?"
        if (_obj.ConditionType == ConditionType.CounterpartyBU)
          return lenspec.Etalon.ContractConditions.Resources.CounterpartyBUName;
        
        // "Тип контрагента – {0}?"
        if (_obj.ConditionType == ConditionType.CounterpartyTypelenspec)
        {
          var counterpartyType = _obj.Info.Properties.CounterpartyTypelenspec.GetLocalizedValue(_obj.CounterpartyTypelenspec);
          return ContractConditions.Resources.CounterpartyTypeNameFormat(counterpartyType);
        }
        
        // "Профиль компании, если Контрагент – Наша организация?"
        if (_obj.ConditionType == ConditionType.CompanyProfile)
        {
          return ContractConditions.Resources.CompanyProfileName;
        }
      }
      return base.GetConditionName();
    }
  }
}