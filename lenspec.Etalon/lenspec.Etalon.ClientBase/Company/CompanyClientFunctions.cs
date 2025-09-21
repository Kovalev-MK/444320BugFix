using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Company;

namespace lenspec.Etalon.Client
{
  // Добавлено avis
  
  partial class CompanyFunctions
  {
    
    /// <summary>
    /// В случае заполненной группы КА разрешить редактирование категории КА.
    /// </summary>
    [Public]
    public void IsChangeGroupCounterparty()
    {
      // Если поле группы контрагента заполнено, то разрешаем редактирование категории контрагента.
      _obj.State.Properties.CategoryCounterpartyavis.IsEnabled = _obj.GroupCounterpartyavis != null;
    }
    
    /// <summary>
    /// Изменение обязательных полей в зависимости от выбранной категории контрагентов.
    /// </summary>
    [Public]
    public void ChangeRequiredProp()
    {
      var isFilledCategory = _obj.CategoryCounterpartyavis != null;
      
      _obj.State.Properties.LegalAddress.IsRequired = isFilledCategory;
      _obj.State.Properties.Region.IsRequired =       isFilledCategory;
      _obj.State.Properties.City.IsRequired =         isFilledCategory;
      
      // Если не выбрана категория, сбрасываем обязательные поля.
      if (!isFilledCategory)
      {
        _obj.State.Properties.LegalName.IsRequired = false;
        _obj.State.Properties.TIN.IsRequired = false;
        _obj.State.Properties.TRRC.IsRequired = false;
      }
      
      // Установка обязательных полей в зависимости от группы.
      GroupRequiredProp();
    }
    
    /// <summary>
    /// Устанавливаем обязательность полей в зависимости от выбранной группы.
    /// </summary>
    private void GroupRequiredProp()
    {
      // Проверка по чекбоксам групп.
      if (_obj.GroupCounterpartyavis == null)
        return;
      
      // Установка обязательности полей ИНН.
      _obj.State.Properties.TIN.IsRequired = _obj.GroupCounterpartyavis.IsTINRequired == true;
      // Установка обязательности полей КПП.
      _obj.State.Properties.TRRC.IsRequired = _obj.GroupCounterpartyavis.IsTRRCRequired == true;
      // Установка обязательности полей Юридическое наименование.
      _obj.State.Properties.LegalName.IsRequired = _obj.GroupCounterpartyavis.IsLegalNameRequired == true;
      // Скрываем поле нерезидент.
      _obj.State.Properties.Nonresident.IsVisible = _obj.GroupCounterpartyavis.IsNonresidentRequired == true;
    }
  }
  
  // Конец добавлено avis
}