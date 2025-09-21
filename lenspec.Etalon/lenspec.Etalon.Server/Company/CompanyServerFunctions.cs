using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Company;

namespace lenspec.Etalon.Server
{
  
  partial class CompanyFunctions
  {
    
    [Public]
    /// <summary>
    /// Проверить наличие обязательных полей для выгрузки записи в 1С.
    /// </summary>
    /// <returns>Сообщение об ошибке.</returns>
    public string CheckRequiredFieldsForExport()
    {
      var errors = new List<string>();
      var emptyFields = new List<string>();
      if (string.IsNullOrEmpty(_obj.LegalName) || string.IsNullOrWhiteSpace(_obj.LegalName))
        emptyFields.Add(_obj.Info.Properties.LegalName.LocalizedName);
      
      if (string.IsNullOrEmpty(_obj.TIN) || string.IsNullOrWhiteSpace(_obj.TIN))
        emptyFields.Add(_obj.Info.Properties.TIN.LocalizedName);
      
      if (_obj.GroupCounterpartyavis.IdDirectum5 != 17896408 && _obj.GroupCounterpartyavis.IdDirectum5 != 17896409 &&
          (string.IsNullOrEmpty(_obj.TRRC) || string.IsNullOrWhiteSpace(_obj.TRRC)))
        emptyFields.Add(_obj.Info.Properties.TRRC.LocalizedName);
      
      if (string.IsNullOrEmpty(_obj.PostalAddress) || string.IsNullOrWhiteSpace(_obj.PostalAddress))
        emptyFields.Add(_obj.Info.Properties.PostalAddress.LocalizedName);
      
      if (emptyFields.Any())
        errors.Add(lenspec.Etalon.Companies.Resources.CompanyHasEmptyRequiredFieldsFormat(string.Join(", ", emptyFields)));
      
      return string.Join(Environment.NewLine, errors);
    }
    
    /// <summary>
    /// Создать задачу с типом «Согласование включения в реестр квалифицированных контрагентов/ о включении в Список дисквалифицированных контрагентов».
    /// </summary>
    /// <returns>Карточка задачи.</returns>
    [Remote(IsPure = true)]
    public lenspec.Tenders.IApprovalCounterpartyRegisterTask CreateApprovalCounterpartyRegisterTask(List<avis.EtalonContracts.IPresenceRegion> presenceRegions,
                                                                                                    string procedure,
                                                                                                    string registerKind)
    {
      var task = lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Create();
      task.Counterparty = _obj;
      
      if (procedure == Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.Procedure.GetLocalizedValue(Tenders.ApprovalCounterpartyRegisterTask.Procedure.Inclusion))
        task.Procedure = Tenders.ApprovalCounterpartyRegisterTask.Procedure.Inclusion;
      
      if (procedure == Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.Procedure.GetLocalizedValue(Tenders.ApprovalCounterpartyRegisterTask.Procedure.Exclusion))
        task.Procedure = Tenders.ApprovalCounterpartyRegisterTask.Procedure.Exclusion;
      
      if (registerKind == Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.RegisterKind.GetLocalizedValue(Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Contractor))
        task.RegisterKind = Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Contractor;
      
      if (registerKind == Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.RegisterKind.GetLocalizedValue(Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Provider))
        task.RegisterKind = Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Provider;
      
      foreach (var presenceRegion in presenceRegions)
      {
        var item = task.PresenceRegions.AddNew();
        item.PresenceRegion = presenceRegion;
      }
      
      #region Добавление вложений
      
      //      task.CounterpartyGroup.Companies.Add(_obj);
      //      var counterpartyDocuments = Sungero.Docflow.CounterpartyDocuments.GetAll(x => Equals(_obj, x.Counterparty));
      //      if (!counterpartyDocuments.Any())
      //      {
//
      //      }
      //      task.CounterpartyDocumentGroup.CounterpartyDocuments.Add();
      
      #endregion
      
      return task;
    }
    
    /// <summary>
    /// Запуск АО для обновления записей справочников Реестр поставщиков и Реестр подрядчиков.
    /// </summary>
    /// <remarks>Запускается если изменено хотябы одно из полей группы Согласование ДБ</remarks>
    public void UpdateRegistries()
    {
      var isApprovalCardGroupChanged = _obj.State.Properties.ResultApprovalDEBavis.IsChanged || _obj.State.Properties.ResponsibleDEBavis.IsChanged ||
        _obj.State.Properties.InspectionDateDEBavis.IsChanged || _obj.State.Properties.ApprovalPeriodavis.IsChanged;
      
      if (isApprovalCardGroupChanged)
      {
        var contractorRegistries = lenspec.Tenders.ContractorRegisters.GetAll(x => Equals(_obj, x.Counterparty));
        if (contractorRegistries.Any())
        {
          var asyncHandler = avis.ApprovingCounterpartyDEB.AsyncHandlers.UpdateContractorRegister.Create();
          asyncHandler.CompanyId = _obj.Id;
          asyncHandler.ContractorRegisterIds = string.Join(",", contractorRegistries.Select(x => x.Id));
          asyncHandler.ExecuteAsync();
        }
        
        var providerRegistries = lenspec.Tenders.ProviderRegisters.GetAll(x => Equals(_obj, x.Counterparty));
        if (providerRegistries.Any())
        {
          var asyncHandler = avis.ApprovingCounterpartyDEB.AsyncHandlers.UpdateProviderRegister.Create();
          asyncHandler.CompanyId = _obj.Id;
          asyncHandler.ProviderRegisterIds = string.Join(",", providerRegistries.Select(x => x.Id));
          asyncHandler.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Найти догвоора для действия 'все действующие договоры'
    /// </summary>
    [Remote(IsPure = true)]
    public List<lenspec.Etalon.IContract> GetActiveContractList()
    {
      var contracts = lenspec.Etalon.Contracts.GetAll(x => _obj.Equals(x.Counterparty) &&
                                                      x.LifeCycleState != Sungero.Contracts.Contract.LifeCycleState.Draft &&
                                                      x.LifeCycleState != Sungero.Contracts.Contract.LifeCycleState.Obsolete &&
                                                      x.LifeCycleState != Sungero.Contracts.Contract.LifeCycleState.Terminated &&
                                                      ((x.IsFrameworkavis.HasValue && x.IsFrameworkavis.Value && x.ContractTypeavis != null && x.ContractTypeavis.Code != 3) ||
                                                       (x.RemainingAmountlenspec.HasValue && x.RemainingAmountlenspec.Value > 0)));
      
      return contracts.ToList();
    }
    
    /// <summary>
    /// Получить список тендерных документов для КА.
    /// </summary>
    /// <returns>Тендерные документы.</returns>
    [Remote(IsPure =true)]
    public override List<Sungero.Docflow.IOfficialDocument> GetTenderDocuments()
    {
      var documents = base.GetTenderDocuments();
      documents.AddRange(lenspec.Tenders.AccreditationCommitteeProtocols.GetAll(x => x.Counterparty == _obj).ToList());
      
      return documents;
    }
    
    /// <summary>
    /// Обновить статус компании в реестре поставщиков/подрядчиков.
    /// </summary>
    /// <param name="isProvider">Признак поставщика.</param>
    /// <param name="isContractor">Признак подрядчика.</param>
    [Public, Remote(IsPure = true)]
    public void UpdateRegisterStatus(bool isProvider, bool isContractor)
    {
      var asyncHandler = lenspec.Etalon.Module.Parties.AsyncHandlers.UpdateCompanyRegisterStatuslenspec.Create();
      asyncHandler.CompanyId =    _obj.Id;
      asyncHandler.IsProvider =   _obj.IsProvideravis == true;
      asyncHandler.IsContractor = _obj.IsContractoravis == true;
      var needUpdate = false;
      
      // Обновить признак поставщика.
      if (isProvider && _obj.IsProvideravis != true)
      {
        asyncHandler.IsProvider = true;
        needUpdate = true;
      }
      
      // Обновить признак подрядчика.
      if (isContractor && _obj.IsContractoravis != true)
      {
        asyncHandler.IsContractor = true;
        needUpdate = true;
      }
      
      if (needUpdate)
        asyncHandler.ExecuteAsync();
    }
  }
}