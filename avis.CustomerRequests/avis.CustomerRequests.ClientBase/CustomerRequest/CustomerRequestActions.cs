using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustomerRequest;
using lenspec.Etalon.OutgoingLetter;


namespace avis.CustomerRequests.Client
{
  partial class CustomerRequestActions
  {
    //ТЗ в Трекере DIRRXMIGR-54
    public virtual void CreateOutgoingLetter(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      //По кнопке создается исходящее письмо, заполняются поля согласно Таблице Соотношения полей из ТЗ
      var outgoingLetter = lenspec.Etalon.OutgoingLetters.Create();

      outgoingLetter.DeliveryMethod = _obj.DeliveryMethod;
      outgoingLetter.Subject = _obj.Subject;
      outgoingLetter.Correspondent = _obj.Client;
      outgoingLetter.ClientContractlenspec = _obj.SDAContracts.FirstOrDefault()?.Contract;
      outgoingLetter.ManagementContractMKDavis = _obj.ManagementContractsMKD.FirstOrDefault()?.ManagementContractMKD;
      outgoingLetter.OurCFlenspec = _obj.ObjectOfProject?.OurCF;
      outgoingLetter.InResponseToCustomavis = Sungero.Docflow.OfficialDocuments.As(_obj);
      outgoingLetter.BusinessUnit = _obj.BusinessUnit;
      outgoingLetter.Show();
    }

    public virtual bool CanCreateOutgoingLetter(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      //Проверка, сохранено ли обращение клиента, для возможности создания исх. письма
      return !_obj.State.IsInserted && !_obj.State.IsChanged;
    }

    public virtual void AddRegistrationStamp(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверить, что преобразование уже запущено, чтобы не запускать еще раз при повторном нажатии.
      // Вместо этого будет показан диалог о том, что преобразование в процессе.
      int convertingVersionIdParamValue = -1;
      bool addingRegistrationStampIsInProcess = e.Params.TryGetValue(Sungero.Docflow.Constants.OfficialDocument.ConvertingVersionId, out convertingVersionIdParamValue) &&
        convertingVersionIdParamValue == _obj.LastVersion.Id;
      
      // Преобразование в PDF.
      Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult result = null;
      if (!addingRegistrationStampIsInProcess)
      {
        // Проверки возможности преобразования и наложения отметки.
        result = Functions.CustomerRequest.ValidatePdfConvertibilityByExtension(_obj);
        if (!result.HasErrors)
        {
          var position = Functions.CustomerRequest.ShowAddRegistrationStampDialog(_obj);
          if (position == null)
            return;
          
          var convartionResult = Functions.CustomerRequest.Remote.AddRegistrationStamp(_obj, position.RightIndent, position.BottomIndent);
          
          if (convartionResult.IsOnConvertion)
            e.Params.AddOrUpdate(Sungero.Docflow.Constants.OfficialDocument.ConvertingVersionId, _obj.LastVersion.Id);
          
          // Успешная интерактивная конвертация.
          if (!convartionResult.HasErrors && convartionResult.IsFastConvertion)
          {
            Dialogs.NotifyMessage(Sungero.Docflow.OfficialDocuments.Resources.ConvertionDone);
            return;
          }
        }
      }
      
      // Сообщение об ошибке при асинхронном преобразовании.
      if (!addingRegistrationStampIsInProcess && result.HasErrors)
      {
        Dialogs.NotifyMessage(result.ErrorMessage);
        return;
      }
    }

    public virtual bool CanAddRegistrationStamp(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var isRegistered = _obj.RegistrationState == avis.CustomerRequests.CustomerRequest.RegistrationState.Registered;
      var isDesktop = ClientApplication.ApplicationType == ApplicationType.Desktop;
      return !isDesktop &&
        !_obj.State.IsInserted &&
        !_obj.State.IsChanged &&
        _obj.HasVersions &&
        _obj.AccessRights.CanUpdate() &&
        Locks.GetLockInfo(_obj).IsLockedByMe &&
        isRegistered &&
        _obj.ExchangeState == null;
    }

    public override void SendActionItem(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.HasVersions || _obj.LastVersion.Body.Size == 0)
      {
        Dialogs.ShowMessage(avis.CustomerRequests.CustomerRequests.Resources.NeedToAddDocumentVersion, MessageType.Information);
        throw new OperationCanceledException();
      }

      if (_obj.RegistrationNumber == null)
      {
        Dialogs.ShowMessage(avis.CustomerRequests.CustomerRequests.Resources.NeedToRegisterDocument, MessageType.Information);
        throw new OperationCanceledException();
      }

      var tasks = lenspec.Etalon.ActionItemExecutionTasks.GetAll(x => x.Status == lenspec.Etalon.ActionItemExecutionTask.Status.InProcess ||
                                                                 x.Status == lenspec.Etalon.ActionItemExecutionTask.Status.Suspended ||
                                                                 x.Status == lenspec.Etalon.ActionItemExecutionTask.Status.UnderReview ||
                                                                 x.Status == lenspec.Etalon.ActionItemExecutionTask.Status.Draft)
        .Where(x => x.AttachmentDetails.Any(att => att.AttachmentId == _obj.Id));
      
      if (tasks.Any())
      {
        Dialogs.ShowMessage(avis.CustomerRequests.CustomerRequests.Resources.DocumentHasBeenSentToExecutionFormat(string.Join(", ", tasks.Select(x => x.Id.ToString()).ToList())),
                            MessageType.Information);
        throw new OperationCanceledException();
      }
      
      tasks = lenspec.Etalon.ActionItemExecutionTasks.GetAll(x => x.Status == lenspec.Etalon.ActionItemExecutionTask.Status.Completed)
        .Where(x => x.AttachmentDetails.Any(att => att.AttachmentId == _obj.Id));
      if (tasks.Any())
      {
        Dialogs.ShowMessage(avis.CustomerRequests.CustomerRequests.Resources.DocumentHasBeenExecutedFormat(string.Join(", ", tasks.Select(x => x.Id.ToString()).ToList())),
                            MessageType.Information);
        throw new OperationCanceledException();
      }

      base.SendActionItem(e);
    }

    public override bool CanSendActionItem(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendActionItem(e);
    }

    public override void ShowRegistrationPane(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ShowRegistrationPane(e);
      _obj.State.Properties.Created.IsVisible = _obj.State.Properties.LifeCycleState.IsVisible;
    }

    public override bool CanShowRegistrationPane(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanShowRegistrationPane(e);
    }

  }

}