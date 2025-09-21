using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForPayment.ApplicationForPayment;

namespace lenspec.ApplicationsForPayment.Shared
{
  partial class ApplicationForPaymentFunctions
  {
    
    /// <summary>
    /// Для ЗНО выгрузить в 1С КА, Третью сторону и Договор.
    /// </summary>
    /// <returns>Сообщение об ошибке.</returns>
    public string UnloadMissingDataByCounterparty()
    {
      var errors = new List<string>();
      
      var counterparty = lenspec.Etalon.Counterparties.As(_obj.Counterparty);
      if (counterparty != null && counterparty.Export1CStatelenspec != lenspec.Etalon.Counterparty.Export1CStatelenspec.Yes)
      {
        var errorMessage = lenspec.Etalon.Module.Parties.PublicFunctions.Module.UnloadCounterpartyInto1C(counterparty);
        if (!string.IsNullOrEmpty(errorMessage))
          errors.Add(errorMessage);
      }
      
      var thirdSide = lenspec.Etalon.Counterparties.As(_obj.ThirdSide);
      if (thirdSide != null && thirdSide.Export1CStatelenspec != lenspec.Etalon.Counterparty.Export1CStatelenspec.Yes)
      {
        var errorMessage = lenspec.Etalon.Module.Parties.PublicFunctions.Module.UnloadCounterpartyInto1C(thirdSide);
        if (!string.IsNullOrEmpty(errorMessage))
          errors.Add(errorMessage);
      }
      
      var contract = lenspec.Etalon.Contracts.As(_obj.Contract);
      if (contract != null && contract.SyncStatus1cavis != lenspec.Etalon.Contract.SyncStatus1cavis.Sync)
      {
        try
        {
          if (lenspec.Etalon.PublicFunctions.ContractualDocument.UnloadingContracts(contract))
          {
            if (contract.ConstructionObjectsavis.Any() && !lenspec.Etalon.PublicFunctions.ContractualDocument.UnloadingContractsDetails(contract))
              errors.Add(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ErrorMessageContractAutoExport1C);
            else
              lenspec.Etalon.PublicFunctions.ContractualDocument.UpdateContractSyns(contract);
          }
          else
            errors.Add(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ErrorMessageContractAutoExport1C);
        }
        catch(Exception ex)
        {
          Logger.ErrorFormat("UnloadMissingDataByCounterparty - заявка на оплату {0} - ", ex, _obj.Id);
          errors.Add(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ErrorMessageContractAutoExport1C);
        }
      }
      
      return errors.Any() ? string.Join(Environment.NewLine, errors) : null;
    }
    
    /// <summary>
    /// Проверить наличие обязательных полей КА.
    /// </summary>
    /// <returns>Сообщение об ошибке.</returns>
    public string CheckCounterpartyFields(Sungero.Parties.ICounterparty counterparty)
    {
      var errorMessage = string.Empty;
      if (counterparty == null)
        return errorMessage;
      
      var person = lenspec.Etalon.People.As(counterparty);
      var company = lenspec.Etalon.Companies.As(counterparty);
      var bank = lenspec.Etalon.Banks.As(counterparty);
      
      if (person != null)
        errorMessage = lenspec.Etalon.PublicFunctions.Person.CheckRequiredFieldsForExport(person);
      else if (company != null)
        errorMessage = lenspec.Etalon.PublicFunctions.Company.CheckRequiredFieldsForExport(company);
      else if (bank != null)
        errorMessage = lenspec.Etalon.PublicFunctions.Bank.CheckRequiredFieldsForExport(bank);
      
      return errorMessage;
    }
    
    /// <summary>
    /// Очистить Номер и Дату основания.
    /// </summary>
    public void FillReasonNumberAndDate()
    {
      if (_obj.Contract == null && _obj.IncomingInvoice == null && _obj.Memo == null)
      {
        _obj.ReasonNumber = null;
        _obj.ReasonDate = null;
      }
      else if (_obj.IncomingInvoice != null)
      {
        _obj.ReasonNumber = _obj.IncomingInvoice.Number;
        _obj.ReasonDate = _obj.IncomingInvoice.Date;
      }
      else if (_obj.Memo != null)
      {
        _obj.ReasonNumber = _obj.Memo.RegistrationNumber;
        _obj.ReasonDate = _obj.Memo.RegistrationDate;
      }
      else if (_obj.Contract != null)
      {
        var contract = lenspec.Etalon.Contracts.As(_obj.Contract);
        if (contract != null)
        {
          _obj.ReasonNumber = contract.RegistrationNumber;
          _obj.ReasonDate = contract.RegistrationDate;
        }
        
        var clientContract = lenspec.SalesDepartmentArchive.SDAClientContracts.As(_obj.Contract);
        if (clientContract != null)
        {
          _obj.ReasonNumber = clientContract.ClientDocumentNumber;
          _obj.ReasonDate = clientContract.ClientDocumentDate;
        }
      }
    }
    
    /// <summary>
    /// Проверить Общую сумму.
    /// </summary>
    /// <returns>Сообщение с ошибкой.</returns>
    public string CheckTotalAmount()
    {
      var error = string.Empty;
      if (_obj.TotalAmount != null && _obj.TotalAmount.Value == 0 &&
          CallContext.CalledFrom(lenspec.ApplicationsForPayment.ApplicationForPayments.Info))
        error = lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.IncorrectTotalAmountErrorMessage;
      
      return error;
    }
    
    /// <summary>
    /// Проверить что поле Договор имеет тип Contract и признак ИСП = да
    /// </summary>
    /// <returns>Если Тип = договор и ИСП = да, то True. Иначе False</returns>
    public bool IsIBO()
    {
      var contract = lenspec.Etalon.Contracts.As(_obj.Contract);
      return contract != null && contract.IsICPlenspec.HasValue && contract.IsICPlenspec.Value;
    }
    
    /// <summary>
    /// Проверить согласованность контрагента-третьей стороны.
    /// </summary>
    /// <returns>Сообщение с ошибкой, если не согласован по какой-то причинею</returns>
    public string CheckApprovingThirdSide()
    {
      var notAssessedError = string.Empty;
      var thirdSide = lenspec.Etalon.Counterparties.As(_obj.ThirdSide);
      if (thirdSide != null)
      {
        if (thirdSide.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.NotAssessed)
          notAssessedError = lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.NotAssessedThirdSideMessage;
        if (thirdSide.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopNotRecomend)
          notAssessedError = lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.BadThirdSideErrorMessage;
      }
      return notAssessedError;
    }
    
    /// <summary>
    /// Обновить статус согласования документа.
    /// </summary>
    /// <param name="newState">Новый статус.</param>
    /// <param name="taskId">ИД задачи на согласование или null, если задача не указана.</param>
    [Public]
    public override void UpdateDocumentApprovalState(Enumeration? newState, long? taskId)
    {
      if (_obj.InternalApprovalState != lenspec.ApplicationsForPayment.ApplicationForPayment.InternalApprovalState.Approved)
        base.UpdateDocumentApprovalState(newState, taskId);
    }
    
    /// <summary>
    /// Проверить статус согласования договора.
    /// </summary>
    /// <returns>Сообщение с ошибкой, если документ не утвержден.</returns>
    public string CheckContractInternalApprovalState()
    {
      var notSignedError = string.Empty;
      var document = _obj.Contract;
      var isContract = Sungero.Contracts.Contracts.Is(document);
      if (document != null && isContract && document.InternalApprovalState != Sungero.Docflow.OfficialDocument.InternalApprovalState.Signed)
      {
        notSignedError = lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ContractIsNotSigned;
      }
      return notSignedError;
    }
    
    /// <summary>
    /// Очистить все поля из группы ДОКУМЕНТ-ОСНОВАНИЕ.
    /// </summary>
    public void ClearBasisDocument()
    {
      _obj.Contract = null;
      _obj.IncomingInvoice = null;
      _obj.ContractStatements.Clear();
      _obj.Waybills.Clear();
      _obj.UTDs.Clear();
      _obj.IncomingLetter = null;
      _obj.Memo = null;
      _obj.SimpleDocument = null;
      _obj.CustomerRequest = null;
      _obj.CaseNumber = null;
      _obj.ResolutionNumber = null;
      _obj.UIN = null;
    }
    
    /// <summary>
    /// Заполнить поля группы Документ-основание
    /// </summary>
    public void FillBasisDocument()
    {
      var castedIncomingInvoice = lenspec.Etalon.IncomingInvoices.As(_obj.IncomingInvoice);
      if (castedIncomingInvoice != null && castedIncomingInvoice.SetLenspeclenspec.Any())
      {
        var contractStatementKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Sungero.FinancialArchive.PublicConstants.Module.Initialize.ContractStatementKind);
        var waybillDocumentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Sungero.FinancialArchive.PublicConstants.Module.Initialize.WaybillDocumentKind);
        
        foreach (var line in castedIncomingInvoice.SetLenspeclenspec)
        {
          if (line != null && line.Document != null)
          {
            if (Sungero.FinancialArchive.UniversalTransferDocuments.Is(line.Document))
            {
              var newField = _obj.UTDs.AddNew();
              newField.UTD = Sungero.FinancialArchive.UniversalTransferDocuments.As(line.Document);
            }
            else if (Equals(line.Document.DocumentKind, contractStatementKind))
            {
              var newField = _obj.ContractStatements.AddNew();
              newField.ContractStatement = Sungero.FinancialArchive.ContractStatements.As(line.Document);
            }
            else if (Equals(line.Document.DocumentKind, waybillDocumentKind))
            {
              var newField = _obj.Waybills.AddNew();
              newField.Waybill = Sungero.FinancialArchive.Waybills.As(line.Document);
            }
          }
        }
      }
    }
    
    /// <summary>
    /// Проверить согласованность контрагента
    /// </summary>
    /// <returns>Сообщение с ошибкой, если не согласован по какой-то причине</returns>
    public string CheckApprovingCounterparty()
    {
      var notAssessedError = string.Empty;
      var counterparty = lenspec.Etalon.Counterparties.As(_obj.Counterparty);
      if (counterparty != null)
      {
        if (counterparty.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.NotAssessed)
          notAssessedError = lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.NotAssessedCounterpartyMessage;
        if (counterparty.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopNotRecomend)
          notAssessedError = lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.BadCounterpartyErrorMessage;
      }
      return notAssessedError;
    }

    /// <summary>
    /// Заполнить тип платежа
    /// </summary>
    public void SetPaymentType()
    {
      if (_obj.Category == lenspec.ApplicationsForPayment.ApplicationForPayment.Category.ForBailiffs)
        _obj.PaymentType = lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Sanction;
      else
        _obj.PaymentType = null;
    }
    
    /// <summary>
    /// Получить документы, связанные типом связи "Приложение".
    /// </summary>
    /// <returns>Документы, связанные типом связи "Приложение".</returns>
    [Public]
    public override List<Sungero.Docflow.IOfficialDocument> GetAddenda()
    {
      var addenda = base.GetAddenda();
      var relatedDocuments = _obj.Relations.GetRelatedAndRelatedFromDocuments()
        .Select(x => Sungero.Docflow.OfficialDocuments.As(x))
        .Where(x => x != null)
        .ToList();
      addenda.AddRange(relatedDocuments);
      return addenda;
    }
    
    /// <summary>
    /// Проверить дубли.
    /// </summary>
    /// <returns>True, если дубликаты имеются, иначе - false.</returns>
    public bool HaveDuplicates()
    {
      return _obj.LifeCycleState != lenspec.ApplicationsForPayment.ApplicationForPayment.LifeCycleState.Obsolete &&
        Functions.ApplicationForPayment.Remote.GetDuplicates(_obj).Any();
    }

    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var documentKind = _obj.DocumentKind;
      
      if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      
      if (documentKind == null || !documentKind.GenerateDocumentName.Value)
        return;
      
      var name = string.Empty;
      
      /* Имя в формате:
        <Вид документа> на сумму <Общая сумма> <Сокращенное наименование валюты> между <Контрагент> и <Наша орг.> по региону: <Регион дирекции>, Тип платежа: <Тип платежа>
       */
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.TotalAmount.HasValue && _obj.Currency != null)
          name += lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.AmountNamePartFormat(Math.Round(_obj.TotalAmount.Value, 2).ToString(), _obj.Currency.ShortName);
        
        if (_obj.Counterparty != null && _obj.BusinessUnit != null)
          name += lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.BetweenNamePartFormat(_obj.Counterparty.Name, _obj.BusinessUnit.Name);
        
        if (_obj.DirectorateRegion != null)
          name += lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.RegionNamePart + _obj.DirectorateRegion.Name;
        
        if (_obj.PaymentType != null)
          name += lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.PaymentTypeNamePart + ApplicationForPayments.Info.Properties.PaymentType.GetLocalizedValue(_obj.PaymentType);
      }
      
      if (string.IsNullOrWhiteSpace(name))
      {
        if (_obj.VerificationState == null)
          name = Sungero.Docflow.Resources.DocumentNameAutotext;
        else
          name = _obj.DocumentKind.ShortName;
      }
      else if (documentKind != null)
      {
        name = documentKind.ShortName + name;
      }
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      if (name.Length > _obj.Info.Properties.Name.Length)
        _obj.Name = name.Substring(0, _obj.Info.Properties.Name.Length);
      else
        _obj.Name = name;
    }
    
    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      _obj.State.Properties.Subject.IsRequired = false;
    }
    
    /// <summary>
    /// Сменить доступность реквизитов документа.
    /// </summary>
    /// <param name="isEnabled">True, если свойства должны быть доступны.</param>
    /// <param name="isRepeatRegister">Перерегистрация.</param>
    public override void ChangeDocumentPropertiesAccess(bool isEnabled, bool isRepeatRegister)
    {
      base.ChangeDocumentPropertiesAccess(isEnabled, isRepeatRegister);
      
      ChangeApplicationForPaymentPropertiesAccess();
    }
    
    /// <summary>
    /// Изменить отображение и доступность свойств заявки.
    /// </summary>
    public virtual void ChangeApplicationForPaymentPropertiesAccess()
    {
      var isNoContract = _obj.PaymentType == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.NoContract;
      var isRefunds = _obj.PaymentType == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Refunds;
      var isPrepayment = _obj.PaymentType == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Prepayment;
      var isPostpay = _obj.PaymentType == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Postpay;
      var isRegulatory = _obj.PaymentType == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Regulatory;
      var isSanction = _obj.PaymentType == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Sanction;
      var isTransitPayment = _obj.PaymentType == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.TransitPayment;
      var isSubsidyDownPaym = _obj.PaymentType == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.SubsidyDownPaym;
      var isCategoryForClients = _obj.Category == lenspec.ApplicationsForPayment.ApplicationForPayment.Category.ForClients;
      
      var contract = lenspec.Etalon.Contracts.As(_obj.Contract);
      var isICP = contract != null && contract.IsICPlenspec.HasValue && contract.IsICPlenspec.Value;
      
      // Видимость.
      _obj.State.Properties.IncomingLetter.IsVisible = isRefunds || isSanction;
      _obj.State.Properties.CustomerRequest.IsVisible =  isRefunds || isTransitPayment || isSubsidyDownPaym;
      _obj.State.Properties.Memo.IsVisible =  isRefunds || isSanction || isRegulatory;
      _obj.State.Properties.Contract.IsVisible =  isRefunds || isPrepayment || isPostpay || isTransitPayment || isSubsidyDownPaym;
      _obj.State.Properties.IncomingInvoice.IsVisible = !isRefunds && !isRegulatory && !isSanction && !isTransitPayment && !isSubsidyDownPaym;
      _obj.State.Properties.ContractStatements.IsVisible = isPostpay;
      _obj.State.Properties.UTDs.IsVisible = isPostpay;
      _obj.State.Properties.Waybills.IsVisible = isPostpay;
      _obj.State.Properties.ThirdSide.IsVisible = !isNoContract;
      _obj.State.Properties.SimpleDocument.IsVisible = isRegulatory || isTransitPayment || isSubsidyDownPaym;
      _obj.State.Properties.ThirdSideBankDetail.IsVisible = _obj.ThirdSide != null;
      _obj.State.Properties.CaseNumber.IsVisible = isSanction;
      _obj.State.Properties.ResolutionNumber.IsVisible = isSanction;
      _obj.State.Properties.UIN.IsVisible = isSanction;
      _obj.State.Properties.NoAccount.IsVisible = isPostpay || isPrepayment;
      
      var isReasonDocument = _obj.State.Properties.Contract.IsVisible || _obj.State.Properties.IncomingInvoice.IsVisible || _obj.State.Properties.Memo.IsVisible;
      _obj.State.Properties.ReasonNumber.IsVisible = isReasonDocument;
      _obj.State.Properties.ReasonDate.IsVisible = isReasonDocument;
      
      // Доступность.
      _obj.State.Properties.Department.IsEnabled = false;
      _obj.State.Properties.ObjectAnProjects.Properties.DetailingWorkType.IsEnabled = lenspec.SalesDepartmentArchive.SDAClientContracts.As(_obj.Contract) == null;
      _obj.State.Properties.CounterpartyBankDetail.IsEnabled = _obj.Counterparty != null;
      _obj.State.Properties.TotalAmount.IsEnabled = !_obj.ObjectAnProjects.Any(x => x != null && x.Amount != null);
      _obj.State.Properties.ObjectAnProjects.IsEnabled = !isCategoryForClients;
      _obj.State.Properties.VatAmount.IsEnabled = _obj.VatRate == null || string.IsNullOrWhiteSpace(_obj.VatRate.Sid) ||
        (_obj.VatRate.Sid != lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateWithoutVatSid && _obj.VatRate.Sid != lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateZeroPercentSid);
      
      // Обязательность.
      _obj.State.Properties.IncomingInvoice.IsRequired = isNoContract || ((isPrepayment || isPostpay) && _obj.NoAccount != true);
      _obj.State.Properties.Contract.IsRequired = isPrepayment || isPostpay || isTransitPayment || isSubsidyDownPaym;
      _obj.State.Properties.CaseNumber.IsRequired = isSanction;
      _obj.State.Properties.ResolutionNumber.IsRequired = isSanction;
      _obj.State.Properties.UIN.IsRequired = isSanction;
      _obj.State.Properties.IncomingLetter.IsRequired = isSanction;
      _obj.State.Properties.Memo.IsRequired = isSanction;
      _obj.State.Properties.ReasonNumber.IsRequired = _obj.ReasonDate.HasValue;
      _obj.State.Properties.ReasonDate.IsRequired = !string.IsNullOrEmpty(_obj.ReasonNumber) && !string.IsNullOrWhiteSpace(_obj.ReasonNumber);
      _obj.State.Properties.ThirdSideBankDetail.IsRequired = _obj.ThirdSide != null;
      // Таблю часть как свойство обязательно только для ИСП, а столбцы - всегда.
      _obj.State.Properties.ObjectAnProjects.IsRequired = isICP && !isCategoryForClients;
      _obj.State.Properties.ObjectAnProjects.Properties.ObjectAnProject.IsRequired = true;
      _obj.State.Properties.ObjectAnProjects.Properties.DetailingWorkType.IsRequired = isICP;
      _obj.State.Properties.ObjectAnProjects.Properties.Amount.IsRequired = true;
      _obj.State.Properties.ObjectAnProjects.Properties.VatAmount.IsRequired = true;
      _obj.State.Properties.ObjectAnProjects.Properties.VatRate.IsRequired = true;
    }
  }
}