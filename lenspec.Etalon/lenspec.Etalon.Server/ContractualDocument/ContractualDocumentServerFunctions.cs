using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractualDocument;
using lenspec.Etalon.Integrations;

namespace lenspec.Etalon.Server
{
  partial class ContractualDocumentFunctions
  {
    /// <summary>
    /// Заполнить карточку документа по аналогии с другим внутригрупповым документом.
    /// </summary>
    /// <returns>Cозданный документ.</returns>
    [Remote]
    public lenspec.Etalon.IContractualDocument FillFromIntragroupDocument()
    {
      var newDocument = lenspec.Etalon.ContractualDocuments.Null;
      
      if (lenspec.Etalon.Contracts.Is(_obj))
        newDocument = lenspec.Etalon.Contracts.Create();
      else if (lenspec.Etalon.SupAgreements.Is(_obj))
        newDocument = lenspec.Etalon.SupAgreements.Create();
      else if (avis.EtalonContracts.AttachmentContractDocuments.Is(_obj))
        newDocument = avis.EtalonContracts.AttachmentContractDocuments.Create();
      
      var businessUnit = _obj.BusinessUnit;
      Functions.ContractualDocument.FillFromAnotherDocument(newDocument, _obj.Id);
      
      // Общие поля
      // Контрагент и НОР меняются местами
      var counterparty = Etalon.BusinessUnits.GetAll(x => Equals(x.Company, _obj.Counterparty)).FirstOrDefault();
      newDocument.Counterparty = businessUnit.Company;
      newDocument.BusinessUnit = counterparty;
      
      if (_obj.HasVersions)
      {
        using (var body = _obj.LastVersion.Body.Read())
          newDocument.CreateVersionFrom(body, _obj.AssociatedApplication.Extension);
      }
      
      // Договоры и Доп. соглашения
      if ((lenspec.Etalon.Contracts.Is(_obj) && lenspec.Etalon.Contracts.Is(newDocument)) ||
          (lenspec.Etalon.SupAgreements.Is(_obj) && lenspec.Etalon.SupAgreements.Is(newDocument)))
      {
        if (_obj.Archiveavis == true)
        {
          newDocument.LifeCycleState = _obj.LifeCycleState;
          newDocument.InternalApprovalState = _obj.InternalApprovalState;
          newDocument.ExternalApprovalState = _obj.ExternalApprovalState;
          
          var stringDepartmentId = EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.DepartmentForClosedEmployees);
          
          if (!string.IsNullOrEmpty(stringDepartmentId))
          {
            var departmentId = Convert.ToInt32(stringDepartmentId);
            var departmentForClosedEmployees = Sungero.Company.Departments.GetAll(x => x.Id == departmentId).SingleOrDefault();
            newDocument.Department = departmentForClosedEmployees;
          }
        }
      }
      
      // Договоры
      if (lenspec.Etalon.Contracts.Is(_obj) && lenspec.Etalon.Contracts.Is(newDocument))
      {
        newDocument.Archiveavis = _obj.Archiveavis;
      }
      
      return newDocument;
    }
    
    /// <summary>
    /// Проверить наличие договорного документа в системе с указанным ИД и соответствие типов текущего документа и найденного
    /// </summary>
    /// <param name="documentId">ИД</param>
    /// <returns>Сообщение с ошибкой если она есть</returns>
    [Remote]
    public string CheckDocumentAvailabilityAndType(long documentId)
    {
      var errorMessage = string.Empty;
      var anotherDocument = lenspec.Etalon.ContractualDocuments.Null;
      AccessRights.AllowRead(() => { anotherDocument = lenspec.Etalon.ContractualDocuments.GetAll(x => x.Id == documentId).SingleOrDefault(); });
      if (anotherDocument == null)
        errorMessage = "Документ с указанным ИД не найден.";
      else if ((lenspec.Etalon.Contracts.Is(_obj) && lenspec.Etalon.SupAgreements.Is(anotherDocument)) || (lenspec.Etalon.SupAgreements.Is(_obj) && lenspec.Etalon.Contracts.Is(anotherDocument)))
        errorMessage = "Тип найденного документа отличается от типа текущего документа.";
      return errorMessage;
    }
    
    /// <summary>
    /// Изменение статуса синхронизации документа
    /// </summary>
    [Public]
    public void UpdateContractSyns()
    {
      var asyncChangeSynchronizationStatus = avis.EtalonContracts.AsyncHandlers.AsyncChangeSynchronizationStatus.Create();
      asyncChangeSynchronizationStatus.ContractualDocumentId = _obj.Id;
      asyncChangeSynchronizationStatus.ExecuteAsync();
    }
    
    /// <summary>
    /// Заполнить карточку документа по аналогии с другим документом
    /// </summary>
    /// <param name="documentId">ИД документа, данные которого копируются в текущий</param>
    [Remote]
    public void FillFromAnotherDocument(long documentId)
    {
      var anotherDocument = lenspec.Etalon.ContractualDocuments.Null;
      AccessRights.AllowRead(() => { anotherDocument = lenspec.Etalon.ContractualDocuments.Get(documentId); });
      // Общие поля
      _obj.DocumentKind = anotherDocument.DocumentKind;
      _obj.BusinessUnit = Sungero.Company.Employees.Current.Department?.BusinessUnit;
      _obj.ResponsibleEmployee = anotherDocument.ResponsibleEmployee;
      
      var registersIds = lenspec.Etalon.PublicFunctions.OfficialDocument.GetDocumentRegistersIdsByDocumentAvis(_obj, Sungero.Docflow.RegistrationSetting.SettingType.Numeration);
      var defaultDocumentRegister = Sungero.Docflow.Shared.DocumentRegisterFunctions.GetDefaultDocRegister(_obj, registersIds, Sungero.Docflow.RegistrationSetting.SettingType.Numeration);
      
      if (defaultDocumentRegister != null)
      {
        _obj.DocumentRegister = defaultDocumentRegister;
        _obj.RegistrationNumber = anotherDocument.RegistrationNumber;
        _obj.RegistrationDate = anotherDocument.RegistrationDate;
        _obj.RegistrationState = anotherDocument.RegistrationState;
      }

      _obj.OurSignatory = null;
      
      // Договоры и Доп. соглашения
      if ((lenspec.Etalon.Contracts.Is(_obj) && lenspec.Etalon.Contracts.Is(anotherDocument)) ||
          (lenspec.Etalon.SupAgreements.Is(_obj) && lenspec.Etalon.SupAgreements.Is(anotherDocument)))
      {
        _obj.ContractTypeavis = anotherDocument.ContractTypeavis;
        _obj.GroupContractTypeavis = anotherDocument.GroupContractTypeavis;
        _obj.ContractKindavis = anotherDocument.ContractKindavis;
        _obj.ContractCategoryavis = anotherDocument.ContractCategoryavis;
        _obj.IsStandard = anotherDocument.IsStandard;
        _obj.OurCFavis = anotherDocument.OurCFavis;
        _obj.SubjectContractavis = anotherDocument.SubjectContractavis;
        _obj.ValidFrom = anotherDocument.ValidFrom;
        _obj.ValidTill = anotherDocument.ValidTill;
        _obj.TotalAmount = anotherDocument.TotalAmount;
        _obj.Currency = anotherDocument.Currency;
        _obj.VatRate = anotherDocument.VatRate;
        _obj.Nalogavis = anotherDocument.Nalogavis;
        _obj.Objectlenspec = anotherDocument.Objectlenspec;
        _obj.Regionlenspec = anotherDocument.Regionlenspec;
        _obj.Districtlenspec = anotherDocument.Districtlenspec;
        _obj.DeliveryMethod = anotherDocument.DeliveryMethod;
        _obj.IsICPlenspec = anotherDocument.IsICPlenspec;
        _obj.IsSMRavis = anotherDocument.IsSMRavis;
        _obj.Noteavis = anotherDocument.Noteavis;
        _obj.Department = null;
        _obj.PresenceRegionlenspec = anotherDocument.PresenceRegionlenspec;
        _obj.GroupContractTypeavis = anotherDocument.GroupContractTypeavis;
        _obj.ContractKindavis = anotherDocument.ContractKindavis;
        _obj.ThirdSideavis = anotherDocument.ThirdSideavis;
        
        var incomeContractType = avis.EtalonContracts.PublicFunctions.ContractType.Remote.GetContractTypeByConstantCode(EtalonDatabooks.PublicConstants.ConstantDatabook.IncomeContractType);
        var expensiveContractType = avis.EtalonContracts.PublicFunctions.ContractType.Remote.GetContractTypeByConstantCode(EtalonDatabooks.PublicConstants.ConstantDatabook.ExpensiveContractType);
        
        if (incomeContractType != null && expensiveContractType != null)
        {
          if (Equals(_obj.ContractTypeavis, incomeContractType))
            _obj.ContractTypeavis = expensiveContractType;
          else if (Equals(_obj.ContractTypeavis, expensiveContractType))
            _obj.ContractTypeavis = incomeContractType;
        }
      }
      
      // Договоры
      if (lenspec.Etalon.Contracts.Is(_obj) && lenspec.Etalon.Contracts.Is(anotherDocument))
      {
        var anotherContract = lenspec.Etalon.Contracts.As(anotherDocument);
        var currentContract = lenspec.Etalon.Contracts.As(_obj);
        currentContract.IsFrameworkavis = anotherContract.IsFrameworkavis;
        currentContract.DaysToFinishWorks = anotherContract.DaysToFinishWorks;
        currentContract.IsAutomaticRenewal = anotherContract.IsAutomaticRenewal;
      }
      // Доп соглашения
      if (lenspec.Etalon.SupAgreements.Is(_obj) && lenspec.Etalon.SupAgreements.Is(anotherDocument))
      {
        var anotherSupAgr = lenspec.Etalon.SupAgreements.As(anotherDocument);
        var currentSupAgr = lenspec.Etalon.SupAgreements.As(_obj);
        
        currentSupAgr.IsTerminationavis = anotherSupAgr.IsTerminationavis;
        currentSupAgr.LeadingDocument = null;
      }
      // Заполнение коллекции
      _obj.ConstructionObjectsavis.Clear();
      foreach (var line in anotherDocument.ConstructionObjectsavis)
      {
        var currentDocumentLine = _obj.ConstructionObjectsavis.AddNew();
        currentDocumentLine.DetailingWorkType = line.DetailingWorkType;
        currentDocumentLine.MeasureSizeAmountavis = line.MeasureSizeAmountavis;
        currentDocumentLine.Number = line.Number;
        currentDocumentLine.ObjectAnProject = line.ObjectAnProject;
        currentDocumentLine.Summ = line.Summ;
      }
    }


    /// <summary>
    /// Выгрузка данных с вкладки «Объекты строительства» договорного документа в Integra
    /// </summary>
    [Public]
    public bool UnloadingContractsDetails()
    {
      //Соберем данные для выгрузки ОС
      var businessUnit    = lenspec.Etalon.BusinessUnits.GetAll();
      var contractRXID    = _obj.Id;
      int? contractID     = _obj.IdDirectumavis;
      if (contractID == null)
        contractID     = 0;
      string stOrgCode    = lenspec.Etalon.BusinessUnits.GetAll().Where(x => x.Id == _obj.BusinessUnit.Id).FirstOrDefault().ExternalCodeavis;
      DateTime modTime    = Calendar.Now;
      
      Logger.Debug($"Avis - UnloadingContractsDetails. Выгрузка данных по договорному документу ИД = {contractRXID}");
      
      foreach (var constructionObject in _obj.ConstructionObjectsavis)
      {
        Logger.Debug($"Avis - UnloadingContractsDetails. Выгрузка Объекта строительства ИД = {constructionObject.Id}");
        
        string contrSites   = "-";
        if(constructionObject.ObjectAnProject != null)
          if (constructionObject.ObjectAnProject.Code1c != null)
            contrSites = constructionObject.ObjectAnProject.Code1c;
        
        // Тикет DIRRXMIGR-1095, поменял с Вида работ(WorkType) на Детализацию видов работ(DetailingWorkType)
        string workType     = "-";
        if(constructionObject.DetailingWorkType != null && !string.IsNullOrEmpty(constructionObject.DetailingWorkType.Code1c))
          workType = constructionObject.DetailingWorkType.Code1c;
        
        double? sumDouble   = constructionObject.Summ;
        float sum           = (float)sumDouble;
        int? xrecID         = constructionObject.Number;
        if (xrecID == null)
          xrecID     = 0;
        int? xrecIdFrom1C  = constructionObject.NumberInContract;
        if (xrecIdFrom1C == null)
          xrecIdFrom1C     = 0;
        string rx          = "TRUE";
        lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingContractsOSIntegrationHelper(contractRXID, contractID, stOrgCode,
                                                                                                       contrSites, workType, sum,
                                                                                                       xrecID, xrecIdFrom1C, modTime, rx);
        //Контрагент – Зеркало НОР
        var correspondent = Sungero.Parties.Companies.As(_obj.Counterparty);
        var nor = businessUnit.Where(x => x.Company.Equals(correspondent)).SingleOrDefault();
        if (nor != null)
        {
          string nor2Code         = nor.ExternalCodeavis;
          if (!string.IsNullOrWhiteSpace(nor2Code) && !string.IsNullOrEmpty(nor2Code))
            lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingContractsOSIntegrationHelper(contractRXID, contractID, nor2Code,
                                                                                                           contrSites, workType, sum,
                                                                                                           xrecID, xrecIdFrom1C, modTime, rx);
        }
        
        //        //Третья сторона - Зеркало НОР
        //        if (_obj.ThirdSideavis != null)
        //        {
        //          var companythirdSide = Sungero.Parties.Companies.As(_obj.ThirdSideavis);
        //          var thirdSide        = businessUnit.Where(x => x.Company.Equals(companythirdSide)).SingleOrDefault();
        //          if (thirdSide != null)
        //          {
        //            string thirdSideCode = thirdSide.ExternalCodeavis;
        //            if (!string.IsNullOrWhiteSpace(thirdSideCode) && !string.IsNullOrEmpty(thirdSideCode))
        //              lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingContractsOSIntegrationHelper(contractRXID, contractID, thirdSideCode,
        //                                                                                                             contrSites, workType, sum,
        //                                                                                                             xrecID, xrecIdFrom1C, modTime, rx);
        //          }
        //        }
      }
      return true;
    }

    /// <summary>
    /// Выгрузка данных договорного документа в Integra.
    /// </summary>
    [Public]
    public bool UnloadingContracts()
    {
      Logger.Debug($"Avis - UnloadingContracts. Выгрузка ДД с ИД {_obj.Id}. Старт.");
      
      var businessUnit  = lenspec.Etalon.BusinessUnits.GetAll();
      //Соберем данные для выгрузки
      var contractRXID    = _obj.Id;
      int? contractID     = _obj.IdDirectumavis;
      if (contractID == null)
        contractID     = 0;
      string stOrgCode    = businessUnit.Where(x => x.Id == _obj.BusinessUnit.Id).FirstOrDefault().ExternalCodeavis;
      string contractorID = "-";
      if(_obj.Counterparty != null)
      {
        var company = lenspec.Etalon.Companies.As(_obj.Counterparty);
        if (company != null)
        {
          if(company.ExternalCodeavis != null)
            contractorID = company.ExternalCodeavis;
        }
      }
      string contractNo   = _obj.RegistrationNumber;
      if(String.IsNullOrEmpty(contractNo))
        contractNo   = "-";
      DateTime? date      = _obj.RegistrationDate;
      DateTime modtime    = Calendar.Now;
      string subjContract = _obj.SubjectContractavis;
      long idMainContract = 0;
      long idMainContractRX = 0;
      //Если доп соглашение, то тогда надо вычислять
      if(lenspec.Etalon.SupAgreements.Is(_obj))
      {
        var sup = lenspec.Etalon.SupAgreements.As(_obj);
        idMainContract = sup.LeadingDocument.IdDirectumavis ?? 0;
        idMainContractRX = sup.LeadingDocument.Id;
      }
      DateTime? startDate = _obj.ValidFrom;
      DateTime? expDate   = _obj.ValidTill;
      int? contrType      = _obj.ContractTypeavis.Code;
      if (contrType == null)
        contrType      = 0;
      decimal sum         = Convert.ToDecimal(_obj.TotalAmount);
      decimal taxGroup    = 0;
      if (_obj.VatRate != null && _obj.VatRate.Rate.HasValue)
        taxGroup = _obj.VatRate.Sid != Sungero.Docflow.Constants.Module.VatRateWithoutVatSid
          ? Convert.ToDecimal(_obj.VatRate.Rate.Value)
          : -1;
      if (_obj.VatRate.Sid == lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateMixedPercentSid)
        taxGroup = 20;
      string currency               = _obj.Currency.NumericCode;
      decimal taxSum                = Convert.ToDecimal(_obj.Nalogavis);
      int isp                       = Convert.ToInt32(_obj.IsICPlenspec);
      int smr                       = Convert.ToInt32(_obj.IsSMRavis);
      string author                 = _obj.ResponsibleEmployee.Name;
      string region                 = _obj.PresenceRegionlenspec?.Name;
      string fiasDistrictCode       = "-";
      if (_obj.Districtlenspec != null)
        fiasDistrictCode = _obj.Districtlenspec.Id.ToString();
      string nameDocumentTypeGroup  = _obj.GroupContractTypeavis.Name;
      string nameDocumentType       = "-";
      if(_obj.ContractKindavis != null)
        nameDocumentType = _obj.ContractKindavis.Name;
      string category               = _obj.ContractCategoryavis.Name;
      int action                    = 2;
      int phys                      = 0;
      long contractorRXID           = _obj.Counterparty.Id;
      string rx                     = "TRUE";
      
      
      //Контрагент – Контрагент1 Третья сторона – Пусто Наша орг. – НОР1
      lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingContractsIntegrationHelper(contractRXID, contractID, stOrgCode, contractorID, contractNo, date, modtime, subjContract, idMainContract, startDate,
                                                                                                   expDate, contrType, sum, taxGroup, currency, taxSum, isp, smr, author, region, fiasDistrictCode, nameDocumentTypeGroup,
                                                                                                   nameDocumentType, category, action, phys, contractorRXID, idMainContractRX, rx);
      
      //Контрагент – Зеркало НОР2 Третья сторона – Пусто Наша орг. – НОР1
      var correspondent = Sungero.Parties.Companies.As(_obj.Counterparty);
      var nor = businessUnit.Where(x => x.Company.Equals(correspondent)).SingleOrDefault();
      if (nor != null)
      {
        string nor2Code         = nor.ExternalCodeavis;
        string contractorIDNor1 = "-";
        int? contrTypeNor2       = contrType;
        if (contrType == 2)
          contrTypeNor2 = 3;
        if (contrType == 3)
          contrTypeNor2 = 2;
        var companyNor1 = lenspec.Etalon.Companies.As(_obj.BusinessUnit.Company);
        if (companyNor1 != null)
        {
          if(companyNor1.ExternalCodeavis != null)
            contractorIDNor1 = companyNor1.ExternalCodeavis;
        }
        lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingContractsIntegrationHelper(contractRXID, contractID, nor2Code, contractorIDNor1, contractNo, date, modtime, subjContract, idMainContract, startDate,
                                                                                                     expDate, contrTypeNor2, sum, taxGroup, currency, taxSum, isp, smr, author, region, fiasDistrictCode, nameDocumentTypeGroup,
                                                                                                     nameDocumentType, category, action, phys, contractorRXID, idMainContractRX, rx);
      }
      
      //      if (_obj.ThirdSideavis != null)
      //      {
      //        string thirdSideID = "-";
      //        var companythirdSide = lenspec.Etalon.Companies.As(_obj.ThirdSideavis);
      //        if (companythirdSide != null)
      //        {
      //          if(companythirdSide.ExternalCodeavis != null)
      //            thirdSideID = companythirdSide.ExternalCodeavis;
      //        }
      //        //Контрагент – Контрагент1 Третья сторона – Контрагент2 Наша орг. – НОР1
      //        lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingContractsIntegrationHelper(contractRXID, contractID, stOrgCode, thirdSideID, contractNo, date, modtime, subjContract, idMainContract, startDate,
      //                                                                                                     expDate, contrType, sum, taxGroup, currency, taxSum, isp, smr, author, region, fiasDistrictCode, nameDocumentTypeGroup,
      //                                                                                                     nameDocumentType, category, action, phys, contractorRXID, idMainContractRX, rx);
      //        if (nor != null)
      //        {
      //          string nor2Code         = nor.ExternalCodeavis;
      //          int? contrTypeNor2      = 0 ;
      //          if (contrType == 2)
      //            contrTypeNor2 = 3;
      //          if (contrType == 3)
      //            contrTypeNor2 = 2;
      //          //Контрагент – Зеркало НОР2 Третья сторона – Контрагент1 Наша орг. – НОР1
      //          lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingContractsIntegrationHelper(contractRXID, contractID, nor2Code, thirdSideID, contractNo, date, modtime, subjContract, idMainContract, startDate,
      //                                                                                                       expDate, contrTypeNor2, sum, taxGroup, currency, taxSum, isp, smr, author, region, fiasDistrictCode, nameDocumentTypeGroup,
      //                                                                                                       nameDocumentType, category, action, phys, contractorRXID, idMainContractRX, rx);
//
      //        }
      //        var correspondentThird = Sungero.Parties.Companies.As(_obj.ThirdSideavis);
      //        var thirdSide = businessUnit.Where(x => x.Company.Equals(correspondentThird)).SingleOrDefault();
      //        //Контрагент – Контрагент1 Третья сторона – Зеркало НОР2 Наша орг. – НОР1
      //        if (thirdSide != null)
      //        {
      //          string nor3Code = thirdSide.ExternalCodeavis;
      //          string contractorIDNor1 = Convert.ToString(_obj.BusinessUnit.Company.Id);
      //          lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingContractsIntegrationHelper(contractRXID, contractID, nor3Code, contractorIDNor1, contractNo, date, modtime, subjContract, idMainContract, startDate,
      //                                                                                                       expDate, contrType, sum, taxGroup, currency, taxSum, isp, smr, author, region, fiasDistrictCode, nameDocumentTypeGroup,
      //                                                                                                       nameDocumentType, category, action, phys, contractorRXID, idMainContractRX, rx);
      //          lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingContractsIntegrationHelper(contractRXID, contractID, nor3Code, contractorID, contractNo, date, modtime, subjContract, idMainContract, startDate,
      //                                                                                                       expDate, contrType, sum, taxGroup, currency, taxSum, isp, smr, author, region, fiasDistrictCode, nameDocumentTypeGroup,
      //                                                                                                       nameDocumentType, category, action, phys, contractorRXID, idMainContractRX, rx);
      //        }
      //      }

      Logger.Debug($"Avis - UnloadingContracts. Выгрузка ДД с ИД {_obj.Id}. Завершение.");
      return true;
    }
    
    
    
    /// <summary>
    /// Блокируем поле сумма на вкладке свойство.
    /// При заполненной табличной части.
    /// </summary>
    [Public]
    public void BlockTotalAmmount()
    {
      if (lenspec.Etalon.Contracts.Is(_obj) && _obj.ConstructionObjectsavis.Any(x => x.Summ != null) ||
          lenspec.Etalon.SupAgreements.Is(_obj) && _obj.ConstructionObjectsavis.Any(x => x.MeasureSizeAmountavis != null))
      {
        _obj.State.Properties.TotalAmount.IsEnabled = false;
        return;
      }
      
      //      foreach (var constructionObject in _obj.ConstructionObjectsavis)
      //      {
      //        // Блокируем если в Объектах строительства есть хотя бы 1 сумма.
      //        if (constructionObject.MeasureSizeAmountavis != null && constructionObject.MeasureSizeAmountavis > 0 && lenspec.Etalon.SupAgreements.Is(_obj))
      //        {
      //          _obj.State.Properties.TotalAmount.IsEnabled = false;
      //          return;
      //        }
//
      //        // Блокируем если в Объектах строительства есть хотя бы 1 сумма.
      //        if (constructionObject.Summ != null && constructionObject.Summ > 0 && lenspec.Etalon.Contracts.Is(_obj))
      //        {
      //          _obj.State.Properties.TotalAmount.IsEnabled = false;
      //          return;
      //        }
      //      }
      
      // Разблокируем поля.
      //_obj.State.Properties.TotalAmount.IsEnabled = true;
    }
    
    /// <summary>
    /// Возвращает список Id подписывающих по критериям.
    /// </summary>
    /// <returns>Список тех, кто имеет право подписи.</returns>
    /// <remarks>Исключаются права подписи, выданные всем пользователям.</remarks>
    [Remote(IsPure = true)]
    public override List<long> GetSignatoriesIds()
    {
      return base.GetSignatoriesIds();
    }
    
    /// <summary>
    /// Проверить наличие права подписи со всеми сотрудниками.
    /// </summary>
    /// <returns>True - Если есть право подписи со всеми сотрудниками.</returns>
    [Remote(IsPure = true)]
    public override bool SignatorySettingWithAllUsersExist()
    {
      return base.SignatorySettingWithAllUsersExist();
    }
    
    /// <summary>
    /// Блокируем поля при изменении "Тип договора".
    /// </summary>
    [Public]
    public void EditContractType()
    {
      // Проверка что имя типа "Безвозмездный".
      if (_obj?.ContractTypeavis?.Name == lenspec.Etalon.ContractualDocuments.Resources.Gratuitous)
      {
        _obj.State.Properties.VatRate.IsEnabled = false;
        _obj.State.Properties.TotalAmount.IsEnabled = false;
        
        if (lenspec.Etalon.Contracts.Is(_obj))
          _obj.State.Properties.ConstructionObjectsavis.Properties.Summ.IsEnabled = false;
        
        if (lenspec.Etalon.SupAgreements.Is(_obj))
          _obj.State.Properties.ConstructionObjectsavis.Properties.MeasureSizeAmountavis.IsEnabled = false;
        
        EditNoNds();
        return;
      }
      
      _obj.State.Properties.VatRate.IsEnabled = true;
      _obj.State.Properties.TotalAmount.IsEnabled = true;
      _obj.State.Properties.ConstructionObjectsavis.Properties.Summ.IsEnabled = true;
      _obj.State.Properties.ConstructionObjectsavis.Properties.MeasureSizeAmountavis.IsEnabled = true;
      
      BlockTotalAmmount();
    }
    
    /// <summary>
    /// Устанавливает без НДС и сумму 0.
    /// </summary>
    [Public]
    public void EditNoNds()
    {
      // Изменяем НДС на "Без ндс" если он отличается от этого значения.
      if (_obj.VatRate == null || _obj.VatRate?.Sid != Sungero.Docflow.Constants.Module.VatRateWithoutVatSid)
        _obj.VatRate = Sungero.Commons.VatRates.GetAll(x => x.Sid == Sungero.Docflow.Constants.Module.VatRateWithoutVatSid).FirstOrDefault();
      
      if (_obj.TotalAmount == null || _obj.TotalAmount != 0)
        _obj.TotalAmount = 0;
      
      foreach (var constructionObject in _obj.ConstructionObjectsavis)
      {
        if (constructionObject.Summ == null || constructionObject.Summ > 0 && lenspec.Etalon.Contracts.Is(_obj))
          constructionObject.Summ = 0;
        
        if (constructionObject.MeasureSizeAmountavis == null || constructionObject.MeasureSizeAmountavis > 0 && lenspec.Etalon.SupAgreements.Is(_obj))
          constructionObject.MeasureSizeAmountavis = 0;
      }
    }
    
    /// <summary>
    /// Устанавливает без НДС и сумму 0.
    /// </summary>
    [Public]
    public void Edit0Nds()
    {
      // Изменяем НДС на "0%" если он отличается от этого значения.
      if (_obj.VatRate == null || _obj.VatRate?.Sid != Sungero.Docflow.Constants.Module.VatRateZeroPercentSid)
        _obj.VatRate = Sungero.Commons.VatRates.GetAll(x => x.Sid == Sungero.Docflow.Constants.Module.VatRateZeroPercentSid).FirstOrDefault();
      
      if (_obj.TotalAmount == null || _obj.TotalAmount != 0)
        _obj.TotalAmount = 0;
    }
    
    /// <summary>
    /// Делаем обязательным поле "ИСП" в зависимости от группы вида.
    /// </summary>
    [Public]
    public void IsRequiredPropertyOurCF()
    {
      if (_obj?.GroupContractTypeavis?.RequireOurCF == null || _obj?.GroupContractTypeavis?.RequireOurCF == avis.EtalonContracts.GroupContractType.RequireOurCF.No)
        _obj.State.Properties.OurCFavis.IsRequired = false;
      else
        _obj.State.Properties.OurCFavis.IsRequired = true;
    }
    
    /// <summary>
    /// Подсчитываем налог и выводим в свойство.
    /// </summary>
    [Public]
    public void CalculateNalog()
    {
      if (_obj.VatRate != null && _obj.VatRate.Rate.HasValue && _obj.VatRate.Sid != Sungero.Docflow.Constants.Module.VatRateWithoutVatSid)
        _obj.Nalogavis = _obj.TotalAmount * _obj.VatRate.Rate.Value / (100 + _obj.VatRate.Rate.Value);
      else
        _obj.Nalogavis = 0;
    }
  }
}