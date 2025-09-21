using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Person;

namespace lenspec.Etalon.Shared
{
  partial class PersonFunctions
  {
    /// <summary>
    /// Проверка свойств документа, удостоверяющего личность перед сохранением.
    /// </summary>
    /// <returns>Список ошибок.</returns>
    [Public]
    public override System.Collections.Generic.Dictionary<Sungero.Domain.Shared.IPropertyInfo, string> CheckIdentityProperties()
    {
      return base.CheckIdentityProperties();
    }
    
    //Добавлено Avis Expert
    
    public override string GetCounterpartyDuplicatesErrorText()
    {
      // Не проверять для закрытых записей.
      if (_obj.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
        return string.Empty;
      
      var isSystemUser = Users.Current.IsSystem.HasValue && Users.Current.IsSystem.Value;
      var duplicates = isSystemUser ? base.GetDuplicates(true) : this.GetDuplicates(true);
      var errorText = isSystemUser ? Sungero.Parties.Shared.CounterpartyFunctions.GenerateCounterpartyDuplicatesErrorText(duplicates, string.Empty) : GeneratePersonDuplicatesErrorText(duplicates);
      return errorText;
    }
    
    public static string GeneratePersonDuplicatesErrorText(List<Sungero.Parties.ICounterparty> duplicates)
    {
      var errorText = string.Empty;
      if (duplicates.Any())
        errorText = lenspec.Etalon.People.Resources.ErrorTextDuplicatePerson;
      return errorText;
    }
    
    public override List<Sungero.Parties.ICounterparty> GetDuplicates(bool excludeClosed)
    {
      var duplicates = new List<Sungero.Parties.ICounterparty>();
      var matchingEntries = Sungero.Parties.People.GetAll(x => Equals(x.Name.ToLower(), _obj.Name.ToLower()) && Equals(_obj.DateOfBirth, x.DateOfBirth) && x.Id != _obj.Id);
      if (excludeClosed)
        matchingEntries = matchingEntries.Where(x => x.Status == Sungero.Parties.Person.Status.Active);
      
      duplicates.AddRange(matchingEntries);
      return duplicates;
    }
    
    public void SetEnabledAndRequiredProperties()
    {
      #region Обязательность полей
      /*
      _obj.State.Properties.DateOfBirth.IsRequired = _obj.IsEmployeeGKavis == true || _obj.IsClientBuyersavis == true || _obj.IsClientOwnersavis == true;
      
      var isRequiredAddress = _obj.IsLawyeravis != true && _obj.IsOtheravis != true &&
        (_obj.IsEmployeeGKavis == true || _obj.IsClientBuyersavis == true || _obj.IsClientOwnersavis == true);
      
      _obj.State.Properties.LegalAddress.IsRequired = isRequiredAddress;
      _obj.State.Properties.PostalAddress.IsRequired = isRequiredAddress;
      _obj.State.Properties.Phones.IsRequired = isRequiredAddress;
      _obj.State.Properties.Email.IsRequired = isRequiredAddress;
       */
      if(_obj.IsEmployeeGKavis == true)
      {
   //     _obj.State.Properties.Phones.IsRequired = false;
        _obj.State.Properties.Email.IsRequired = false;
      }
      #endregion
      
      #region Доступность полей
      if (!Users.Current.IncludedIn(Roles.Administrators))
      {
        // Права на создание/изменение контрагентов, персон и контактных лиц.
        var canCreateCounterparty = Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid);
        // Полные права на Клиенты (собственники) для Канцелярий УК.
        var canChangeIsClientOwner = Users.Current.IncludedIn(lenspec.Etalon.Module.Parties.Constants.Module.IsClientOwnerAccessGuid);
        // Права на просмотр всех полей в Персонах (перс. данные).
        var canReadPersonalData = Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.ViewPersonalData);
        var isEnabledMainPersonalData = (Users.Current.IsSystem == true) ||
          (canCreateCounterparty && _obj.IsEmployeeGKavis != true && _obj.IsClientBuyersavis != true && _obj.IsClientOwnersavis != true) ||
          (canChangeIsClientOwner && _obj.IsEmployeeGKavis != true && _obj.IsClientBuyersavis != true && _obj.IsLawyeravis != true && _obj.IsOtheravis != true);
        
        // Вкладка Свойства.
        // Фамилия
        _obj.State.Properties.LastName.IsEnabled      = isEnabledMainPersonalData;
        
        // Имя
        _obj.State.Properties.FirstName.IsEnabled     = isEnabledMainPersonalData;
        
        // Отчество
        _obj.State.Properties.MiddleName.IsEnabled    = isEnabledMainPersonalData;
        
        // Дата рождения
        _obj.State.Properties.DateOfBirth.IsEnabled   = isEnabledMainPersonalData;
        
        // Пол
        _obj.State.Properties.Sex.IsEnabled           = isEnabledMainPersonalData;
        
        // Гражданство
        _obj.State.Properties.Citizenship.IsEnabled   = isEnabledMainPersonalData;
        _obj.State.Properties.Citizenship.IsVisible   = canReadPersonalData;
        
        // ИНН
        _obj.State.Properties.TIN.IsEnabled           = isEnabledMainPersonalData;
        _obj.State.Properties.TIN.IsVisible           = canReadPersonalData;
        
        // Состояние
        _obj.State.Properties.Status.IsEnabled        = isEnabledMainPersonalData;
        
        // СНИЛС
        _obj.State.Properties.INILA.IsEnabled         = isEnabledMainPersonalData;
        _obj.State.Properties.INILA.IsVisible         = canReadPersonalData;
        
        // Согласие на обработку перс. данных
        _obj.State.Properties.IsPersonalDataavis.IsEnabled = canChangeIsClientOwner && _obj.IsClientOwnersavis == true;
        _obj.State.Properties.IsPersonalDataavis.IsVisible = canReadPersonalData;
        
        // Адрес регистрации
        _obj.State.Properties.LegalAddress.IsEnabled          = isEnabledMainPersonalData;
        _obj.State.Properties.LegalAddress.IsVisible          = canReadPersonalData;
        
        // Почтовый адрес
        _obj.State.Properties.PostalAddress.IsEnabled         = canReadPersonalData;
        _obj.State.Properties.PostalAddress.IsVisible         = canReadPersonalData;
        
        // Телефоны
        _obj.State.Properties.Phones.IsEnabled                = isEnabledMainPersonalData;
        _obj.State.Properties.Phones.IsVisible                = canReadPersonalData;
        
        // Эл. почта
        _obj.State.Properties.Email.IsEnabled                 = isEnabledMainPersonalData;
        _obj.State.Properties.Email.IsVisible                 = canReadPersonalData;
        
        // Адвокаты/нотариусы/совет дома
        _obj.State.Properties.IsLawyeravis.IsEnabled          = canCreateCounterparty;
        
        // Клиенты (покупатели, дольщики)
        _obj.State.Properties.IsClientBuyersavis.IsVisible    = canChangeIsClientOwner;
        
        // Клиенты (собственники)
        _obj.State.Properties.IsClientOwnersavis.IsEnabled    = canChangeIsClientOwner;
        _obj.State.Properties.IsClientOwnersavis.IsVisible    = canChangeIsClientOwner;
        
        // Сотрудник ГК
        _obj.State.Properties.IsEmployeeGKavis.IsVisible      = canChangeIsClientOwner;
        
        // Прочие
        _obj.State.Properties.IsOtheravis.IsEnabled           = canCreateCounterparty;
        
        // Примечание
        _obj.State.Properties.Note.IsEnabled                  = isEnabledMainPersonalData;
        
        // Вкладка Адрес.
        _obj.State.Pages.Address.IsVisible                    = canReadPersonalData;
        
        // Вкладка Перс. данные.
        _obj.State.Pages.PersonalDataavis.IsVisible           = canReadPersonalData;
        
        // Вкладка Документ, удостоверяющий личность
        _obj.State.Pages.IdentityDocument.IsVisible           = canReadPersonalData;
        
        // Код клиента в Инвест
        _obj.State.Properties.CodeInvestavis.IsVisible        = canReadPersonalData;
        
        // ИД ЛК
        _obj.State.Properties.CodeLKavis.IsVisible            = canReadPersonalData;
        
        // Лицевой счет
        _obj.State.Properties.PersonalAccountavis.IsVisible   = canReadPersonalData;
        
        #endregion
      }
    }
  }
}