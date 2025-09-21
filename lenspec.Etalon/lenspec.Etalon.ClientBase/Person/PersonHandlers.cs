using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Person;

namespace lenspec.Etalon
{
  // Добавлено avis.
  
  partial class PersonClientHandlers
  {
    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.Person.SetEnabledAndRequiredProperties(_obj);
      base.Refresh(e);
      
      #region Права по ТЗ
      //  Если стоит галка Сотрудник ГК, Клиенты (дольщики, покупатели) или Клиенты (собственники)
      var isRequiredGKorClient = (_obj.IsEmployeeGKavis == true || _obj.IsClientBuyersavis == true || _obj.IsClientOwnersavis == true);
      _obj.State.Properties.DateOfBirth.IsRequired = isRequiredGKorClient;
      _obj.State.Properties.LegalAddress.IsRequired = isRequiredGKorClient;
      _obj.State.Properties.PostalAddress.IsRequired = isRequiredGKorClient;
      //_obj.State.Properties.Phones.IsRequired = isRequiredGKorClient;
      _obj.State.Properties.Email.IsRequired = isRequiredGKorClient;
      if(_obj.IsEmployeeGKavis == true)
      {
   //     _obj.State.Properties.Phones.IsRequired = false;
        _obj.State.Properties.Email.IsRequired = false;
      }
      _obj.State.Properties.PostalAddress.IsRequired = isRequiredGKorClient;
      #endregion
      
      #region Устаревшее
      /*
      // Блокируем все поля.
      Etalon.PublicFunctions.Person.BlockAllProperties(_obj);
      // Разрешаем редактирование полей для роли "Права на создание всех контрагентов".
      var createCounterpartyRoleGuid = avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid;
      var createCounterpartyRole = Roles.GetAll(r => Equals(r.Sid, createCounterpartyRoleGuid)).FirstOrDefault();
      Etalon.PublicFunctions.Person.IsCounterpartyRole(_obj, createCounterpartyRole);
      
      // Открываем доступ к редактированию поля емейл, для делопроизводителей.
      var clerks = Sungero.Docflow.PublicFunctions.DocumentRegister.Remote.GetClerks();
      if (Employees.Current.IncludedIn(clerks))
        _obj.State.Properties.Email.IsEnabled = true;
      
      // Открываем доступ к редактированию поля дата рождения для "Полные права на управление архивом для управляющих компаний ЖКХ" и "Права на создание всех видов контрагентов."
      var fullPermitArhivJKHGuid = lenspec.Etalon.Module.Parties.PublicConstants.Module.FullPermitArhivJKH;
      var fullPermitArhivJKH = Roles.GetAll(r => Equals(r.Sid, fullPermitArhivJKHGuid)).FirstOrDefault();
      
      if (Employees.Current.IncludedIn(createCounterpartyRole))
      {
        if (_obj.IsEmployeeGKavis.HasValue && _obj.IsClientBuyersavis.HasValue && _obj.IsClientOwnersavis.HasValue &&
            _obj.IsEmployeeGKavis.Value == false && _obj.IsClientBuyersavis.Value == false && _obj.IsClientOwnersavis.Value == false)
        {
          _obj.State.Properties.DateOfBirth.IsEnabled = true;
        }
      }
      
      base.Refresh(e);
      _obj.State.Properties.ExternalCodeavis.IsEnabled = false;
      _obj.State.Properties.CodeInvestavis.IsEnabled = false;
      
      if (Employees.Current.IncludedIn(fullPermitArhivJKH))
      {
        if (_obj.IsClientOwnersavis != true)
        {
          _obj.State.Properties.DateOfBirth.IsEnabled = false;
        }
        else
        {
          if ((_obj.IsEmployeeGKavis.HasValue && _obj.IsEmployeeGKavis == true ||
               _obj.IsClientBuyersavis.HasValue && _obj.IsClientBuyersavis.Value == true) &&
              _obj.IsClientOwnersavis.HasValue && _obj.IsClientOwnersavis == true)
          {
            _obj.State.Properties.DateOfBirth.IsEnabled = false;
          }
          if (_obj.IsEmployeeGKavis != true && _obj.IsClientBuyersavis != true &&
              _obj.IsClientOwnersavis.HasValue && _obj.IsClientOwnersavis == true)
          {
            _obj.State.Properties.DateOfBirth.IsEnabled = true;
          }
        }
      }
      
      // Перс. данные видны и доступны только входящим в соответствующую роль.
      var viewPersonalDataRole = Roles.GetAll(r => Equals(r.Sid, lenspec.EtalonDatabooks.PublicConstants.Module.ViewPersonalData)).FirstOrDefault();
      var allowPersonalData = Employees.Current.IncludedIn(Roles.Administrators) || Employees.Current.IncludedIn(viewPersonalDataRole);
      
      _obj.State.Properties.LegalAddress.IsVisible = _obj.State.Properties.LegalAddress.IsEnabled = allowPersonalData;
      _obj.State.Properties.PostalAddress.IsVisible = _obj.State.Properties.PostalAddress.IsEnabled = allowPersonalData;
      _obj.State.Properties.Phones.IsVisible = _obj.State.Properties.Phones.IsEnabled = allowPersonalData;
      _obj.State.Properties.Email.IsVisible = _obj.State.Properties.Email.IsEnabled = allowPersonalData;
      _obj.State.Properties.IsClientOwnersavis.IsVisible = _obj.State.Properties.IsClientOwnersavis.IsEnabled = allowPersonalData;
      
      #region ТЗ Доработки карточек документов для управляющих компаний (ЖКХ)
      if (Employees.Current.IncludedIn(fullPermitArhivJKH) && Employees.Current.IncludedIn(viewPersonalDataRole))
      {
        _obj.State.Properties.IsClientBuyersavis.IsEnabled = false;
      }
      else
      {
        _obj.State.Properties.IsClientBuyersavis.IsEnabled = allowPersonalData;
      }
      _obj.State.Properties.IsClientBuyersavis.IsVisible = allowPersonalData;
      #endregion
      
      _obj.State.Properties.INILA.IsVisible = _obj.State.Properties.INILA.IsEnabled = allowPersonalData;
      _obj.State.Properties.Citizenship.IsVisible = _obj.State.Properties.Citizenship.IsEnabled = allowPersonalData;
      _obj.State.Properties.TIN.IsVisible = _obj.State.Properties.TIN.IsEnabled = allowPersonalData;
      _obj.State.Properties.PersonalAccountavis.IsVisible = _obj.State.Properties.PersonalAccountavis.IsEnabled = allowPersonalData;
      _obj.State.Pages.Address.IsVisible = allowPersonalData && _obj.IsEmployeeGKavis == true;
       */
      #endregion
    }
  }
  // Конец добавлено avis.
}