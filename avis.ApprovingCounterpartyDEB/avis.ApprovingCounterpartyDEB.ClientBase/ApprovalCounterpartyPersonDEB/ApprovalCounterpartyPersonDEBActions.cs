using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB;

namespace avis.ApprovingCounterpartyDEB.Client
{
  partial class ApprovalCounterpartyPersonDEBActions
  {
    public override void Resume(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var counterparty = _obj.ApprovalDatabook.Counterparties.SingleOrDefault();
      if (counterparty != null)
      {
        var activeTask = Functions.ApprovalCounterpartyPersonDEB.Remote.GetActiveTaskByCounterparty(counterparty);
        if (activeTask != null)
        {
          var errorMessage = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.ErrorMessageHereIsActiveTaskFormat(activeTask.Started.Value.ToShortDateString(), activeTask.Author);
          e.AddError(errorMessage);
          return;
        }
      }
      if (Functions.ApprovalCounterpartyPersonDEB.CheckDuplicates(_obj))
        return;
      base.Resume(e);
    }

    public override bool CanResume(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanResume(e);
    }

    public override void Restart(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var counterparty = _obj.ApprovalDatabook.Counterparties.SingleOrDefault();
      if (counterparty != null)
      {
        var activeTask = Functions.ApprovalCounterpartyPersonDEB.Remote.GetActiveTaskByCounterparty(counterparty);
        if (activeTask != null)
        {
          var errorMessage = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.ErrorMessageHereIsActiveTaskFormat(activeTask.Started.Value.ToShortDateString(), activeTask.Author);
          e.AddError(errorMessage);
          return;
        }
      }
      if (Functions.ApprovalCounterpartyPersonDEB.CheckDuplicates(_obj))
        return;
      base.Restart(e);
    }

    public override bool CanRestart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanRestart(e);
    }

    public override void Start(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var counterparty = _obj.ApprovalDatabook.Counterparties.SingleOrDefault();
      if (counterparty != null)
      {
        var activeTask = Functions.ApprovalCounterpartyPersonDEB.Remote.GetActiveTaskByCounterparty(counterparty);
        if (activeTask != null)
        {
          var errorMessage = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.ErrorMessageHereIsActiveTaskFormat(activeTask.Started.Value.ToShortDateString(), activeTask.Author);
          e.AddError(errorMessage);
          return;
        }
      }
      if (Functions.ApprovalCounterpartyPersonDEB.CheckDuplicates(_obj))
        return;
      
      // Актуализировать Выписки из ЕГРЮЛ по КА.
      var extractFromEGRULKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(lenspec.Etalon.Module.Parties.PublicConstants.Module.ExtractFromEGRULKind);
      var extractFromEGRUL = _obj.CounterpartyInfo.All.Select(x => lenspec.Etalon.CounterpartyDocuments.As(x)).Where(x => x != null && x.DocumentKind == extractFromEGRULKind && x.Counterparty != null);
      var changedDocuments = string.Empty;
      foreach (var document in extractFromEGRUL)
      {
        try
        {
          if (!lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.CheckForCounterpartyChangesEGRUL(document.Counterparty.TIN, document.CustomDocumentDatelenspec))
          {
            var error = lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.GetExcerptEGRUL(document.Counterparty.TIN, document);
            if (!string.IsNullOrEmpty(error))
              throw new ApplicationException(error);
            
            document.CustomDocumentDatelenspec = Calendar.Today;
            document.Save();
            changedDocuments += string.Format("{0}{1} (ИД {2})", Environment.NewLine, document.Name, document.Id);
          }
        }
        catch(Exception ex)
        {
          e.AddError(string.Format(avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.UpdateDocumentsErrorMessageFormat(document.Name, document.Id, ex.Message)));
        }
      }
      if (!string.IsNullOrEmpty(changedDocuments) &&
          !Dialogs.CreateConfirmDialog(avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.StartTaskConfirmMessage,
                                       avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.DocumentsHaveBeenUpdatedFormat(changedDocuments)).Show())
        return;
      
      base.Start(e);
    }

    public override bool CanStart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanStart(e);
    }

  }


}