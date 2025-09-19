using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.CounterpartyDocument;

namespace lenspec.Etalon.Client
{
  partial class CounterpartyDocumentVersionsActions
  {
    public override void ConvertVersionToPdfavis(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      base.ConvertVersionToPdfavis(e);
    }

    public override bool CanConvertVersionToPdfavis(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return false;
    }

    public override void CreateVersion(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      base.CreateVersion(e);
    }

    public override bool CanCreateVersion(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      bool isExtractFromEGRULKind;
      if (e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind)
        return false;
      
      return base.CanCreateVersion(e);
    }

    public override void CreateDocumentFromVersion(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      base.CreateDocumentFromVersion(e);
    }

    public override bool CanCreateDocumentFromVersion(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      bool isExtractFromEGRULKind;
      if (e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind)
        return false;
      
      return base.CanCreateDocumentFromVersion(e);
    }

    public override void ImportVersion(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      base.ImportVersion(e);
    }

    public override bool CanImportVersion(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      bool isExtractFromEGRULKind;
      if (e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind)
        return false;
      
      return base.CanImportVersion(e);
    }

  }

  partial class CounterpartyDocumentActions
  {
    public override void CreateFromTemplate(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateFromTemplate(e);
    }

    public override bool CanCreateFromTemplate(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      bool isExtractFromEGRULKind;
      if (e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind)
        return false;
      
      return base.CanCreateFromTemplate(e);
    }

    public override void CreateFromScanner(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateFromScanner(e);
    }

    public override bool CanCreateFromScanner(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      bool isExtractFromEGRULKind;
      if (e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind)
        return false;
      
      return base.CanCreateFromScanner(e);
    }

    public override void CreateFromFile(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateFromFile(e);
    }

    public override bool CanCreateFromFile(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      bool isExtractFromEGRULKind;
      if (e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind)
        return false;
      
      return base.CanCreateFromFile(e);
    }

    public override void ScanInNewVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ScanInNewVersion(e);
    }

    public override bool CanScanInNewVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      bool isExtractFromEGRULKind;
      if (e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind)
        return false;
      
      return base.CanScanInNewVersion(e);
    }

    public override void ImportInLastVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ImportInLastVersion(e);
    }

    public override bool CanImportInLastVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      bool isExtractFromEGRULKind;
      if (e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind)
        return false;
      
      return base.CanImportInLastVersion(e);
    }

    public override void ImportInNewVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ImportInNewVersion(e);
    }

    public override bool CanImportInNewVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      bool isExtractFromEGRULKind;
      if (e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind)
        return false;
      
      return base.CanImportInNewVersion(e);
    }

    public override void CreateVersionFromLastVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateVersionFromLastVersion(e);
    }

    public override bool CanCreateVersionFromLastVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      bool isExtractFromEGRULKind;
      if (e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind)
        return false;
      
      return base.CanCreateVersionFromLastVersion(e);
    }

    public virtual void FillFromEGRULlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var counterparty = _obj.Counterparty;
      if (counterparty == null)
      {
        e.AddWarning(lenspec.Etalon.CounterpartyDocuments.Resources.NeedFillCounterparty);
        return;
      }
      
      if (_obj.HasVersions)
      {
        // Запрос 1.
        try
        {
          var changesIsActual = lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.CheckForCounterpartyChangesEGRUL(counterparty.TIN, _obj.CustomDocumentDatelenspec);
          if (changesIsActual && !Dialogs.CreateConfirmDialog(lenspec.Etalon.CounterpartyDocuments.Resources.FillFromEGRULConfirmMessage).Show())
            return;
        }
        catch(Exception ex)
        {
          e.AddError(ex.Message);
          return;
        }
      }
      // Запрос 2.
      var error = lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.GetExcerptEGRUL(counterparty.TIN, _obj);
      if (!string.IsNullOrEmpty(error))
      {
        e.AddError(error);
        return;
      }
      _obj.CustomDocumentDatelenspec = Calendar.Today;
      _obj.Subject = string.Empty;
    }

    public virtual bool CanFillFromEGRULlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      bool isExtractFromEGRULKind;
      return _obj.AccessRights.CanUpdate() && _obj.AccessRights.CanUpdateBody() &&
        e.Params.TryGetValue(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, out isExtractFromEGRULKind) && isExtractFromEGRULKind;
    }

    public virtual void ShowDuplicateslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.CounterpartyDocument.Remote.GetDuplicates(_obj);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(CounterpartyDocuments.Resources.DuplicatesNotFound);
    }

    public virtual bool CanShowDuplicateslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }
}