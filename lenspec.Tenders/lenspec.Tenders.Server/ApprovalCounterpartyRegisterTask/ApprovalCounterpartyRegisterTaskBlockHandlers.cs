using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using lenspec.Tenders.ApprovalCounterpartyRegisterTask;

namespace lenspec.Tenders.Server.ApprovalCounterpartyRegisterTaskBlocks
{

  partial class ProcessingOfApprovalResultsBlockHandlers
  {

    public virtual void ProcessingOfApprovalResultsBlockStart()
    {
      _block.Text = _obj.ApprovalResult;
    }

    public virtual void ProcessingOfApprovalResultsBlockCompleteAssignment(lenspec.Tenders.IProcessingOfApprovalResultsAssignment assignment)
    {
      
    }
  }

  partial class CommitteeApprovalBlockHandlers
  {

    public virtual void CommitteeApprovalBlockCompleteAssignment(lenspec.Tenders.ICommitteeApprovalAssignment assignment)
    {
      if (assignment.Result == Tenders.CommitteeApprovalAssignment.Result.Forward)
        assignment.Forward(assignment.ForwardTo, ForwardingLocation.Next, assignment.ForwardDeadline);
      
      var documents = new List<Sungero.Content.IElectronicDocument>();
      documents.AddRange(assignment.CounterpartyDocumentGroup.CounterpartyDocuments.ToList());
      documents.AddRange(assignment.TenderDocumentGroup.TenderDocuments.ToList());
      documents.AddRange(assignment.JustificationDocumentGroup.TenderDocuments.ToList());
      documents.AddRange(assignment.JustificationDocumentGroup.CounterpartyDocuments.ToList());
      
      foreach (var document in documents)
        Sungero.Docflow.PublicFunctions.Module.GrantAccessRightsOnDocument(document, assignment.Task.Author, DefaultAccessRightsTypes.Read);
      
      var attachments = Functions.ApprovalCounterpartyRegisterTask.GetDocumentsFromSubtasks(_obj, assignment);
      foreach (var document in attachments)
      {
        if
          (
            _obj.JustificationDocumentGroup.TenderDocuments.Contains(document) ||
            _obj.JustificationDocumentGroup.CounterpartyDocuments.Contains(document) ||
            _obj.CounterpartyDocumentGroup.CounterpartyDocuments.Contains(document) ||
            _obj.QualificationFormGroup.TenderAccreditationForms.Contains(document) ||
            _obj.TenderDocumentGroup.TenderDocuments.Contains(document)
           )
          continue;
        
        if (Sungero.Docflow.CounterpartyDocuments.Is(document))
          _obj.JustificationDocumentGroup.CounterpartyDocuments.Add(Sungero.Docflow.CounterpartyDocuments.As(document));
        else if (lenspec.Tenders.TenderDocuments.Is(document))
          _obj.JustificationDocumentGroup.TenderDocuments.Add(lenspec.Tenders.TenderDocuments.As(document));
      }
    }
  }

  partial class ApprovalResultsSummaryBlockHandlers
  {

    public virtual void ApprovalResultsSummaryBlockExecute()
    {
      var approvalResults = Functions.ApprovalCounterpartyRegisterTask.GetApprovalResultsCount(_obj);
      _obj.ApprovalResult = lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Resources.ApprovalResultsCountFormat(approvalResults.Approved, approvalResults.Rejected);
    }
  }

}