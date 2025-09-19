using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorney;

namespace lenspec.Etalon.Shared
{
  partial class PowerOfAttorneyFunctions
  {
    //Avis-Expert>>
    
    public override void FillPrincipalFields(Sungero.Company.IEmployee preparedBy, Nullable<Enumeration> agentType)
    {
      _obj.BusinessUnit = preparedBy?.Department?.BusinessUnit;
      _obj.Department = preparedBy?.Department;
    }
    
    /// <summary>
    /// Заполнить общие полномочия
    /// </summary>
    public void FillPowers()
    {
      _obj.Powers = string.Empty;
      if (_obj.PowerListlenspec.Any())
        _obj.Powers += string.Join(",\n", _obj.PowerListlenspec.Where(x => x.Authority != null).Select(x => x.Authority.Name));
      if (!string.IsNullOrWhiteSpace(_obj.FreePowerslenspec))
        _obj.Powers += !string.IsNullOrEmpty(_obj.Powers) ? "\n" + _obj.FreePowerslenspec : _obj.FreePowerslenspec;
    }

    /// <summary>
    /// Заполнить чекбокс Проект
    /// </summary>
    public void FillIsProject()
    {
      var requestCreatePOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
      var requestCreateNPOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
      var isProject = _obj.DocumentKind != null && (_obj.DocumentKind.Equals(requestCreatePOAKind) || _obj.DocumentKind.Equals(requestCreateNPOAKind));
      _obj.IsProjectPOAavis = isProject;
    }

    /// <summary>
    /// Добавление связанных документов в задачу
    /// </summary>
    /// <param name="group"></param>
    public override void AddRelatedDocumentsToAttachmentGroup(Sungero.Workflow.Interfaces.IWorkflowEntityAttachmentGroup @group)
    {
      var linkedDocuments = _obj.Relations.GetRelatedDocuments(Sungero.Docflow.Constants.Module.SimpleRelationName);
      foreach (var linkedDoc in linkedDocuments)
      {
        if (!group.All.Contains(linkedDoc))
          group.All.Add(linkedDoc);
      }
    }
    
    /// <summary>
    /// Сменить доступность реквизитов документа, блокируемых после регистрации.
    /// </summary>
    /// <param name="isEnabled">True, если свойства должны быть доступны.</param>
    /// <param name="isRepeatRegister">True, если повторная регистрация.</param>
    public override void ChangeDocumentPropertiesAccess(bool isEnabled, bool isRepeatRegister)
    {
      base.ChangeDocumentPropertiesAccess(isEnabled, isRepeatRegister);
      
      var poaProperties = _obj.State.Properties;
      poaProperties.CountCopiesavis.IsEnabled = isEnabled;
      poaProperties.RegistryNumavis.IsEnabled = isEnabled;
      poaProperties.PreparedBy.IsEnabled = isEnabled;
      poaProperties.BusinessUnit.IsEnabled = isEnabled;
      poaProperties.Cityavis.IsEnabled = isEnabled;
      poaProperties.OurSignatory.IsEnabled = isEnabled;
      poaProperties.DocKindsavis.IsEnabled = isEnabled;
      poaProperties.AuthorityKindsavis.IsEnabled = isEnabled;
      poaProperties.Counterpartyavis.IsEnabled = isEnabled;
      poaProperties.Amountavis.IsEnabled = isEnabled;
      poaProperties.Termavis.IsEnabled = isEnabled;
      poaProperties.TXTDatePOAavis.IsEnabled = isEnabled;
      poaProperties.ContractCategoryavis.IsEnabled = isEnabled;
    }
    
    
    /// <summary>
    /// Проверка поля NeedOtherAuthority
    /// </summary>
    public void CheckNeedOtherAuthority(bool? needOtherAuthority)
    {
      if(needOtherAuthority == true)
      {
        _obj.State.Properties.Authoritiesavis.IsEnabled = true;
        _obj.State.Properties.Authoritiesavis.IsRequired = true;
        _obj.State.Properties.AuthorityKindsavis.IsEnabled = true;
        _obj.State.Properties.AuthorityKindsavis.IsRequired = true;
      }
      else
      {
        _obj.Authoritiesavis.Clear();
        _obj.AuthorityKindsavis.Clear();
        _obj.State.Properties.Authoritiesavis.IsEnabled = false;
        _obj.State.Properties.Authoritiesavis.IsRequired = false;
        _obj.State.Properties.AuthorityKindsavis.IsEnabled = false;
        _obj.State.Properties.AuthorityKindsavis.IsRequired = false;
      }
    }
    
    /// <summary>
    /// Имя
    /// </summary>
    public override void FillName()
    {
      var issuedTo = string.Empty;
      if (_obj.IsManyRepresentatives != true)
      {
        var representativeName = string.Empty;
        if (_obj.IssuedTo != null)
          representativeName = CommonLibrary.PersonFullName.Create(_obj.IssuedTo.Person.LastName, _obj.IssuedTo.Person.FirstName, _obj.IssuedTo.Person.MiddleName, CommonLibrary.PersonFullNameDisplayFormat.InitialsAndLastName).ToString();
        else if (_obj.IssuedToParty != null)
        {
          var person =  Sungero.Parties.People.As(_obj.IssuedToParty);
          if (person != null)
            representativeName = CommonLibrary.PersonFullName.Create(person.LastName, person.FirstName, person.MiddleName, CommonLibrary.PersonFullNameDisplayFormat.InitialsAndLastName).ToString();
          else
            representativeName = _obj.IssuedToParty.Name;
        }
      }
      else
      {
        foreach (var line in _obj.Representatives)
        {
          if (line != null && line.IssuedTo != null)
          {
            var person = Sungero.Parties.People.As(line.IssuedTo);
            var name = person != null ? CommonLibrary.PersonFullName.Create(person.LastName, person.FirstName, person.MiddleName, CommonLibrary.PersonFullNameDisplayFormat.InitialsAndLastName).ToString() : line.IssuedTo.Name;
            issuedTo += !string.IsNullOrEmpty(issuedTo) ? ", " + name : name;
          }
        }
      }
      
      var number = string.Empty;
      var date = string.Empty;
      var poaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Sungero.Docflow.PublicConstants.Module.Initialize.PowerOfAttorneyKind);
      var nPoaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.DocumentNotarialKindGuid);
      if (Equals(poaKind, _obj.DocumentKind) || Equals(nPoaKind, _obj.DocumentKind))
      {
        if (_obj.DocumentKind.Equals(poaKind) && !string.IsNullOrEmpty(_obj.RegistrationNumber))
          number = " №" + _obj.RegistrationNumber;
        if (_obj.DocumentKind.Equals(nPoaKind) && !string.IsNullOrEmpty(_obj.RegistryNumavis))
          number = " №" + _obj.RegistryNumavis;
        if (_obj.ValidFrom != null)
          date = " от " + _obj.ValidFrom.Value.ToShortDateString();
      }
      
      _obj.Name = string.Format("{0}{1}{2}{3}", _obj.DocumentKind?.Name, !string.IsNullOrEmpty(issuedTo) ? $" для {issuedTo}" : string.Empty, number, date);
    }
    
    //<<Avis-Expert
  }
}