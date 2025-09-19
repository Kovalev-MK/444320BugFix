using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderDocument;

namespace lenspec.Tenders
{
  partial class TenderDocumentSharedHandlers
  {

    //Добавлено Avis Expert
    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      var documentKindGuid = Functions.TenderDocument.GetDocumentKindGuid(e.NewValue);
      e.Params.AddOrUpdate(Constants.TenderDocument.Params.DocumentKindGuid, documentKindGuid);
      
      // Решение КК о включении КА в реестр.
      if (Equals(documentKindGuid, Constants.Module.DecisionOnInclusionOfCounterpartyKind))
        _obj.Procedure = TenderDocument.Procedure.Inclusion;
      // Решение КК об исключении КА из реестра.
      else if (Equals(documentKindGuid, Constants.Module.DecisionOnExclusionOfCounterpartyKind))
        _obj.Procedure = TenderDocument.Procedure.Exclusion;

      // Доступность действия создания PDF со штампом.
      var isCounterpartyRegistryDecision = Functions.TenderDocument.IsCounterpartyRegistryDecision(documentKindGuid);
      var isAdministrator = false;
      e.Params.TryGetValue(Constants.TenderDocument.Params.IsAdministrator, out isAdministrator);
      var canCreatePdfWithStamp = Functions.TenderDocument.CanCreatePdfWithStampAvis(isCounterpartyRegistryDecision, isAdministrator);
      e.Params.AddOrUpdate(Constants.TenderDocument.Params.CanCreatePDFWithStamp, canCreatePdfWithStamp);

      DropHiddenFields(documentKindGuid);
    }
    
    /// <summary>
    /// Обнулить скрытые с карточки поля в зависимости от вида документа.
    /// </summary>
    /// <param name="documentKindGuid">GUID вида документа.</param>
    private void DropHiddenFields(Guid documentKindGuid)
    {
      // Определяем вид/группу видов документа.
      var isPurchaseRequisition =             Equals(documentKindGuid, Constants.Module.PurchaseRequisitionKind);
      var isQualificationSelectionProtocol =  Equals(documentKindGuid, Constants.Module.QualificationSelectionProtocolKind);
      var isCounterpartyRegistryMemo =        Functions.TenderDocument.IsCounterpartyRegistryMemo(documentKindGuid);
      var isCounterpartyRegistryDecision =    Functions.TenderDocument.IsCounterpartyRegistryDecision(documentKindGuid);
      var isRegistryMemoOrDecision =          isCounterpartyRegistryMemo || isCounterpartyRegistryDecision;
      
      // Решение о включении/исключении КА.
      if (isCounterpartyRegistryDecision)
        if (_obj.BusinessUnit != null)  { _obj.BusinessUnit = null; }
      
      // [НЕ] решение о включении/исключении КА.
      if (!isCounterpartyRegistryDecision)
      {
        if (_obj.Procedure != null)       { _obj.Procedure = null;      }
        if (_obj.QCDecision != null)      { _obj.QCDecision = null;     }
        if (_obj.QCDecisionDate != null)  { _obj.QCDecisionDate = null; }
        if (_obj.ApprovalResult != null)  { _obj.ApprovalResult = null; }
      }
      
      // Решение/СЗ о включении/исключении КА.
      if (isRegistryMemoOrDecision)
      {
        if (_obj.TenderSelectionSubject != null)  { _obj.TenderSelectionSubject = null; }
        if (_obj.OurCF != null)                   { _obj.OurCF.Clear();                 }
        if (_obj.ObjectAnProjects != null)        { _obj.ObjectAnProjects.Clear();      }
      }
      
      // [НЕ] протокол квалификационного отбора.
      if (!isQualificationSelectionProtocol)
        if (_obj.DateQualificationSelection != null) { _obj.DateQualificationSelection = null; }
      
      // Для СЗ на включение/исключение проставить признак.
      _obj.IsQualificationDocumentlenspec = isCounterpartyRegistryMemo;
    }
    
    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.Constants.Module.SimpleRelationName, e.OldValue, e.NewValue);
    }
    //конец Добавлено Avis Expert

  }
}