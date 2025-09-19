using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalCheckingAssignment;

namespace lenspec.Etalon.Server
{
  partial class ApprovalCheckingAssignmentFunctions
  {

    //Добавлено Avis Expert

    /// <summary>
    /// Проверить заполненность полей Экспорт 1С и Дата экспорта в 1С
    /// </summary>
    /// <param name="document">Согласуемый документ.</param>
    /// <returns>Сообщение об ошибке, если у документа не заполнены поля, иначе пустая строка.</returns>
    [Remote]
    public string Check1CFields(Sungero.Docflow.IOfficialDocument document)
    {
      var result = string.Empty;
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var currentStage = Functions.ApprovalTask.GetStage(task, Sungero.Docflow.ApprovalStage.StageType.SimpleAgr);
      
      if (currentStage != null)
      {
        var stage = Etalon.ApprovalStages.As(currentStage.Stage);
        
        if (stage != null && stage.IsCheckExport1CApplicationForPaymentlenspec.HasValue && stage.IsCheckExport1CApplicationForPaymentlenspec.Value == true &&
            ApplicationsForPayment.ApplicationForPayments.Is(document))
        {
          var applicationForPayment = ApplicationsForPayment.ApplicationForPayments.As(document);
          
          if (applicationForPayment.Export1CState != ApplicationsForPayment.ApplicationForPayment.Export1CState.Yes && !applicationForPayment.Export1CDate.HasValue)
            result = lenspec.Etalon.ApprovalSimpleAssignments.Resources.NeedExport1C;
        }
      }
      
      return result;
    }
    
    /// <summary>
    /// Проверить наличие версии документа.
    /// </summary>
    /// <param name="document">Согласуемый документ.</param>
    /// <returns>Сообщение об ошибке, если документ не имеет версий, иначе пустая строка.</returns>
    [Remote]
    public string CheckDocumentVersions(Sungero.Docflow.IOfficialDocument document)
    {
      var result = string.Empty;
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var currentStage = Functions.ApprovalTask.GetStage(task, Sungero.Docflow.ApprovalStage.StageType.SimpleAgr);
      if (currentStage != null)
      {
        var stage = Etalon.ApprovalStages.As(currentStage.Stage);
        if (stage != null && stage.IsCheckVersionlenspec.HasValue && stage.IsCheckVersionlenspec.Value == true && !document.HasVersions)
        {
          if (lenspec.ElectronicDigitalSignatures.EDSApplications.Is(document))
            result = lenspec.Etalon.ApprovalSimpleAssignments.Resources.NeedAddEDSApplicationtVersion;
          else
            result = lenspec.Etalon.ApprovalSimpleAssignments.Resources.NeedAddDocumentVersion;
        }
      }
      return result;
    }
    
    /// <summary>
    /// Обновить способ доставки в документе.
    /// </summary>
    /// <param name="deliveryMethod">Способ доставки.</param>
    [Remote]
    public void RefreshDocumentDeliveryMethod(Sungero.Docflow.IMailDeliveryMethod deliveryMethod)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && Sungero.RecordManagement.OutgoingLetters.Is(document))
      {
        document.DeliveryMethod = deliveryMethod;
      }
    }
    
    /// <summary>
    /// Вернуть признак этапа «Способ доставки» доступен для редактирования.
    /// </summary>
    /// <returns>True, если признак выставлен, иначе false.</returns>
    [Remote]
    public bool GetDeliveryMethodIsEditable()
    {
      var isEditable = false;
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var currentStage = Functions.ApprovalTask.GetStage(task, Sungero.Docflow.ApprovalStage.StageType.SimpleAgr);
      if (currentStage != null)
      {
        var stage = Etalon.ApprovalStages.As(currentStage.Stage);
        isEditable = stage != null && stage.DeliveryMethodIsEditableavis.HasValue && stage.DeliveryMethodIsEditableavis.Value == true;
      }
      return isEditable;
    }

    /// <summary>
    /// Проверить резервирование основного документа.
    /// </summary>
    /// <param name="document">Согласуемый документ.</param>
    /// <returns>Сообщение об ошибке, если документ не зарезервирован, иначе пустая строка.</returns>
    [Remote]
    public string CheckReservationDocument(Sungero.Docflow.IOfficialDocument document)
    {
      var result = string.Empty;
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var currentStage = Functions.ApprovalTask.GetStage(task, Sungero.Docflow.ApprovalStage.StageType.SimpleAgr);
      if (currentStage != null)
      {
        var stage = Etalon.ApprovalStages.As(currentStage.Stage);
        if (stage != null && stage.CheckReservationlenspec.HasValue && stage.CheckReservationlenspec.Value == true &&
            document.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Reserved &&
            document.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered)
        {
          result = lenspec.Etalon.ApprovalSimpleAssignments.Resources.NeedMainDocumentReservation;
        }
      }
      return result;
    }
    /// <summary>
    /// Автоматически обновить поля в теле документа.
    /// </summary>
    /// <returns>Сообщение об ошибке, если не удалось обновить поля в теле документа, иначе пустая строка.</returns>
    [Remote]
    public string UpdateDocumentBody()
    {
      var result = string.Empty;
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var currentStage = Functions.ApprovalTask.GetStage(task, Sungero.Docflow.ApprovalStage.StageType.SimpleAgr);
      if (currentStage != null)
      {
        var stage = Etalon.ApprovalStages.As(currentStage.Stage);
        if (stage != null)
        {
          var updateTemplateBeforeExecute = stage.UpdateTemplateBeforeExecuteavis.HasValue && stage.UpdateTemplateBeforeExecuteavis.Value == true;
          var updateTemplateNumberAndDateBeforeExecute = stage.UpdateTemplateNumberAndDateBeforeExecuteavis.HasValue && stage.UpdateTemplateNumberAndDateBeforeExecuteavis.Value == true;
          if (updateTemplateBeforeExecute || updateTemplateNumberAndDateBeforeExecute)
          {
            _obj.Save();
            result = Etalon.PublicFunctions.ApprovalTask.UpdateDocumentBody(task, updateTemplateBeforeExecute, updateTemplateNumberAndDateBeforeExecute);
          }
        }
      }
      return result;
    }
    //конец Добавлено Avis Expert
  }
}