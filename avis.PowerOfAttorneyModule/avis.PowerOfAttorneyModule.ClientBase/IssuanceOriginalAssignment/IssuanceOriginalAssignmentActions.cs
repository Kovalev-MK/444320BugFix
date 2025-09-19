using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.IssuanceOriginalAssignment;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class IssuanceOriginalAssignmentActions
  {

    public virtual void GetOriginal(Sungero.Domain.Client.ExecuteActionArgs e)
    {
//      var documents = _obj.State.Attachments.SelectedAttachments;
//      if(!documents.Any())
//      {
//        e.AddError(avis.PowerOfAttorneyModule.IssuanceOriginalAssignments.Resources.ErrorMessageEmptySelectedAttachments);
//        return;
//      }
//      if(!documents.All(x => _obj.POAs.PowerOfAttorneys.Select(i => i.Id).Contains(x.Id)))
//      {
//        e.AddError(avis.PowerOfAttorneyModule.IssuanceOriginalAssignments.Resources.ErrorMessageUncorrectGroup);
//        return;
//      }
//      var dialog = Dialogs.CreateInputDialog(avis.PowerOfAttorneyModule.IssuanceOriginalAssignments.Resources.IssuanceAssignmentDialogTitle);
//      foreach(var doc in documents)
//      {
//        var offDoc = Sungero.Docflow.OfficialDocuments.As(doc);
//        var link = dialog.AddHyperlink(offDoc.Name);
//        link.SetOnExecute(() => {doc.Show();});
//      }
//      var task = PowerOfAttorneyModule.ExecutionPowerOfAttorneys.As(_obj.Task);
//      var recipients = PowerOfAttorneyModule.Functions.ExecutionPowerOfAttorney.Remote.GetAttorneyPerformers(task);
//      var author = lenspec.Etalon.Employees.As(_obj.Author);
//      // Если автор задачи не был ранее добавлен в список Кому выдано, то добавить.
//      if (!recipients.Contains(author))
//        recipients.Add(author);
//      var selected = dialog.AddSelect(avis.PowerOfAttorneyModule.IssuanceOriginalAssignments.Resources.AddSelectTitleGetOrigignalActionDialog, true, Sungero.Company.Employees.Null).From(recipients);
//      if(dialog.Show() == DialogButtons.Ok)
//      {
//        foreach(var doc in documents)
//        {
//          var poa = lenspec.Etalon.PowerOfAttorneys.As(doc);
//          var line = poa.Tracking.AddNew();
//          line.Action = Sungero.Docflow.OfficialDocumentTracking.Action.Delivery;
//          line.DeliveredTo = selected.Value;
//          line.IsOriginal = true;
//          line.DeliveryDate = Calendar.Today;
//          line.ReturnDeadline = null;
//          poa.Save();
//        }
//        e.AddInformation(avis.PowerOfAttorneyModule.IssuanceOriginalAssignments.Resources.InformationMessageIssuanseOriginalAction);
//      }
    }

    public virtual bool CanGetOriginal(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Sungero.Company.Employees.Current.Id == _obj.Performer.Id || Sungero.Company.Employees.Current.IncludedIn(Roles.Administrators);
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var powerOfAttorneys = _obj.POAs.PowerOfAttorneys;
      foreach(var poa in powerOfAttorneys)
      {
        if(!poa.Tracking.Any())
        {
          e.AddError(avis.PowerOfAttorneyModule.IssuanceOriginalAssignments.Resources.ErrorMessageCompleteAction);
          return;
        }
      }
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}