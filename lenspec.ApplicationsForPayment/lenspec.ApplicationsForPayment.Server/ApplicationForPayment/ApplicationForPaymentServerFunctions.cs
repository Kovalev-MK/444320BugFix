using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForPayment.ApplicationForPayment;

namespace lenspec.ApplicationsForPayment.Server
{
  partial class ApplicationForPaymentFunctions
  {
    
    /// <summary>
    /// Построить сводку по документу.
    /// </summary>
    /// <returns>Сводка по документу.</returns>
    public override StateView GetDocumentSummary()
    {
      const string none = "-";
      var documentSummary = StateView.Create();
      documentSummary.AddDefaultLabel(string.Empty);
      var block = documentSummary.AddBlock();
      
      var contract = lenspec.Etalon.Contracts.As(_obj.Contract);
      if (contract != null)
      {
        // Дней отсрочки по договору.
        var daysOfContractDeferment = contract.DaysOfDefermentlenspec != null ? contract.DaysOfDefermentlenspec.Value.ToString() : none;
        block.AddLabel(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.DaysOfContractDefermentFormat(daysOfContractDeferment));
      }
      
      return documentSummary;
    }

    /// <summary>
    /// Выгрузка документа в Integra.
    /// </summary>
    [Public]
    public bool UnloadingDocuments()
    {
      var businessUnit  = lenspec.Etalon.BusinessUnits.GetAll();
      //Соберем данные для выгрузки
      int action                        = 2;
      string stOrgCode                  = businessUnit.Where(x => x.Id == _obj.BusinessUnit.Id).FirstOrDefault().ExternalCodeavis;
      string contractorID               = "-";
      if(_obj.Counterparty != null)
      {
        var company = lenspec.Etalon.Companies.As(_obj.Counterparty);
        if (company != null)
        {
          if(company.ExternalCodeavis != null)
            contractorID = company.ExternalCodeavis;
        }
      }
      string requestNo                  = !string.IsNullOrEmpty(_obj.ReasonNumber) && !string.IsNullOrWhiteSpace(_obj.ReasonNumber)
        ? _obj.ReasonNumber
        : _obj.RegistrationNumber;
      DateTime? date                    = _obj.ReasonDate.HasValue
        ? _obj.ReasonDate
        : _obj.RegistrationDate;
      DateTime modTime                  = Calendar.Now;
      long? iDMainContract              = 0;
      if(_obj.Contract != null)
      {
        iDMainContract = lenspec.Etalon.OfficialDocuments.As(_obj.Contract).IdDirectumavis;
      }
      if(iDMainContract == null)
        iDMainContract = 0;
      decimal sum                       = Convert.ToDecimal(_obj.TotalAmount);
      decimal? taxGroup                 = 0;
      if(_obj.VatRate != null)
      {
        if(_obj.VatRate.Sid == Sungero.Docflow.Constants.Module.VatRateWithoutVatSid)
          taxGroup = -1;
        else if (_obj.VatRate.Sid == lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateMixedPercentSid)
          taxGroup = 20;
        else
          taxGroup = Convert.ToDecimal(_obj.VatRate.Rate);
      }
      string currency                   = _obj.Currency.NumericCode;
      decimal taxSum                    = Convert.ToDecimal(_obj.VatAmount);
      long payReqID                     = _obj.Id;
      string author                     = _obj.Author.Name;
      string recipientID                = "-";
      int prePay                        = 0;
      if (_obj.PaymentKind == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentKind.Advance)
        prePay = 1;
      if (_obj.PaymentKind == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentKind.NoAdvance)
        prePay = 2;
      if (_obj.PaymentKind == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentKind.AccountingApp)
        prePay = 3;
      string reqName                    = _obj.Name;
      string subject                    = _obj.SubjectPaymentPurpose;
      DateTime? planPayDate             = _obj.PlannedPaymentDate;
      int targetSpending                = 0;
      string exportActionTitle          = "-";
      int exportTaskID                  = 0;
      string region                     = "-";
      if(_obj.DirectorateRegion != null)
        region = _obj.DirectorateRegion.Name;
      string directorateDivision        = "-";
      if(_obj.Directorate != null)
        directorateDivision = _obj.Directorate.Code;
      if(directorateDivision == null)
        directorateDivision = "-";
      string departmentDivision         = "-";
      if(_obj.Department != null)
        departmentDivision = _obj.Department.Code;
      if(departmentDivision == null)
        departmentDivision = "-";
      int requestType                   = 0;
      if (_obj.Category == lenspec.ApplicationsForPayment.ApplicationForPayment.Category.Standard)
        requestType = 1;
      if (_obj.Category == lenspec.ApplicationsForPayment.ApplicationForPayment.Category.ForBailiffs)
        requestType = 2;
      if (_obj.Category == lenspec.ApplicationsForPayment.ApplicationForPayment.Category.ForClients)
        requestType = 3;
      DateTime? dateActualPayment       = new DateTime(1900, 1, 1);
      string paymentStatus              = "-";
      decimal actualPaymentAmount       = 0;
      string decodingCashItems          = "-";
      if(_obj.DecodingBudgetItem != null)
        decodingCashItems = _obj.DecodingBudgetItem.Code1C;
      string caseNumber                 = _obj.CaseNumber;
      if(caseNumber == null)
        caseNumber = "-";
      string basisPayment               = _obj.SubjectPaymentPurpose;
      string customerAgreementNumber    = "-";
      string resolutionNumber           = _obj.ResolutionNumber;
      if(resolutionNumber == null)
        resolutionNumber = "-";
      string win                        = _obj.UIN;
      if(win == null)
        win = "-";
      string contractorINN              = "-";
      string contractorKPP              = "-";
      string recipientINN               = "-";
      string recipientKPP               = "-";
      string rx                         = "TRUE";
      string note_Text                  = "-";
      DateTime? earlyPayment_Date       = new DateTime(1900, 1, 1);
      DateTime? contractPayment_Date    = new DateTime(1900, 1, 1);
      decimal totalAmount_WithDiscount  = 0;
      long contractorRXID               = _obj.Counterparty.Id;
      string recipient                  = "-";
      long recipientRXID                = 0;
      if(_obj.ThirdSide != null)
      {
        recipientRXID = _obj.ThirdSide.Id;
        if (lenspec.Etalon.Companies.Is(_obj.ThirdSide) == true)
        {
          var thirdSide   = lenspec.Etalon.Companies.GetAll().Where(x => x.Id == _obj.ThirdSide.Id).FirstOrDefault();
          recipient  = thirdSide.ExternalCodeavis;
          if(recipient == null)
            recipient = "-";
        }
      }
      string contractorRS               = _obj.CounterpartyBankDetail.BankAccount;
      string contractorBIK              = _obj.CounterpartyBankDetail.BIC;
      string contractorKS               = _obj.CounterpartyBankDetail.CorrespondentAccount;
      string contractorBank             = _obj.CounterpartyBankDetail.Bank.LegalName;
      string recipientRS                = "-";
      string recipientBIK               = "-";
      string recipientKS                = "-";
      string recipientBank              = "-";
      if(_obj.ThirdSideBankDetail != null)
      {
        recipientRS                = _obj.ThirdSideBankDetail.BankAccount;
        recipientBIK               = _obj.ThirdSideBankDetail.BIC;
        recipientKS                = _obj.ThirdSideBankDetail.CorrespondentAccount;
        recipientBank              = _obj.ThirdSideBankDetail.Bank.LegalName;
      }
      long iDMainContractRX             = payReqID;
      if(_obj.Contract != null)
        iDMainContractRX             = _obj.Contract.Id;
      string departmentRegion           = "-";
      if(_obj.DepartmentRegion != null)
        departmentRegion = _obj.DepartmentRegion.Name;
      string fam                        = "-";
      string name                       = "-";
      string otch                       = "-";
      string clientINN                  = "-";
      string famTP                      = "-";
      string nameTP                     = "-";
      string otchTP                     = "-";
      string clientINNTP                = "-";
      if (lenspec.Etalon.People.Is(_obj.Counterparty) == true)
      {
        var people    = lenspec.Etalon.People.GetAll().Where(x => x.Id == _obj.Counterparty.Id).FirstOrDefault();
        fam           = people.LastName;
        name          = people.FirstName;
        if(!string.IsNullOrEmpty(people.MiddleName) && !string.IsNullOrWhiteSpace(people.MiddleName))
          otch          = people.MiddleName;
        clientINN     = people.TIN;
      }
      if (lenspec.Etalon.People.Is(_obj.ThirdSide) == true)
      {
        var peopleTP    = lenspec.Etalon.People.GetAll().Where(x => x.Id == _obj.ThirdSide.Id).FirstOrDefault();
        famTP           = peopleTP.LastName;
        nameTP          = peopleTP.FirstName;
        if(!string.IsNullOrEmpty(peopleTP.MiddleName) && !string.IsNullOrWhiteSpace(peopleTP.MiddleName))
          otchTP          = peopleTP.MiddleName;
        clientINNTP     = peopleTP.TIN;
      }
      
      //Выгрузим данные в PaymentRequest
      lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingPaymentRequest(action, stOrgCode, contractorID, requestNo, date, modTime,
                                                                                       iDMainContract, sum, taxGroup, currency, taxSum, payReqID, author, recipient,
                                                                                       prePay, reqName, subject, planPayDate, targetSpending, exportActionTitle, exportTaskID,
                                                                                       region, directorateDivision, departmentDivision, requestType, dateActualPayment, paymentStatus,
                                                                                       actualPaymentAmount, decodingCashItems, caseNumber, basisPayment, customerAgreementNumber,
                                                                                       resolutionNumber, win, contractorINN, contractorKPP, recipientINN, recipientKPP, rx,
                                                                                       note_Text, earlyPayment_Date, contractPayment_Date, totalAmount_WithDiscount, contractorRXID,
                                                                                       recipientID, recipientRXID, contractorRS, contractorBIK, contractorKS, contractorBank,
                                                                                       recipientRS, recipientBIK, recipientKS, recipientBank, iDMainContractRX, departmentRegion,
                                                                                       fam, name, otch, clientINN, famTP, nameTP, otchTP, clientINNTP);
      //Если есть ОС то выгрузи
      int headerID        = lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingPaymentRequestDetailHeaderID(payReqID);
      string kbk          = _obj.BudgetItem.Code1C;
      if(kbk == null)
        kbk = "-";
      foreach(var os in _obj.ObjectAnProjects)
      {
        string contrSites = "-";
        if(os.ObjectAnProject != null)
        {
          contrSites = os.ObjectAnProject.Code1c;
          if(contrSites == null)
            contrSites = "-";
        }
        string workType = "-";
        if(os.DetailingWorkType != null)
        {
          workType = os.DetailingWorkType.Code1c;
          if(workType == null)
            workType = "-";
        }
        long numStr         = os.Number ?? default(int);
        decimal sumOS       = 0;
        if(os.Amount != null)
          sumOS = Convert.ToDecimal(os.Amount);
        decimal taxSumOS    = 0;
        if(os.VatAmount != null)
          taxSumOS = Convert.ToDecimal(os.VatAmount);
        decimal taxGroupOS = taxGroup ?? default(decimal);
        if(os.VatRate != null)
        {
          if (os.VatRate.Sid == Sungero.Docflow.Constants.Module.VatRateWithoutVatSid)
            taxGroupOS = -1;
          else if (os.VatRate.Sid == lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateMixedPercentSid)
            taxGroupOS = 20;
          else
            taxGroupOS = Convert.ToDecimal(os.VatRate.Rate);
        }
        lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingPaymentRequestDetail(contrSites, workType, payReqID, stOrgCode, sumOS, modTime, taxGroupOS, taxSumOS, headerID, kbk, numStr, rx);
      }
      //Если ос нет, то все равно выгружаем табличную часть с кастомными значениями
      if(_obj.ObjectAnProjects.Count == 0)
      {
        string contrSites   = "Ц00000963";
        string workType     = "0";
        long numStr         = 0;
        decimal sumOS       = sum;
        decimal taxSumOS    = taxSum;
        decimal taxGroupOS  = taxGroup ?? default(decimal);
        lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingPaymentRequestDetail(contrSites, workType, payReqID, stOrgCode, sumOS, modTime, taxGroupOS, taxSumOS, headerID, kbk, numStr, rx);
      }
      
      //Если нет договора или категория судебным приставам
      if(_obj.Contract == null && _obj.Category != lenspec.ApplicationsForPayment.ApplicationForPayment.Category.ForClients)
      {
        long contractRXID             = payReqID;
        long contractID               = 0;
        string contractNo             = !string.IsNullOrEmpty(_obj.ReasonNumber) && !string.IsNullOrWhiteSpace(_obj.ReasonNumber)
          ? _obj.ReasonNumber
          : _obj.RegistrationNumber;
        string subjContract           = _obj.SubjectPaymentPurpose;
        long idMainContract           = 0;
        DateTime? startDate           = _obj.ReasonDate.HasValue
          ? _obj.ReasonDate
          : _obj.RegistrationDate;
        DateTime? expDate             = new DateTime(1900, 1, 1);
        decimal taxGroupC             = taxGroup ?? default(decimal);
        int? contrType                = 2;
        int isp                       = 0;
        int smr                       = 0;
        string fiasDistrictCode       = "-";
        string nameDocumentTypeGroup  = "-";
        string nameDocumentType       = "-";
        string category               = "-";
        if (_obj.Category != lenspec.ApplicationsForPayment.ApplicationForPayment.Category.ForBailiffs)
        {
          if(_obj.PaymentType == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.NoContract)
            category = "Договор на сумму менее 500 000 руб.";
        }
        int phys                      = 0;
        lenspec.Etalon.Module.Integration.PublicFunctions.Module.UnloadingPaymentRequestContract(contractRXID, contractID, stOrgCode, contractorID,
                                                                                                 contractNo, date, modTime, subjContract, idMainContract, startDate,
                                                                                                 expDate, contrType, sum, taxGroupC, currency, taxSum, isp, smr,
                                                                                                 author, region, fiasDistrictCode, nameDocumentTypeGroup, nameDocumentType,
                                                                                                 category, action, phys, rx, contractorRXID);
      }
      return true;
    }


    /// <summary>
    /// Изменение статуса синхронизации документа
    /// </summary>
    [Public]
    public void UpdateDocumentSyns()
    {
      var asyncExportApplicationForPayment = lenspec.ApplicationsForPayment.AsyncHandlers.AsyncExportApplicationForPayment.Create();
      asyncExportApplicationForPayment.DocumentId = _obj.Id;
      asyncExportApplicationForPayment.ExecuteAsync();
    }


    /// <summary>
    /// Получить дубли.
    /// </summary>
    /// <returns>Записи с совпадающими полями.</returns>
    [Remote(IsPure = true)]
    public IQueryable<lenspec.ApplicationsForPayment.IApplicationForPayment> GetDuplicates()
    {
      var applications = lenspec.ApplicationsForPayment.ApplicationForPayments.GetAll();
      
      applications = applications.Where(x => !Equals(_obj, x) && x.LifeCycleState != lenspec.ApplicationsForPayment.ApplicationForPayment.LifeCycleState.Obsolete &&
                                        _obj.BusinessUnit == x.BusinessUnit && _obj.Counterparty == x.Counterparty && (!_obj.TotalAmount.HasValue || _obj.TotalAmount == x.TotalAmount));
      
      applications = applications.Where(x => (_obj.IncomingInvoice != null && _obj.IncomingInvoice == x.IncomingInvoice) || (_obj.Memo != null && _obj.Memo == x.Memo) ||
                                        (_obj.SimpleDocument != null && _obj.SimpleDocument == x.SimpleDocument) || (_obj.IncomingLetter != null && _obj.IncomingLetter == x.IncomingLetter) ||
                                        (_obj.CustomerRequest != null && _obj.CustomerRequest == x.CustomerRequest));
      
      var applicationDupplicatesByCollectionDocuments = new List<lenspec.ApplicationsForPayment.IApplicationForPayment>();
      
      foreach (var line in _obj.ContractStatements)
        applicationDupplicatesByCollectionDocuments.AddRange(applications.Where(x => x.ContractStatements.Any(cs => Equals(cs.ContractStatement, line.ContractStatement))).ToList());
      foreach (var line in _obj.Waybills)
        applicationDupplicatesByCollectionDocuments.AddRange(applications.Where(x => x.Waybills.Any(w => Equals(w.Waybill, line.Waybill))).ToList());
      foreach (var line in _obj.UTDs)
        applicationDupplicatesByCollectionDocuments.AddRange(applications.Where(x => x.UTDs.Any(u => Equals(u.UTD, line.UTD))).ToList());
      
      
      return applicationDupplicatesByCollectionDocuments.AsQueryable();
    }
  }
}
