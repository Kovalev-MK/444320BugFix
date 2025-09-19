using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Condition;

namespace lenspec.Etalon.Shared
{
  partial class ConditionFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Получить словарь поддерживаемых типов условий.
    /// </summary>
    /// <returns>
    /// Словарь.
    /// Ключ - GUID типа документа.
    /// Значение - список поддерживаемых условий.
    /// </returns>
    [Public]
    public override System.Collections.Generic.Dictionary<string, List<Enumeration?>> GetSupportedConditions()
    {
      var baseSupport = base.GetSupportedConditions();
      baseSupport[EtalonDatabooks.PublicConstants.Module.ApplicationBUCreationTypeGuid].Add(ConditionType.UploadBusinessU);
      baseSupport[avis.PowerOfAttorneyModule.PublicConstants.Module.FormalizedPowerOfAttorneyTypeGuid].Add(ConditionType.RequiresApprov);
      baseSupport[avis.PowerOfAttorneyModule.PublicConstants.Module.FormalizedPowerOfAttorneyTypeGuid].Add(ConditionType.RepresType);
      baseSupport[Constants.Docflow.Condition.OutgoingLetterGuid].Add(ConditionType.BusinessUnitAddresseeavis);
      baseSupport[avis.PowerOfAttorneyModule.PublicConstants.Module.FormalizedPowerOfAttorneyTypeGuid].Add(ConditionType.FtsListState);
      baseSupport[avis.PowerOfAttorneyModule.PublicConstants.Module.PowerOfAttorneyTypeGuid].Add(ConditionType.RepresType);
      baseSupport[lenspec.ElectronicDigitalSignatures.PublicConstants.Module.EDSApplicationTypeGuid].Add(ConditionType.IsUsedInRX);
      baseSupport[lenspec.ElectronicDigitalSignatures.PublicConstants.Module.EDSApplicationTypeGuid].Add(ConditionType.EmployeeRegion);
      baseSupport[lenspec.ElectronicDigitalSignatures.PublicConstants.Module.EDSApplicationTypeGuid].Add(ConditionType.EDSAppCategory);
      
      var types = Sungero.Docflow.PublicFunctions.DocumentKind.GetDocumentGuids(typeof(Sungero.Docflow.IOfficialDocument));
      foreach(var typeGuid in types)
      {
        baseSupport[typeGuid].Add(ConditionType.DocumentIsSignedavis);
      }
      
      return baseSupport;
    }
    
    /// <summary>
    /// Проверить условие.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="task">Задача на согласование.</param>
    /// <returns>Результат проверки условия. Структуру формата - выполнение условия, сообщение об ошибке.</returns>
    public override Sungero.Docflow.Structures.ConditionBase.ConditionResult CheckCondition(Sungero.Docflow.IOfficialDocument document,
                                                                                            Sungero.Docflow.IApprovalTask task)
    {
      if (_obj.ConditionType == ConditionType.UploadBusinessU)
      {
        var application = EtalonDatabooks.ApplicationBUCreationDocuments.As(document);
        if (application != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(application.UploadBusinessUnit == true, string.Empty);
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, lenspec.Etalon.Conditions.Resources.DocumentTypeErrorMessage);
      }
      
      if (_obj.ConditionType == ConditionType.RequiresApprov)
      {
        var FPOA = lenspec.Etalon.FormalizedPowerOfAttorneys.As(document);
        if (FPOA != null)
        {
          var authorities = FPOA.Authoritiesavis;
          if (authorities.Any(x => x.Authority != null && avis.PowerOfAttorneyModule.Authorities.As(x.Authority).NeedMandatoryApproval == true))
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(true, string.Empty);
          else
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(false, string.Empty);
        }
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, lenspec.Etalon.Conditions.Resources.DocumentTypeErrorMessage);
      }
      
      if (_obj.ConditionType == ConditionType.RepresType)
      {
        var poa = Sungero.Docflow.PowerOfAttorneyBases.As(document);
        if (poa != null)
        {
          var castedRepresentatives = poa.Representatives as Sungero.Domain.Shared.IChildEntityCollection<lenspec.Etalon.IPowerOfAttorneyBaseRepresentatives>;
          var result = ((poa.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.Employee || 
                         castedRepresentatives.Any(x => x.AgentType == Sungero.Docflow.PowerOfAttorneyBaseRepresentatives.AgentType.Person && x.Employeelenspec != null)) && 
                         _obj.RepresentativeTypeavis == lenspec.Etalon.Condition.RepresentativeTypeavis.Employee) ||
            (poa.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.LegalEntity && _obj.RepresentativeTypeavis == lenspec.Etalon.Condition.RepresentativeTypeavis.LegalEntity) ||
            (poa.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.Entrepreneur && _obj.RepresentativeTypeavis == lenspec.Etalon.Condition.RepresentativeTypeavis.SoleEntrepren) ||
            (poa.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.Person && _obj.RepresentativeTypeavis == lenspec.Etalon.Condition.RepresentativeTypeavis.Individual);
          
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(result, string.Empty);
        }
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, lenspec.Etalon.Conditions.Resources.DocumentTypeErrorMessage);
      }

      // Адресатами должны быть только зеркала НОР.
      if (_obj.ConditionType == ConditionType.BusinessUnitAddresseeavis)
      {
        var outgoingLetter = Sungero.RecordManagement.OutgoingLetters.As(document);
        if (outgoingLetter != null)
        {
          var correspondents = outgoingLetter.Addressees.Select(x => x.Correspondent);
          if (correspondents.Any(x => !Sungero.Parties.Companies.Is(x)))
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(false, string.Empty);
          
          var companies = correspondents.Select(x => Sungero.Parties.Companies.As(x)).Where(x => x != null);
          var businessUnits = Sungero.Company.BusinessUnits.GetAll().Where(x => x.Company != null);
          foreach (var company in companies)
          {
            if (!businessUnits.Any(x => company.Equals(x.Company)))
              return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(false, string.Empty);
          }
          
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(true, string.Empty);
        }
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, lenspec.Etalon.Conditions.Resources.DocumentTypeErrorMessage);
      }
      
      if (_obj.ConditionType == ConditionType.DocumentIsSignedavis)
      {
        if (task != null && Etalon.ApprovalTasks.As(task).DocumentIsSignedlenspec != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(Etalon.ApprovalTasks.As(task).DocumentIsSignedlenspec, string.Empty);
        
        if (document != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(document.InternalApprovalState == Sungero.Docflow.OfficialDocument.ExternalApprovalState.Signed, string.Empty);
        
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.FtsListState)
      {
        if (task != null && Etalon.ApprovalTasks.As(task).DocumentFtsListStatelenspec != null)
        {
          var taskFtsState = Etalon.ApprovalTasks.As(task).DocumentFtsListStatelenspec;
          var result = _obj.FtsListStateavis == lenspec.Etalon.Condition.FtsListStateavis.Registered && taskFtsState == Etalon.ApprovalTask.DocumentFtsListStatelenspec.Registered ||
            _obj.FtsListStateavis == lenspec.Etalon.Condition.FtsListStateavis.Rejected && taskFtsState == Etalon.ApprovalTask.DocumentFtsListStatelenspec.Rejected;
          
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(result, string.Empty);
        }
        
        var FPOA = lenspec.Etalon.FormalizedPowerOfAttorneys.As(document);
        if (FPOA != null)
        {
          var result = _obj.FtsListStateavis == lenspec.Etalon.Condition.FtsListStateavis.Registered && FPOA.FtsListState == lenspec.Etalon.FormalizedPowerOfAttorney.FtsListState.Registered ||
            _obj.FtsListStateavis == lenspec.Etalon.Condition.FtsListStateavis.Rejected && FPOA.FtsListState == lenspec.Etalon.FormalizedPowerOfAttorney.FtsListState.Rejected;
          
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(result, string.Empty);
        }
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, lenspec.Etalon.Conditions.Resources.DocumentTypeErrorMessage);
      }
      
      if (_obj.ConditionType == ConditionType.IsUsedInRX)
      {
        var edsApplication = lenspec.ElectronicDigitalSignatures.EDSApplications.As(document);
        if (edsApplication != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(edsApplication.IsUsedInRX == true, string.Empty);
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, lenspec.Etalon.Conditions.Resources.DocumentTypeErrorMessage);
      }
      
      if (_obj.ConditionType == ConditionType.EmployeeRegion)
      {
        var edsApplication = lenspec.ElectronicDigitalSignatures.EDSApplications.As(document);
        if (edsApplication != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(edsApplication.EmployeeRegion == _obj.EmployeeRegionlenspec, string.Empty);
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, lenspec.Etalon.Conditions.Resources.DocumentTypeErrorMessage);
      }
      
      if (_obj.ConditionType == ConditionType.EDSAppCategory)
      {
        var edsApplication = lenspec.ElectronicDigitalSignatures.EDSApplications.As(document);
        if (edsApplication != null)
        {
          var result = edsApplication.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Renewal ||
            edsApplication.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.InitialIssue;
          
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(result, string.Empty);
        }
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, lenspec.Etalon.Conditions.Resources.DocumentTypeErrorMessage);
      }
      
      return base.CheckCondition(document, task);
    }
    
    public override void ChangePropertiesAccess()
    {
      base.ChangePropertiesAccess();
      var isRepresType = _obj.ConditionType == lenspec.Etalon.Condition.ConditionType.RepresType;
      _obj.State.Properties.RepresentativeTypeavis.IsVisible = isRepresType;
      _obj.State.Properties.RepresentativeTypeavis.IsRequired = isRepresType;
      
      var isFtsListState = _obj.ConditionType == lenspec.Etalon.Condition.ConditionType.FtsListState;
      _obj.State.Properties.FtsListStateavis.IsVisible = isFtsListState;
      _obj.State.Properties.FtsListStateavis.IsRequired = isFtsListState;
      
      var isEmployeeRegion = _obj.ConditionType == lenspec.Etalon.Condition.ConditionType.EmployeeRegion;
      _obj.State.Properties.EmployeeRegionlenspec.IsVisible = isEmployeeRegion;
      _obj.State.Properties.EmployeeRegionlenspec.IsRequired = isEmployeeRegion;
    }
    
    public override void ClearHiddenProperties()
    {
      base.ClearHiddenProperties();
      if (!_obj.State.Properties.RepresentativeTypeavis.IsVisible)
        _obj.RepresentativeTypeavis = null;
      
      if (!_obj.State.Properties.FtsListStateavis.IsVisible)
        _obj.FtsListStateavis = null;
      
      if (!_obj.State.Properties.EmployeeRegionlenspec.IsVisible)
        _obj.EmployeeRegionlenspec = null;
    }
    
    //конец Добавлено Avis Expert
  }
}