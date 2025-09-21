using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using avis.PowerOfAttorneyModule.RecallPowerOfAttorney;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class RecallPowerOfAttorneyRouteHandlers
  {

    public virtual void CompleteAssignment10(avis.PowerOfAttorneyModule.IFormLetterAndRecallPowerOfAttorney assignment, avis.PowerOfAttorneyModule.Server.FormLetterAndRecallPowerOfAttorneyArguments e)
    {
      if (assignment.Result == PowerOfAttorneyModule.FormLetterAndRecallPowerOfAttorney.Result.Forward)
        assignment.Forward(assignment.Forward);
      
      if (assignment.Result == PowerOfAttorneyModule.FormLetterAndRecallPowerOfAttorney.Result.Complete)
      {
        var parallelAssignments = ApprovingCounterpartyDEB.PublicFunctions.Module.GetParallelAssignments(assignment);
        var activeParallelAssignments = parallelAssignments
          .Where(a => a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
          .Where(a => !Equals(a, assignment))
          .Where(a => FormLetterAndRecallPowerOfAttorneys.Is(a));
        foreach (var parallelAssignment in activeParallelAssignments)
        {
          if (!string.IsNullOrEmpty(parallelAssignment.ActiveText))
            parallelAssignment.ActiveText += Environment.NewLine;
          
          if (assignment.CompletedBy != null && !assignment.CompletedBy.Equals(assignment.Performer))
            parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserWithOnBehalfOfFormat(assignment.CompletedBy.Name,
                                                                                                                            assignment.Performer.Name);
          else
            parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserFormat(assignment.Performer.Name);

          parallelAssignment.Abort();
        }
      }
    }

    public virtual void StartBlock9(Sungero.Workflow.Server.NoticeArguments e)
    {
      var application = _obj.MainAttachment.ApplicationRelinquishmentAuthorities.SingleOrDefault();
      e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.SubjectNotify, application.BusinessUnit, _obj.Attorney, _obj.Created.Value.ToShortDateString());
      e.Block.Performers.Add(_obj.Attorney);
    }

    public virtual void StartBlock10(avis.PowerOfAttorneyModule.Server.FormLetterAndRecallPowerOfAttorneyArguments e)
    {
      e.Block.RelativeDeadlineDays = 1;
      var application = _obj.MainAttachment.ApplicationRelinquishmentAuthorities.SingleOrDefault();
      e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.SubjectFormLetterAndRecallPowerOfAttorneyAssignment, application.BusinessUnit, _obj.Attorney.Name, _obj.Created.Value.ToShortDateString());
      var performer = Roles.GetAll(x => x.Sid == Constants.Module.RoleGuidResponsibleLawyer).SingleOrDefault();
      e.Block.Performers.Add(performer);
    }

    public virtual void CompleteAssignment7(avis.PowerOfAttorneyModule.IRecallPowerOfAttorneyAssignment assignment, avis.PowerOfAttorneyModule.Server.RecallPowerOfAttorneyAssignmentArguments e)
    {
      if (assignment.Result == PowerOfAttorneyModule.RecallPowerOfAttorneyAssignment.Result.Forward)
        assignment.Forward(assignment.Forward);
      
      if (assignment.Result == PowerOfAttorneyModule.RecallPowerOfAttorneyAssignment.Result.Complete)
      {
        var parallelAssignments = ApprovingCounterpartyDEB.PublicFunctions.Module.GetParallelAssignments(assignment);
        var activeParallelAssignments = parallelAssignments
          .Where(a => a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
          .Where(a => !Equals(a, assignment))
          .Where(a => RecallPowerOfAttorneyAssignments.Is(a));
        foreach (var parallelAssignment in activeParallelAssignments)
        {
          if (!string.IsNullOrEmpty(parallelAssignment.ActiveText))
            parallelAssignment.ActiveText += Environment.NewLine;
          
          if (assignment.CompletedBy != null && !assignment.CompletedBy.Equals(assignment.Performer))
            parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserWithOnBehalfOfFormat(assignment.CompletedBy.Name,
                                                                                                                            assignment.Performer.Name);
          else
            parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserFormat(assignment.Performer.Name);

          parallelAssignment.Abort();
        }
      }
    }

    public virtual void StartBlock7(avis.PowerOfAttorneyModule.Server.RecallPowerOfAttorneyAssignmentArguments e)
    {
      e.Block.RelativeDeadlineDays = 1;
      var application = _obj.MainAttachment.ApplicationRelinquishmentAuthorities.SingleOrDefault();
      e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.SubjectSigningAssignment, _obj.Attorney, _obj.Created.Value.ToShortDateString());
      var performer = Roles.GetAll(x => x.Sid == Constants.Module.RoleGuidResponsibleLawyer).SingleOrDefault();
      e.Block.Performers.Add(performer);
    }

    public virtual void Script5Execute()
    {
      var application = _obj.MainAttachment.ApplicationRelinquishmentAuthorities.SingleOrDefault();
      var body = Sungero.Docflow.PublicFunctions.OfficialDocument.GetBodyToConvertToPdf(application, application.LastVersion, true);
      if (body == null || body.Body == null || body.Body.Length == 0)
      {
        Sungero.Docflow.PublicFunctions.Module.LogPdfConverting("Cannot get version body", application, application.LastVersion);
        return;
      }
      
      System.IO.Stream pdfDocumentStream = null;
      using (var inputStream = new System.IO.MemoryStream(body.Body))
      {
        try
        {
          var signature = Signatures.Get(application).FirstOrDefault(x => x.SignatureType == SignatureType.Approval);
          var htmlStamp = string.Empty;
          var logo = string.Empty;
          if (signature.SignCertificate != null)
          {
            htmlStamp = Sungero.Docflow.Resources.HtmlStampTemplateForCertificate;
            logo = Sungero.Docflow.Resources.HtmlStampLogoForCertificate;
            logo = logo.Replace("{Image}", Sungero.Docflow.Resources.SignatureStampSampleLogo);
            htmlStamp = htmlStamp.Replace("{Logo}", logo);
            htmlStamp = htmlStamp.Replace("{Title}", Sungero.Docflow.Resources.SignatureStampSampleTitle);
            htmlStamp = htmlStamp.Replace("{SignatoryFullName}", signature.Signatory.Name);
            htmlStamp = htmlStamp.Replace("{Thumbprint}", signature.SignCertificate.Thumbprint.ToLower());
            htmlStamp = htmlStamp.Replace("{Validity}", $"с {signature.SignCertificate.NotBefore.Value.ToShortDateString()} по {signature.SignCertificate.NotAfter.Value.ToShortDateString()}");
            htmlStamp = htmlStamp.Replace("{SigningDate}", string.Empty);
          }
          else
          {
            htmlStamp = Sungero.Docflow.Resources.HtmlStampTemplateForSignature;
            logo = Sungero.Docflow.Resources.HtmlStampLogoForSignature;
            logo = logo.Replace("{Image}", Sungero.Docflow.Resources.SignatureStampSampleLogo);
            htmlStamp = htmlStamp.Replace("{Logo}", logo);
            htmlStamp = htmlStamp.Replace("{Title}", Sungero.Docflow.Resources.SignatureStampSampleTitle);
            htmlStamp = htmlStamp.Replace("{SignatoryFullName}", signature.Signatory.Name);
            htmlStamp = htmlStamp.Replace("{SignatoryId}", signature.Signatory.Id.ToString());
            htmlStamp = htmlStamp.Replace("{SigningDate}", string.Empty);
          }
          
          pdfDocumentStream = Sungero.Docflow.IsolatedFunctions.PdfConverter.GeneratePdf(inputStream, body.Extension);
          pdfDocumentStream = Sungero.Docflow.IsolatedFunctions.PdfConverter.AddSignatureStamp(pdfDocumentStream, body.Extension, htmlStamp, Sungero.Docflow.Resources.SignatureMarkAnchorSymbol,
                                                                                               Sungero.Docflow.Constants.Module.SearchablePagesLimit);
        }
        catch (Exception ex)
        {
          if (ex is AppliedCodeException)
            Logger.Error(Sungero.Docflow.Resources.PdfConvertErrorFormat(application.Id), ex.InnerException);
          else
            Logger.Error(Sungero.Docflow.Resources.PdfConvertErrorFormat(application.Id), ex);
        }
      }
      application.LastVersion.PublicBody.Write(pdfDocumentStream);
      application.LastVersion.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension(Sungero.Docflow.Constants.OfficialDocument.Extensions.Pdf);
      pdfDocumentStream.Close();
      application.Save();
    }

    public virtual void StartBlock4(avis.PowerOfAttorneyModule.Server.SigningAssignmentArguments e)
    {
      e.Block.RelativeDeadlineDays = 1;
      e.Block.Subject = string.Format(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.SubjectSigningAssignment, _obj.Attorney, _obj.Created.Value.ToShortDateString());
      e.Block.Performers.Add(_obj.Attorney);
    }

    public virtual bool Decision3Result()
    {
      return lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(_obj.Attorney);
    }

  }
}