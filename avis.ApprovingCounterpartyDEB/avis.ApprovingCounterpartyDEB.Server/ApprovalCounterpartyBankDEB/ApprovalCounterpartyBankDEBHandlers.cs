using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEB;

namespace avis.ApprovingCounterpartyDEB
{

  partial class ApprovalCounterpartyBankDEBServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      //FIXME Логика связанная с полями, которые пока что надо скрыть
      /*
      if (lenspec.Etalon.Companies.Is(_obj.Counterparty))
      {
        if (_obj.IsProvider != true && _obj.IsContractor != true)
        {
          e.AddError(ApprovalCounterpartyBankDEBs.Resources.ProviderContractorNotTrue);
          return;
        }
        if (_obj.IsProvider != true && _obj.Materials.Any())
        {
          e.AddError("Указаны материалы, поставляемые контрагентом, поставьте галочку «Поставщик» или очистите поля с материалами");
          return;
        }
        if (_obj.IsContractor != true && _obj.WorkKinds.Any())
        {
          e.AddError("Указаны виды работ, выполняемые контрагентом, поставьте галочку «Подрядчик» или очистите поля с видами работ");
          return;
        }
        
        if (_obj.IsProvider == true || _obj.IsContractor == true)
        {
          if (_obj.IsProvider == true && _obj.Materials.Count == default && _obj.IsContractor == true && _obj.WorkKinds.Count == default)
          {
            e.AddError(ApprovalCounterpartyBankDEBs.Resources.MaterialAndKindWorkErrorMessage);
            return;
          }
          else if (_obj.IsProvider == true && _obj.Materials.Count == default)
          {
            e.AddError(ApprovalCounterpartyBankDEBs.Resources.EmptyMaterialsErrorMessage);
            return;
          }
          else if (_obj.IsContractor == true && _obj.WorkKinds.Count == default)
          {
            e.AddError(ApprovalCounterpartyBankDEBs.Resources.EmptyWorkKindErrorMessage);
            return;
          }
        }
      }
      */
      
      if (_obj.IsAmountBigestYearAmount == avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEB.IsAmountBigestYearAmount.Yes)
      {
        if (_obj.BusinessUnit.TenderAmountavis > _obj.EstimatedAmountTransaction)
        {
          e.AddError(ApprovalCounterpartyBankDEBs.Resources.TenderAmountBiggerYearAmount);
          return;
        }
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.IsProvider = false;
      _obj.IsContractor = false;
      _obj.CounterpartyType = ApprovalCounterpartyBankDEBs.Resources.CounterpartyType;
      _obj.IsNeedQualification = false;
    }
  }

  partial class ApprovalCounterpartyBankDEBCounterpartyPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> CounterpartyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => (Sungero.Parties.Banks.Is(x) && Sungero.Parties.Banks.As(x).HeadBankavis == null) || (Sungero.Parties.Companies.Is(x) && Sungero.Parties.Companies.As(x).HeadCompany == null));
      return query;
    }
  }

  partial class ApprovalCounterpartyBankDEBMaterialsMaterialGroupPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MaterialsMaterialGroupFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q=> q.Status == avis.EtalonParties.MaterialGroup.Status.Active);
      return query;
    }
  }

  partial class ApprovalCounterpartyBankDEBMaterialsMaterialPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MaterialsMaterialFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q => q.Status == avis.EtalonParties.Material.Status.Active);
      return query;
    }
  }

  partial class ApprovalCounterpartyBankDEBWorkKindsWorkGroupPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WorkKindsWorkGroupFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q => q.Status == avis.EtalonParties.WorkGroup.Status.Active);
      return query;
    }
  }

  partial class ApprovalCounterpartyBankDEBWorkKindsWorkKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WorkKindsWorkKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q => q.Status == avis.EtalonParties.WorkKind.Status.Active);
      return query;
    }
  }
}