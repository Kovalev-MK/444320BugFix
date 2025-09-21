using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.CreateCompanyTask;

namespace avis.EtalonParties
{
  // Добавлено avis.
  
  partial class CreateCompanyTaskSharedHandlers
  {

    public virtual void ResponsibleByCounterpartyChanged(avis.EtalonParties.Shared.CreateCompanyTaskResponsibleByCounterpartyChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue)
        return;
      
      if (_obj.AttachmentGroup.All.Contains(e.OldValue))
      {
        _obj.AttachmentGroup.All.Remove(e.OldValue);
      }
      
      if (e.NewValue != null && !_obj.AttachmentGroup.All.Contains(e.NewValue))
      {
        _obj.AttachmentGroup.All.Add(e.NewValue);
      }
      
      // Генерируем тему задачи.
      PublicFunctions.CreateCompanyTask.NameGenerator(_obj);
    }

    public virtual void DatabookActionTypeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        PublicFunctions.CreateCompanyTask.ShowProperties(_obj);
    }
    
    /// <summary>
    /// Изменение значения свойства "Контрагент".
    /// </summary>
    /// <param name="e"></param>
    public virtual void CompanyChanged(avis.EtalonParties.Shared.CreateCompanyTaskCompanyChangedEventArgs e)
    {
      if (_obj.AttachmentGroup.All.Contains(e.OldValue))
      {
        _obj.AttachmentGroup.All.Remove(e.OldValue);
      }
      
      if (_obj.TypeRequest == CreateCompanyTask.TypeRequest.ResponsibleByCounterparty &&
          _obj.DatabookActionType == CreateCompanyTask.DatabookActionType.NewEntry &&
          e.NewValue != null && !_obj.AttachmentGroup.All.Contains(e.NewValue))
      {
        _obj.AttachmentGroup.All.Add(e.NewValue);
      }
      
      // Генерируем тему задачи.
      PublicFunctions.CreateCompanyTask.NameGenerator(_obj);
      
      if (e.NewValue != e.OldValue && e.NewValue != null)
        _obj.TIN = e.NewValue.TIN;
    }
    
    /// <summary>
    /// Изменение значения свойства "ФИО персоны".
    /// </summary>
    /// <param name="e"></param>
    public virtual void FIOPersonChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      // Генерируем тему задачи.
      PublicFunctions.CreateCompanyTask.NameGenerator(_obj);
    }
    
    /// <summary>
    /// Изменение значения свойства "ИНН".
    /// </summary>
    /// <param name="e"></param>
    public virtual void TINChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      // Генерируем тему задачи.
      PublicFunctions.CreateCompanyTask.NameGenerator(_obj);
      
      // Установка обязательности полей, для создания нового контрагента.
      PublicFunctions.CreateCompanyTask.IsRequiredProperties(_obj);
    }
    /// <summary>
    /// Изменение значения свойства "Наименование контрагента".
    /// </summary>
    /// <param name="e"></param>
    public virtual void CompanyNameChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      // Генерируем тему задачи.
      PublicFunctions.CreateCompanyTask.NameGenerator(_obj);
      
      // Установка обязательности полей, для создания нового контрагента.
      PublicFunctions.CreateCompanyTask.IsRequiredProperties(_obj);
    }
    
    /// <summary>
    /// Изменение значения свойства "Персона".
    /// </summary>
    /// <param name="e"></param>
    public virtual void PersonChanged(avis.EtalonParties.Shared.CreateCompanyTaskPersonChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue)
        return;
      
      // Задаём значение ИНН из персоны.
      if (e.NewValue != null && _obj.TypeRequest == CreateCompanyTask.TypeRequest.EditEntry)
        _obj.TIN = e.NewValue.TIN;
      
      // Генерируем тему задачи.
      PublicFunctions.CreateCompanyTask.NameGenerator(_obj);
    }
    
    /// <summary>
    /// Изменение значения свойства "Тип запроса".
    /// </summary>
    /// <param name="e"></param>
    public virtual void TypeRequestChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        PublicFunctions.CreateCompanyTask.ShowProperties(_obj);
      
      if (e.NewValue == CreateCompanyTask.TypeRequest.ResponsibleByCounterparty && _obj.TypeObject == CreateCompanyTask.TypeObject.Person)
      {
        _obj.TypeObject = null;
      }
      
      // Генерируем тему задачи.
      PublicFunctions.CreateCompanyTask.NameGenerator(_obj);
      
      // Установка обязательности полей, для создания нового контрагента.
      PublicFunctions.CreateCompanyTask.IsRequiredProperties(_obj);
    }
    
    /// <summary>
    /// Изменение значения свойства "Объект".
    /// </summary>
    /// <param name="e"></param>
    public virtual void TypeObjectChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      // Отображаем свойства в зависимости от выбранных значений объекта и типа заявки.
      if (e.NewValue != e.OldValue)
        PublicFunctions.CreateCompanyTask.ShowProperties(_obj);
      
      // Генерируем тему задачи.
      PublicFunctions.CreateCompanyTask.NameGenerator(_obj);
      
      // Установка обязательности полей, для создания нового контрагента.
      PublicFunctions.CreateCompanyTask.IsRequiredProperties(_obj);
    }
  }
  
  // Конец добавлено avis
}