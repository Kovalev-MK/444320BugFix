using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Contract;

namespace lenspec.Etalon.Server
{
  partial class ContractFunctions
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
    /// Получить наши организации для фильтрации подходящих прав подписи.
    /// </summary>
    /// <returns>Наши организации.</returns>
    public override List<Sungero.Company.IBusinessUnit> GetBusinessUnits()
    {
      // Проверка вхождения в роль "Пользователи с правами на указание в документах сотрудников из любых НОР".
      var roleSid = lenspec.EtalonDatabooks.PublicConstants.Module.RightsToSelectAnyEmployees;
      var hasRightsToSelectAnyEmployees = Users.Current.IncludedIn(roleSid);
      
      // Если сотрудник входит в роль, отправляем пустой список НОР в GetSignatureSettingsQuery().
      // Доступные для выбора сотрудники по НОР не фильтруются (см. Functions.SignatureSetting.GetSignatureSettings()).
      if (hasRightsToSelectAnyEmployees)
        return new List<Sungero.Company.IBusinessUnit>() { };
      else
        return base.GetBusinessUnits();
    }

    /// <summary>
    /// Высчитать остаточную сумму договора
    /// </summary>
    [Public]
    public void CalculateRemainingAmount()
    {
      var remainintAmount = default(double);
      AccessRights.AllowRead(() =>
                             {
                               var supAgreements = lenspec.Etalon.SupAgreements.GetAll(x => Equals(x.LeadingDocument, _obj) && x.TotalAmount.HasValue && x.TotalAmount.Value != default(double) &&
                                                                                       (x.LifeCycleState == lenspec.Etalon.SupAgreement.LifeCycleState.Active ||
                                                                                        x.LifeCycleState == lenspec.Etalon.SupAgreement.LifeCycleState.Closed));
                               var incomingInvoices = lenspec.Etalon.IncomingInvoices.GetAll(x => Equals(x.Contact, _obj) && x.TotalAmount.HasValue && x.TotalAmount.Value != default(double) &&
                                                                                             Equals(_obj.Counterparty, x.Counterparty));
                               var supAgreementsAmount = default(double);
                               var incomingInvoicesAmount = default(double);
                               var contractAmount = _obj.TotalAmount.HasValue ? _obj.TotalAmount.Value : 0;
                               if (supAgreements.Any())
                                 supAgreementsAmount = supAgreements.Sum(x => x.TotalAmount.Value);
                               if (incomingInvoices.Any())
                                 incomingInvoicesAmount = incomingInvoices.Sum(x => x.TotalAmount.Value);
                               remainintAmount = contractAmount + supAgreementsAmount - incomingInvoicesAmount;
                             });
      _obj.RemainingAmountlenspec = remainintAmount;
    }
  }
}