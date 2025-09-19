using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.FormalizedPowerOfAttorney;

namespace lenspec.Etalon.Shared
{
  partial class FormalizedPowerOfAttorneyFunctions
  {
    
    public override bool CheckRequiredPropertiesValues()
    {
      if (_obj.PrincipalRepresentativeTypelenspec == lenspec.Etalon.FormalizedPowerOfAttorney.PrincipalRepresentativeTypelenspec.LegalEntity)
      {
        if (_obj.SoleExecutiveAuthoritylenspec == null || string.IsNullOrEmpty(_obj.SoleExecutiveAuthoritylenspec.TIN) || string.IsNullOrEmpty(_obj.SoleExecutiveAuthoritylenspec.TRRC) ||
            string.IsNullOrEmpty(_obj.SoleExecutiveAuthoritylenspec.PSRN))
        {
          Logger.ErrorFormat("Execute CheckRequiredPropertiesValues: formalized power of attorney id {0}, sole executive authority data not specified (TIN, TRRC, PSRN).", _obj.Id);
          return false;
        }
        
        if (_obj.SoleExecutiveAuthoritylenspec.CEO == null || _obj.SoleExecutiveAuthoritylenspec.CEO.Person == null || string.IsNullOrEmpty(_obj.SoleExecutiveAuthoritylenspec.CEO.Person.TIN) ||
            string.IsNullOrEmpty(_obj.SoleExecutiveAuthoritylenspec.CEO.Person.INILA))
        {
          Logger.ErrorFormat("Execute CheckRequiredPropertiesValues: formalized power of attorney id {0}, sole executive authority not specified CEO data (TIN, INILA).", _obj.Id);
          return false;
        }
      }
      return base.CheckRequiredPropertiesValues();
    }
    
    /// <summary>
    /// Заполнить поле Полномочия с контролем количества символов
    /// </summary>
    /// <returns>Если количество символов привысило допустимое значение - True, иначе - False</returns>
    /// <remarks>Контроль длины необходим для вывода сообщения пользователю, если конечная строка значения была обрезана</remarks>
    public bool FillPowersAndCheckLenght()
    {
      var generalPowers = string.Empty;
      
      if (_obj.StructuredPowers != null && _obj.StructuredPowers.Any(x => x.Power != null))
      {
        var structuredPowerNames = _obj.StructuredPowers.Where(x => x.Power != null).Select(x => x.Power.Name).ToList();
        generalPowers = string.Join("\n", structuredPowerNames);
      }
      
      if(!string.IsNullOrWhiteSpace(_obj.FreeFormPowerslenspec))
      {
        generalPowers = string.IsNullOrEmpty(generalPowers) ? _obj.FreeFormPowerslenspec : generalPowers + Environment.NewLine + _obj.FreeFormPowerslenspec;
      }
      
      var isLenghtExceeded = generalPowers.Length > Etalon.Constants.Docflow.FormalizedPowerOfAttorney.PowersLength;
      _obj.Powers = isLenghtExceeded ? generalPowers.Substring(0, Etalon.Constants.Docflow.FormalizedPowerOfAttorney.PowersLength) : generalPowers;
      return isLenghtExceeded;
    }
    
    public override void ChangePowersFieldsVisibility()
    {
      var isSigned = _obj.InternalApprovalState == lenspec.Etalon.FormalizedPowerOfAttorney.InternalApprovalState.Signed;
      _obj.State.Properties.StructuredPowers.IsRequired = _obj.FpoaKindlenspec == lenspec.Etalon.FormalizedPowerOfAttorney.FpoaKindlenspec.State && !isSigned;
      _obj.State.Properties.StructuredPowers.IsEnabled = !isSigned;
      
      _obj.State.Properties.FreeFormPowerslenspec.IsVisible = _obj.FpoaKindlenspec == lenspec.Etalon.FormalizedPowerOfAttorney.FpoaKindlenspec.NonState;
      _obj.State.Properties.Powers.IsEnabled = _obj.State.Properties.Powers.IsRequired = false;
      
      var isDelegatedFieldsVisible = _obj.IsDelegated.HasValue && _obj.IsDelegated.Value;
      _obj.State.Properties.MainPoAPrincipal.IsVisible = isDelegatedFieldsVisible;
      _obj.State.Properties.MainPoARegistrationNumber.IsVisible = isDelegatedFieldsVisible;
      _obj.State.Properties.MainPoAUnifiedNumber.IsVisible = isDelegatedFieldsVisible;
      _obj.State.Properties.MainPoAValidFrom.IsVisible = isDelegatedFieldsVisible;
      _obj.State.Properties.MainPoAValidTill.IsVisible = isDelegatedFieldsVisible;
      
      _obj.State.Properties.PowersType.IsEnabled = false;
    }
    
    /// <summary>
    /// Заполнить имя.
    /// </summary>
    public override void FillName()
    {
      var issuedTo = string.Empty;
      if (!_obj.Representatives.Any())
        issuedTo = _obj.IssuedTo != null ? _obj.IssuedTo.Name : _obj.IssuedToParty != null ? _obj.IssuedToParty.Name : string.Empty;
      else
      {
        foreach (var line in _obj.Representatives)
        {
          if (line != null && line.IssuedTo != null)
            issuedTo += !string.IsNullOrEmpty(issuedTo) ? ", " + line.IssuedTo.Name : line.IssuedTo.Name;
        }
      }
      if (!string.IsNullOrEmpty(issuedTo))
        issuedTo = $"Электронная доверенность для {issuedTo}.";
      
      var principal = string.Empty;
      if (_obj.BusinessUnit != null)
        principal = $"Доверитель {_obj.BusinessUnit.Name}.";
      
      var registrationInfo = string.Empty;
      if (!string.IsNullOrEmpty(_obj.UnifiedRegistrationNumber))
        registrationInfo = string.Format("№ {0} ", _obj.UnifiedRegistrationNumber);
      if (!string.IsNullOrEmpty(_obj.RegistrationNumber) && _obj.RegistrationDate != null)
        registrationInfo += string.Format("(рег.№ {0}) от {1}.", _obj.RegistrationNumber, _obj.RegistrationDate.Value.ToString("d"));
      
      _obj.Name = string.Format("{0} {1} {2}", issuedTo, principal, registrationInfo);
    }
  }
}