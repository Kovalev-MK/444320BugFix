using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractualDocument;

namespace lenspec.Etalon.Client
{
  partial class ContractualDocumentVersionsActions
  {
    public override void ImportVersion(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      base.ImportVersion(e);
      
      var contractualDocument = Etalon.ContractualDocuments.As(e.RootEntity);
      Functions.ContractualDocument.ChangeIsStandardProperty(contractualDocument);
    }

    public override bool CanImportVersion(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return base.CanImportVersion(e);
    }

  }

  partial class ContractualDocumentActions
  {
    public virtual void CreateCopyOfIntragroupDocumentlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var document = Functions.ContractualDocument.Remote.FillFromIntragroupDocument(_obj);
      // Параметр для добавления связи в событии сохранения, т.к. не все поля копируются из другого документа и сохранить данный документ в текущий момент не удастся
      e.Params.AddOrUpdate(Constants.Contracts.ContractualDocument.Params.CreateFromDocument, _obj.Id);
      ((Sungero.Domain.Shared.IExtendedEntity)document).Params[Constants.Contracts.ContractualDocument.Params.IsCreatedFromIntragroupDocument] = true;
      
      document.Show();
    }

    public virtual bool CanCreateCopyOfIntragroupDocumentlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && !_obj.State.IsChanged;
    }

    public override void ScanInNewVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ScanInNewVersion(e);
    }

    public override bool CanScanInNewVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanScanInNewVersion(e);
    }

    public virtual void CalculateIntoAccountSupAgreementlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var report = avis.EtalonContracts.Reports.GetAmountContractualDocument();
      report.DocumentId = _obj.Id;
      report.Open();
    }

    public virtual bool CanCalculateIntoAccountSupAgreementlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Sungero.Contracts.Contracts.Is(_obj) || Sungero.Contracts.SupAgreements.Is(_obj);
    }

    public virtual void CreateFromlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Введите ИД договорного документа");
      var inputId = dialog.AddString("ИД", true);
      dialog.Buttons.AddOkCancel();
      if (dialog.Show() == DialogButtons.Ok)
      {
        long id = 0;
        var isValidValueId = long.TryParse(inputId.Value, out id);
        var errorMessage = Functions.ContractualDocument.Remote.CheckDocumentAvailabilityAndType(_obj, id);
        if (!isValidValueId || !string.IsNullOrEmpty(errorMessage))
        {
          e.AddError(errorMessage);
          return;
        }
        Functions.ContractualDocument.Remote.FillFromAnotherDocument(_obj, id);
        // Параметр для добавления связи в событии сохранения, т.к. не все поля копируются из другого документа и сохранить данный документ в текущий момент не удасться
        e.Params.AddOrUpdate(Constants.Contracts.ContractualDocument.Params.CreateFromDocument, id);
      }
    }

    public virtual bool CanCreateFromlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверка на дубликаты задач
      if (lenspec.Etalon.PublicFunctions.ApprovalTask.CheckDuplicates(_obj, false))
        return;
      
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

    public override void ShowRegistrationPane(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.State.Properties.SyncStatus1cavis.IsVisible == true)
        _obj.State.Properties.SyncStatus1cavis.IsVisible = false;
      else
        _obj.State.Properties.SyncStatus1cavis.IsVisible = true;
      
      base.ShowRegistrationPane(e);
    }

    public override bool CanShowRegistrationPane(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanShowRegistrationPane(e);
    }

    
    /// <summary>
    /// ������� ����������.
    /// </summary>
    /// <param name="e"></param>
    public override void CreateAddendum(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      //base.CreateAddendum(e);
      
      var attachmentContractDoucment = avis.EtalonContracts.AttachmentContractDocuments.Create();
      
      attachmentContractDoucment.LeadingDocument = _obj;
      
      attachmentContractDoucment.Show();
    }

    public override bool CanCreateAddendum(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCreateAddendum(e);
    }

    public override void CreateFromTemplate(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      e.Params.Add(Constants.Contracts.ContractualDocument.Params.CreateFromAction, true);
      
      base.CreateFromTemplate(e);
      
      e.Params.Remove(Constants.Contracts.ContractualDocument.Params.CreateFromAction);
    }

    public override bool CanCreateFromTemplate(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCreateFromTemplate(e);
    }

    public override void ImportInLastVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      e.Params.Add(Constants.Contracts.ContractualDocument.Params.CreateFromAction, true);
      
      base.ImportInLastVersion(e);
      
      e.Params.Remove(Constants.Contracts.ContractualDocument.Params.CreateFromAction);
      
      var isCalledFromCollection = CallContext.CalledDirectlyFromCollection(Sungero.Docflow.OfficialDocuments.Info);
      if (isCalledFromCollection)
        Functions.ContractualDocument.ChangeIsStandardProperty(_obj);
    }

    public override bool CanImportInLastVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanImportInLastVersion(e);
    }

    public override void ImportInNewVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      e.Params.Add(Constants.Contracts.ContractualDocument.Params.CreateFromAction, true);
      
      base.ImportInNewVersion(e);
      
      e.Params.Remove(Constants.Contracts.ContractualDocument.Params.CreateFromAction);
      
      var isCalledFromCollection = CallContext.CalledDirectlyFromCollection(Sungero.Docflow.OfficialDocuments.Info);
      if (isCalledFromCollection)
        Functions.ContractualDocument.ChangeIsStandardProperty(_obj);
    }

    public override bool CanImportInNewVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanImportInNewVersion(e);
    }

    public override void CreateFromScanner(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      e.Params.Add(Constants.Contracts.ContractualDocument.Params.CreateFromAction, true);
      
      base.CreateFromScanner(e);
      
      e.Params.Remove(Constants.Contracts.ContractualDocument.Params.CreateFromAction);
    }

    public override bool CanCreateFromScanner(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCreateFromScanner(e);
    }

    public override void CreateFromFile(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      e.Params.Add(Constants.Contracts.ContractualDocument.Params.CreateFromAction, true);
      
      base.CreateFromFile(e);
      
      e.Params.Remove(Constants.Contracts.ContractualDocument.Params.CreateFromAction);
    }

    public override bool CanCreateFromFile(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCreateFromFile(e);
    }

    public virtual void Unload1Cavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      bool contractUloaded    = true;
      bool contractOsUloaded  = true;
      contractUloaded = PublicFunctions.ContractualDocument.UnloadingContracts(_obj);
      if (contractUloaded)
      {
        //Если есть ОС то и их выгрузим
        if (_obj.ConstructionObjectsavis.Count() > 0)
        {
          contractOsUloaded = PublicFunctions.ContractualDocument.UnloadingContractsDetails(_obj);
          if (!contractOsUloaded)
          {
            Dialogs.ShowMessage(lenspec.Etalon.ContractualDocuments.Resources.ErrorContractOsUnload, MessageType.Warning);
          }
        }
      }
      else Dialogs.ShowMessage(lenspec.Etalon.ContractualDocuments.Resources.ErrorContractUnload, MessageType.Warning);
      if (contractUloaded && contractOsUloaded)
      {
        PublicFunctions.ContractualDocument.UpdateContractSyns(_obj);
      }
    }
    public virtual bool CanUnload1Cavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (_obj.InternalApprovalState == lenspec.Etalon.ContractualDocument.InternalApprovalState.Signed)
        return true;
      
      return false;
    }
  }
}