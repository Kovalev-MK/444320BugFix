using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters.Server
{
  public class ModuleAsyncHandlers
  {

    /// <summary>
    /// Асинхронный обработчик для смены поля Статус писем в карточке Заявки на массовую рассылку уведомлений.
    /// </summary>
    /// <param name="args">Аргументы асинхронного обработчика.</param>
    public virtual void AsyncUpdateLettersStatus(lenspec.OutgoingLetters.Server.AsyncHandlerInvokeArgs.AsyncUpdateLettersStatusInvokeArgs args)
    {
      Logger.Debug("Avis - AsyncUpdateLettersStatus - сменить поле Статус писем в Заявке на массовую рассылку.");
      var massMailingApplication = lenspec.OutgoingLetters.MassMailingApplications.GetAll(x => x.Id == args.MassMailingApplicationId).SingleOrDefault();
      if (massMailingApplication == null)
      {
        Logger.ErrorFormat("Avis - AsyncUpdateLettersStatus - Заявка {0} не найдена", args.MassMailingApplicationId);
        args.Retry = false;
        return;
      }
      
      var lockInfo = Locks.GetLockInfo(massMailingApplication);
      if (lockInfo.IsLocked)
      {
        args.Retry = true;
        return;
      }
      
      var isForcedLocked = false;
      try
      {
        isForcedLocked = Locks.TryLock(massMailingApplication);
        if (!isForcedLocked)
          throw new ApplicationException($"не удалось установить блокировку на карточку заявки {args.MassMailingApplicationId}");
        
        if (args.LettersAreFormed)
        {
          massMailingApplication.LettersStatus = lenspec.OutgoingLetters.MassMailingApplication.LettersStatus.Formed;
          massMailingApplication.Save();
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - AsyncUpdateLettersStatus - ", ex);
        if (args.RetryIteration > 100)
        {
          args.Retry = false;
          var performer = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
          if (performer != null)
          {
            var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices(lenspec.OutgoingLetters.Resources.NoticeSubjectForUpdateLettersStatus, performer);
            notice.Attachments.Add(lenspec.OutgoingLetters.MassMailingApplications.Get(args.MassMailingApplicationId));
            notice.ActiveText = lenspec.OutgoingLetters.Resources.NoticeActiveTextForUpdateLettersStatusFormat(ex.Message);
            notice.Start();
          }
        }
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(massMailingApplication);
      }
    }
    
    /// <summary>
    /// Асинхронный обработчик для формирования исходящих писем для рассылки.
    /// </summary>
    /// <param name="args">Аргументы асинхронного обработчика.</param>
    public virtual void CreateOutgoingLettersForMassMailing(lenspec.OutgoingLetters.Server.AsyncHandlerInvokeArgs.CreateOutgoingLettersForMassMailingInvokeArgs args)
    {
      Logger.Debug("Avis - CreateOutgoingLettersForMassMailing - формирование исх. писем для массовой рассылки.");
      var massMailingApplication = lenspec.OutgoingLetters.MassMailingApplications.GetAll(x => x.Id == args.MassMailingApplicationId).SingleOrDefault();
      if (massMailingApplication == null)
      {
        Logger.ErrorFormat("Avis - CreateOutgoingLettersForMassMailing - Заявка {0} не найдена", args.MassMailingApplicationId);
        args.Retry = false;
        return;
      }
      var isLocked = false;
      try
      {
        if (massMailingApplication.MailingType == null || massMailingApplication.MailingType.Template == null)
          throw new ApplicationException("Укажите шаблон для создания исх. писем в карточке Типа рассылки.");
        
        if (Locks.GetLockInfo(massMailingApplication).IsLocked)
        {
          args.Retry = true;
          return;
        }
        
        isLocked = Locks.TryLock(massMailingApplication);
        if (!isLocked)
        {
          Logger.ErrorFormat("Avis - CreateOutgoingLettersForMassMailing - не удалось заблокировать заявку {0}", args.MassMailingApplicationId);
          args.Retry = true;
          return;
        }

        var outgoingLettersAuthor = Sungero.CoreEntities.Users.Get(args.UserId);
        var attachments = new List<Sungero.Docflow.IOfficialDocument>();
        attachments.Add(massMailingApplication);
        var objectAnProject = massMailingApplication.ObjectAnProject;
        if (objectAnProject != null)
        {
          // Находит все документы типа «Клиентский договор», в полях которых указан «Объект» с карточки Заявки на массовую рассылку.
          var clientContracts = lenspec.SalesDepartmentArchive.SDAClientContracts
            .GetAll(x => x.LifeCycleState == lenspec.SalesDepartmentArchive.SDAClientContract.LifeCycleState.Active && x.ObjectAnProject != null && x.ObjectAnProject.Equals(objectAnProject))
            .AsEnumerable();
          
          // Если заполнены клиентские договора, то берем их оттуда.
          if (massMailingApplication.CollectionClientContract.Count() > 0)
          {
            var sdaClientContracts = new List<lenspec.SalesDepartmentArchive.ISDAClientContract>();
            foreach (var clientContract in massMailingApplication.CollectionClientContract)
              sdaClientContracts.Add(clientContract.ClientContract);
            
            clientContracts = sdaClientContracts.AsEnumerable();
          }
          
          if (massMailingApplication.MailingType.ChangeOfMeasurements.HasValue && massMailingApplication.MailingType.ChangeOfMeasurements.Value == true)
          {
            clientContracts = clientContracts
              .Where(x => x.Premises != null && (x.Premises.EditSquere.HasValue && !x.Premises.EditSquere.Value.Equals(0.00) &&
                                                 x.Premises.EditPrice.HasValue && !x.Premises.EditPrice.Value.Equals(0.00)))
              .AsEnumerable();
          }
          foreach(var clientContract in clientContracts)
          {
            // Вычисляет записи справочника «Персоны», связанных с этими договорами (по полю «Клиент»).
            var people = clientContract.CounterpartyClient.Where(x => Sungero.Parties.People.Is(x.ClientItem)).Select(x => Sungero.Parties.People.As(x.ClientItem)).ToList<Sungero.Parties.IPerson>();
            
            // Если требуется доформирование писем, то исключаем дубли по КД и КА.
            //if (massMailingApplication.LettersStatus == lenspec.OutgoingLetters.MassMailingApplication.LettersStatus.FormationRequired)
            //{
              var correspondentIds = massMailingApplication.Relations.GetRelatedDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName)
                .Where(x => lenspec.Etalon.OutgoingLetters.Is(x) && lenspec.Etalon.OutgoingLetters.As(x).ClientContractlenspec == clientContract && lenspec.Etalon.OutgoingLetters.As(x).Correspondent != null)
                .Select(x => lenspec.Etalon.OutgoingLetters.As(x).Correspondent.Id)
                .ToList()
                .Distinct();
              people = people.Where(x => !correspondentIds.Contains(x.Id)).ToList<Sungero.Parties.IPerson>();
            //}
            
            foreach(var person in people)
            {
              var template = massMailingApplication.MailingType.Template;
              var outgoingLetter = lenspec.Etalon.OutgoingLetters.CreateFrom(template);
              // вид документа - по умолчанию
              outgoingLetter.ClientContractlenspec = clientContract;
              outgoingLetter.OurCFlenspec = objectAnProject.OurCF;
              outgoingLetter.RepresentativeByPowerOfAttorneyNumberlenspec = massMailingApplication.RepresentativeByPowerOfAttorneyNumber;
              outgoingLetter.PowerOfAttorneyDatelenspec = massMailingApplication.PowerOfAttorneyDate;
              outgoingLetter.Correspondent = person;
              outgoingLetter.AddressOfRecipientlenspec = person.PostalAddress;
              outgoingLetter.PreparedBy = massMailingApplication.PreparedBy;
              outgoingLetter.Department = massMailingApplication.Department;
              outgoingLetter.BusinessUnit = massMailingApplication.BusinessUnit;
              outgoingLetter.OurSignatory = massMailingApplication.OurSignatory;
              outgoingLetter.DeliveryMethod = massMailingApplication.DeliveryMethod;
              outgoingLetter.MassMailingApplicationlenspec = massMailingApplication;
              outgoingLetter.Subject = massMailingApplication.NotificationContent;
              outgoingLetter.Author = outgoingLettersAuthor;
              
              outgoingLetter.AccessRights.Grant(outgoingLettersAuthor, DefaultAccessRightsTypes.FullAccess);
              
              outgoingLetter.Relations.AddFrom(Sungero.Docflow.PublicConstants.Module.AddendumRelationName, massMailingApplication);
              attachments.Add(outgoingLetter);
              
              outgoingLetter.Save();
            }
          }
        }
        massMailingApplication.LettersStatus = lenspec.OutgoingLetters.MassMailingApplication.LettersStatus.Formed;
        massMailingApplication.Save();
        
        SendCreatingOutgoingLettersNotification(outgoingLettersAuthor, massMailingApplication, attachments);
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - CreateOutgoingLettersForMassMailing - ", ex);
        var performer = Users.Get(args.UserId);
        if (performer != null)
        {
          var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices("Процесс 'Создание Исходящих писем для рассылки' завершился ошибкой. Обратитесь к администратору.", performer);
          notice.Attachments.Add(lenspec.OutgoingLetters.MassMailingApplications.Get(args.MassMailingApplicationId));
          notice.ActiveText = ex.Message;
          notice.Start();
        }
      }
      finally
      {
        if (isLocked)
          Locks.Unlock(massMailingApplication);
      }
    }
    
    private void SendCreatingOutgoingLettersNotification(Sungero.CoreEntities.IUser performer, lenspec.OutgoingLetters.IMassMailingApplication massMailingApplication, List<Sungero.Docflow.IOfficialDocument> attachments)
    {
      var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices(lenspec.OutgoingLetters.Resources.OutgoingLettersForMassMailingCreated, new List<Sungero.CoreEntities.IRecipient>() { performer }, attachments.ToArray());
      notice.NeedsReview = false;
      var quantityEnding = attachments.Count % 10;
      var textEnding = string.Empty;
      switch(quantityEnding)
      {
        case 1:
          {
            textEnding = "исходящее письмо";
            break;
          }
        case 2:
        case 3:
        case 4:
          {
            textEnding = "исходящих письма";
            break;
          }
        case 0:
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
          {
            textEnding = "исходящих писем";
            break;
          }
      }
      notice.ActiveText = string.Format("Для документа {0} создано {1} {2}.", massMailingApplication.Name, attachments.Count - 1, textEnding);
      notice.Start();
    }

    /// <summary>
    /// Асинхронный обработчик для массовой отправки исходящих писем.
    /// </summary>
    /// <param name="args">Аргументы асинхронного обработчика.</param>
    public virtual void MassSendingOutgoingLetters(lenspec.OutgoingLetters.Server.AsyncHandlerInvokeArgs.MassSendingOutgoingLettersInvokeArgs args)
    {
      Logger.Debug("Avis - MassSendingOutgoingLetters - отправка эл. писем.");
      args.Retry = false;
      try
      {
        var task = Sungero.Docflow.ApprovalTasks.Get(args.TaskId);
        var outgoingLetters = task.AddendaGroup.OfficialDocuments
          .Where(x => Etalon.OutgoingLetters.Is(x))
          .Select(x => Etalon.OutgoingLetters.As(x))
          .ToList();
        
        var massMailingTask = lenspec.OutgoingLetters.MassMailingNoticeTasks.Create();
        foreach(var letter in outgoingLetters)
        {
          if (string.IsNullOrEmpty(letter.Correspondent.Email) || !letter.Correspondent.Email.Contains("@"))
          {
            var unsentEmail = massMailingTask.UnsentEmails.AddNew();
            unsentEmail.UnsentOutgoingLetter = letter;
            unsentEmail.ErrorMessage = "Некорректное значение поля Эл. почта в карточке Корреспондента";
            continue;
          }
          Transactions.Execute(
            () =>
            {
              try
              {
                var message = Mail.CreateMailMessage();
                var senderEmail = lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.MassMailingEmailAddressCode).FirstOrDefault();
                if (senderEmail != null && !string.IsNullOrEmpty(senderEmail.Value))
                {
                  message.From = senderEmail.Value;
                }
                // Скрытая копия, т.к. в Отправленных не видно, кому уходили письма.
                var bccEmail = lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.MassMailingBCCEmailCode).FirstOrDefault();
                if (bccEmail != null && !string.IsNullOrEmpty(bccEmail.Value))
                {
                  message.Bcc.Add(bccEmail.Value);
                }
                if (!string.IsNullOrEmpty(letter.Correspondent.Email))
                {
                  message.To.Add(letter.Correspondent.Email);
                }
                message.Subject = letter.Name;
                if (letter.HasVersions)
                {
                  // Оригинал
                  var originalVersion = letter.Versions.OrderByDescending(x => x.Id).Skip(1).FirstOrDefault();
                  if (originalVersion != null)
                    message.AddAttachment(originalVersion);
                  // Преобразованное тело
                  message.AddAttachment(letter.LastVersion);
                  // Подписи
                  var signatures = Signatures.Get(letter.LastVersion).Where(s => s.SignatureType == SignatureType.Approval && s.IsValid && s.SignCertificate != null).ToList();
                  AddSignaturesToMailMessage(message, signatures);
                }
                Mail.Send(message);
              }
              catch(Exception ex)
              {
                var unsentEmail = massMailingTask.UnsentEmails.AddNew();
                unsentEmail.UnsentOutgoingLetter = letter;
                unsentEmail.ErrorMessage = ex.Message;
                Logger.ErrorFormat("Avis - MassSendingOutgoingLetters - строка отчета об ошибке, не удалось отправить эл. письмо {0}: ", ex, letter.Id);
              }
            });
        }
        SendMassMailngNotification(massMailingTask, args.UserId);
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - MassSendingOutgoingLetters - ", ex);
        var performer = Users.Get(args.UserId);
        if (performer != null)
        {
          var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices("Процесс 'Массовая отправка исходящих писем' завершился ошибкой. Обратитесь к администратору.", performer);
          notice.Attachments.Add(Sungero.Workflow.Tasks.Get(args.TaskId));
          notice.ActiveText = ex.Message;
          notice.Start();
        }
      }
    }
    
    private void AddSignaturesToMailMessage(Sungero.Core.IEmailMessage message, List<Sungero.Domain.Shared.ISignature> signatures)
    {
      foreach (var signature in signatures)
      {
        System.IO.MemoryStream stream = null;
        try
        {
          var signData = signature.GetDataSignature();
          stream = new System.IO.MemoryStream(signData);
          var signName = string.Format("sign_{0}.{1}", signature.Id, "sig");
          message.AddAttachment(stream, signName);
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Avis - MassSendingOutgoingLetters - signId={0} - ", ex, signature.Id);
        }
      }
    }
    
    private void SendMassMailngNotification(lenspec.OutgoingLetters.IMassMailingNoticeTask massMailingTask, long userId)
    {
      try
      {
        var performer = Users.Get(userId);
        if (performer == null)
          return;
        
        massMailingTask.NotificationPerformer = performer;
        if (massMailingTask.UnsentEmails.Any())
        {
          massMailingTask.Subject = "Формирование исходящих писем завершено с ошибкой. Отправлены не все письма.";
          massMailingTask.ActiveText = "Для просмотра неотправленных писем сформируйте отчет «Неотправленные письма».";
        }
        else
        {
          massMailingTask.Subject = "Формирование исходящих писем завершено, письма отправлены.";
        }
        massMailingTask.Start();
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - SendMassMailngNotification - ", ex);
      }
    }
  }
}