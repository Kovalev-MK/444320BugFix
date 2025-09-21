using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.EDSApplication;

namespace lenspec.ElectronicDigitalSignatures.Client
{
  partial class EDSApplicationActions
  {
    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Если по документу ранее были запущены задачи, то вывести соответствующий диалог.
      if (lenspec.Etalon.PublicFunctions.ApprovalTask.CheckDuplicates(_obj, false))
        return;
      
      if (_obj.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.InitialIssue)
      {
        var passportKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(lenspec.ElectronicDigitalSignatures.PublicConstants.Module.PassportKind);
        var isExistingPassport = _obj.Relations.GetRelatedDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName)
          .Any(x => Sungero.Docflow.OfficialDocuments.Is(x) && Sungero.Docflow.OfficialDocuments.As(x).DocumentKind == passportKind);

        if (!isExistingPassport)
        {
          Dialogs.ShowMessage(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.NeedUploadEDSDocuments, MessageType.Error);
          return;
        }
      }
      
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

    public virtual bool CanShowCancellationInstruction(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var canExecute = Equals(Users.Current, _obj.Author) || Equals(Users.Current, _obj.PreparedBy);
      
      return _obj.IsSelfCancellation == true && canExecute &&
        _obj.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Cancellation;
    }

    public virtual void ShowCancellationInstruction(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.EDSApplication.ShowCancellationInstruction();
    }

    public virtual bool CanShowRenewalInstruction(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var canExecute = Equals(Users.Current, _obj.Author) || Equals(Users.Current, _obj.PreparedBy);
      
      return canExecute && _obj.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Renewal;
    }

    public virtual void ShowRenewalInstruction(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.EDSApplication.ShowRenewalInstruction();
    }

    public virtual void UploadingDocuments(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      
      var passportKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.PassportKind);
      var isExistingPassport = _obj.Relations.GetRelatedDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName)
        .Any(x => Sungero.Docflow.OfficialDocuments.Is(x) && Sungero.Docflow.OfficialDocuments.As(x).DocumentKind == passportKind);
      if (isExistingPassport)
      {
        e.AddError(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.EDSDocumentsUploaded);
        return;
      }
      
      var consentToProcessingKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.ConsentToProcessingKind);
      var template = Sungero.Docflow.DocumentTemplates
        .GetAll(x => x.Status == Sungero.Docflow.DocumentTemplate.Status.Active && x.DocumentKinds.Any(d => d.DocumentKind == consentToProcessingKind))
        .FirstOrDefault();
      if (template == null)
      {
        e.AddError(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.NotFoundTemplateForConsentToProcessing);
        return;
      }
      
      var zip = Functions.EDSApplication.Remote.CreateConsentToProcessing(_obj, consentToProcessingKind, template);
      if (zip == null)
      {
        e.AddError(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.ErrorExportZipForConsentToProcessing);
        return;
      }
      zip.Export();
      
      var dialog = Dialogs.CreateInputDialog(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.UploadingDocumentsDialog);
      dialog.Text = lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.UploadingDocumentsDialogText;
      dialog.Width = 1000;
      var person = dialog.AddSelect(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.PersonDialogParam, true, _obj.PreparedBy?.Person);
      person.IsEnabled = false;
      var passportFile = dialog.AddFileSelect(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.PassportDialogParam, true).WithPlaceholder(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.ChooseFile);
      var consentToProcessingFile = dialog.AddFileSelect(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.ConsentToProcessingDialogParam, true).WithPlaceholder(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.ChooseFile);
      dialog.Buttons.AddOkCancel();
      if (dialog.Show() == DialogButtons.Ok)
      {
        #region Паспорт РФ
        
        var passport = Functions.EDSApplication.Remote.CreatePassport(_obj);
        var fileContent = passportFile.Value.Content;
        // Сохранить содержимое файла в новую версию документа.
        using (var memory = new System.IO.MemoryStream(fileContent))
        {
          var extension = System.IO.Path.GetExtension(passportFile.Value.Name);
          if (extension.Length > 0)
            extension = extension.Substring(1);
          
          passport.CreateVersionFrom(memory, extension);
          passport.Save();
        }
        
        #endregion
        
        #region Согласие на обработку ПД
        
        var consentToProcessing = _obj.Relations.GetRelatedDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName)
          .Where(x => Sungero.Docflow.OfficialDocuments.Is(x) && Sungero.Docflow.OfficialDocuments.As(x).DocumentKind == consentToProcessingKind)
          .FirstOrDefault();
        fileContent = consentToProcessingFile.Value.Content;
        // Сохранить содержимое файла в новую версию документа.
        using (var memory = new System.IO.MemoryStream(fileContent))
        {
          var extension = System.IO.Path.GetExtension(consentToProcessingFile.Value.Name);
          if (extension.Length > 0)
            extension = extension.Substring(1);
          
          consentToProcessing.CreateVersionFrom(memory, extension);
          consentToProcessing.Save();
        }
        
        #endregion
        
        _obj.Save();
      }
    }

    public virtual bool CanUploadingDocuments(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var canExecute = Equals(Users.Current, _obj.Author) || Equals(Users.Current, _obj.PreparedBy);
      
      return !_obj.State.IsInserted && _obj.PreparedBy != null && canExecute &&
        _obj.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.InitialIssue;
    }

  }

}