using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApplicationBUDocumentBase;

namespace lenspec.EtalonDatabooks.Client
{
  partial class ApplicationBUDocumentBaseVersionsActions
  {
    
    //Добавлено Avis Expert
    public override void CreateDocumentFromVersion(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      base.CreateDocumentFromVersion(e);
    }

    public override bool CanCreateDocumentFromVersion(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return false;
    }

    public override void CreateVersion(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      base.CreateVersion(e);
    }

    public override bool CanCreateVersion(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return false;
    }
    //конец Добавлено Avis Expert

  }

  partial class ApplicationBUDocumentBaseActions
  {

    //Добавлено Avis Expert
    public override void CreateVersionFromLastVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateVersionFromLastVersion(e);
    }

    public override bool CanCreateVersionFromLastVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
    }

    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if(!_obj.HasVersions)
      {
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.ApplicationBUDocumentBases.Resources.NeedCreateDocumentVersion, lenspec.EtalonDatabooks.ApplicationBUDocumentBases.Resources.NeedCreateDocumentVersionDescription);
        return;
      }
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

    public virtual void FillFromEGRUL(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var apiKey = Functions.Module.Remote.GetConnectionParams(Etalon.Module.Integration.PublicConstants.Module.KonturFocusEGRULRecordCode);
      if (apiKey == string.Empty)
      {
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.ApplicationBUDocumentBases.Resources.FailedToGetConnectionString, MessageType.Warning);
        return;
      }
      
      var businessUnitName = Functions.Module.Remote.GetKonturFocusBaseRequisites(apiKey, _obj.TIN);
      if (businessUnitName == null)
      {
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.ApplicationBUDocumentBases.Resources.NoOrganizationWithThisTIN, MessageType.Warning);
        return;
      }
      _obj.BusinessUnitName = businessUnitName;
      _obj.LifeCycleState = ApplicationBUDocumentBase.LifeCycleState.Active;
      var error = PublicFunctions.Module.Remote.GetExcerptEGRUL(_obj.TIN, _obj);
      if (!string.IsNullOrEmpty(error))
      {
        e.AddError(error);
        return;
      }
      _obj.Save();
    }

    public virtual bool CanFillFromEGRUL(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }
    //конец Добавлено Avis Expert

  }

}