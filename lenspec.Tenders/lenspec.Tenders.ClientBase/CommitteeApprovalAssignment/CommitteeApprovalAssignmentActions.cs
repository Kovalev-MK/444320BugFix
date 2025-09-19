using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.CommitteeApprovalAssignment;

namespace lenspec.Tenders.Client
{
  partial class CommitteeApprovalAssignmentActions
  {
    public virtual void AddApprover(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog(Sungero.DocflowApproval.EntityApprovalAssignments.Resources.AddApprover);
      var activeAssignments = Functions.CommitteeApprovalAssignment.Remote.GetActiveAssignments(_obj);
      var employee = dialog.AddSelect<Sungero.Company.IEmployee>(Sungero.DocflowApproval.EntityApprovalAssignments.Resources.Approver, true, null)
        .Where(x => x.Status != Sungero.Company.Employee.Status.Closed && !Equals(x, _obj.Performer) &&
               !activeAssignments.Any(p => Equals(p.Performer, x)));
      var defaultDeadline = Sungero.Docflow.PublicFunctions.Module.CheckDeadline(_obj.Deadline, Calendar.Now) ? _obj.Deadline : null;
      var deadline = dialog.AddDate(Sungero.DocflowApproval.EntityApprovalAssignments.Resources.AddApproverDeadline, _obj.Deadline.HasValue, defaultDeadline).AsDateTime();
      var addButton = dialog.Buttons.AddCustom(Sungero.DocflowApproval.EntityApprovalAssignments.Resources.Add);
      dialog.Buttons.AddCancel();
      dialog.SetOnButtonClick(a =>
                              {
                                if (a.IsValid && a.Button == addButton)
                                {
                                  if (Functions.CommitteeApprovalAssignment.Remote.CanForwardTo(_obj, employee.Value))
                                  {
                                    var documents = new List<Sungero.Content.IElectronicDocument>();
                                    documents.AddRange(_obj.TenderDocumentGroup.TenderDocuments.ToList());
                                    documents.AddRange(_obj.CounterpartyDocumentGroup.CounterpartyDocuments.ToList());
                                    documents.AddRange(_obj.JustificationDocumentGroup.TenderDocuments.ToList());
                                    documents.AddRange(_obj.JustificationDocumentGroup.CounterpartyDocuments.ToList());
                                    foreach (var document in documents)
                                      Sungero.Docflow.PublicFunctions.Module.GrantAccessRightsOnDocument(document, _obj.Task.Author, DefaultAccessRightsTypes.Read);
                                    
                                    Sungero.DocflowApproval.PublicFunctions.Module.Remote.AddApprover(_obj, employee.Value, deadline.Value);
                                    var employeeShortName = Sungero.Company.PublicFunctions.Employee.GetShortName(employee.Value, DeclensionCase.Nominative, false);
                                    Dialogs.NotifyMessage(Sungero.DocflowApproval.EntityApprovalAssignments.Resources.NewApproverAddedFormat(employeeShortName));
                                  }
                                  else
                                    a.AddError(Sungero.DocflowApproval.EntityApprovalAssignments.Resources.ApproverAlreadyExistsFormat(employee.Value.Person.ShortName));
                                }
                              });
      
      dialog.SetOnRefresh((r) =>
                          {
                            if (!Sungero.Docflow.PublicFunctions.Module.CheckDeadline(employee.Value ?? Users.Current, deadline.Value, Calendar.Now))
                              r.AddError(Sungero.DocflowApproval.EntityApprovalAssignments.Resources.ImpossibleSpecifyDeadlineLessThanToday, deadline);
                            else
                            {
                              var warnMessage = Sungero.Docflow.PublicFunctions.Module.CheckDeadlineByWorkCalendar(employee.Value ?? Users.Current, deadline.Value);
                              if (!string.IsNullOrEmpty(warnMessage))
                                r.AddWarning(warnMessage);
                            }
                          });
      
      var result = dialog.Show();
    }

    public virtual bool CanAddApprover(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.Status == Status.InProcess && _obj.AccessRights.CanUpdate() && !Sungero.Docflow.PublicFunctions.Module.IsCompetitiveWorkStarted(_obj);
    }

    public virtual void Forward(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (!e.Validate())
        return;
      
      if (!Functions.CommitteeApprovalAssignment.ShowForwardingDialog(_obj))
        e.Cancel();
    }

    public virtual bool CanForward(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }



    public virtual void Rejected(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanRejected(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Approved(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanApproved(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}