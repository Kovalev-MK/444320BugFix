using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ApprovalCounterpartyRegisterTask;

namespace lenspec.Tenders
{
  partial class ApprovalCounterpartyRegisterTaskSharedHandlers
  {

    public virtual void ProcedureChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      Functions.ApprovalCounterpartyRegisterTask.FillName(_obj);
    }

    public virtual void CounterpartyChanged(lenspec.Tenders.Shared.ApprovalCounterpartyRegisterTaskCounterpartyChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      // Обновление имени и вложенной организации.
      Functions.ApprovalCounterpartyRegisterTask.FillName(_obj);
      _obj.CounterpartyGroup.Companies.Clear();
      
      if (e.NewValue != null)
        _obj.CounterpartyGroup.Companies.Add(e.NewValue);
    }

    public virtual void CounterpartyGroupAdded(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      // Добавление выписки из ЕГРЮЛ/ЕГРИП.
      var counterpartyDocuments = lenspec.Etalon.PublicFunctions.CounterpartyDocument.Remote.GetCounterpartyDocuments(
        _obj.Counterparty,
        new List<System.Guid>(){ lenspec.Etalon.Module.Parties.PublicConstants.Module.ExtractFromEGRULKind });
      
      if (counterpartyDocuments.Any())
      {
        foreach (var counterpartyDocument in counterpartyDocuments)
          _obj.CounterpartyDocumentGroup.CounterpartyDocuments.Add(counterpartyDocument);
      }
      // Создание выписки из ЕГРЮЛ/ЕГРИП.
      else
      {
        var counterpartyDocument = lenspec.Etalon.PublicFunctions.CounterpartyDocument.Remote.CreateExtractFromEGRUL(_obj.Counterparty);
        if (counterpartyDocument != null)
          _obj.CounterpartyDocumentGroup.CounterpartyDocuments.Add(counterpartyDocument);
      }
    }

    public override void SubjectChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != null && e.NewValue.Length > Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.Subject.Length)
        _obj.Subject = e.NewValue.Substring(0, Tenders.ApprovalCounterpartyRegisterTasks.Info.Properties.Subject.Length);
      
      if (string.IsNullOrWhiteSpace(e.NewValue))
        _obj.Subject = Sungero.DocflowApproval.Resources.AutoformatTaskSubject;
    }

  }
}