using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorney;

namespace lenspec.Etalon.Server
{
  partial class PowerOfAttorneyFunctions
  {

    #region [Обновление полей]
    
    /// <summary>
    /// Обновить поля карточки.
    /// </summary>
    /// <param name="fields">Обновляемые поля.</param>
    [Public, Remote(IsPure = false)]
    public void UpdateFields(lenspec.Etalon.Structures.Docflow.PowerOfAttorney.IUpdatableFields fields)
    {
      // Наши организации.
      UpdateBusinessUnits(fields.BusinessUnits);
      // Представители.
      UpdateRepresentatives(fields.AgentType, fields.IssuedTo, fields.IssuedToParty);
      // Виды документов.
      UpdateDocumentKinds(fields.DocumentKinds);
      // Категории договора.
      UpdateCategories(fields.ContractCategories);
      // Сумма.
      _obj.Amountavis = fields.Amount;
      // Срок действия.
      UpdateValidDates(fields.ValidFrom, fields.ValidTill);
      
      _obj.Save();
    }
    
    /// <summary>
    /// Обновить список наших организаций.
    /// </summary>
    /// <param name="businessUnits">Наши организации.</param>
    /// <remarks>Не сохраняет сущность.</remarks>
    private void UpdateBusinessUnits(List<Sungero.Company.IBusinessUnit> businessUnits)
    {
      _obj.OurBusinessUavis.Clear();
      foreach (var businessUnit in businessUnits)
      {
        var newLine = _obj.OurBusinessUavis.AddNew();
        newLine.Company = businessUnit;
      }
    }
    
    /// <summary>
    /// Обновить представителей.
    /// </summary>
    /// <param name="agentType">Тип представителя.</param>
    /// <param name="issuedTo">Кому выдана (сотрудник).</param>
    /// <param name="issuedToParty">Кому выдана (контрагент).</param>
    /// <remarks>Не сохраняет сущность.</remarks>
    private void UpdateRepresentatives(Enumeration? agentType, Sungero.Company.IEmployee issuedTo, Sungero.Parties.ICounterparty issuedToParty)
    {
      if (_obj.IsManyRepresentatives == true)
      {
        // Если необходимо обновление множественных представителей, добавьте сюда обработчик.
      }
      else
      {
        // Тип представителя.
        if (agentType != _obj.AgentType)
          _obj.AgentType = agentType;
        
        // Кому.
        if (agentType == lenspec.Etalon.PowerOfAttorney.AgentType.Employee)
          _obj.IssuedTo = issuedTo;
        else
          _obj.IssuedToParty = issuedToParty;
      }
    }
    
    /// <summary>
    /// Обновить список видов документов.
    /// </summary>
    /// <param name="documentKinds">Виды документов.</param>
    /// <remarks>Не сохраняет сущность.</remarks>
    private void UpdateDocumentKinds(List<Sungero.Docflow.IDocumentKind> documentKinds)
    {
      _obj.DocKindsavis.Clear();
      foreach (var documentKind in documentKinds)
      {
        var newLine = _obj.DocKindsavis.AddNew();
        newLine.Kind = documentKind;
      }
    }
    
    /// <summary>
    /// Обновить категории договора.
    /// </summary>
    /// <param name="contractCategories">Категории договора.</param>
    /// <remarks>Не сохраняет сущность.</remarks>
    private void UpdateCategories(List<Sungero.Contracts.IContractCategory> contractCategories)
    {
      _obj.ContractCategoryavis.Clear();
      foreach (var contractCategory in contractCategories)
      {
        var newLine = _obj.ContractCategoryavis.AddNew();
        newLine.Category = contractCategory;
      }
    }
    
    /// <summary>
    /// Обновить срок действия.
    /// </summary>
    /// <param name="validFrom">Действует с.</param>
    /// <param name="validTill">Действует по.</param>
    /// <remarks>Не сохраняет сущность.</remarks>
    private void UpdateValidDates(DateTime validFrom, DateTime validTill)
    {
      // Действует с.
      if (!Equals(_obj.ValidFrom, validFrom))
        _obj.ValidFrom = validFrom;
      
      // Действует по.
      if (!Equals(_obj.ValidTill, validTill))
        _obj.ValidTill = validTill;
    }
    
    #endregion [Обновление полей]

    [Public, Remote(IsPure = true)]
    public static Enumeration? GetAgentType(string agentTypeName)
    {
      if (String.IsNullOrEmpty(agentTypeName))
        return null;
      
      // Person.
      if (agentTypeName == PowerOfAttorneys.Info.Properties.AgentType.GetLocalizedValue(lenspec.Etalon.PowerOfAttorney.AgentType.Person))
        return lenspec.Etalon.PowerOfAttorney.AgentType.Person;
      // Enterpreneur.
      if (agentTypeName == PowerOfAttorneys.Info.Properties.AgentType.GetLocalizedValue(lenspec.Etalon.PowerOfAttorney.AgentType.Entrepreneur))
        return lenspec.Etalon.PowerOfAttorney.AgentType.Entrepreneur;
      // LegalEntity.
      if (agentTypeName == PowerOfAttorneys.Info.Properties.AgentType.GetLocalizedValue(lenspec.Etalon.PowerOfAttorney.AgentType.LegalEntity))
        return lenspec.Etalon.PowerOfAttorney.AgentType.LegalEntity;
      // Employee.
      if (agentTypeName == PowerOfAttorneys.Info.Properties.AgentType.GetLocalizedValue(lenspec.Etalon.PowerOfAttorney.AgentType.Employee))
        return lenspec.Etalon.PowerOfAttorney.AgentType.Employee;
      
      throw new ArgumentException(lenspec.Etalon.PowerOfAttorneys.Resources.UnrecognizedAgentType);
    }
    
    /// <summary>
    /// Проверить одинковые ли ответственные делопроизводители в указанных НОР
    /// </summary>
    /// <returns>True - одинаковые. Иначе - false</returns>
    public bool CheckResponsibleKindRoles()
    {
      var result = true;
      if (_obj.OurBusinessUavis.Count > 1)
      {
        var firstBusinessUnit = lenspec.Etalon.BusinessUnits.As(_obj.OurBusinessUavis.FirstOrDefault().Company);
        if (firstBusinessUnit.RoleKindEmployeelenspec.Any(x => x.RoleKind.Name.Equals(lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk)))
        {
          var firstPerformerLine = firstBusinessUnit.RoleKindEmployeelenspec.FirstOrDefault(x => x.RoleKind.Name.Equals(lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk));
          var firstPerformer = firstPerformerLine.Employee;
          foreach (var line in _obj.OurBusinessUavis)
          {
            var convertCompany = lenspec.Etalon.BusinessUnits.As(line.Company);
            if (convertCompany.RoleKindEmployeelenspec.Any(x => x.RoleKind.Name.Equals(lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk) && x.Employee.Equals(firstPerformer)))
              continue;
            else
            {
              result = false;
              break;
            }
          }
        }
        else
          result = false;
      }
      
      return result;
    }
    
    /// <summary>
    /// Создать приложение к доверенности из файла
    /// </summary>
    /// <param name="name">Имя файла с расширением</param>
    /// <param name="fileContent">Объект ByteArray</param>
    [Remote]
    public void CreateAddendumPowerOfAttorney(string name, Sungero.Docflow.Structures.Module.IByteArray fileContent)
    {
      var regex = new System.Text.RegularExpressions.Regex(@"(.*)\.(.+)$");
      var tempData = regex.Match(name);
      var docName = tempData.Groups[1].Value;
      var extension = tempData.Groups[2].Value;
      var document = Sungero.Docflow.SimpleDocuments.Create();
      document.Name = docName;
      document.DocumentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.DocumentAttorneyKindGuid);
      using (var stream = new System.IO.MemoryStream(fileContent.Bytes))
      {
        var version = document.CreateVersionFrom(stream, extension);
        document.Relations.AddFrom(Sungero.Docflow.Constants.Module.SimpleRelationName, _obj);
        document.Save();
      }
    }
    
    public override StateView GetDocumentSummary()
    {
      var stateView = StateView.Create();
      var block = stateView.AddBlock();
      var kind = _obj.DocumentKind != null ? _obj.DocumentKind.Name : string.Empty;
      var countCopies = _obj.CountCopiesavis != null ? _obj.CountCopiesavis.Value : 0;
      var ourBusinessUnits = _obj.OurBusinessUavis.Any() ? string.Join(", ", _obj.OurBusinessUavis.Select(x => x.Company.Name)) : string.Empty;
      block.AddLabel(string.Format("Вид документа: {0}", kind));
      block.AddLineBreak();
      block.AddLabel(string.Format("Количество экземпляров: {0}", countCopies));
      block.AddLineBreak();
      block.AddLabel(string.Format("Наши организации: {0}", ourBusinessUnits));
      return stateView;
    }
    
    public override List<Sungero.Company.IBusinessUnit> GetBusinessUnits()
    {
      if (_obj.IsProjectPOAavis == true)
      {
        var businessUnits = new List<Sungero.Company.IBusinessUnit>() { };
        
        if (_obj.OurBusinessUavis.Any())
          businessUnits.AddRange(_obj.OurBusinessUavis.Select(x => x.Company));
        
        return businessUnits;
      }
      else
        return base.GetBusinessUnits();
    }
  }
}