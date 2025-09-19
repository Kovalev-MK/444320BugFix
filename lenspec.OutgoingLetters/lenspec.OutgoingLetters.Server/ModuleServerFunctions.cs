using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters.Server
{
  public class ModuleFunctions
  {
    //Добавлено Avis Expert
    
    /// <summary>
    /// Зарегистрировать список документов, обновить поля в теле документа от системного пользователя.
    /// </summary>
    /// <param name="documents">Список документов.</param>
    /// <returns>Сообщения об ошибках, либо пустая строка при успешном выполнении.</returns>
    [Public]
    public static string RegisterListOfDocumentsBySystemUser(List<Sungero.Docflow.IOfficialDocument> documents)
    {
      var errors = string.Empty;
      var lockedCardsCount = 0;
      var lockedVersions = 0;
      
      foreach (var document in documents)
      {
        var lockInfoCard = Locks.GetLockInfo(document);
        if (lockInfoCard != null && lockInfoCard.IsLocked)
        {
          lockedCardsCount++;
          continue;
        }
        if (document.HasVersions)
        {
          var lockInfoBody = Locks.GetLockInfo(document.LastVersion.Body);
          if (lockInfoBody != null && lockInfoBody.IsLocked)
          {
            lockedVersions++;
            continue;
          }
        }
        try
        {
          Logger.DebugFormat("Avis - RegisterListOfDocumentsBySystemUser - регистрация документа с ИД {0}", document.Id);
          var registersIds = lenspec.Etalon.PublicFunctions.OfficialDocument.GetDocumentRegistersIdsByDocumentAvis(document, Sungero.Docflow.RegistrationSetting.SettingType.Registration);
          var defaultDocumentRegister = Sungero.Docflow.Shared.DocumentRegisterFunctions.GetDefaultDocRegister(document, registersIds, Sungero.Docflow.RegistrationSetting.SettingType.Registration);
          if (defaultDocumentRegister == null)
          {
            Logger.ErrorFormat("Avis - RegisterListOfDocumentsBySystemUser - для документа {0} не найден журнал регистрации", document.Id);
            continue;
          }
          string nextNumber = string.Empty;
          if (defaultDocumentRegister != null)
            nextNumber = Sungero.Docflow.PublicFunctions.DocumentRegister.Remote.GetNextNumber(defaultDocumentRegister, document, Calendar.UserToday);
          if (string.IsNullOrEmpty(nextNumber))
          {
            Logger.ErrorFormat("Avis - RegisterListOfDocumentsBySystemUser - для документа {0} не удалось вычислить Рег.№", document.Id);
            continue;
          }
          
          Sungero.Docflow.PublicFunctions.OfficialDocument.RegisterDocument(document, defaultDocumentRegister, Calendar.UserToday, nextNumber, false, true);
          document.UpdateTemplateParameters();
          document.Save();
        }
        catch(Exception ex)
        {
          errors += string.Format("\r\n{0} - {1}", document.Id, ex.Message);
        }
      }
      if (lockedCardsCount > 0)
      {
        errors += string.Format("Невозможно зарегистрировать {0} документов - карточка заблокирована другим пользователем.\r\n", lockedCardsCount);
      }
      if (lockedVersions > 0)
      {
        errors += string.Format("Невозможно зарегистрировать {0} документов - версия документа заблокирована другим пользователем.\r\n", lockedVersions);
      }
      return errors;
    }
    
    /// <summary>
    /// Зарегистрировать документ, обновить поля в теле документа от имени сотрудника.
    /// </summary>
    /// <param name="documents">Список документов.</param>
    /// <returns>Сообщения об ошибках, либо пустая строка при успешном выполнении.</returns>
    [Public]
    public static string RegisterListOfDocuments(List<Sungero.Docflow.IOfficialDocument> documents)
    {
      var errors = string.Empty;
      var lockedCardsCount = 0;
      var lockedVersions = 0;
      var defaultRegisterNotFound = 0;
      foreach (var document in documents)
      {
        if (document.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered)
        {
          
          var lockInfoCard = Locks.GetLockInfo(document);
          if (lockInfoCard != null && lockInfoCard.IsLocked)
          {
            lockedCardsCount++;
            continue;
          }
          
          if (document.HasVersions)
          {
            var lockInfoBody = Locks.GetLockInfo(document.LastVersion.Body);
            if (lockInfoBody != null && lockInfoBody.IsLocked)
            {
              lockedVersions++;
              continue;
            }
          }
          
          try
          {
            var regParams = Sungero.Docflow.Server.OfficialDocumentFunctions.GetRegistrationDialogParams(document, Sungero.Docflow.RegistrationSetting.SettingType.Registration);
            if (regParams.DefaultRegister == null)
            {
              defaultRegisterNotFound++;
              continue;
            }
            // Зарегистрировать документ из группы Приложения.
            Sungero.Docflow.PublicFunctions.OfficialDocument.RegisterDocument(document, regParams.DefaultRegister, Calendar.UserToday, regParams.NextNumber, false, true);
            // Обновить поля в теле документа.
            document.UpdateTemplateParameters();
            document.Save();
          }
          catch(Exception ex)
          {
            errors += " - " + ex.Message;
          }
        }
      }
      if (lockedCardsCount > 0)
      {
        errors += string.Format("Невозможно зарегистрировать {0} документов - карточка заблокирована другим пользователем.\r\n", lockedCardsCount);
      }
      if (lockedVersions > 0)
      {
        errors += string.Format("Невозможно зарегистрировать {0} документов - версия документа заблокирована другим пользователем.\r\n", lockedVersions);
      }
      if (defaultRegisterNotFound > 0)
      {
        errors += string.Format("Невозможно зарегистрировать {0} документов - для регистрации документа необходим подходящий журнал.\r\n", defaultRegisterNotFound);
      }
      return errors;
    }
    
    /// <summary>
    /// Получить ИД задач, в которые вложены Заявки на массовую рассылку.
    /// </summary>
    /// <returns>Список ИД задач.</returns>
    [Public]
    public List<long> GetCurrentUserTaskWithMassMailingApplication()
    {
      var tasks = Sungero.Workflow.Tasks.GetAll(x => x.Author.Equals(Users.Current))
        .Where(task => task.AttachmentDetails.Any(a => a.AttachmentTypeGuid == Constants.Module.MassMailingApplicationType && a.AttachmentId != null))
        .Select(task => task.Id)
        .ToList();
      return tasks;
    }
    
    /// <summary>
    /// Создать Заявку на массовую рассылку.
    /// </summary>
    /// <returns>Заявка на массовую рассылку.</returns>
    [Remote, Public]
    public static OutgoingLetters.IMassMailingApplication CreateMassMailingApplication()
    {
      return OutgoingLetters.MassMailingApplications.Create();
    }
    
    /// <summary>
    /// Создать исходящее письмо.
    /// </summary>
    /// <returns>Исходящее письмо.</returns>
    [Remote, Public]
    public static Sungero.RecordManagement.IOutgoingLetter CreateOutgoingLetter()
    {
      return Sungero.RecordManagement.OutgoingLetters.Create();
    }
    //конец Добавлено Avis Expert
  }
}