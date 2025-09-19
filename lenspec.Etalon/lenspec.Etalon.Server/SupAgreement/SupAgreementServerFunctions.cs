using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.SupAgreement;

namespace lenspec.Etalon.Server
{
  partial class SupAgreementFunctions
  {
    
    public override StateView GetDocumentSummary()
    {
      var documentSummary = StateView.Create();
      var block = documentSummary.AddBlock();
      
      // Краткое имя документа.
      var documentName = _obj.DocumentKind.Name;
      if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
        documentName += OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
      
      if (_obj.RegistrationDate != null)
        documentName += OfficialDocuments.Resources.DateFrom + _obj.RegistrationDate.Value.ToString("d");
      
      block.AddLabel(documentName);
      
      // Типовое/Не типовое.
      var isStandardLabel = _obj.IsStandard.Value ? SupAgreements.Resources.IsStandartSupAgreement : SupAgreements.Resources.IsNotStandartSupAgreement;
      block.AddLabel(string.Format("({0})", isStandardLabel));
      block.AddLineBreak();
      block.AddEmptyLine();
      
      // НОР.
      block.AddLabel(string.Format("{0}: ", _obj.Info.Properties.BusinessUnit.LocalizedName));
      if (_obj.BusinessUnit != null)
        block.AddLabel(Hyperlinks.Get(_obj.BusinessUnit));
      else
        block.AddLabel("-");
      
      block.AddLineBreak();
      
      // Контрагент.
      block.AddLabel(string.Format("{0}: ", _obj.Info.Properties.Counterparty.LocalizedName));
      if (_obj.Counterparty != null)
      {
        block.AddLabel(Hyperlinks.Get(_obj.Counterparty));
        if (_obj.Counterparty.Nonresident == true)
          block.AddLabel(string.Format("({0})", _obj.Counterparty.Info.Properties.Nonresident.LocalizedName).ToLower());
      }
      else
      {
        block.AddLabel("-");
      }
      
      block.AddLineBreak();
      
      var contractSubject = !string.IsNullOrEmpty(_obj.SubjectContractavis) ? _obj.SubjectContractavis : "-";
      block.AddLabel(string.Format("{0}: {1}", _obj.Info.Properties.SubjectContractavis.LocalizedName, contractSubject));
      block.AddLineBreak();
      
      var amount = this.GetTotalAmountDocumentSummary(_obj.TotalAmount);
      var amountText = string.Format("{0}: {1}", _obj.Info.Properties.TotalAmount.LocalizedName, amount);
      block.AddLabel(amountText);
      block.AddLineBreak();
      
      var currencyText = string.Format("{0}: {1}", _obj.Info.Properties.Currency.LocalizedName, _obj.Currency);
      block.AddLabel(currencyText);
      block.AddLineBreak();
      
      // Срок действия.
      var validity = "-";
      var validFrom = _obj.ValidFrom.HasValue
        ? string.Format("{0} {1} ", Sungero.Contracts.ContractBases.Resources.From, _obj.ValidFrom.Value.ToShortDateString())
        : string.Empty;
      var validTill = _obj.ValidTill.HasValue
        ? string.Format("{0} {1}", Sungero.Contracts.ContractBases.Resources.Till, _obj.ValidTill.Value.ToShortDateString())
        : string.Empty;
      if (!string.IsNullOrEmpty(validFrom) || !string.IsNullOrEmpty(validTill))
        validity = string.Format("{0}{1}", validFrom, validTill);
      
      var validityText = string.Format("{0}: {1}", Sungero.Contracts.ContractBases.Resources.Validity, validity);
      block.AddLabel(validityText);
      block.AddEmptyLine();
      
      var note = !string.IsNullOrEmpty(_obj.Noteavis) ? _obj.Noteavis : "-";
      var noteText = string.Format("{0}: {1}", _obj.Info.Properties.Noteavis.LocalizedName, note);
      block.AddLabel(noteText);
      
      return documentSummary;
    }
    
    /// <summary>
    /// Посчитать остаточную сумму договора
    /// </summary>
    public void CalculateRemainingAmountLeadingContract()
    {
      var handler = avis.EtalonContracts.AsyncHandlers.CalculateRemainingAmount.Create();
      handler.ContractId = _obj.LeadingDocument.Id;
      handler.ExecuteAsync();
    }
    
    /// <summary>
    /// Расторгаем ведущий договор.
    /// </summary>
    [Public]
    public void TerminatedLeadingContract()
    {
      // Проверяем что ведущий договор указан.
      if (_obj.LeadingDocument == null)
        return;
      
      // Проверяем что ведущий договор не расторгнут.
      if (_obj.LeadingDocument.LifeCycleState == Sungero.Contracts.ContractBase.LifeCycleState.Terminated)
        return;
      
      // Меняем состояние ведущего договора на расторгнут.
      if (_obj.InternalApprovalState == lenspec.Etalon.SupAgreement.InternalApprovalState.Signed && _obj.IsTerminationavis == true)
      {
        _obj.LeadingDocument.LifeCycleState = Sungero.Contracts.ContractBase.LifeCycleState.Terminated;
        _obj.LeadingDocument.Save();
      }
    }
  }
}