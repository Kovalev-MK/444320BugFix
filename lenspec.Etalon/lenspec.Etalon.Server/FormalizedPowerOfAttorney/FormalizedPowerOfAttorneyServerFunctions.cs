using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.FormalizedPowerOfAttorney;

namespace lenspec.Etalon.Server
{
  partial class FormalizedPowerOfAttorneyFunctions
  {
    
    public override bool ValidateGeneratedFormalizedPowerOfAttorneyXml(Sungero.Docflow.Structures.Module.IByteArray xml)
    {
      var isSoleExecutiveAuthority = _obj.PrincipalRepresentativeTypelenspec == lenspec.Etalon.FormalizedPowerOfAttorney.PrincipalRepresentativeTypelenspec.LegalEntity && _obj.SoleExecutiveAuthoritylenspec != null;
      var isForeignRepresentative = (_obj.IssuedTo != null && _obj.IssuedTo.Person.IdentityKind.Code == "10") || 
        (Sungero.Parties.People.Is(_obj.IssuedToParty) && Sungero.Parties.People.As(_obj.IssuedToParty).IdentityKind.Code == "10") || _obj.Representatives.Any(x => Sungero.Parties.People.Is(x.IssuedTo) &&
                                                                                                                                                               Sungero.Parties.People.As(x.IssuedTo).IdentityKind.Code == "10");
      return isSoleExecutiveAuthority || isForeignRepresentative ? true : base.ValidateGeneratedFormalizedPowerOfAttorneyXml(xml);
    }
    
    public override string GetFormalizedPowerOfAttorneyAsHtml(Sungero.Content.IElectronicDocumentVersions version)
    {
      if (version == null)
        return string.Empty;
      
      var mainBlock = ProducePoaHTML();
      
      var notarialBlock = string.Empty;
      if (_obj.IsNotarized.HasValue && _obj.IsNotarized.Value)
        notarialBlock = ProduceNotarialBlockPoaHTML();
      
      var result = string.Format(lenspec.Etalon.FormalizedPowerOfAttorneys.Resources.HtmlFPoATemplate, lenspec.Etalon.FormalizedPowerOfAttorneys.Resources.HtmlStylesFPoA, mainBlock, notarialBlock);
      return result;
    }
    
    private string ProducePoaHTML()
    {
      var delegationPhrase = _obj.IsDelegated.HasValue && _obj.IsDelegated.Value ? $"в порядке передоверия на основании доверенности {_obj.MainPoAUnifiedNumber}" : string.Empty;
      var principals = ProducePrincipalsHtml();
      var agents = ProduceAgentsHtml();
      var powers = ProducePowersHtml();
      var retrustPhrase = _obj.DelegationType == Sungero.Docflow.FormalizedPowerOfAttorney.DelegationType.WithDelegation ? "с правом передоверия." : "без права передоверия.";
      var block = string.Format(lenspec.Etalon.FormalizedPowerOfAttorneys.Resources.HtmlFPoABlockDataTemplate,
                                _obj.RegistrationDate.Value.ToShortDateString(), _obj.RegistrationNumber, _obj.UnifiedRegistrationNumber, delegationPhrase, principals, agents, powers, _obj.ValidFrom.Value.ToShortDateString(), _obj.ValidTill.Value.ToShortDateString(), retrustPhrase);
      
      return block;
    }
    
    private string ProduceNotarialBlockPoaHTML()
    {
      var block = string.Empty;
      return block;
    }
    
    private string ProducePrincipalsHtml()
    {
      var generalData = string.Format("Наименование юридического лица: {0}, ИНН:{1}, КПП:{2}, ОГРН:{3}, Адрес:{4}.", _obj.BusinessUnit.Name, _obj.BusinessUnit.TIN, _obj.BusinessUnit.TRRC,
                                      _obj.BusinessUnit.PSRN, _obj.BusinessUnit.LegalAddress);
      
      if (_obj.PrincipalRepresentativeTypelenspec == lenspec.Etalon.FormalizedPowerOfAttorney.PrincipalRepresentativeTypelenspec.LegalEntity && _obj.SoleExecutiveAuthoritylenspec != null)
      {
        var executiveBodyPhrase = "В лице единоличного исполнительного органа:";
        var executiveBodyData = string.Format("Наименование юридического лица: {0}, ИНН:{1}, КПП:{2}, ОГРН:{3}, Адрес:{4}.", _obj.SoleExecutiveAuthoritylenspec.Name, _obj.SoleExecutiveAuthoritylenspec.TIN, _obj.SoleExecutiveAuthoritylenspec.TRRC, _obj.SoleExecutiveAuthoritylenspec.PSRN, _obj.SoleExecutiveAuthoritylenspec.LegalAddress);
        var jobTitle = _obj.SoleExecutiveAuthoritylenspec.CEO.JobTitle != null ? string.Format("{0}: ", _obj.SoleExecutiveAuthoritylenspec.CEO.JobTitle.Name) : string.Empty;
        var executivePerson = string.Format("{0}{1}, ИНН:{2}, СНИЛС:{3}.", jobTitle, _obj.SoleExecutiveAuthoritylenspec.CEO.Name, _obj.SoleExecutiveAuthoritylenspec.CEO.Person.TIN, _obj.SoleExecutiveAuthoritylenspec.CEO.Person.INILA);
        return string.Format("{0} <br><br> {1} <br><br> {2} <br> {3}", generalData, executiveBodyPhrase, executiveBodyData, executivePerson);
      }
      else
      {
        var executiveBodyPhrase = "В лице:";
        var jobTitle = _obj.BusinessUnit.CEO.JobTitle != null ? string.Format("{0}: ", _obj.BusinessUnit.CEO.JobTitle.Name) : string.Empty;
        var executivePerson = string.Format("{0}{1}, ИНН:{2}, СНИЛС:{3}.", jobTitle, _obj.BusinessUnit.CEO.Name, _obj.BusinessUnit.CEO.Person.TIN, _obj.BusinessUnit.CEO.Person.INILA);
        return string.Format("{0} <br> {1} {2}", generalData, executiveBodyPhrase, executivePerson);
      }
    }
    
    private string ProduceAgentsHtml()
    {
      var agentDataList = string.Empty;
      foreach (var agent in _obj.Representatives)
      {
        if (agent != null)
        {
          if (!string.IsNullOrEmpty(agentDataList))
            agentDataList += "<br>";
          if (agent.AgentType == Sungero.Docflow.PowerOfAttorneyBaseRepresentatives.AgentType.LegalEntity)
          {
            var company = Sungero.Parties.CompanyBases.As(agent.IssuedTo);
            agentDataList += string.Format("Наименование юридического лица: {0}, ИНН:{1}, КПП:{2}, ОГРН:{3}, Адрес:{4}.", company.Name, company.TIN, company.TRRC,company.PSRN, company.LegalAddress);
          }
          if (agent.AgentType == Sungero.Docflow.PowerOfAttorneyBaseRepresentatives.AgentType.Person)
          {
            var person = Sungero.Parties.People.As(agent.IssuedTo);
            agentDataList += string.Format("{0}, дата рождения: {1}, СНИЛС:{2}, ИНН:{3}, документ удостоверяющий личность:{4}", person.Name,
                                           person.DateOfBirth.Value.ToShortDateString(), person.INILA, person.TIN, CreateIdentityDocumentString(person));
          }
          if (agent.AgentType == Sungero.Docflow.PowerOfAttorneyBaseRepresentatives.AgentType.Entrepreneur)
          {
            var company = Sungero.Parties.CompanyBases.As(agent.IssuedTo);
            agentDataList += string.Format("Наименование юридического лица: {0}, ИНН:{1}, КПП:{2}, ОГРН:{3}, Адрес:{4}.", company.Name, company.TIN, company.TRRC,company.PSRN, company.LegalAddress);
            if (agent.Agent != null)
            {
              var person = agent.Agent;
              agentDataList += string.Format("<br> {0}, дата рождения: {1}, СНИЛС:{2}, ИНН:{3}, документ удостоверяющий личность:{4}", person.Name,
                                             person.DateOfBirth.Value.ToShortDateString(), person.INILA, person.TIN, CreateIdentityDocumentString(person));
            }
          }
        }
      }
      return agentDataList;
    }
    
    private string CreateIdentityDocumentString(Sungero.Parties.IPerson person)
    {
      var identityDocument = string.Format("{0}{1}{2}{3}", person.IdentitySeries + person.IdentityNumber,
                                           string.IsNullOrEmpty(person.IdentityAuthority) ? string.Empty : $", выдан {person.IdentityAuthority}",
                                           person.IdentityDateOfIssue.HasValue ? $", дата выдачи {person.IdentityDateOfIssue.Value.ToShortDateString()}" : string.Empty,
                                           string.IsNullOrEmpty(person.IdentityAuthorityCode) ? string.Empty : $", код подразделения {person.IdentityAuthorityCode}");
      return identityDocument;
    }
    
    private string ProducePowersHtml()
    {
      var powerDataList = string.Empty;
      foreach (var line in _obj.StructuredPowers)
      {
        if (line != null && line.Power != null)
        {
          if (!string.IsNullOrEmpty(powerDataList))
            powerDataList += "<br>";
          powerDataList += $"• {line.Power.Code}_{line.Power.Name}";
        }
      }
      
      if (!string.IsNullOrEmpty(_obj.FreeFormPowerslenspec))
      {
        if (!string.IsNullOrEmpty(powerDataList))
          powerDataList += "<br>";
        powerDataList += _obj.FreeFormPowerslenspec;
      }
      return powerDataList;
    }
    
    public override void AddLegalEntityPrincipalV3(Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.Доверенность poa)
    {
      var lep = Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.PoAV3Builder.CreateLegalEntityPrincipal(poa);
      FillDirectLegalEntityPrincipalXml(lep);
      
      // Если у НОР есть ЕИО, то заполняется элемент СВЮЛ, иначе заполняется СвФЛ
      if (_obj.PrincipalRepresentativeTypelenspec == lenspec.Etalon.FormalizedPowerOfAttorney.PrincipalRepresentativeTypelenspec.LegalEntity && _obj.SoleExecutiveAuthoritylenspec != null)
      {
        lep.ЕИОФЛ = Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.РосОргДоверТипЕИОФЛ.Item0;
        lep.ЕИОУК = Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.РосОргДоверТипЕИОУК.Item1;
        lep.ЛицоБезДов[0].СвФЛ = null;
        
        lep.ЛицоБезДов[0].СВЮЛ = new Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.ЛицоБезДовТипСВЮЛ();
        var head = lep.ЛицоБезДов[0].СВЮЛ;
        FillLegalEntityPrincipalLikeManagmentCompanyXml(head);
      }
      else
      {
        var head = lep.ЛицоБезДов[0].СвФЛ;
        FillPrincipalPersonDataXml(head);
      }
    }
    
    /// <summary>
    /// Заполнение элемента "Сведения о российском юридическом лице (СвРосОрг)".
    /// </summary>
    /// <param name="lep"></param>
    private void FillDirectLegalEntityPrincipalXml(Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.РосОргДоверТип lep)
    {
      if (_obj.BusinessUnit != null)
      {
        var org = lep.СвРосОрг;
        org.НаимОрг = _obj.BusinessUnit.LegalName;
        org.ИННЮЛ = _obj.BusinessUnit.TIN;
        org.ОГРН = _obj.BusinessUnit.PSRN;
        org.КПП = _obj.BusinessUnit.TRRC;
        org.АдрРег.Item = _obj.BusinessUnit.LegalAddress;
        org.АдрРег.Регион = _obj.BusinessUnit.Region?.Code;
      }
    }
    
    /// <summary>
    /// Заполнить элемент "Сведения о доверители (СВЮЛ)".
    /// </summary>
    /// <param name="head">Объект представляющий сведения доверителя ЮЛ (СВЮЛ)</param>
    private void FillLegalEntityPrincipalLikeManagmentCompanyXml(Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.ЛицоБезДовТипСВЮЛ head)
    {
      head.СвЮЛЕИО = new Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.СвОргТип()
      {
        НаимОрг = _obj.SoleExecutiveAuthoritylenspec.Name,
        ИННЮЛ = _obj.SoleExecutiveAuthoritylenspec.TIN,
        КПП = _obj.SoleExecutiveAuthoritylenspec.TRRC,
        ОГРН = _obj.SoleExecutiveAuthoritylenspec.PSRN,
        АдрРег = new Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.АдрТип()
        {
          Регион = _obj.SoleExecutiveAuthoritylenspec.Region?.Code,
          ItemElementName = Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.ItemChoiceType.АдрРФ,
          Item = _obj.SoleExecutiveAuthoritylenspec.LegalAddress
        }
      };
      var personArray = new Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.СвФЛТип[1];
      personArray[0] = new Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.СвФЛТип()
      {
        СведФЛ = new Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.СведФЛТип()
        {
          ФИО = new Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.ФИОТип()
        }
      };
      
      head.СвФЛ = personArray;
      
      FillPrincipalPersonDataXml(head.СвФЛ.First());
    }
    
    /// <summary>
    /// Заполнить элемент "Сведения о доверители (СвФЛ)".
    /// </summary>
    /// <param name="head">Объект представляющий сведения доверителя ФЛ (СвФЛ)</param>
    private void FillPrincipalPersonDataXml(Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.СвФЛТип head)
    {
      var employee = _obj.OurSignatory;
      if (employee != null && employee.Person != null)
      {
        head.ИННФЛ = employee.Person.TIN;
        head.СНИЛС = Sungero.Parties.PublicFunctions.Person.GetFormattedInila(employee.Person);
        if (employee.JobTitle != null)
          head.Должность = employee.JobTitle.Name;
        
        head.СведФЛ.ФИО.Имя = employee.Person.FirstName;
        head.СведФЛ.ФИО.Фамилия = employee.Person.LastName;
        head.СведФЛ.ФИО.Отчество = employee.Person.MiddleName;
        head.СведФЛ.ПрГражд = Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAV3.PoAV3Enums.CitizenshipFlagToNative(Sungero.FormalizeDocumentsParser.PowerOfAttorney.Model.PoAEnums.CitizenshipFlag.Russia);
      }
    }
    
    /// <summary>
    /// Создать задачу на согл. по регламенту если есть правила согласования по документу.
    /// <returns></returns>
    /// </summary>
    [Remote]
    public lenspec.Etalon.IApprovalTask CheckApprovalRulesAndCreateTask()
    {
      var task = ApprovalTasks.Null;
      var availableApprovalRules = Sungero.Docflow.PublicFunctions.OfficialDocument.Remote.GetApprovalRules(_obj);
      if (availableApprovalRules.Any())
      {
        task = ApprovalTasks.Create();
        task.DocumentGroup.All.Add(_obj);
      }
      return task;
    }

    /// <summary>
    /// Получить наши организации для фильтрации подходящих прав подписи.
    /// </summary>
    /// <returns>Наши организации.</returns>
    public override List<Sungero.Company.IBusinessUnit> GetBusinessUnits()
    {
      // Проверка вхождения в роль "Пользователи с правами на указание в документах сотрудников из любых НОР".
      var roleSid = lenspec.EtalonDatabooks.PublicConstants.Module.RightsToSelectAnyEmployees;
      var hasRightsToSelectAnyEmployees = Users.Current.IncludedIn(roleSid);
      
      // Если сотрудник входит в роль, отправляем пустой список НОР в GetSignatureSettingsQuery().
      // Доступные для выбора сотрудники по НОР не фильтруются (см. Functions.SignatureSetting.GetSignatureSettings()).
      if (hasRightsToSelectAnyEmployees)
        return new List<Sungero.Company.IBusinessUnit>() { };
      else
        return base.GetBusinessUnits();
    }
  }
}