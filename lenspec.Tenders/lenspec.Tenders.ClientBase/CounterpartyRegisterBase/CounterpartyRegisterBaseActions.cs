using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.CounterpartyRegisterBase;

namespace lenspec.Tenders.Client
{
  partial class CounterpartyRegisterBaseActions
  {
    public virtual void UploadInformationFromQuestionnaire(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog(lenspec.Tenders.CounterpartyRegisterBases.Resources.UploadCounterpartyData);
      var counterparty = dialog.AddSelect(Sungero.Parties.Counterparties.Info.LocalizedName, false, _obj.Counterparty);
      counterparty.IsEnabled = false;
      var tenderAccreditationForms = _obj.Counterparty == null ?
        Tenders.TenderAccreditationForms.GetAll() :
        Tenders.TenderAccreditationForms.GetAll(x => Equals(x.Counterparty, _obj.Counterparty));
      var tenderAccreditationForm = dialog
        .AddSelect(lenspec.Tenders.CounterpartyRegisterBases.Resources.ChooseTenderAccreditationForm, true, Tenders.TenderAccreditationForms.Null)
        .From(tenderAccreditationForms);
      
      tenderAccreditationForm.WithPlaceholder(
        tenderAccreditationForms.Any() ?
        string.Empty :
        lenspec.Tenders.CounterpartyRegisterBases.Resources.TenderAccreditationFormsNotFoundForCounterparty
       );
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        var isChanged = _obj.State.IsChanged;
        var error = Functions.CounterpartyRegisterBase.Remote.UploadCounterpartyData(_obj, tenderAccreditationForm.Value);
        if (!string.IsNullOrEmpty(error))
          e.AddError(error);
        else if (_obj.State.IsChanged && _obj.State.IsChanged != isChanged)
        {
          try
          {
            _obj.Save();
            Dialogs.ShowMessage(lenspec.Tenders.CounterpartyRegisterBases.Resources.UploadInformationFromQuestionnaireResultMessage, MessageType.Information);
          }
          catch(Exception)
          {
            Dialogs.ShowMessage(lenspec.Tenders.CounterpartyRegisterBases.Resources.UploadInformationFromQuestionnaireErrorMessage, MessageType.Error);
          }
        }
      }
    }

    public virtual bool CanUploadInformationFromQuestionnaire(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && _obj.AccessRights.CanUpdate();
    }

    public virtual void ShowQualificationDocuments(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Tenders.SpecialFolders.QualificationDocuments.Items.Show();
    }

    public virtual bool CanShowQualificationDocuments(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

    public virtual void ShowContractualDocuments(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var documents = new List<Sungero.Docflow.IOfficialDocument>();
      if (_obj.Counterparty != null)
      {
        // ДД и ЗНО.
        documents.AddRange(Sungero.Docflow.ContractualDocumentBases.GetAll(x => Equals(x.Counterparty, _obj.Counterparty))
                           .Where(x => Sungero.Contracts.ContractualDocuments.Is(x) || lenspec.ApplicationsForPayment.ApplicationForPayments.Is(x))
                           .ToList<Sungero.Docflow.IOfficialDocument>());
        // Вх. счета.
        documents.AddRange(Sungero.Contracts.IncomingInvoices.GetAll(x => Equals(x.Counterparty, _obj.Counterparty)).ToList<Sungero.Docflow.IOfficialDocument>());
      }
      
      documents.Show();
    }

    public virtual bool CanShowContractualDocuments(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

  }

}