using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.FormalizedPowerOfAttorney;

namespace lenspec.Etalon.Client
{
  partial class FormalizedPowerOfAttorneyFunctions
  {
    
    /// <summary>
    /// Проверить заполнение обязательных реквизитов для формирования МЧД.
    /// </summary>
    /// <returns>Сообщение об ошибках.</returns>
    public string CheckRequiredPropertiesToGenerateBody()
    {
      var fieldsWithErrors = string.Empty;
      
      #region Представитель.
      
      // Для сотрудника обязательны для заполнения ИНН, СНИЛС, дата рождения, гражданство.
      if (_obj.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.Employee && _obj.IssuedTo != null)
      {
        var fieldNames = new List<string>();
        
        if (string.IsNullOrEmpty(_obj.IssuedTo.Person.TIN))
          fieldNames.Add(_obj.IssuedTo.Person.Info.Properties.TIN.LocalizedName);
        if (string.IsNullOrEmpty(_obj.IssuedTo.Person.INILA))
          fieldNames.Add(_obj.IssuedTo.Person.Info.Properties.INILA.LocalizedName);
        if (!_obj.IssuedTo.Person.DateOfBirth.HasValue)
          fieldNames.Add(_obj.IssuedTo.Person.Info.Properties.DateOfBirth.LocalizedName);
        if (_obj.IssuedTo.Person.Citizenship == null)
          fieldNames.Add(_obj.IssuedTo.Person.Info.Properties.Citizenship.LocalizedName);
        
        if (fieldNames.Any())
          fieldsWithErrors += string.Format(" Для Представителя заполните поля {0}.", string.Join(", ", fieldNames));
      }
      // Для ФЛ обязательны для заполнения ИНН, СНИЛС, дата рождения, гражданство.
      if (_obj.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.Person && _obj.IssuedToParty != null
          && Sungero.Parties.People.Is(_obj.IssuedToParty))
      {
        var person = Sungero.Parties.People.As(_obj.IssuedToParty);
        
        var fieldNames = new List<string>();
        
        if (string.IsNullOrEmpty(person.TIN))
          fieldNames.Add(person.Info.Properties.TIN.LocalizedName);
        if (string.IsNullOrEmpty(person.INILA))
          fieldNames.Add(person.Info.Properties.INILA.LocalizedName);
        if (!person.DateOfBirth.HasValue)
          fieldNames.Add(person.Info.Properties.DateOfBirth.LocalizedName);
        if (person.Citizenship == null)
          fieldNames.Add(person.Info.Properties.Citizenship.LocalizedName);
        
        if (fieldNames.Any())
          fieldsWithErrors += string.Format(" Для Представителя заполните поля {0}.", string.Join(", ", fieldNames));
      }
      // Для ИП обязательны для заполнения ИНН, ОГРН, СНИЛС, дата рождения и гражданство представителя.
      if (_obj.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.Entrepreneur && _obj.IssuedToParty != null &&
          Sungero.Parties.Companies.Is(_obj.IssuedToParty) && _obj.Representative != null)
      {
        var fieldNames = new List<string>();
        
        if (string.IsNullOrEmpty(_obj.IssuedToParty.PSRN))
          fieldNames.Add(_obj.IssuedToParty.Info.Properties.PSRN.LocalizedName);
        
        if (string.IsNullOrEmpty(_obj.Representative.TIN))
          fieldNames.Add(_obj.Representative.Info.Properties.TIN.LocalizedName);
        if (string.IsNullOrEmpty(_obj.Representative.INILA))
          fieldNames.Add(_obj.Representative.Info.Properties.INILA.LocalizedName);
        if (!_obj.Representative.DateOfBirth.HasValue)
          fieldNames.Add(_obj.Representative.Info.Properties.DateOfBirth.LocalizedName);
        if (_obj.Representative.Citizenship == null)
          fieldNames.Add(_obj.Representative.Info.Properties.Citizenship.LocalizedName);
        
        if (fieldNames.Any())
          fieldsWithErrors += string.Format(" Для Представителя заполните поля {0}.", string.Join(", ", fieldNames));
      }
      // Для ЮЛ обязательны для заполнения ИНН и КПП, дата рождения и гражданство представителя.
      if (_obj.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.LegalEntity && _obj.IssuedToParty != null &&
          Sungero.Parties.Companies.Is(_obj.IssuedToParty) && _obj.Representative != null)
      {
        var company = Sungero.Parties.Companies.As(_obj.IssuedToParty);
        var fieldNames = new List<string>();
        
        if (string.IsNullOrEmpty(company.TIN))
          fieldNames.Add(company.Info.Properties.TIN.LocalizedName);
        if (string.IsNullOrEmpty(company.TRRC))
          fieldNames.Add(company.Info.Properties.TRRC.LocalizedName);
        
        if (!_obj.Representative.DateOfBirth.HasValue)
          fieldNames.Add(_obj.Representative.Info.Properties.DateOfBirth.LocalizedName);
        if (_obj.Representative.Citizenship == null)
          fieldNames.Add(_obj.Representative.Info.Properties.Citizenship.LocalizedName);
        
        if (fieldNames.Any())
          fieldsWithErrors += string.Format(" Для Представителя заполните поля {0}.", string.Join(", ", fieldNames));
      }
      
      #endregion
      
      
      #region Доверитель.
      
      // Для НОР обязательны ОГРН, ИНН, КПП, юридический адрес.
      if (_obj.BusinessUnit != null)
      {
        var fieldNames = new List<string>();
        
        if (string.IsNullOrEmpty(_obj.BusinessUnit.PSRN))
          fieldNames.Add(_obj.BusinessUnit.Info.Properties.PSRN.LocalizedName);
        if (string.IsNullOrEmpty(_obj.BusinessUnit.TIN))
          fieldNames.Add(_obj.BusinessUnit.Info.Properties.TIN.LocalizedName);
        if (string.IsNullOrEmpty(_obj.BusinessUnit.TRRC))
          fieldNames.Add(_obj.BusinessUnit.Info.Properties.TRRC.LocalizedName);
        if (string.IsNullOrEmpty(_obj.BusinessUnit.LegalAddress))
          fieldNames.Add(_obj.BusinessUnit.Info.Properties.LegalAddress.LocalizedName);
        
        if (fieldNames.Any())
          fieldsWithErrors += string.Format(" Для Нашей организации заполните поля {0}.", string.Join(", ", fieldNames));
      }
      // Для подписывающего обязательны СНИЛС, должность, дата рождения, гражданство, а также документ, подтверждающий полномочия.
      if (_obj.OurSignatory != null)
      {
        var fieldNames = new List<string>();
        
        if (string.IsNullOrEmpty(_obj.OurSignatory.Person.INILA))
          fieldNames.Add(_obj.OurSignatory.Person.Info.Properties.INILA.LocalizedName);
        if (_obj.OurSignatory.JobTitle == null)
          fieldNames.Add(_obj.OurSignatory.Info.Properties.JobTitle.LocalizedName);
        if (!_obj.OurSignatory.Person.DateOfBirth.HasValue)
          fieldNames.Add(_obj.OurSignatory.Person.Info.Properties.DateOfBirth.LocalizedName);
        if (_obj.OurSignatory.Person.Citizenship == null)
          fieldNames.Add(_obj.OurSignatory.Person.Info.Properties.Citizenship.LocalizedName);
        
        if (fieldNames.Any())
          fieldsWithErrors += string.Format(" Для Подписанта заполните поля {0}.", string.Join(", ", fieldNames));
      }
      
      #endregion
      
      return fieldsWithErrors;
    }

  }
}