using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForPayment.ApplicationForPayment;

namespace lenspec.ApplicationsForPayment
{
  partial class ApplicationForPaymentObjectAnProjectsSharedHandlers
  {

    public virtual void ObjectAnProjectsVatRateChanged(lenspec.ApplicationsForPayment.Shared.ApplicationForPaymentObjectAnProjectsVatRateChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;

      Functions.ApplicationForPayment.ChangeApplicationForPaymentPropertiesAccess(_obj.ApplicationForPayment);
      if (e.NewValue == null || e.NewValue.Sid == lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateMixedPercentSid)
        _obj.VatAmount = null;
      else if (e.NewValue.Sid == Sungero.Docflow.Constants.Module.VatRateWithoutVatSid)
        _obj.VatAmount = 0;
      else if (_obj.Amount.HasValue)
        _obj.VatAmount = Sungero.Commons.PublicFunctions.Module.GetVatAmountFromTotal(_obj.Amount.Value, e.NewValue);
    }

    public virtual void ObjectAnProjectsAmountChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
        _obj.VatAmount = null;
      
      if (e.NewValue.HasValue && _obj.VatRate != null && _obj.VatRate.Sid != lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateMixedPercentSid)
        _obj.VatAmount = Sungero.Commons.PublicFunctions.Module.GetVatAmountFromTotal(e.NewValue.Value, _obj.VatRate);
    }
  }

  partial class ApplicationForPaymentObjectAnProjectsSharedCollectionHandlers
  {

    public virtual void ObjectAnProjectsDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextItems = _obj.ObjectAnProjects.Where(x => x.Id > _deleted.Id);
      foreach(var item in nextItems)
      {
        item.Number = Convert.ToInt32(item.Number) - 1;
      }
    }

    public virtual void ObjectAnProjectsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = (_obj.ObjectAnProjects.Max(a => a.Number) ?? 0) + 1;
      
      if (_added.VatAmount == null)
        _added.VatAmount = 0;
      
      if (_added.Amount == null)
        _added.Amount = 0;
      
      if (_obj.VatRate != null && _obj.VatRate.Sid != lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateMixedPercentSid)
        _added.VatRate = _obj.VatRate;
    }
  }

  partial class ApplicationForPaymentSharedHandlers
  {

    public virtual void ReasonDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      Functions.ApplicationForPayment.ChangeApplicationForPaymentPropertiesAccess(_obj);
    }

    public virtual void ReasonNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      Functions.ApplicationForPayment.ChangeApplicationForPaymentPropertiesAccess(_obj);
    }

    public override void VatRateChanged(Sungero.Docflow.Shared.ContractualDocumentBaseVatRateChangedEventArgs e)
    {
      base.VatRateChanged(e);
      
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      Functions.ApplicationForPayment.ChangeApplicationForPaymentPropertiesAccess(_obj);
      
      if (!_obj.ObjectAnProjects.Any())
      {
        Sungero.Docflow.PublicFunctions.ContractualDocumentBase.FillVatAmount(_obj, _obj.TotalAmount, e.NewValue);
        Sungero.Docflow.PublicFunctions.ContractualDocumentBase.FillNetAmount(_obj, _obj.TotalAmount, _obj.VatAmount);
      }
      else if (e.NewValue != null && e.NewValue.Sid != lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateMixedPercentSid)
      {
        foreach (var objectAnProject in _obj.ObjectAnProjects)
        {
          objectAnProject.VatRate = e.NewValue;
        }
      }
    }

    public virtual void ObjectAnProjectsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      double totalAmount = 0;
      double vatAmount = 0;
      foreach (var line in _obj.ObjectAnProjects)
      {
        if (line != null && line.Amount.HasValue)
          totalAmount += line.Amount.Value;
        
        if (line != null && line.VatAmount.HasValue)
          vatAmount += line.VatAmount.Value;
      }
      _obj.TotalAmount = _obj.ObjectAnProjects.Any(x => x.Amount != null) ? Math.Round(totalAmount, 2) : _obj.TotalAmount;
      _obj.VatAmount = _obj.ObjectAnProjects.Any(x => x.VatAmount != null) ? Math.Round(vatAmount, 2) : _obj.VatAmount;
    }

    public virtual void UTDsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      foreach (var line in _obj.UTDs)
      {
        if (line != null && line.UTD != null)
          _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, line.State.Properties.UTD.PreviousValue, line.UTD);
      }
    }

    public virtual void WaybillsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      foreach (var line in _obj.Waybills)
      {
        if (line != null && line.Waybill != null)
          _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, line.State.Properties.Waybill.PreviousValue, line.Waybill);
      }
    }

    public virtual void ContractStatementsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      foreach (var line in _obj.ContractStatements)
      {
        if (line != null && line.ContractStatement != null)
          _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, line.State.Properties.ContractStatement.PreviousValue, line.ContractStatement);
      }
    }

    public virtual void CategoryChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (e.NewValue == lenspec.ApplicationsForPayment.ApplicationForPayment.Category.ForClients)
        _obj.ObjectAnProjects.Clear();
      
      Functions.ApplicationForPayment.SetPaymentType(_obj);
      Functions.ApplicationForPayment.ClearBasisDocument(_obj);
    }

    public virtual void ThirdSideChanged(lenspec.ApplicationsForPayment.Shared.ApplicationForPaymentThirdSideChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      Functions.ApplicationForPayment.ChangeApplicationForPaymentPropertiesAccess(_obj);
      Functions.ApplicationForPayment.ClearBasisDocument(_obj);
      
      var errorMessage = Functions.ApplicationForPayment.CheckApprovingThirdSide(_obj);
      e.Params.AddOrUpdate(Constants.ApplicationForPayment.IsSafeThirdSideParam, errorMessage);
      
      errorMessage = Functions.ApplicationForPayment.CheckCounterpartyFields(_obj, e.NewValue);
      if (!string.IsNullOrEmpty(errorMessage))
        errorMessage = lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ErrorMessageFieldNamePartFormat(_obj.Info.Properties.ThirdSide.LocalizedName) + errorMessage;
      
      e.Params.AddOrUpdate(Constants.ApplicationForPayment.IsThirdSideWithEmptyFields, errorMessage);
    }

    public virtual void NoAccountChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      Functions.ApplicationForPayment.ChangeApplicationForPaymentPropertiesAccess(_obj);
    }

    public virtual void DirectorateRegionChanged(lenspec.ApplicationsForPayment.Shared.ApplicationForPaymentDirectorateRegionChangedEventArgs e)
    {
      FillName();
    }

    public override void CounterpartyChanged(Sungero.Docflow.Shared.ContractualDocumentBaseCounterpartyChangedEventArgs e)
    {
      base.CounterpartyChanged(e);
      
      // Очищаем Расчетный счет КА, если изменилось либо очистилось поле КА.
      if (_obj.CounterpartyBankDetail != null)
        if (e.NewValue == null || !Equals(_obj.CounterpartyBankDetail.Counterparty, e.NewValue))
          _obj.CounterpartyBankDetail = null;
      
      FillName();
      Functions.ApplicationForPayment.ClearBasisDocument(_obj);
      
      var errorMessage = Functions.ApplicationForPayment.CheckApprovingCounterparty(_obj);
      e.Params.AddOrUpdate(Constants.ApplicationForPayment.IsSafeCounterpartyParam, errorMessage);
      
      errorMessage = Functions.ApplicationForPayment.CheckCounterpartyFields(_obj, e.NewValue);
      if (!string.IsNullOrEmpty(errorMessage))
        errorMessage = lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ErrorMessageFieldNamePartFormat(_obj.Info.Properties.Counterparty.LocalizedName) + errorMessage;
      
      e.Params.AddOrUpdate(Constants.ApplicationForPayment.IsCounterpartyWithEmptyFields, errorMessage);
    }

    public override void TotalAmountChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      base.TotalAmountChanged(e);
      
      FillName();
      
      Sungero.Docflow.PublicFunctions.ContractualDocumentBase.FillVatAmount(_obj, e.NewValue, _obj.VatRate);
      Sungero.Docflow.PublicFunctions.ContractualDocumentBase.FillNetAmount(_obj, e.NewValue, _obj.VatAmount);
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      
      // Проверка совпадения нового значения с главной НОР Эталон для выставления состояния полей (ГК Эталон АО)
      var constantBusinessUnitId = lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(x => x.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.ApplicationForPaymentBusinessUnitId).SingleOrDefault();
      long mainBusinessUnitId;
      if (constantBusinessUnitId != null && !string.IsNullOrEmpty(constantBusinessUnitId.Value) && e.NewValue != null && long.TryParse(constantBusinessUnitId.Value, out mainBusinessUnitId))
        e.Params.AddOrUpdate(Constants.ApplicationForPayment.IsBusinessUnitMainParam, e.NewValue.Id == mainBusinessUnitId);
      
      FillName();
      Functions.ApplicationForPayment.ClearBasisDocument(_obj);
      
      if (!Equals(e.NewValue, e.OldValue))
        _obj.DecodingBudgetItem = null;
    }

    public virtual void ContractChanged(lenspec.ApplicationsForPayment.Shared.ApplicationForPaymentContractChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, e.OldValue, e.NewValue);
      
      _obj.ObjectAnProjects.Clear();
      if (e.NewValue != null)
      {
        var contract = lenspec.Etalon.Contracts.As(e.NewValue);
        if (contract != null)
        {
          foreach (var item in contract.ConstructionObjectsavis)
          {
            var objectAnProject = _obj.ObjectAnProjects.AddNew();
            objectAnProject.ObjectAnProject = item.ObjectAnProject;
            objectAnProject.DetailingWorkType = item.DetailingWorkType;
          }
        }
      }
      
      var notSignedContractMessage = Functions.ApplicationForPayment.CheckContractInternalApprovalState(_obj);
      e.Params.AddOrUpdate(Constants.ApplicationForPayment.IsSignedContractParam, notSignedContractMessage);
      
      Functions.ApplicationForPayment.FillReasonNumberAndDate(_obj);
    }

    public virtual void ActualPaymentDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue != null)
        _obj.PaymentStatus = ApplicationForPayment.PaymentStatus.Completed;
    }

    public virtual void CustomerRequestChanged(lenspec.ApplicationsForPayment.Shared.ApplicationForPaymentCustomerRequestChangedEventArgs e)
    {
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, e.OldValue, e.NewValue);
    }

    public virtual void IncomingLetterChanged(lenspec.ApplicationsForPayment.Shared.ApplicationForPaymentIncomingLetterChangedEventArgs e)
    {
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, e.OldValue, e.NewValue);
    }

    public virtual void SimpleDocumentChanged(lenspec.ApplicationsForPayment.Shared.ApplicationForPaymentSimpleDocumentChangedEventArgs e)
    {
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, e.OldValue, e.NewValue);
    }

    public virtual void MemoChanged(lenspec.ApplicationsForPayment.Shared.ApplicationForPaymentMemoChangedEventArgs e)
    {
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, e.OldValue, e.NewValue);
      
      Functions.ApplicationForPayment.FillReasonNumberAndDate(_obj);
    }

    public virtual void IncomingInvoiceChanged(lenspec.ApplicationsForPayment.Shared.ApplicationForPaymentIncomingInvoiceChangedEventArgs e)
    {
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, e.OldValue, e.NewValue);
      
      if (e.NewValue != null)
      {
        _obj.VatRate = e.NewValue.VatRate;
        _obj.Currency = e.NewValue.Currency;
        if (e.NewValue.Contract != null)
        {
          if (Sungero.Contracts.Contracts.Is(e.NewValue.Contract))
            _obj.Contract = e.NewValue.Contract;
          
          if (Sungero.Contracts.SupAgreements.Is(e.NewValue.Contract) || avis.EtalonContracts.AttachmentContractDocuments.Is(e.NewValue.Contract))
            _obj.Contract = e.NewValue.Contract.LeadingDocument;
        }
        
        if (!_obj.ObjectAnProjects.Any(x => x != null && x.Amount != null))
          _obj.TotalAmount = e.NewValue.TotalAmount;
        
        if (!_obj.ObjectAnProjects.Any(x => x != null && x.VatAmount != null))
          _obj.VatAmount = e.NewValue.VatAmount;
        
        Functions.ApplicationForPayment.FillBasisDocument(_obj);
      }
      else
      {
        _obj.VatRate = null;
        _obj.Currency = null;
        _obj.Contract = null;
        if (!_obj.ObjectAnProjects.Any(x => x != null && x.Amount != null))
          _obj.TotalAmount = null;
        
        if (!_obj.ObjectAnProjects.Any(x => x != null && x.VatAmount != null))
          _obj.VatAmount = null;
      }
      
      Functions.ApplicationForPayment.FillReasonNumberAndDate(_obj);
    }

    public virtual void PaymentTypeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (e.NewValue == ApplicationForPayment.PaymentType.NoContract && _obj.ThirdSide != null)
        _obj.ThirdSide = null;
      
      Functions.ApplicationForPayment.ClearBasisDocument(_obj);
      
      if (e.NewValue == ApplicationForPayment.PaymentType.Refunds)
      {
        _obj.Directorate = null;
        _obj.DirectorateRegion = null;
        _obj.DepartmentByDirectorate = null;
        _obj.BudgetItem = null;
        _obj.DecodingBudgetItem = null;
      }
      else
      {
        #region Изменения для Транзитного платежа и Субсидирования
        
        var vatRateWithoutVat = Sungero.Commons.VatRates.GetAll(x => x.Status == Sungero.Commons.VatRate.Status.Active && x.Sid == Sungero.Docflow.Constants.Module.VatRateWithoutVatSid).FirstOrDefault();
        var currencyRUB = Sungero.Commons.Currencies.GetAll(x => x.Status == Sungero.Commons.Currency.Status.Active && x.NumericCode == Sungero.Docflow.Resources.CurrencyNumericCodeRUB).FirstOrDefault();
        long budgetItemId = 0;
        long decodingBudgetItemId = 0;
        long directorateId = 0;
        long directorateRegionId = 0;
        long departmentId = 0;
        
        if (e.NewValue == ApplicationForPayment.PaymentType.TransitPayment)
        {
          _obj.PaymentKind = ApplicationForPayment.PaymentKind.NoAdvance;
          _obj.VatRate = vatRateWithoutVat;
          _obj.Currency = currencyRUB;
          if (long.TryParse(EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(EtalonDatabooks.PublicConstants.ConstantDatabook.SharedConstructionBudgetItemTransit), out budgetItemId))
            _obj.BudgetItem = lenspec.ContractualWork.BudgetItemses.GetAll(x => x.Id == budgetItemId).SingleOrDefault();
          
          if (long.TryParse(EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(EtalonDatabooks.PublicConstants.ConstantDatabook.SharedConstructionBudgetItemTransitDecoding), out decodingBudgetItemId))
            _obj.DecodingBudgetItem = lenspec.ContractualWork.DecodingBudgetItemses.GetAll(x => x.Id == decodingBudgetItemId).SingleOrDefault();
          
          if (long.TryParse(EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(EtalonDatabooks.PublicConstants.ConstantDatabook.DirectorateTransit), out directorateId))
            _obj.Directorate = Sungero.Company.Departments.GetAll(x => x.Id == directorateId).SingleOrDefault();
          
          if (long.TryParse(EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(EtalonDatabooks.PublicConstants.ConstantDatabook.DirectorateRegionTransit), out directorateRegionId))
            _obj.DirectorateRegion = avis.EtalonContracts.PresenceRegions.GetAll(x => x.Id == directorateRegionId).SingleOrDefault();
          
          if (long.TryParse(EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(EtalonDatabooks.PublicConstants.ConstantDatabook.DepartmentTransit), out departmentId))
            _obj.DepartmentByDirectorate = Sungero.Company.Departments.GetAll(x => x.Id == departmentId).SingleOrDefault();
        }
        
        if (e.NewValue == ApplicationForPayment.PaymentType.SubsidyDownPaym)
        {
          _obj.PaymentKind = ApplicationForPayment.PaymentKind.NoAdvance;
          _obj.VatRate = vatRateWithoutVat;
          _obj.Currency = currencyRUB;
          _obj.DecodingBudgetItem = null;
          _obj.DirectorateRegion = null;
          _obj.DepartmentByDirectorate = null;
          if (long.TryParse(EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(EtalonDatabooks.PublicConstants.ConstantDatabook.SharedConstructionBudgetItemSubsidy), out budgetItemId))
            _obj.BudgetItem = lenspec.ContractualWork.BudgetItemses.GetAll(x => x.Id == budgetItemId).SingleOrDefault();
          
          if (long.TryParse(EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(EtalonDatabooks.PublicConstants.ConstantDatabook.DirectorateSubsidy), out directorateId))
            _obj.Directorate = Sungero.Company.Departments.GetAll(x => x.Id == directorateId).SingleOrDefault();
        }
        
        #endregion
      }
      
      FillName();
    }

  }
}