using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalRegistrationAssignment;

namespace lenspec.Etalon.Client
{
  partial class ApprovalRegistrationAssignmentActions
  {
    //Добавлено Avis Expert
    
    public virtual void SendActionItemlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanSendActionItemlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowUnregisteredAddendasavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var mainDocument = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var addendas = _obj.AddendaGroup.OfficialDocuments
        .Where(x => Sungero.RecordManagement.OutgoingLetters.Is(x) &&
               Sungero.RecordManagement.OutgoingLetters.As(x).RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered);
      addendas.Show();
    }

    public virtual bool CanShowUnregisteredAddendasavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var isMassMailingApplication = false;
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        isMassMailingApplication = lenspec.OutgoingLetters.MassMailingApplications.Is(document);
      }
      return isMassMailingApplication;
    }

    public virtual void RegisterDocumentAddendaavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var addendas = _obj.AddendaGroup.OfficialDocuments.Where(x => Sungero.RecordManagement.OutgoingLetters.Is(x) &&
                                                               x.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered)
        .ToList();
      if (!addendas.Any())
      {
        e.AddError(lenspec.Etalon.ApprovalRegistrationAssignments.Resources.DocumentsToRegisterNotFound);
        return;
      }
      
      var error = lenspec.OutgoingLetters.PublicFunctions.Module.RegisterListOfDocuments(addendas);
      if (!string.IsNullOrEmpty(error))
      {
        e.AddError(error);
        return;
      }
      else
      {
        Dialogs.ShowMessage(lenspec.Etalon.ApprovalRegistrationAssignments.Resources.AddendasAreRegistered, MessageType.Information);
        _obj.Save();
      }

    }

    public virtual bool CanRegisterDocumentAddendaavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var isMassMailingApplication = false;
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        isMassMailingApplication = lenspec.OutgoingLetters.MassMailingApplications.Is(document);
      }
      return isMassMailingApplication;
    }

    public virtual bool CanRegisterDocumentavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      return document != null && (document.RegistrationState == null ||
                                  (document.RegistrationState != null && document.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered));
    }

    public virtual void RegisterDocumentavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        if (document.RegistrationState != null && document.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered)
        {
          e.AddError(lenspec.Etalon.ApprovalRegistrationAssignments.Resources.DocumentIsRegistered);
          return;
        }
        var error = Etalon.Client.OfficialDocumentFunctions.RegisterDocumentAvis(Etalon.OfficialDocuments.As(document));
        if (!string.IsNullOrEmpty(error))
        {
          e.AddError(error);
          return;
        }
      }
    }

    public override void Execute(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.First();
      
      #region Проверить регистрацию документа

      var registrationState = document.RegistrationState;
      if (registrationState == null || (Enumeration)registrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered)
      {
        e.AddError(Sungero.Docflow.ApprovalTasks.Resources.ToPerformNeedRegisterDocument, _obj.Info.Actions.RegisterDocumentavis);
        return;
      }
      
      #endregion
      
      
      #region Обновление полей в теле документа
      
      var isBodyUpdated = Functions.ApprovalRegistrationAssignment.Remote.UpdateDocumentBody(_obj);
      if (!string.IsNullOrEmpty(isBodyUpdated) && !isBodyUpdated.Equals(lenspec.Etalon.ApprovalTasks.Resources.FailedToUpdateFieldsInDocumentsPrefix))
      {
        e.AddError(isBodyUpdated);
        return;
      }
      
      #endregion
      
      
      #region Заявка на массовую рассылку
      
      //      if (lenspec.OutgoingLetters.MassMailingApplications.Is(document))
      //      {
      //        var mainDocument = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      //        var notRegisteredAddenda = mainDocument.Relations.GetRelated(Sungero.Docflow.Constants.Module.AddendumRelationName)
      //          .Select(x => Sungero.RecordManagement.OutgoingLetters.As(x))
      //          .Where(x => x != null && x.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered);
      //        if (notRegisteredAddenda != null && notRegisteredAddenda.Any())
      //        {
      //          e.AddError(lenspec.Etalon.ApprovalRegistrationAssignments.Resources.AddendaMustBeRegistered, _obj.Info.Actions.ShowUnregisteredAddendasavis);
      //          return;
      //        }
      //      }
      
      if (lenspec.OutgoingLetters.MassMailingApplications.Is(document))
      {
        var notRegisteredAddenda = _obj.AddendaGroup.OfficialDocuments.Where(x => Sungero.RecordManagement.OutgoingLetters.Is(x) && x.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered);
        if (notRegisteredAddenda != null && notRegisteredAddenda.Any())
        {
          e.AddError(lenspec.Etalon.ApprovalRegistrationAssignments.Resources.AddendaMustBeRegistered, _obj.Info.Actions.RegisterDocumentAddendaavis);
          return;
        }
      }
      
      #endregion
      
      
      #region Проверка, что среди адресатов нет сотрудников без УЗ, если активно действие “Несколько адресатов”
      
      //Проверим, является ли документ входящим письмом
      if (Etalon.IncomingLetters.Is(document))
      {
        var docletter = Etalon.IncomingLetters.As(document);
        var users = new List<Sungero.Company.IEmployee>();
        
        //Проверим, что в поле «Корреспондент» указана организация = зеркалу нашей организации
        var businessUnit = lenspec.Etalon.BusinessUnits.GetAll();
        var correspondent = Sungero.Parties.Companies.As(docletter.Correspondent);
        if (correspondent != null)
        {
          var nor = businessUnit.Where(x => x.Company.Equals(correspondent)).SingleOrDefault();
          if (nor != null)
          {
            //Проверим, что активно действие Несколько адресатов
            if(docletter.IsManyAddressees == true)
            {
              //Вычислим сотрудников указанных на вкладке адресат
              foreach(var addressee in docletter.Addressees)
              {
                if(addressee.Addressee.Login == null || addressee.Addressee.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed )
                {
                  users.Add(addressee.Addressee);
                }
              }
            }
            else
            {
              if (users.Count > 0)
              {
                if (docletter.Addressee.Login == null || docletter.Addressee.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed)
                {
                  users.Add(docletter.Addressee);
                }
              }
            }
            if (users.Count > 0)
            {
              e.AddError(lenspec.Etalon.ApprovalRegistrationAssignments.Resources.AddresseesMustBeAutomatedFormat(string.Join(", ", users.Select(x => x.Name).ToList())));
              return;
            }
          }
        }
      }
      
      #endregion
      
      base.Execute(e);
    }

    public override bool CanExecute(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return base.CanExecute(e);
    }
    //конец Добавлено Avis Expert

  }

}