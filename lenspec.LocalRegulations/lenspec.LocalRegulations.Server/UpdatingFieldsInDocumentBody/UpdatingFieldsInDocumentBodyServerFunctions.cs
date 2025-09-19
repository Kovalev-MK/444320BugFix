using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.LocalRegulations.UpdatingFieldsInDocumentBody;

namespace lenspec.LocalRegulations.Server
{
  partial class UpdatingFieldsInDocumentBodyFunctions
  {

    /// <summary>
    /// Обновить поля в теле документа.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = base.Execute(approvalTask);
      if (_obj.UpdateAllFields != true && _obj.UpdateRegistrationData != true)
        return result;
      try
      {
        Logger.DebugFormat("Avis - UpdatingFieldsInDocumentBody - задача ИД {0}", approvalTask.Id);
        var documents = new List<Sungero.Docflow.IOfficialDocument>();
        documents.AddRange(approvalTask.DocumentGroup.OfficialDocuments.AsEnumerable());
        documents.AddRange(approvalTask.AddendaGroup.OfficialDocuments.AsEnumerable());
        
        var documentsToUpdate = new List<Sungero.Docflow.IOfficialDocument>();
        #region Отбор документов для обновления тел
        foreach(var document in documents)
        {
          // Если у документа нет тела или оно не docx, то дальнейшие проверки бессмысленны.
          if (!document.HasVersions || document.LastVersion.Body == null || document.LastVersion.AssociatedApplication.Extension != "docx")
            continue;
          
          // В подписанных документах не обновлять поля.
          var approvalSignature = Signatures.Get(document.LastVersion).Where(s => s.SignatureType == SignatureType.Approval);
          if (approvalSignature != null && approvalSignature.Any())
            continue;
          
          documentsToUpdate.Add(document);
        }
        #endregion
        
        #region Проверка блокировок
        var containsLockedDocs = CheckLockedDocuments(documentsToUpdate);
        if (containsLockedDocs)
        {
          Logger.DebugFormat("Avis - UpdatingFieldsInDocumentBody - задача ИД {0} - есть заблокированные документы.", approvalTask.Id);
          return this.GetRetryResult("Есть заблокированные карточки/тела документов.");
        }
        #endregion
        
        var updateTemplateErrors = string.Empty;
        foreach(var document in documentsToUpdate)
        {
          var isForcedLocked = false;
          try
          {
            isForcedLocked = Locks.TryLock(document);
            if (!isForcedLocked)
            {
              updateTemplateErrors += $"\r\nне удалось заблокировать документ {document.Name} (ИД {document.Id})";
              continue;
            }
            
            if (_obj.UpdateAllFields == true)
            {
              document.UpdateTemplateParameters();
              document.Save();
            }
            else if (_obj.UpdateRegistrationData == true)
            {
              var errorUpdate = lenspec.Etalon.PublicFunctions.ApprovalTask.UpdateTemplateParameterNumberAndDate(document);
              if (!string.IsNullOrEmpty(errorUpdate))
                updateTemplateErrors += $"\r\n{document.Name} (ИД {document.Id}) - {errorUpdate}";
              
              #region Для Исх. письма обработать поле В ответ на № и Дату
              if (Sungero.RecordManagement.OutgoingLetters.Is(document) && lenspec.Etalon.OutgoingLetters.As(document).InResponseTo != null)
              {
                errorUpdate = lenspec.Etalon.PublicFunctions.ApprovalTask.UpdateTemplateParameterInResponseTo(document);
                if (!string.IsNullOrEmpty(errorUpdate))
                  updateTemplateErrors += $"\r\n{document.Name} (ИД {document.Id}) - {errorUpdate}";
              }
              #endregion
            }
          }
          catch(Exception ex)
          {
            Logger.ErrorFormat("Avis - UpdatingFieldsInDocumentBody - задача ИД {0}, документ ИД {1} - ", ex, approvalTask.Id, document.Id);
            updateTemplateErrors += $"\r\n{document.Name} (ИД {document.Id}) - {ex.Message}";
          }
          finally
          {
            if (isForcedLocked)
              Locks.Unlock(document);
          }
        }
        if (!string.IsNullOrEmpty(updateTemplateErrors))
        {
          Logger.ErrorFormat("Avis - UpdatingFieldsInDocumentBody - задача ИД {0} - {1}", approvalTask.Id, updateTemplateErrors);
          SendAdministratorEDMSNotification("Ошибка сценария обновления полей в телах документов",
                                            string.Format("Не удалось обновить поля в телах документов:{0}", updateTemplateErrors));
          // нужно поменять на переповтор?
          return this.GetRetryResult("Были ошибки при обновлении тел документов.");
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - UpdatingFieldsInDocumentBody - задача ИД {0}", ex, approvalTask.Id);
        SendAdministratorEDMSNotification("Ошибка сценария обновления полей в телах документов",
                                          string.Format("Сценарий обновления полей (задача с ИД {0}) завершен непредвиденной ошибкой {1} {2}", approvalTask.Id, ex.Message, ex.InnerException?.Message));
        return this.GetErrorResult(ex.Message);
      }
      return result;
    }
    
    /// <summary>
    /// Проверить документы на блокировку карточки или тела.
    /// </summary>
    /// <param name="documents">Список документов для проверки.</param>
    /// <returns>Список документов, у которых заблокированы карточка или тело.</returns>
    public bool CheckLockedDocuments(List<Sungero.Docflow.IOfficialDocument> documents)
    {
      var containsLockedDocs = false;
      foreach(var document in documents)
      {
        // Если заблокирована карточка документа, то не обновлять поля.
        var lockInfoMessage = string.Empty;
        var lockInfoCard = Locks.GetLockInfo(document);
        if (lockInfoCard != null && lockInfoCard.IsLocked)
        {
          lockInfoMessage += $"\r\nкарточка заблокирована пользователем {lockInfoCard.OwnerName}";
        }
        // Если заблокировано тело документа, то не обновлять поля.
        var lockInfoBody = Locks.GetLockInfo(document.LastVersion.Body);
        if (lockInfoBody != null && lockInfoBody.IsLocked)
        {
          lockInfoMessage += $"\r\nтело заблокировано пользователем {lockInfoBody.OwnerName}";
        }
        if (!string.IsNullOrEmpty(lockInfoMessage))
        {
          Logger.ErrorFormat("Avis - UpdatingFieldsInDocumentBody - документ ИД {0} - {1}", document.Id, lockInfoMessage);
          containsLockedDocs = true;
          SendAdministratorEDMSNotification(string.Format("Ошибка сценария обновления полей в теле документа {0}", document.Name),
                                            string.Format("Не удалось обновить поля в теле документа {0} (ИД {1}):{2}", document.Name, document.Id, lockInfoMessage),
                                            document.Id);
        }
      }
      return containsLockedDocs;
    }
    
    /// <summary>
    /// Отправить уведомление Администратору СЭД об ошибке в сценарии.
    /// </summary>
    /// <param name="subject">Тема.</param>
    /// <param name="activeText">Текст уведомления.</param>
    /// <param name="documentId">ИД документа-вложения.</param>
    public void SendAdministratorEDMSNotification(string subject, string activeText, long? documentId = null)
    {
      var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
      var asyncHandler = LocalRegulations.AsyncHandlers.AsyncFunctionStageErrorNotification.Create();
      asyncHandler.Subject = subject;
      asyncHandler.ActiveText = activeText;
      asyncHandler.PerformerId = administratorEDMSRole.Id;
      if (documentId != null)
        asyncHandler.AttachmentId = documentId.Value;
      asyncHandler.ExecuteAsync();
    }
  }
}