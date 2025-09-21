using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.FormLetterAndRecallPowerOfAttorney;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class FormLetterAndRecallPowerOfAttorneyActions
  {
    public virtual void RecallPowerOfAttorney(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var documents = _obj.State.Attachments.SelectedAttachments;
      if (!documents.Any())
      {
        e.AddError(avis.PowerOfAttorneyModule.RecallPowerOfAttorneyAssignments.Resources.ErrorMessageEmptySelectedAttachments);
        return;
      }
      if (!documents.All(x => _obj.PowerOfAttorney.PowerOfAttorneys.Select(i => i.Id).Contains(x.Id)))
      {
        e.AddError(avis.PowerOfAttorneyModule.RecallPowerOfAttorneyAssignments.Resources.ErrorMessageEmptySelectedPowerOfAttorneys);
        return;
      }
      var requestRecall = _obj.MainAttachment.ApplicationRelinquishmentAuthorities.SingleOrDefault();
      var dialog = Dialogs.CreateInputDialog(avis.PowerOfAttorneyModule.RecallPowerOfAttorneyAssignments.Resources.DialogRecallTitle);
      var reasonRecall = dialog.AddString("Причина", true);
      if (dialog.Show() == DialogButtons.Ok)
      {
        foreach(var document in documents)
        {
          var powerOfAttorney = lenspec.Etalon.PowerOfAttorneys.As(document);
          if (powerOfAttorney.DateAbortPOAavis != null)
          {
            e.AddWarning(string.Format("Доверенность с номером '{0}' была отозвана ранее.", powerOfAttorney.RegistrationNumber ?? "незарегестрированная доверенность"));
            continue;
          }
          powerOfAttorney.RevokeReasonavis = reasonRecall.Value;
          powerOfAttorney.DateAbortPOAavis = requestRecall.RevocationDate;
          powerOfAttorney.IsRevokedavis = true;
          powerOfAttorney.LifeCycleState = lenspec.Etalon.PowerOfAttorney.LifeCycleState.Obsolete;
          powerOfAttorney.Save();
        }
        e.AddInformation(avis.PowerOfAttorneyModule.RecallPowerOfAttorneyAssignments.Resources.InformationMessageRecallPoas);
      }
    }

    public virtual bool CanRecallPowerOfAttorney(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void FormOutgoingLetter(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var task = RecallPowerOfAttorneys.As(_obj.Task);
      var letter = lenspec.Etalon.OutgoingLetters.Create();
      letter.Correspondent = task.Attorney.Person;
      letter.Show();
    }

    public virtual bool CanFormOutgoingLetter(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void Forward(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (_obj.Forward == null)
      {
        e.AddError(avis.PowerOfAttorneyModule.RecallPowerOfAttorneyAssignments.Resources.ErrorMessageEmptyForward);
        return;
      }
      
      if (_obj.Forward == Sungero.Company.Employees.Current)
      {
        e.AddError(avis.PowerOfAttorneyModule.RecallPowerOfAttorneyAssignments.Resources.ErrorMessageOldPerformer);
        return;
      }
    }

    public virtual bool CanForward(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var poas = _obj.PowerOfAttorney.PowerOfAttorneys.Select(x => lenspec.Etalon.PowerOfAttorneys.As(x)).ToList();
      foreach (var item in poas)
      {
        if (!item.DateAbortPOAavis.HasValue)
        {
          e.AddError(avis.PowerOfAttorneyModule.FormLetterAndRecallPowerOfAttorneys.Resources.ErrorMessageNeedFillDateAbortPOA);
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