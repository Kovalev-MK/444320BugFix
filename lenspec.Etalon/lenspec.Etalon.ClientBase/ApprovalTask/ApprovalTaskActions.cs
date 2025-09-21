using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalTask;

namespace lenspec.Etalon.Client
{
  partial class ApprovalTaskActions
  {

    public virtual void AddProtocolTClenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var documents = Functions.ApprovalTask.Remote.GetJustificationDocuments(_obj);
      if (!documents.Any())
      {
        e.AddError(lenspec.Etalon.ApprovalTasks.Resources.JustificationDocumentsNotFound);
        return;
      }
      
      // Выбрать документ-обоснование из диалогового окна.
      var justificationDocument = Functions.ApprovalTask.ShowJustificationDocumentSelectDialog(documents);
      if (justificationDocument != null)
      {
        if (_obj.OtherGroup.All.Contains(justificationDocument))
          e.AddWarning(lenspec.Etalon.ApprovalTasks.Resources.DocumentAlreadyAddedFormat(justificationDocument.Name));
        else
          _obj.OtherGroup.All.Add(justificationDocument);
      }
    }

    public virtual bool CanAddProtocolTClenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      return Sungero.Contracts.ContractualDocuments.Is(document);
    }

    public override void Restart(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Отдельная проверка на дубликаты для исходящих писем, т.к. они используются в массовой рассылке и согласовываются в области приложений.
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var isMassMailing = lenspec.OutgoingLetters.MassMailingApplications.Is(document);
      if (isMassMailing)
      {
        var outgoingLetters = _obj.AddendaGroup.OfficialDocuments.Where(x => lenspec.Etalon.OutgoingLetters.Is(x));
        foreach (var letter in outgoingLetters)
          if (Functions.ApprovalTask.CheckDuplicates(_obj, letter, isMassMailing))
            return;
      }
      
      // Проверка на дубликаты
      var isOutgoingLetter = lenspec.Etalon.OutgoingLetters.Is(document);
      if (Functions.ApprovalTask.CheckDocumentType(_obj) && Functions.ApprovalTask.CheckDuplicates(_obj, isOutgoingLetter))
        return;

      base.Restart(e);
    }

    public override bool CanRestart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanRestart(e);
    }

    public override void Resume(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      //Отдельная проверка на дубликаты для исходящих писем, т.к. они используются в массовой рассылке и согласовываются в области приложений
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var isMassMailing = lenspec.OutgoingLetters.MassMailingApplications.Is(document);
      if (isMassMailing)
      {
        var outgoingLetters = _obj.AddendaGroup.OfficialDocuments.Where(x => lenspec.Etalon.OutgoingLetters.Is(x));
        foreach (var letter in outgoingLetters)
          if (Functions.ApprovalTask.CheckDuplicates(_obj, letter, isMassMailing))
            return;
      }
      // Проверка на дубликаты
      var isOutgoingLetter = lenspec.Etalon.OutgoingLetters.Is(document);
      if (Functions.ApprovalTask.CheckDocumentType(_obj) && Functions.ApprovalTask.CheckDuplicates(_obj, isOutgoingLetter))
        return;
      
      base.Resume(e);
    }

    public override bool CanResume(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanResume(e);
    }

    public override void Abort(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Abort(e);
    }

    public override bool CanAbort(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanAbort(e);
    }
    
    public virtual void RegisterDocumentslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // TODO: НЕ УДАЛЯТЬ ЗАКОММЕНТИРОВАННОЕ!!! для массовых рассылок
      //      var mainDocument = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      //      if (mainDocument == null)
      //      {
      //        e.AddError("Вложите заявку на рассылку массовых уведомлений");
      //        return;
      //      }
      //      try
      //      {
      //        var addendas = mainDocument.Relations.GetRelated(Sungero.Docflow.Constants.Module.AddendumRelationName)
      //          .Where(x => Sungero.RecordManagement.OutgoingLetters.Is(x))
      //          .Select(x => Sungero.Docflow.OfficialDocuments.As(x))
      //          .ToList();
      //        if (!addendas.Any(x => x.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered))
      //        {
      //          e.AddError("Нет исходящих писем для регистрации");
      //          return;
      //        }
      //        _obj.Save();
      //        lenspec.OutgoingLetters.PublicFunctions.Module.RegisterOutgoingLettersInParts(addendas, _obj.Id);
      //      }
      //      catch(Exception ex)
      //      {
      //        e.AddError(ex.Message);
      //        return;
      //      }
    }

    public virtual bool CanRegisterDocumentslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
      // TODO: НЕ УДАЛЯТЬ ЗАКОММЕНТИРОВАННОЕ!!! для массовых рассылок
      //      var isMassMailingApplication = false;
      //      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      //      if (document != null)
      //      {
      //        isMassMailingApplication = lenspec.OutgoingLetters.MassMailingApplications.Is(document) &&
      //          (Users.Current.IncludedIn(Sungero.Docflow.PublicConstants.Module.RoleGuid.ClerksRole) || Users.Current.IncludedIn(Roles.Administrators));
      //      }
      //      return isMassMailingApplication;
    }

    public override void Start(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      
      // Если в карточке задачи отсутствует какой-либо связанный документ Заявки на оплату, доложить.
      if (lenspec.ApplicationsForPayment.ApplicationForPayments.Is(document))
      {
        var relatedDocuments = Sungero.Docflow.PublicFunctions.OfficialDocument.GetAddenda(document);
        foreach (var addenda in relatedDocuments)
          if (!_obj.AddendaGroup.All.Contains(addenda))
            _obj.AddendaGroup.All.Add(addenda);
      }
      
      // Проверить наличие настройки согласования до диалового окна подтверждения.
      if (_obj.ApprovalRule != null)
      {
        var businessUnit = document?.BusinessUnit;
        var approvalRuleAllVersions = PublicFunctions.ApprovalTask.Remote.GetAllRuleVersions(_obj.ApprovalRule);
        if (businessUnit != null)
        {
          var approvalSettings = EtalonDatabooks.ApprovalSettings.GetAll()
            .Where(x => x.Status == EtalonDatabooks.ApprovalSetting.Status.Active &&
                   x.BusinessUnit != null && x.BusinessUnit.Equals(businessUnit) &&
                   x.ApprovalRule != null && approvalRuleAllVersions.Contains(x.ApprovalRule));
          
          // Обычные правила согласования.
          if (Etalon.ApprovalRules.Is(_obj.ApprovalRule) && Etalon.ApprovalRules.As(_obj.ApprovalRule).CheckApprovalSettinglenspec == true && !approvalSettings.Any())
          {
            Dialogs.ShowMessage(lenspec.Etalon.ApprovalTasks.Resources.BusinessUnitDoesNotHaveApprovalSetting, MessageType.Warning);
            return;
          }
          // Правила согласования для договоров.
          if (Sungero.Contracts.ContractsApprovalRules.Is(_obj.ApprovalRule) && lenspec.Etalon.ContractualDocuments.Is(document) && !approvalSettings.Any())
          {
            e.AddError(lenspec.Etalon.ApprovalTasks.Resources.ErrorMessageEmptyContractsApprovalRuleList);
            return;
          }
        }
      }
      
      // Проверить, совпадает ли способ доставки в карточке задачи на согласование и в карточке документа, если нет, обновить способ доставки в карточке задачи.
      if (document != null && document.DeliveryMethod != _obj.DeliveryMethod)
        _obj.DeliveryMethod = document.DeliveryMethod;
      
      // Проверка на дубликаты.
      // Отдельная проверка на дубликаты для исходящих писем, т.к. они используются в массовой рассылке и согласовываются в области приложений.
      var isMassMailing = lenspec.OutgoingLetters.MassMailingApplications.Is(document);
      if (isMassMailing)
      {
        var outgoingLetters = _obj.AddendaGroup.OfficialDocuments.Where(x => lenspec.Etalon.OutgoingLetters.Is(x));
        foreach (var letter in outgoingLetters)
        {
          if (Functions.ApprovalTask.CheckDuplicates(_obj, letter, isMassMailing))
            return;
        }
      }
      
      // Проверка на дубликаты
      var isCheckAddendaGroup = lenspec.Etalon.OutgoingLetters.Is(document) || lenspec.Etalon.PowerOfAttorneys.Is(document);
      if (Functions.ApprovalTask.CheckDocumentType(_obj) && Functions.ApprovalTask.CheckDuplicates(_obj, isCheckAddendaGroup))
        return;
      
      base.Start(e);
    }

    public override bool CanStart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanStart(e);
    }
  }
}