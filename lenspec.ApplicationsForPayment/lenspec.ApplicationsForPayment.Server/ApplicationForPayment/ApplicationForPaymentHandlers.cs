using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForPayment.ApplicationForPayment;

namespace lenspec.ApplicationsForPayment
{
  partial class ApplicationForPaymentObjectAnProjectsVatRatePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ObjectAnProjectsVatRateFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.Sid != lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateEighteenPercentSid &&
                          x.Sid != lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateMixedPercentSid);
      
      return query;
    }
  }

  partial class ApplicationForPaymentThirdSideBankDetailPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ThirdSideBankDetailFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.ThirdSide != null)
        query = query.Where(x => Equals(x.Counterparty, _obj.ThirdSide));
      
      return query;
    }
  }

  partial class ApplicationForPaymentDirectorateRegionPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> DirectorateRegionFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.IsUseToAFP.HasValue && x.IsUseToAFP.Value);
      return query;
    }
  }

  partial class ApplicationForPaymentVatRatePropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> VatRateFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.VatRateFiltering(query, e);
      query = query.Where(x => x.Sid != lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateEighteenPercentSid);
      
      return query;
    }
  }

  partial class ApplicationForPaymentCreatingFromServerHandler
  {

    public override void CreatingFrom(Sungero.Domain.CreatingFromEventArgs e)
    {
      base.CreatingFrom(e);
      e.Without(_info.Properties.Export1CDate);
      e.Without(_info.Properties.Export1CState);
    }
  }

  partial class ApplicationForPaymentUniversalTransferDocumentPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> UniversalTransferDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query;
    }
  }

  partial class ApplicationForPaymentWaybillPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WaybillFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query;
    }
  }

  partial class ApplicationForPaymentContractStatementPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContractStatementFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query;
    }
  }

  partial class ApplicationForPaymentUTDsUTDPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> UTDsUTDFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_root.BusinessUnit != null && _root.Counterparty != null && avis.EtalonIntergation.BusinessUnits.GetAll().Where(c => c.Company == _root.Counterparty).Any())
      {
        var company = lenspec.Etalon.BusinessUnits.GetAll().Where(c => c.Company == _root.Counterparty).First();
        query = query.Where(x => (x.BusinessUnit == _root.BusinessUnit && x.Counterparty == _root.Counterparty) ||
                            (x.BusinessUnit == company &&
                             x.Counterparty == _root.BusinessUnit.Company));
      }
      else
      {
        if (_root.BusinessUnit != null)
          query = query.Where(x => _root.BusinessUnit == x.BusinessUnit);
        
        if (_root.Counterparty != null && _root.ThirdSide != null)
        {
          query = query.Where(x => _root.Counterparty == x.Counterparty || _root.ThirdSide == x.Counterparty);
        }
        else if (_root.Counterparty != null)
        {
          query = query.Where(x => _root.Counterparty == x.Counterparty);
        }
        else if (_root.ThirdSide != null)
        {
          query = query.Where(x => _root.ThirdSide == x.Counterparty);
        }
      }
      return query;
    }
  }

  partial class ApplicationForPaymentWaybillsWaybillPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WaybillsWaybillFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_root.BusinessUnit != null && _root.Counterparty != null && avis.EtalonIntergation.BusinessUnits.GetAll().Where(c => c.Company == _root.Counterparty).Any())
      {
        var company = lenspec.Etalon.BusinessUnits.GetAll().Where(c => c.Company == _root.Counterparty).First();
        query = query.Where(x => (x.BusinessUnit == _root.BusinessUnit && x.Counterparty == _root.Counterparty) ||
                            (x.BusinessUnit == company &&
                             x.Counterparty == _root.BusinessUnit.Company));
      }
      else
      {
        
        if (_root.BusinessUnit != null)
          query = query.Where(x => _root.BusinessUnit == x.BusinessUnit);
        
        if (_root.Counterparty != null && _root.ThirdSide != null)
        {
          query = query.Where(x => _root.Counterparty == x.Counterparty || _root.ThirdSide == x.Counterparty);
        }
        else if (_root.Counterparty != null)
        {
          query = query.Where(x => _root.Counterparty == x.Counterparty);
        }
        else if (_root.ThirdSide != null)
        {
          query = query.Where(x => _root.ThirdSide == x.Counterparty);
        }
      }
      return query;
    }
  }

  partial class ApplicationForPaymentContractStatementsContractStatementPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContractStatementsContractStatementFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_root.BusinessUnit != null && _root.Counterparty != null && avis.EtalonIntergation.BusinessUnits.GetAll().Where(c => c.Company == _root.Counterparty).Any())
      {
        var company = lenspec.Etalon.BusinessUnits.GetAll().Where(c => c.Company == _root.Counterparty).First();
        query = query.Where(x => (x.BusinessUnit == _root.BusinessUnit && x.Counterparty == _root.Counterparty) ||
                            (x.BusinessUnit == company &&
                             x.Counterparty == _root.BusinessUnit.Company));
      }
      else
      {
        if (_root.BusinessUnit != null)
          query = query.Where(x => _root.BusinessUnit == x.BusinessUnit);
        
        if (_root.Counterparty != null && _root.ThirdSide != null)
          query = query.Where(x => _root.Counterparty == x.Counterparty || _root.ThirdSide == x.Counterparty);
        else if (_root.Counterparty != null)
          query = query.Where(x => _root.Counterparty == x.Counterparty);
        else if (_root.ThirdSide != null)
          query = query.Where(x => _root.ThirdSide == x.Counterparty);
      }
      
      return query;
    }
  }


  partial class ApplicationForPaymentCounterpartyBankDetailPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CounterpartyBankDetailFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Counterparty != null)
        query = query.Where(x => x.Counterparty == _obj.Counterparty);
      
      return query;
    }
  }

  partial class ApplicationForPaymentObjectAnProjectsDetailingWorkTypePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ObjectAnProjectsDetailingWorkTypeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      
      if (_obj.ApplicationForPayment.Contract != null)
      {
        var detailingWorkType = new List<avis.EtalonContracts.IDetailingWorkType>();
        
        // Для клиентского договора оставить выборку пустой.
        var sdaClientContract = lenspec.SalesDepartmentArchive.SDAClientContracts.As(_obj.ApplicationForPayment.Contract);
        if (sdaClientContract != null)
        {
          return query.Where(x => detailingWorkType.Contains(x));
        }
        
        var contract = lenspec.Etalon.Contracts.As(_obj.ApplicationForPayment.Contract);
        if (contract != null && contract.ConstructionObjectsavis.Any(x => x.DetailingWorkType != null))
        {
          detailingWorkType.AddRange(contract.ConstructionObjectsavis.Select(x => x.DetailingWorkType).Where(x => x != null).ToList().Distinct());
          
          var fromSupAgreement = lenspec.Etalon.SupAgreements.GetAll(x => Equals(x.LeadingDocument, contract) && x.ConstructionObjectsavis.Any())
            .SelectMany(x => x.ConstructionObjectsavis.Select(o => o.DetailingWorkType)).Where(x => x != null).ToList().Distinct();
          detailingWorkType.AddRange(fromSupAgreement);
          
          query = query.Where(x => detailingWorkType.Contains(x));
        }
      }
      
      return query;
    }
  }

  partial class ApplicationForPaymentDecodingBudgetItemPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> DecodingBudgetItemFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      
      var isBusinessUnitMain = false;
      e.Params.TryGetValue(Constants.ApplicationForPayment.IsBusinessUnitMainParam, out isBusinessUnitMain);
      if (isBusinessUnitMain)
        query = query.Where(x => x.CodeOracle != null && x.CodeOracle != string.Empty);
      
      return query;
    }
  }

  partial class ApplicationForPaymentBudgetItemPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> BudgetItemFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      return query;
    }
  }


  partial class ApplicationForPaymentObjectAnProjectsObjectAnProjectPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ObjectAnProjectsObjectAnProjectFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.IsLinkToInvest != true && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      
      if (_obj.ApplicationForPayment.Contract != null)
      {
        var contract = lenspec.Etalon.Contracts.As(_obj.ApplicationForPayment.Contract);
        if (contract != null && contract.ConstructionObjectsavis.Any(x => x.ObjectAnProject != null))
        {
          var objectAnProjects = new List<lenspec.EtalonDatabooks.IObjectAnProject>();
          objectAnProjects.AddRange(contract.ConstructionObjectsavis.Select(x => x.ObjectAnProject).Where(x => x != null).ToList().Distinct());
          
          var fromSupAgreement = lenspec.Etalon.SupAgreements.GetAll(x => Equals(x.LeadingDocument, contract) && x.ConstructionObjectsavis.Any())
            .SelectMany(x => x.ConstructionObjectsavis.Select(o => o.ObjectAnProject)).Where(x => x != null).ToList().Distinct();
          objectAnProjects.AddRange(fromSupAgreement);
          
          query = query.Where(x => objectAnProjects.Contains(x));
        }
        
        var sdaClientContract = lenspec.SalesDepartmentArchive.SDAClientContracts.As(_obj.ApplicationForPayment.Contract);
        if (sdaClientContract != null && sdaClientContract.ObjectAnProject != null)
        {
          query = query.Where(x => Equals(sdaClientContract.ObjectAnProject, x));
        }
      }
      
      return query;
    }
  }

  partial class ApplicationForPaymentDepartmentByDirectoratePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> DepartmentByDirectorateFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Directorate != null)
      {
        var subDepartments = lenspec.Etalon.Module.Company.PublicFunctions.Module.GetSubDepartments(_obj.Directorate);
        query = query.Where(x => subDepartments.Contains(x));
      }
      
      return query;
    }
  }

  partial class ApplicationForPaymentDirectoratePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> DirectorateFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => lenspec.Etalon.Departments.As(x).IsBudgetFormedlenspec == lenspec.Etalon.Department.IsBudgetFormedlenspec.Yes);
      
      if (_obj.DepartmentByDirectorate != null)
      {
        var department = _obj.DepartmentByDirectorate;
        var headOffices = new List<long>();
        while (department.HeadOffice != null)
        {
          headOffices.Add(department.HeadOffice.Id);
          department = department.HeadOffice;
        }
        query = query.Where(x => headOffices.Contains(x.Id));
      }
      return query;
    }
  }

  partial class ApplicationForPaymentServerHandlers
  {

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.PaymentStatus != ApplicationForPayment.PaymentStatus.Refused && Functions.ApplicationForPayment.HaveDuplicates(_obj))
      {
        e.AddError(Sungero.Commons.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
        return;
      }
      
      var hasError = false;
      var message = Functions.ApplicationForPayment.CheckTotalAmount(_obj);
      if (!string.IsNullOrEmpty(message))
      {
        e.AddError(message);
        hasError = true;
      }
      
      message = Functions.ApplicationForPayment.CheckCounterpartyFields(_obj, _obj.Counterparty);
      if (!string.IsNullOrEmpty(message))
      {
        message = lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ErrorMessageFieldNamePartFormat(_obj.Info.Properties.Counterparty.LocalizedName) + message;
        
        if (Etalon.People.Is(_obj.Counterparty))
          e.AddWarning(message);
        else
        {
          e.AddError(message);
          hasError = true;
        }
      }
      
      message = Functions.ApplicationForPayment.CheckApprovingCounterparty(_obj);
      if (!string.IsNullOrEmpty(message))
      {
        e.AddError(message);
        hasError = true;
      }
      
      message = Functions.ApplicationForPayment.CheckContractInternalApprovalState(_obj);
      if (!string.IsNullOrEmpty(message))
      {
        e.AddError(message);
        hasError = true;
      }
      
      message = Functions.ApplicationForPayment.CheckCounterpartyFields(_obj, _obj.ThirdSide);
      if (!string.IsNullOrEmpty(message))
      {
        message = lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ErrorMessageFieldNamePartFormat(_obj.Info.Properties.ThirdSide.LocalizedName) + message;
        
        if (Etalon.People.Is(_obj.ThirdSide))
          e.AddWarning(message);
        else
        {
          e.AddError(message);
          hasError = true;
        }
      }
      
      message = Functions.ApplicationForPayment.CheckApprovingThirdSide(_obj);
      if (!string.IsNullOrEmpty(message))
      {
        e.AddError(message);
        hasError = true;
      }
      
      if (_obj.PaymentType == ApplicationForPayment.PaymentType.NoContract && _obj.TotalAmount.HasValue && _obj.Currency != null)
      {
        //Если валюта рубль, то лимит 300к
        if  (_obj.TotalAmount.Value > Constants.ApplicationForPayment.LimitOfTotalAmountWithoutContract && _obj.Currency.NumericCode == Sungero.Docflow.Resources.CurrencyNumericCodeRUB)
        {
          e.AddError(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.TotalAmountExceedsLimit);
          hasError = true;
        }
        
        //Если валюта Доллар США или Евро, то лимит 3к
        if (_obj.TotalAmount.Value > Constants.ApplicationForPayment.LimitOfTotalAmountWithoutContractUSDEUR &&
            (_obj.Currency.NumericCode == Sungero.Docflow.Resources.CurrencyNumericCodeUSD || _obj.Currency.NumericCode == Sungero.Docflow.Resources.CurrencyNumericCodeEUR))
        {
          e.AddError(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.LimitOfTotalAmountWithoutContractUS);
          hasError = true;
        }
      }
      
      if (hasError)
        return;
      
      base.BeforeSave(e);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.NoAccount = false;
      _obj.Currency = Sungero.Commons.Currencies.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active &&
                                                        x.NumericCode == Sungero.Docflow.Resources.CurrencyNumericCodeRUB)
        .FirstOrDefault();
      _obj.PaymentStatus = ApplicationForPayment.PaymentStatus.InProgress;
      _obj.Export1CState = Export1CState.No;
    }
  }

  partial class ApplicationForPaymentCustomerRequestPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CustomerRequestFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Counterparty != null)
        query = query.Where(x => x.Client == lenspec.Etalon.People.As(_obj.Counterparty));
      
      return query;
    }
  }

  partial class ApplicationForPaymentIncomingLetterPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> IncomingLetterFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.BusinessUnit != null)
        query = query.Where(x => _obj.BusinessUnit == x.BusinessUnit);
      
      if (_obj.Counterparty != null || _obj.ThirdSide != null)
        query = query.Where(x => (_obj.Counterparty != null && _obj.ThirdSide != null && (_obj.Counterparty == x.Correspondent || _obj.ThirdSide == x.Correspondent)) ||
                            (_obj.Counterparty != null && _obj.Counterparty == x.Correspondent) || (_obj.ThirdSide != null && _obj.ThirdSide == x.Correspondent));
      
      return query;
    }
  }

  partial class ApplicationForPaymentIncomingInvoicePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> IncomingInvoiceFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.LifeCycleState != lenspec.Etalon.IncomingInvoice.LifeCycleState.Paid &&
                          x.LifeCycleState != lenspec.Etalon.IncomingInvoice.LifeCycleState.Active &&
                          x.LifeCycleState != lenspec.Etalon.IncomingInvoice.LifeCycleState.Obsolete);
      
      // НОР и КА заполнены, КА – НОР.
      if (_obj.BusinessUnit != null && _obj.Counterparty != null && avis.EtalonIntergation.BusinessUnits.GetAll().Where(c => c.Company == _obj.Counterparty).Any())
      {
        var company = avis.EtalonIntergation.BusinessUnits.GetAll().Where(c => c.Company == _obj.Counterparty).First();
        query = query.Where(x => (x.BusinessUnit == _obj.BusinessUnit && x.Counterparty == _obj.Counterparty) ||
                            (x.BusinessUnit == company &&
                             x.Counterparty == _obj.BusinessUnit.Company));
      }
      else
      {
        if (_obj.BusinessUnit != null)
          query = query.Where(x => Equals(_obj.BusinessUnit, x.BusinessUnit));
        
        if (_obj.Counterparty != null || _obj.ThirdSide != null)
          query = query.Where(x => (_obj.Counterparty != null && _obj.ThirdSide != null && (_obj.Counterparty == x.Counterparty || _obj.ThirdSide == x.Counterparty)) ||
                              (_obj.Counterparty != null && _obj.Counterparty == x.Counterparty) || (_obj.ThirdSide != null && _obj.ThirdSide == x.Counterparty));
      }
      return query;
    }
  }

  partial class ApplicationForPaymentContractPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContractFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Category == ApplicationForPayment.Category.ForClients)
        query = query.Where(x => lenspec.SalesDepartmentArchive.SDAClientContracts.Is(x));
      else if (_obj.Category == ApplicationForPayment.Category.Standard)
        query = query.Where(x => Sungero.Contracts.Contracts.Is(x));
      else
        query = query.Where(x => Sungero.Contracts.Contracts.Is(x) || lenspec.SalesDepartmentArchive.SDAClientContracts.Is(x));
      
      if (_obj.BusinessUnit != null && _obj.Counterparty != null && avis.EtalonIntergation.BusinessUnits.GetAll().Where(c => c.Company == _obj.Counterparty).Any())
      {
        var company = lenspec.Etalon.BusinessUnits.GetAll().Where(c => c.Company == _obj.Counterparty).First();
        query = query.Where(x => (x.BusinessUnit == _obj.BusinessUnit &&
                                  (Sungero.Contracts.Contracts.As(x).Counterparty == _obj.Counterparty ||
                                   lenspec.SalesDepartmentArchive.SDAClientContracts.As(x).CounterpartyClient.Any(c => c.ClientItem == _obj.Counterparty)) ||
                                  (x.BusinessUnit == company &&
                                   (_obj.BusinessUnit.Company == Sungero.Contracts.Contracts.As(x).Counterparty) ||
                                   lenspec.SalesDepartmentArchive.SDAClientContracts.As(x).CounterpartyClient.Any(c => c.ClientItem == _obj.BusinessUnit.Company))));
      }
      else
      {
        if (_obj.BusinessUnit != null && !(_obj.Category == ApplicationForPayment.Category.ForClients && _obj.PaymentType == ApplicationForPayment.PaymentType.TransitPayment))
          query = query.Where(x => _obj.BusinessUnit == x.BusinessUnit);
        
        if (_obj.Counterparty != null && _obj.ThirdSide != null)
          query = query.Where(x => (Sungero.Contracts.Contracts.Is(x) &&
                                    Sungero.Contracts.Contracts.As(x).Counterparty == _obj.Counterparty ||  Sungero.Contracts.Contracts.As(x).Counterparty == _obj.ThirdSide) ||
                              (lenspec.SalesDepartmentArchive.SDAClientContracts.Is(x) &&
                               lenspec.SalesDepartmentArchive.SDAClientContracts.As(x).CounterpartyClient.Any(c => c.ClientItem == _obj.Counterparty || c.ClientItem == _obj.ThirdSide)));
        else if (_obj.Counterparty != null)
          query = query.Where(x => Sungero.Contracts.Contracts.Is(x) && Sungero.Contracts.Contracts.As(x).Counterparty == _obj.Counterparty ||
                              lenspec.SalesDepartmentArchive.SDAClientContracts.Is(x) && lenspec.SalesDepartmentArchive.SDAClientContracts.As(x).CounterpartyClient.Any(c => c.ClientItem == _obj.Counterparty));
        else if (_obj.ThirdSide != null)
          query = query.Where(x => Sungero.Contracts.Contracts.Is(x) && Sungero.Contracts.Contracts.As(x).Counterparty == _obj.ThirdSide ||
                              lenspec.SalesDepartmentArchive.SDAClientContracts.Is(x) && lenspec.SalesDepartmentArchive.SDAClientContracts.As(x).CounterpartyClient.Any(c => c.ClientItem == _obj.ThirdSide));
      }
      return query;
    }
  }

}