using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorney;

namespace lenspec.Etalon.Client
{
  partial class PowerOfAttorneyActions
  {
    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверка на дубликаты задач
      if (Functions.ApprovalTask.CheckDuplicates(_obj, true))
        return;
      
      // Вид документа "Заявка на оформление нотариальной доверенности".
      var requestToCreateNotarialPowerOfAttorneyKind = Sungero.Docflow.PublicFunctions.DocumentKind
        .GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
      
      // Для ВЭД "Заявка на оформление нотариальной доверенности"
      // проверяем наличие связанного документа поверенных.
      if (Equals(_obj.DocumentKind, requestToCreateNotarialPowerOfAttorneyKind))
      {
        // Проверка наличия хотя бы одного документа вида "Документы поверенных".
        var relatedAndRelatedFromDocuments = _obj.Relations.GetRelatedAndRelatedFromDocuments();
        var documentAttorneyKind = Sungero.Docflow.PublicFunctions.DocumentKind
          .GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.DocumentAttorneyKindGuid);
        var hasDocumentAttorney = relatedAndRelatedFromDocuments
          .Where(x => Sungero.Docflow.OfficialDocuments.Is(x) && Equals(documentAttorneyKind, Sungero.Docflow.OfficialDocuments.As(x).DocumentKind))
          .Any();
        
        if (!hasDocumentAttorney)
        {
          e.AddError(lenspec.Etalon.PowerOfAttorneys.Resources.RelatedDocumentsAttorneyNotFound);
          return;
        }
      }
      
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

    public override void ShowRegistrationPane(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ShowRegistrationPane(e);
    }

    public override bool CanShowRegistrationPane(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanShowRegistrationPane(e) && _obj.IsProjectPOAavis != true;
    }

    public override bool CanCancelRegistration(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCancelRegistration(e) && _obj.IsProjectPOAavis != true;
    }

    public override void CancelRegistration(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CancelRegistration(e);
    }

    public override void AssignNumber(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.AssignNumber(e);
    }

    public override bool CanAssignNumber(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanAssignNumber(e) && _obj.IsProjectPOAavis != true;
    }

    public override void ChangeRequisites(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ChangeRequisites(e);
    }

    public override bool CanChangeRequisites(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanChangeRequisites(e) && _obj.IsProjectPOAavis != true;
    }


    public override void Register(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Register(e);
    }

    public override bool CanRegister(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanRegister(e) && _obj.IsProjectPOAavis != true;
    }

    
    //Avis-Expert>>
    
    public virtual void SendForApprovalPOAavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      var tasksWithThisDocument = avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.GetAll(t => t.AttachmentDetails.Any(i => i.AttachmentId == _obj.Id) &&
                                                                                              t.Status != avis.PowerOfAttorneyModule.ExecutionPowerOfAttorney.Status.Aborted);
      if(tasksWithThisDocument.Any())
      {
        e.AddError(lenspec.Etalon.PowerOfAttorneys.Resources.ErrorMessageActionSendForApprovalPOA);
        return;
      }
      var task = avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Create();
      task.ProjectPOA.PowerOfAttorneys.Add(_obj);
      task.Show();
    }

    public virtual bool CanSendForApprovalPOAavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.IsProjectPOAavis == true;
    }

    public override void CreateFromFile(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateFromFile(e);
    }

    public override bool CanCreateFromFile(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var hasRights = false;
      e.Params.TryGetValue(lenspec.Etalon.Constants.Docflow.PowerOfAttorney.Params.IsInRoleRightsToAttachScans, out hasRights);
      return hasRights;
    }

    public virtual void AddAttorneyDocumentsavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if(_obj.IssuedTo == null && _obj.IssuedToParty == null && !_obj.Representatives.Any())
      {
        e.AddError(lenspec.Etalon.PowerOfAttorneys.Resources.ErrorMessageAddAttorneyDocuments);
        return;
      }
      var dialog = Dialogs.CreateInputDialog(lenspec.Etalon.PowerOfAttorneys.Resources.AddAttorneyDocumentsDialogTitle, lenspec.Etalon.PowerOfAttorneys.Resources.AddAttorneyDocumentsDialogText);
      
      var issuedTo = string.Empty;
      if (_obj.Representatives.Any())
      { 
        var issuedToNames = _obj.Representatives.Where(x => x.AgentType == Sungero.Docflow.PowerOfAttorneyBaseRepresentatives.AgentType.Person && x.IssuedTo != null).Select(x => x.IssuedTo.Name);
        issuedTo = string.Join(", ", issuedToNames);
      }
      else
        issuedTo = _obj.IssuedTo != null ? _obj.IssuedTo.Name : _obj.IssuedToParty.Name;
      
      var attorneysLable = dialog.AddMultilineString("Поверенные", false, string.Empty);
      attorneysLable.Value = issuedTo;
      attorneysLable.IsEnabled = false;
      
      var fileSelected = dialog.AddFileSelectMany(lenspec.Etalon.PowerOfAttorneys.Resources.AddAttorneyDocumentsDialogSelectManyTitle, true);
      dialog.Buttons.AddOkCancel();
      if(dialog.Show() == DialogButtons.Ok)
      {
        foreach (var selected in fileSelected.Value)
        {
          byte[] content;
          using (var stream = selected.OpenReadStream())
          {
            using (var memory = new System.IO.MemoryStream())
            {
              stream.CopyTo(memory);
              content = memory.ToArray();
              var contentModel = Sungero.Docflow.Structures.Module.ByteArray.Create(content);
              Functions.PowerOfAttorney.Remote.CreateAddendumPowerOfAttorney(_obj, selected.FileName, contentModel);
            }
          }
        }
        e.AddInformation("Документы поверенных загружены.");
      }
    }

    public virtual bool CanAddAttorneyDocumentsavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
    //<<Avis-Expert
  }
}