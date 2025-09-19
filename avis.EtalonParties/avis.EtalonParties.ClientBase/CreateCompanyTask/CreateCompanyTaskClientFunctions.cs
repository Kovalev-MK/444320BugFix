using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.CreateCompanyTask;

namespace avis.EtalonParties.Client
{
  partial class CreateCompanyTaskFunctions
  {
    // Добавлено Avis Expert.
    
    /// <summary>
    /// Проверить возможность отклонения задания.
    /// </summary>
    /// <param name="assignment">Задание.</param>
    /// <param name="errorMessage">Сообщение об ошибке.</param>
    /// <param name="eventArgs">Аргумент обработчика вызова.</param>
    /// <returns>True - разрешить отклонение, иначе false.</returns>
    public static bool ValidateBeforeReject(Sungero.Workflow.IAssignment assignment, string errorMessage, Sungero.Domain.Client.ExecuteActionArgs eventArgs)
    {
      if (string.IsNullOrWhiteSpace(assignment.ActiveText))
      {
        eventArgs.AddError(errorMessage);
        return false;
      }
      
      if (!eventArgs.Validate())
        return false;
      
      return true;
    }
    
    /// <summary>
    /// Заполняет название задачи, в зависимости от заполненности полей.
    /// </summary>
    [Public]
    public void NameGenerator()
    {
      _obj.Subject = "<Тема будет сформирована автоматически>";
      
      if (_obj.TypeRequest == null)
        return;
      
      if (_obj.TypeObject == CreateCompanyTask.TypeObject.Counterparty && _obj.TypeRequest == CreateCompanyTask.TypeRequest.NewEntry)
        _obj.Subject = $"{_obj.Info.Properties.TypeRequest.GetLocalizedValue(_obj.TypeRequest)} {_obj.CompanyName}, {_obj.TIN}.";
      
      if (_obj.TypeObject == CreateCompanyTask.TypeObject.Counterparty && _obj.TypeRequest == CreateCompanyTask.TypeRequest.EditEntry)
        _obj.Subject = $"{_obj.Info.Properties.TypeRequest.GetLocalizedValue(_obj.TypeRequest)} {_obj?.Company?.Name}, {_obj.TIN}.";
      
      if (_obj.TypeObject == CreateCompanyTask.TypeObject.Person && _obj.TypeRequest == CreateCompanyTask.TypeRequest.EditEntry)
        _obj.Subject = $"{_obj.Info.Properties.TypeRequest.GetLocalizedValue(_obj.TypeRequest)} {_obj?.Person?.Name}.";
      
      if (_obj.TypeObject == CreateCompanyTask.TypeObject.Person && _obj.TypeRequest == CreateCompanyTask.TypeRequest.NewEntry)
        _obj.Subject = $"{_obj.Info.Properties.TypeRequest.GetLocalizedValue(_obj.TypeRequest)} {_obj.FIOPerson}.";
      
      if (_obj.TypeRequest == CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
      {
        if (_obj.DatabookActionType == CreateCompanyTask.DatabookActionType.NewEntry)
        {
          if (_obj.TypeObject == CreateCompanyTask.TypeObject.Counterparty)
            _obj.Subject = $"Необходимо внести изменения в справочник «Ответственные по контрагентам» для контрагента {_obj?.Company?.Name}.";
          if (_obj.TypeObject == CreateCompanyTask.TypeObject.Person)
            _obj.Subject = $"Необходимо внести изменения в справочник «Ответственные по контрагентам» для контрагента {_obj?.Person?.Name}.";
        }
        if (_obj.DatabookActionType == CreateCompanyTask.DatabookActionType.EditEntry)
          _obj.Subject = $"Необходимо внести изменения в справочник «Ответственные по контрагентам» для контрагента {_obj?.ResponsibleByCounterparty?.Counterparty?.Name}.";
      }
      
      if (_obj.Subject.Length > _obj.Info.Properties.Subject.Length)
      {
        _obj.Subject = _obj.Subject.Substring(0, _obj.Info.Properties.Subject.Length);
      }
    }
    
    /// <summary>
    /// Устанавливаем обязательность полей для создания нового контрагента.
    /// </summary>
    [Public]
    public void IsRequiredProperties()
    {
      // Проверяем что это заявка на создание нового контрагента.
      if (_obj.TypeObject != CreateCompanyTask.TypeObject.Counterparty)
        return;
      
      if (_obj.TypeRequest != CreateCompanyTask.TypeRequest.NewEntry)
        return;
    }
    
    /// <summary>
    /// Отображает нужный набор свойств на карточке.
    /// </summary>
    [Public]
    public void ShowProperties()
    {
      _obj.State.Properties.CompanyName.IsRequired = false;
      _obj.State.Properties.Company.IsRequired = false;
      _obj.State.Properties.TIN.IsRequired = false;
      _obj.State.Properties.SpecifyNeedToBeMade.IsRequired = false;
      _obj.State.Properties.Person.IsRequired = false;
      _obj.State.Properties.FIOPerson.IsRequired = false;
      _obj.State.Properties.DateOfBirth.IsRequired = false;
      _obj.State.Properties.DatabookActionType.IsRequired = false;
      _obj.State.Properties.ResponsibleEmployee.IsRequired = false;
      _obj.State.Properties.BusinessUnit.IsRequired = false;
      _obj.State.Properties.ResponsibleByCounterparty.IsRequired = false;
      _obj.State.Properties.Comment.IsRequired = false;
      
      _obj.State.Properties.CompanyName.IsVisible = false;
      _obj.State.Properties.Company.IsVisible = false;
      _obj.State.Properties.TIN.IsVisible = false;
      _obj.State.Properties.SpecifyNeedToBeMade.IsVisible = false;
      _obj.State.Properties.Person.IsVisible = false;
      _obj.State.Properties.FIOPerson.IsVisible = false;
      _obj.State.Properties.DateOfBirth.IsVisible = false;
      _obj.State.Properties.DatabookActionType.IsVisible = false;
      _obj.State.Properties.ResponsibleEmployee.IsVisible = false;
      _obj.State.Properties.BusinessUnit.IsVisible = false;
      _obj.State.Properties.ResponsibleByCounterparty.IsVisible = false;
      _obj.State.Properties.Comment.IsVisible = false;
      
      if (_obj.TypeObject == CreateCompanyTask.TypeObject.Counterparty && _obj.TypeRequest == CreateCompanyTask.TypeRequest.NewEntry)
      {
        _obj.State.Properties.CompanyName.IsVisible = true;
        _obj.State.Properties.TIN.IsVisible = true;
        _obj.State.Properties.Comment.IsVisible = true;
        
        _obj.State.Properties.CompanyName.IsRequired = true;
      }
      
      if (_obj.TypeObject == CreateCompanyTask.TypeObject.Counterparty && _obj.TypeRequest == CreateCompanyTask.TypeRequest.EditEntry)
      {
        _obj.State.Properties.Company.IsVisible = true;
        _obj.State.Properties.TIN.IsVisible = true;
        _obj.State.Properties.SpecifyNeedToBeMade.IsVisible = true;
        
        _obj.State.Properties.Company.IsRequired = true;
        _obj.State.Properties.SpecifyNeedToBeMade.IsRequired = true;
      }
      
      if (_obj.TypeObject == CreateCompanyTask.TypeObject.Person && _obj.TypeRequest == CreateCompanyTask.TypeRequest.NewEntry)
      {
        _obj.State.Properties.FIOPerson.IsVisible = true;
        _obj.State.Properties.DateOfBirth.IsVisible = true;
        _obj.State.Properties.Comment.IsVisible = true;
        
        _obj.State.Properties.FIOPerson.IsRequired = true;
      }
      
      if (_obj.TypeObject == CreateCompanyTask.TypeObject.Person && _obj.TypeRequest == CreateCompanyTask.TypeRequest.EditEntry)
      {
        _obj.State.Properties.Person.IsVisible = true;
        _obj.State.Properties.SpecifyNeedToBeMade.IsVisible = true;
        
        _obj.State.Properties.Person.IsRequired = true;
        _obj.State.Properties.SpecifyNeedToBeMade.IsRequired = true;
      }
      
      // Создание или изменение ответственного по контрагенту.
      if (_obj.TypeRequest == CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
      {
        _obj.State.Properties.DatabookActionType.IsVisible = true;
        _obj.State.Properties.DatabookActionType.IsRequired = true;
        
        if (_obj.DatabookActionType == CreateCompanyTask.DatabookActionType.NewEntry)
        {
          if (_obj.TypeObject == CreateCompanyTask.TypeObject.Counterparty)
          {
            _obj.State.Properties.Company.IsVisible = true;
            _obj.State.Properties.Company.IsRequired = true;
          }
          if (_obj.TypeObject == CreateCompanyTask.TypeObject.Person)
          {
            _obj.State.Properties.Person.IsVisible = true;
            _obj.State.Properties.Person.IsRequired = true;
          }
          _obj.State.Properties.ResponsibleEmployee.IsVisible = true;
          _obj.State.Properties.BusinessUnit.IsVisible = true;
          _obj.State.Properties.Comment.IsVisible = true;
          
          _obj.State.Properties.ResponsibleEmployee.IsRequired = true;
          _obj.State.Properties.BusinessUnit.IsRequired = true;
        }
        if (_obj.DatabookActionType == CreateCompanyTask.DatabookActionType.EditEntry)
        {
          _obj.State.Properties.ResponsibleByCounterparty.IsVisible = true;
          _obj.State.Properties.SpecifyNeedToBeMade.IsVisible = true;
          
          _obj.State.Properties.ResponsibleByCounterparty.IsRequired = true;
          _obj.State.Properties.SpecifyNeedToBeMade.IsRequired = true;
        }
      }
      
      ClearProperties();
    }
    
    /// <summary>
    /// Очищаем отключенные поля.
    /// </summary>
    private void ClearProperties()
    {
      if (_obj.State.Properties.CompanyName.IsVisible == false)
        _obj.CompanyName = "";
      if (_obj.State.Properties.Company.IsVisible == false)
        _obj.Company = null;
      if (_obj.State.Properties.TIN.IsVisible == false)
        _obj.TIN = "";
      if (_obj.State.Properties.SpecifyNeedToBeMade.IsVisible == false)
        _obj.SpecifyNeedToBeMade = "";
      if (_obj.State.Properties.Person.IsVisible == false)
        _obj.Person = null;
      if (_obj.State.Properties.FIOPerson.IsVisible == false)
        _obj.FIOPerson = "";
      if (_obj.State.Properties.DateOfBirth.IsVisible == false)
        _obj.DateOfBirth = null;
      if (_obj.State.Properties.DatabookActionType.IsVisible == false)
        _obj.DatabookActionType = null;
      if (_obj.State.Properties.ResponsibleEmployee.IsVisible == false)
        _obj.ResponsibleEmployee = null;
      if (_obj.State.Properties.BusinessUnit.IsVisible == false)
        _obj.BusinessUnit = null;
      if (_obj.State.Properties.ResponsibleByCounterparty.IsVisible == false)
        _obj.ResponsibleByCounterparty = null;
      if (_obj.State.Properties.Comment.IsVisible == false)
        _obj.Comment = null;
    }
    
    // Конец Добавлено Avis Expert.
  }
}