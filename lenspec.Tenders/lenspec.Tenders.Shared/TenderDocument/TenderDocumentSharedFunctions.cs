using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderDocument;

namespace lenspec.Tenders.Shared
{
  partial class TenderDocumentFunctions
  {
    #region [Формирование имени документа]

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
      
      var decisionOnInclusionOfCounterpartyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DecisionOnInclusionOfCounterpartyKind);
      var decisionOnExclusionOfCounterpartyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DecisionOnExclusionOfCounterpartyKind);
      var memoOnInclusionOfCounterpartyKind =     Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.MemoOnInclusionOfCounterpartyKind);
      var memoOnExclusionOfCounterpartyKind =     Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.MemoOnExclusionOfCounterpartyKind);
      
      var name = string.Empty;
      if (Equals(documentKind, decisionOnInclusionOfCounterpartyKind))
        name = GetDecisionOnInclusionOfCounterpartyName();
      else if (Equals(documentKind, decisionOnExclusionOfCounterpartyKind))
        name = GetDecisionOnExclusionOfCounterpartyName();
      else if (Equals(documentKind, memoOnInclusionOfCounterpartyKind))
        name = GetMemoOnInclusionOfCounterpartyName();
      else if (Equals(documentKind, memoOnExclusionOfCounterpartyKind))
        name = GetMemoOnExclusionOfCounterpartyName();
      else
        name = GetDefaultName();
      
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      _obj.Name = name.Substring(0, Math.Min(_obj.Info.Properties.Name.Length, name.Length));
    }
    
    /// <summary>
    /// Получить наименование документа по умолчанию.
    /// </summary>
    /// <returns>Имя документа.</returns>
    private string GetDefaultName()
    {
      var sB = new System.Text.StringBuilder();
      
      // Имя в формате: <Вид документа> <Контрагенты> для <Наша орг.>, <Предмет тендера>
      using (TenantInfo.Culture.SwitchTo())
      {
        var counterpartyNames = _obj.Counterparties
          .Where(x => x.Counterparty != null)
          .Select(x => x.Counterparty.Name);
        if (counterpartyNames.Any())
        {
          sB.Append(lenspec.Tenders.TenderDocumentBases.Resources.WhitespacePartOfDocName);
          sB.Append(string.Join(
            lenspec.Tenders.TenderDocumentBases.Resources.CommaPartOfDocName,
            counterpartyNames
           ));
        }
        if (_obj.BusinessUnit != null)
        {
          sB.Append(lenspec.Tenders.TenderDocumentBases.Resources.ForPartOfDocName);
          sB.Append(_obj.BusinessUnit.Name);
        }
        if (_obj.TenderSelectionSubject != null)
        {
          sB.Append(lenspec.Tenders.TenderDocumentBases.Resources.CommaPartOfDocName);
          sB.Append(_obj.TenderSelectionSubject);
        }
        
        if (sB.Length == 0)
          return _obj.VerificationState == null ?
            Sungero.Docflow.Resources.DocumentNameAutotext :
            _obj.DocumentKind.ShortName;
        
        else if (_obj.DocumentKind != null)
          return _obj.DocumentKind.ShortName + sB.ToString();
        
        return sB.ToString();
      }
    }
    
    /// <summary>
    /// Получить наименование документа вида "Решение комитета по квалификации включения контрагента в реестр".
    /// </summary>
    /// <returns>Имя документа.</returns>
    private string GetDecisionOnInclusionOfCounterpartyName()
    {
      var date = _obj.QCDecisionDate == null ?
        lenspec.Tenders.TenderDocuments.Resources.FillerFormat(TenderDocuments.Info.Properties.QCDecisionDate.LocalizedName) :
        _obj.QCDecisionDate.Value.ToShortDateString();
      var counterparties = string.Join(", ", _obj.Counterparties.Where(x => x.Counterparty != null).Select(
        x => string.IsNullOrEmpty(x.Counterparty.TIN) ?
        x.Counterparty.Name :
        $"{x.Counterparty.Name} {x.Counterparty.TIN}"
       ));
      
      return TenderDocuments.Resources.DecisionOnInclusionOfCounterpartyNameFormat(
        date,
        counterparties
       );
    }
    
    /// <summary>
    /// Получить наименование документа вида "Решение комитета по квалификации исключения контрагента из реестра".
    /// </summary>
    /// <returns>Имя документа.</returns>
    private string GetDecisionOnExclusionOfCounterpartyName()
    {
      var date = _obj.QCDecisionDate == null ?
        lenspec.Tenders.TenderDocuments.Resources.FillerFormat(TenderDocuments.Info.Properties.QCDecisionDate.LocalizedName) :
        _obj.QCDecisionDate.Value.ToShortDateString();
      var counterparties = string.Join(", ", _obj.Counterparties
                                       .Where(x => x.Counterparty != null)
                                       .Select(
                                         x => string.IsNullOrEmpty(x.Counterparty.TIN) ?
                                         x.Counterparty.Name :
                                         $"{x.Counterparty.Name} {x.Counterparty.TIN}"
                                        ));
      
      return TenderDocuments.Resources.DecisionOnExclusionOfCounterpartyNameFormat(
        date,
        counterparties
       );
    }
    
    /// <summary>
    /// Получить наименование документа вида "Служебная записка на исключение из реестра".
    /// </summary>
    /// <returns>Имя документа.</returns>
    private string GetMemoOnInclusionOfCounterpartyName()
    {
      var date = _obj.RegistrationDate == null ?
        TenderDocuments.Resources.FillerFormat(TenderDocuments.Info.Properties.RegistrationDate.LocalizedName) :
        _obj.RegistrationDate.Value.ToShortDateString();
      var counterparties = string.Join(", ", _obj.Counterparties.Where(x => x.Counterparty != null).Select(x => x.Counterparty.Name));
      
      return TenderDocuments.Resources.MemoOnInclusionOfCounterpartyNameFormat(
        date,
        counterparties
       );
    }
    
    /// <summary>
    /// Получить наименование документа вида "Служебная записка на исключение из реестра".
    /// </summary>
    /// <returns>Имя документа.</returns>
    private string GetMemoOnExclusionOfCounterpartyName()
    {
      var date = _obj.RegistrationDate == null ?
        TenderDocuments.Resources.FillerFormat(TenderDocuments.Info.Properties.RegistrationDate.LocalizedName) :
        _obj.RegistrationDate.Value.ToShortDateString();
      var counterparties = string.Join(", ", _obj.Counterparties.Where(x => x.Counterparty != null).Select(x => x.Counterparty.Name));
      
      return TenderDocuments.Resources.MemoOnExclusionOfCounterpartyNameFormat(
        date,
        counterparties
       );
    }
    
    #endregion [Формирование имени документа]
    
    /// <summary>
    /// Обновить свойства полей (доступность, видимость и обязательность).
    /// </summary>
    /// <param name="documentKindGuid">GUID вида документа.</param>
    /// <remarks>Очистка скрываемых полей в изменении значения DocumentKind.</remarks>
    public void UpdateFieldsProperties(Guid documentKindGuid)
    {
      if (Equals(documentKindGuid, Guid.Empty))
      {
        // Доступность.
        _obj.State.Properties.Name.IsEnabled =                        false;
        // Видимость.
        _obj.State.Properties.BusinessUnit.IsVisible =                false;
        _obj.State.Properties.Counterparties.IsVisible =              false;
        _obj.State.Properties.Author.IsVisible =                      false;
        _obj.State.Properties.TenderSelectionSubject.IsVisible =      false;
        _obj.State.Properties.OurCF.IsVisible =                       false;
        _obj.State.Properties.ObjectAnProjects.IsVisible =            false;
        _obj.State.Properties.Procedure.IsVisible =                   false;
        _obj.State.Properties.QCDecision.IsVisible =                  false;
        _obj.State.Properties.QCDecisionDate.IsVisible =              false;
        _obj.State.Properties.ApprovalResult.IsVisible =              false;
        _obj.State.Properties.DateQualificationSelection.IsVisible =  false;
        return;
      }
      
      // Определяем вид/группу видов документа.
      var isPurchaseRequisition =             Equals(documentKindGuid, Constants.Module.PurchaseRequisitionKind);
      var isQualificationSelectionProtocol =  Equals(documentKindGuid, Constants.Module.QualificationSelectionProtocolKind);
      var isCounterpartyRegistryMemo =        Functions.TenderDocument.IsCounterpartyRegistryMemo(documentKindGuid);
      var isCounterpartyRegistryDecision =    Functions.TenderDocument.IsCounterpartyRegistryDecision(documentKindGuid);
      var isRegistryMemoOrDecision =          isCounterpartyRegistryMemo || isCounterpartyRegistryDecision;

      // Выставляем видимость/доступность/обязательность полей карточки.
      
      // НОР.
      _obj.State.Properties.BusinessUnit.IsVisible =  !isCounterpartyRegistryDecision;
      _obj.State.Properties.BusinessUnit.IsEnabled =  !isCounterpartyRegistryDecision;
      _obj.State.Properties.BusinessUnit.IsRequired = !isCounterpartyRegistryDecision;
      
      // Контрагенты.
      _obj.State.Properties.Counterparties.IsVisible =  true;
      _obj.State.Properties.Counterparties.IsEnabled =  !isCounterpartyRegistryDecision;
      _obj.State.Properties.Counterparties.IsRequired = !isPurchaseRequisition;
      
      // Автор.
      _obj.State.Properties.Author.IsVisible = isCounterpartyRegistryMemo;
      
      // Предмет тендерного отбора.
      _obj.State.Properties.TenderSelectionSubject.IsVisible =  !isRegistryMemoOrDecision;
      _obj.State.Properties.TenderSelectionSubject.IsEnabled =  !isRegistryMemoOrDecision;
      _obj.State.Properties.TenderSelectionSubject.IsRequired = !isRegistryMemoOrDecision;
      
      // ИСП.
      _obj.State.Properties.OurCF.IsVisible =   !isRegistryMemoOrDecision;
      _obj.State.Properties.OurCF.IsEnabled =   !isRegistryMemoOrDecision;
      _obj.State.Properties.OurCF.IsRequired =  false;
      
      // Объекты строительства.
      _obj.State.Properties.ObjectAnProjects.IsVisible =  !isRegistryMemoOrDecision;
      _obj.State.Properties.ObjectAnProjects.IsEnabled =  !isRegistryMemoOrDecision;
      _obj.State.Properties.ObjectAnProjects.IsRequired = false;
      
      // Процедура.
      _obj.State.Properties.Procedure.IsVisible =   isCounterpartyRegistryDecision;
      _obj.State.Properties.Procedure.IsEnabled =   false; // Заполняется программно.
      _obj.State.Properties.Procedure.IsRequired =  isCounterpartyRegistryDecision;
      
      // Решение по квалификации.
      _obj.State.Properties.QCDecision.IsVisible =   isCounterpartyRegistryDecision;
      _obj.State.Properties.QCDecision.IsEnabled =   false; // Заполняется программно.
      _obj.State.Properties.QCDecision.IsRequired =  isCounterpartyRegistryDecision;
      
      // Дата принятия решения.
      _obj.State.Properties.QCDecisionDate.IsVisible =   isCounterpartyRegistryDecision;
      _obj.State.Properties.QCDecisionDate.IsEnabled =   false; // Заполняется программно.
      _obj.State.Properties.QCDecisionDate.IsRequired =  isCounterpartyRegistryDecision;
      
      // Результаты голосования.
      _obj.State.Properties.ApprovalResult.IsVisible =   isCounterpartyRegistryDecision;
      _obj.State.Properties.ApprovalResult.IsEnabled =   false; // Заполняется программно.
      _obj.State.Properties.ApprovalResult.IsRequired =  isCounterpartyRegistryDecision;
      
      // Дата проведения квалификационного отбора.
      _obj.State.Properties.DateQualificationSelection.IsVisible =  isQualificationSelectionProtocol;
      _obj.State.Properties.DateQualificationSelection.IsEnabled =  isQualificationSelectionProtocol;
      _obj.State.Properties.DateQualificationSelection.IsRequired = isQualificationSelectionProtocol;
      
      // Администратором полностью доступна на редактирование карточка Решения комитета по квалификация.
      if (isCounterpartyRegistryDecision && Users.Current.IncludedIn(Roles.Administrators))
      {
        foreach(var property in _obj.State.Properties)
        {
          property.IsEnabled = true;
        }
        foreach(var property in _obj.State.Properties.ObjectAnProjects.Properties)
        {
          property.IsEnabled = true;
        }
      }
    }
    
    /// <summary>
    /// Получить guid вида документа.
    /// </summary>
    /// <param name="documentKind">Вид документа.</param>
    public static Guid GetDocumentKindGuid(Sungero.Docflow.IDocumentKind documentKind)
    {
      var purchaseRequisitionKindGuid =               Constants.Module.PurchaseRequisitionKind;
      var qualificationSelectionProtocolKindGuid =    Constants.Module.QualificationSelectionProtocolKind;
      var commercialOffersSummaryKindGuid =           Constants.Module.CommercialOffersSummaryKind;
      var decisionOnInclusionOfCounterpartyKindGuid = Constants.Module.DecisionOnInclusionOfCounterpartyKind;
      var decisionOnExclusionOfCounterpartyKindGuid = Constants.Module.DecisionOnExclusionOfCounterpartyKind;
      var memoOnInclusionOfCounterpartyKindGuid =     Constants.Module.MemoOnInclusionOfCounterpartyKind;
      var memoOnExclusionOfCounterpartyKindGuid =     Constants.Module.MemoOnExclusionOfCounterpartyKind;
      
      var purchaseRequisitionKind =               Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(purchaseRequisitionKindGuid);
      var qualificationSelectionProtocolKind =    Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(qualificationSelectionProtocolKindGuid);
      var commercialOffersSummaryKind =           Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(commercialOffersSummaryKindGuid);
      var decisionOnInclusionOfCounterpartyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(decisionOnInclusionOfCounterpartyKindGuid);
      var decisionOnExclusionOfCounterpartyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(decisionOnExclusionOfCounterpartyKindGuid);
      var memoOnInclusionOfCounterpartyKind =     Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(memoOnInclusionOfCounterpartyKindGuid);
      var memoOnExclusionOfCounterpartyKind =     Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(memoOnExclusionOfCounterpartyKindGuid);
      
      if (documentKind == null)
        return Guid.Empty;
      // Заявка на закупку.
      if (Equals(documentKind, purchaseRequisitionKind))
        return purchaseRequisitionKindGuid;
      // Протокол квалификационного отбора.
      if (Equals(documentKind, qualificationSelectionProtocolKind))
        return qualificationSelectionProtocolKindGuid;
      // Сводная таблица коммерческих предлдожений.
      if (Equals(documentKind, commercialOffersSummaryKind))
        return commercialOffersSummaryKindGuid;
      // Решение КК о включении КА в реестр.
      if (Equals(documentKind, decisionOnInclusionOfCounterpartyKind))
        return decisionOnInclusionOfCounterpartyKindGuid;
      // Решение КК об исключении КА из реестра.
      if (Equals(documentKind, decisionOnExclusionOfCounterpartyKind))
        return decisionOnExclusionOfCounterpartyKindGuid;
      // Служебная записка на включение в реестр.
      if (Equals(documentKind, memoOnInclusionOfCounterpartyKind))
        return memoOnInclusionOfCounterpartyKindGuid;
      // Служебная записка на исключение из реестра.
      if (Equals(documentKind, memoOnExclusionOfCounterpartyKind))
        return memoOnExclusionOfCounterpartyKindGuid;
      
      // Нераспознанные виды документов.
      throw new ArgumentException(lenspec.Tenders.TenderDocuments.Resources.CompleteTheListOfReturnGUIDs);
    }
    
    /// <summary>
    /// Признак вида документа – решения о включении/исключении КА?
    /// </summary>
    /// <param name="documentKindGuid">GUID вида документа.</param>
    /// <returns>true, если вид документа – решение о включении/исключении КА; Иначе – false.</returns>
    public static bool IsCounterpartyRegistryDecision(Guid documentKindGuid)
    {
      return
        documentKindGuid == Constants.Module.DecisionOnInclusionOfCounterpartyKind ||
        documentKindGuid == Constants.Module.DecisionOnExclusionOfCounterpartyKind;
    }
    
    /// <summary>
    /// Признак вида документа – СЗ на включение/исключение КА?
    /// </summary>
    /// <param name="documentKindGuid">GUID вида документа.</param>
    /// <returns>true, если вид документа – СЗ на включение/исключение КА; Иначе – false.</returns>
    public static bool IsCounterpartyRegistryMemo(Guid documentKindGuid)
    {
      return
        documentKindGuid == Constants.Module.MemoOnInclusionOfCounterpartyKind ||
        documentKindGuid == Constants.Module.MemoOnExclusionOfCounterpartyKind;
    }
    
    /// <summary>
    /// Проверка доступности действия "Создать PDF с установкой штампа".
    /// </summary>
    /// <param name="isCounterpartyRegistryDecision">Вид документа – решения о включении/исключении КА?</param>
    /// <param name="isAdministrator">Пользователь входит в роль "Администраторы"?</param>
    /// <returns>True, если действие доступно; Иначе – false;</returns>
    public static bool CanCreatePdfWithStampAvis(bool isCounterpartyRegistryDecision, bool isAdministrator)
    {
      return isCounterpartyRegistryDecision && isAdministrator;
    }
  }
}