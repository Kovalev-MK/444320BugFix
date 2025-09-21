using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractualDocument;

namespace lenspec.Etalon
{
  partial class ContractualDocumentOurCFavisPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> OurCFavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.Code1C != null && x.Code1C != string.Empty);
      return query;
    }
  }

  partial class ContractualDocumentContractTypeavisPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContractTypeavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      return query;
    }
  }

  partial class ContractualDocumentGroupContractTypeavisPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> GroupContractTypeavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      
      if (_obj.IsICPlenspec.HasValue && _obj.IsICPlenspec.Value)
        query = query.Where(x => x.RequireOurCF == avis.EtalonContracts.GroupContractType.RequireOurCF.Yes);
      else
        query = query.Where(x => x.RequireOurCF == avis.EtalonContracts.GroupContractType.RequireOurCF.No);
      return query;
    }
  }

  partial class ContractualDocumentDepartmentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> DepartmentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.DepartmentFiltering(query, e);
      
      var businessUnitIds = lenspec.EtalonDatabooks.PublicFunctions.Module.GetBusinessUnitIdsFromConstant();
      
      if (_obj.BusinessUnit != null)
      {
        query = query.Where(x => Equals(_obj.BusinessUnit, x.BusinessUnit));
        
        if (businessUnitIds.Any() && businessUnitIds.Contains(_obj.BusinessUnit.Id.ToString()))
          query = query.Where(x => lenspec.Etalon.Departments.As(x).IsBudgetFormedlenspec == lenspec.Etalon.Department.IsBudgetFormedlenspec.Yes);
      }
      
      return query;
    }
  }

  partial class ContractualDocumentObjectlenspecPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ObjectlenspecFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.OurCFavis != null)
        query = query.Where(x => _obj.OurCFavis.Equals(x.OurCF));
      
      return query;
    }
  }

  partial class ContractualDocumentConstructionObjectsavisObjectAnProjectPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ConstructionObjectsavisObjectAnProjectFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.IsLinkToInvest != true);
      if (_root.OurCFavis != null)
        query = query.Where(x => Equals(_root.OurCFavis, x.OurCF));
      return query;
    }
  }

  partial class ContractualDocumentCounterpartyPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> CounterpartyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.CounterpartyFiltering(query, e);
      
      query = query.Where(q => lenspec.Etalon.Counterparties.As(q).ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopPossible ||
                          lenspec.Etalon.Counterparties.As(q).ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.DoesNotReqAppr ||
                          lenspec.Etalon.Counterparties.As(q).ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopWithRisks);
      
      return query;
    }
  }

  partial class ContractualDocumentThirdSideavisPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ThirdSideavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q => q.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopPossible ||
                          q.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.DoesNotReqAppr ||
                          q.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopWithRisks);
      
      return query;
    }
  }

  partial class ContractualDocumentEmployeeSignatoryavisPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> EmployeeSignatoryavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      e.DisableUiFiltering = true;

      if (Functions.ContractualDocument.SignatorySettingWithAllUsersExist(_obj))
        return query;
      
      var signatories = Functions.ContractualDocument.GetSignatoriesIds(_obj);
      
      return query.Where(s => signatories.Contains(s.Id));
    }
  }

  partial class ContractualDocumentContractCategoryavisPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContractCategoryavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query;
    }
  }

  partial class ContractualDocumentContractKindavisPropertyFilteringServerHandler<T>
  {

    /// <summary>
    /// Фильтрация выбора поля "Вид".
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual IQueryable<T> ContractKindavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.GroupContractTypeavis != null)
        query = query.Where(q => q.GroupContractType == _obj.GroupContractTypeavis);
      
      return query;
    }
  }

  partial class ContractualDocumentServerHandlers
  {

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      
      if (_obj.IsStandard == false)
        return;
      
      var historyRow = Functions.ContractualDocument.GetImportOrFromTemplateHistoryRow(_obj);
      
      if (EtalonDatabooks.PublicFunctions.Module.IsNullOrImported(historyRow))
        return;
      
      var templateName = historyRow.Comment;
      var template = Etalon.DocumentTemplates.GetAll(x => x.Name == templateName).FirstOrDefault();
      
      if (template == null || _obj.TemplateIDlenspec == template.Id)
        return;
      
      var asyncHandler = EtalonDatabooks.AsyncHandlers.AsyncChangeContractualDocumentTemplateIdProperty.Create();
      asyncHandler.documentId = _obj.Id;
      asyncHandler.templateId = template.Id;
      asyncHandler.ExecuteAsync();
    }

    public override void BeforeSaveHistory(Sungero.Content.DocumentHistoryEventArgs e)
    {
      base.BeforeSaveHistory(e);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Снять обязательность с полей при сохранении, если это архивный документ.
      if (_obj.Archiveavis.HasValue && _obj.Archiveavis.Value == true)
      {
        foreach(var property in _obj.State.Properties)
        {
          property.IsRequired = false;
        }
      }
      
      base.BeforeSave(e);
      _obj.State.Properties.Subject.IsRequired = false;
      
      // Добавить связь, если документ был заполнен из действия Создать на основе или Создать копию внутригруппового документа
      var relatedDocumentId = default(long);
      if (e.Params.TryGetValue(Constants.Contracts.ContractualDocument.Params.CreateFromDocument, out relatedDocumentId))
      {
        var anotherDocument = Sungero.Docflow.OfficialDocuments.Get(relatedDocumentId);
        _obj.Relations.Add(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, anotherDocument);
      }
      
      Functions.ContractualDocument.IsStandard(_obj, e);
      
      if (!Functions.ContractualDocument.CheckConstructionObjects(_obj))
      {
        e.AddError(lenspec.Etalon.ContractualDocuments.Resources.ConstructionObjectsAreEmptyErrorMessage);
        return;
      }
    }

    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsSMRavis = false;
      _obj.IsICPlenspec = false;
      _obj.SyncStatus1cavis = lenspec.Etalon.ContractualDocument.SyncStatus1cavis.NoSync;
      _obj.VatRate = Sungero.Commons.VatRates.GetAll(x => x.Sid == Sungero.Docflow.Constants.Module.VatRateWithoutVatSid).FirstOrDefault();
      
      var department = lenspec.Etalon.Departments.As(Sungero.Company.Employees.Current.Department);
      var businessUnitIds = lenspec.EtalonDatabooks.PublicFunctions.Module.GetBusinessUnitIdsFromConstant();
      
      if (department != null && businessUnitIds.Contains(department.BusinessUnit.Id.ToString()) && department.IsBudgetFormedlenspec == lenspec.Etalon.Department.IsBudgetFormedlenspec.No)
        _obj.Department = null;
      
    }
  }
}