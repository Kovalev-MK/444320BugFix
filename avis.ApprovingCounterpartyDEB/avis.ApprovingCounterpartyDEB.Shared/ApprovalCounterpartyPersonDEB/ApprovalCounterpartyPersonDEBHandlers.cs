using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalCounterpartyPersonDEBSharedHandlers
  {

    public virtual void ApprovalDatabookDeleted(Sungero.Workflow.Interfaces.AttachmentDeletedEventArgs e)
    {
      RemoveCounterpartyDocuments(e.Attachment);
    }

    public virtual void ApprovalDatabookAdded(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      PullUpCounterpartyDocuments(e.Attachment);
    }

    public virtual void ApprovalDatabookCreated(Sungero.Workflow.Interfaces.AttachmentCreatedEventArgs e)
    {
      PullUpCounterpartyDocuments(e.Attachment);
    }
    
    /// <summary>
    /// Прикрепить связанные сведения о контрагенте.
    /// </summary>
    /// <param name="attachment">Вложение.</param>
    private void PullUpCounterpartyDocuments(Sungero.Domain.Shared.IEntity attachment)
    {
      var counterpartyDocuments = GetCounterpatryDocuments(attachment);
      
      foreach (var counterpartyDocument in counterpartyDocuments)
      {
        if (!_obj.CounterpartyInfo.CounterpartyDocuments.Any(d => Equals(d, counterpartyDocument)))
          _obj.CounterpartyInfo.CounterpartyDocuments.Add(counterpartyDocument);
      }
    }
    
    private void RemoveCounterpartyDocuments(Sungero.Domain.Shared.IEntity attachment)
    {
      var counterpartyDocuments = GetCounterpatryDocuments(attachment);
      
      foreach (var counterpartyDocument in counterpartyDocuments)
      {
        if (_obj.CounterpartyInfo.CounterpartyDocuments.Any(d => d.Equals(counterpartyDocument)))
          _obj.CounterpartyInfo.CounterpartyDocuments.Remove(counterpartyDocument);
      }
      _obj.Save();
    }

    /// <summary>
    /// Получить сведения о контрагенте.
    /// </summary>
    /// <param name="attachment">Вложение.</param>
    /// <returns>Сведения о вложенном контрагенте.</returns>
    private List<lenspec.Etalon.ICounterpartyDocument> GetCounterpatryDocuments(Sungero.Domain.Shared.IEntity attachment)
    {
      if (!Sungero.Parties.CompanyBases.Is(attachment))
        return new List<lenspec.Etalon.ICounterpartyDocument>();
      
      var documentKindGuids = new List<System.Guid>()
      {
        lenspec.Etalon.Module.Parties.PublicConstants.Module.ConstituentDocumentKind,
        lenspec.Etalon.Module.Parties.PublicConstants.Module.CharterAndChangesKind,
        lenspec.Etalon.Module.Parties.PublicConstants.Module.ExtractFromEGRULKind
      };
      
      var companyBase = Sungero.Parties.CompanyBases.As(attachment);
      return lenspec.Etalon.PublicFunctions.CounterpartyDocument.Remote.GetCounterpartyDocuments(companyBase, documentKindGuids);
    }
    
    public virtual void ApprovalDocumentDeleted(Sungero.Workflow.Interfaces.AttachmentDeletedEventArgs e)
    {
      
    }

    public virtual void ApprovalDocumentAdded(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      var document = ApprovalCounterpartyBases.As(e.Attachment);
      _obj.Subject = string.Format("Согласование от {0} контрагента/банка/персоны с ДБ. Контрагент: {1}", Calendar.Today.ToShortDateString(), document.Counterparty);
      _obj.Counterparty = document.Counterparty;
      _obj.ManagerInitiator = avis.ApprovingCounterpartyDEB.PublicFunctions.ApprovalCounterpartyPersonDEB.GetOnlyMangerForApprovalDEB(document);
      _obj.ApprovalDatabook.Counterparties.Add(document.Counterparty);
      var downCastedDocument = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEBs.As(e.Attachment);
      if (downCastedDocument != null)
      {
        _obj.IsNeedQualification = downCastedDocument.IsNeedQualification;
        if (downCastedDocument.EstimatedAmountTransaction.HasValue && downCastedDocument.EstimatedAmountTransaction < Constants.ApprovalCounterpartyBankDEB.MinAmountVerification)
          _obj.CounterpartyLimit = Constants.ApprovalCounterpartyBankDEB.MinAmountVerification - 0.01;
      }
      
      
      var counterpartyDocuments = Sungero.Docflow.CounterpartyDocuments.GetAll(x => Equals(x.Counterparty, _obj.Counterparty)).ToList();
      
      // Если среди сведений о КА нет Выписки из ЕГРЮЛ, то создать ее.
      var extractFromEGRULKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(lenspec.Etalon.Module.Parties.PublicConstants.Module.ExtractFromEGRULKind);
      if (!counterpartyDocuments.Any(x => x.DocumentKind == extractFromEGRULKind))
      {
        var extractFromEGRUL = lenspec.Etalon.CounterpartyDocuments.Create();
        extractFromEGRUL.DocumentKind = extractFromEGRULKind;
        extractFromEGRUL.Counterparty = document.Counterparty;
        extractFromEGRUL.Subject = "Выписка из ЕГРЮЛ";
        counterpartyDocuments.Add(extractFromEGRUL);
      }
      
      Functions.ApprovalCounterpartyPersonDEB.Remote.ActualizeCounterpatyDocuments(counterpartyDocuments.ToList());
      
      foreach (var counterpartyDoc in counterpartyDocuments)
      {
        if (!_obj.CounterpartyInfo.All.Contains(counterpartyDoc))
          _obj.CounterpartyInfo.All.Add(counterpartyDoc);
      }
    }

  }
}