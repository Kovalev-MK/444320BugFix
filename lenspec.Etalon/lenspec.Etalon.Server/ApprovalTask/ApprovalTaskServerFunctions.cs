using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalTask;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace lenspec.Etalon.Server
{
  partial class ApprovalTaskFunctions
  {

    //Добавлено Avis Expert
    
    /// <summary>
    /// Найти задачи на согласование по регламенту в которых главный документ является главным документом или приложением в текущей задаче
    /// </summary>
    /// <param name="isCheckAddendums">Проверять приложения?</param>
    /// <returns>Список задач-дубликатов (прекращенные задачи не попадают в список)</returns>
    [Public, Remote(IsPure = true)]
    public static List<Sungero.Docflow.IApprovalTask> GetTaskDuplicates(Sungero.Docflow.IOfficialDocument document, bool isCheckAddendums)
    {
      var tasks = new List<Sungero.Docflow.IApprovalTask>();
      AccessRights.AllowRead(() =>
                             {
                               tasks.AddRange(Sungero.Docflow.ApprovalTasks.GetAll(x => x.Status != Sungero.Docflow.ApprovalTask.Status.Aborted &&
                                                                                   x.AttachmentDetails.Any(d => d.GroupId == new Guid("08e1ef90-521f-41a1-a13f-c6f175007e54") && d.AttachmentId == document.Id)));
                             });
      
      if (isCheckAddendums)
      {
        AccessRights.AllowRead(() =>
                               {
                                 tasks.AddRange(Sungero.Docflow.ApprovalTasks.GetAll(x => x.Status != Sungero.Docflow.ApprovalTask.Status.Aborted &&
                                                                                     x.AttachmentDetails.Any(d => d.GroupId == new Guid("852b3e7d-f178-47d3-8fad-a64021065cfd") && d.AttachmentId == document.Id)));
                               });
      }
      return tasks;
    }
    
    /// <summary>
    /// Получить документы обоснования для вложения в задачу.
    /// </summary>
    /// <returns>Документы обоснования.</returns>
    [Public, Remote(IsPure = true)]
    public IQueryable<Sungero.Docflow.IOfficialDocument> GetJustificationDocuments()
    {
      // КА из ДД в группе вложений "Документ".
      var contractualDocument = Sungero.Contracts.ContractualDocuments.As(_obj.DocumentGroup.OfficialDocuments.SingleOrDefault());
      var counterparty = contractualDocument?.Counterparty;
      
      return FilterJustificationDocuments(Sungero.Docflow.OfficialDocuments.GetAll(), counterparty);
    }
    
    /// <summary>
    /// Отфильтровать документы по квалификации.
    /// </summary>
    /// <param name="documents">Документы.</param>
    private static IQueryable<Sungero.Docflow.IOfficialDocument> FilterJustificationDocuments(IQueryable<Sungero.Docflow.IOfficialDocument> documents, Sungero.Parties.ICounterparty counterparty)
    {
      return documents.Where(
        x =>
        (
          // Протокол тендерного комитета (Контрагент из договорного документа = Контрагент Протокола ТК; Согласование = Подписан).
          counterparty != null &&
          x.InternalApprovalState == Sungero.Docflow.OfficialDocument.InternalApprovalState.Signed &&
          lenspec.Tenders.TenderCommitteeProtocols.Is(x) &&
          lenspec.Tenders.TenderCommitteeProtocols.As(x).Counterparties.Any(c => Equals(c.Counterparty, counterparty))
         ) ||
        (
          // Служебная записка (Согласование = Подписан).
          x.InternalApprovalState == Sungero.Docflow.OfficialDocument.InternalApprovalState.Signed &&
          Sungero.Docflow.Memos.Is(x)
         ) ||
        (
          // Протокол коллегиальных органов (Состояние = Действующий).
          x.LifeCycleState == Sungero.Docflow.OfficialDocument.LifeCycleState.Active &&
          lenspec.ProtocolsCollegialBodies.ProtocolCollegialBodies.Is(x)
         )
       ).AsQueryable();
    }
    
    /// <summary>
    /// Обработка старта задачи по типу главного документа.
    /// </summary>
    /// <returns>Сообщение об ошибке (при наличии).</returns>
    public string ExecuteByMainDocumentType()
    {
      var errors = string.Empty;
      var document  = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        _obj.DocumentIsSignedlenspec = document.InternalApprovalState == Sungero.Docflow.OfficialDocument.InternalApprovalState.Signed;
        ExecuteFormalizedPowerOfAttorney(document);
        errors += ExecuteContractualDocument();
        errors += ExecutePowerOfAttorney(document);
      }
      return errors;
    }
    
    private string ExecutePowerOfAttorney(Sungero.Docflow.IOfficialDocument document)
    {
      var errors = string.Empty;
      if (!lenspec.Etalon.PowerOfAttorneys.Is(document))
        return errors;
      
      // Проверка наличия тела главного документа при согласовании доверенности
      var requestPoa = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
      if (Equals(requestPoa, document.DocumentKind) && !document.HasVersions)
        errors = lenspec.Etalon.ApprovalTasks.Resources.ErrorMessageEmptyVersionPoaRequest;
      
      return errors;
    }
    
    /// <summary>
    /// Обработка Договорного документа.
    /// </summary>
    private string ExecuteContractualDocument()
    {
      // Поиск договора.
      var errors = string.Empty;
      var contract = GetContractBaseFromDocumentGroup();
      if (contract == null)
        return errors;
      
      // Тендерная сумма.
      var tenderAmount = lenspec.Etalon.BusinessUnits.As(contract.BusinessUnit).TenderAmountavis;
      if (tenderAmount == null || tenderAmount == 0.0)
        return lenspec.Etalon.ApprovalTasks.Resources.ErrorMessageExecuteContractualDocumentEmptyTenderAmount;
      
      // Вычисление общей суммы договора с доп. соглашениями.
      var linkedSupAgreements = Sungero.Contracts.SupAgreements.GetAll(x => Equals(contract, x.LeadingDocument));
      var totalAmount = contract.TotalAmount.HasValue ?
        contract.TotalAmount.Value :
        0.0;
      foreach (var supAgreement in linkedSupAgreements)
        if (supAgreement.TotalAmount.HasValue)
          totalAmount += supAgreement.TotalAmount.Value;

      totalAmount = Math.Round(totalAmount, 2);
      
      // Признак вложенности документа-обоснования.
      var isJustificationDocumentRequired = CheckJustificationDocumentRequired();
      // Контрагент – НОР?
      var isBusinessUnit = Etalon.BusinessUnits.GetAll(x => Equals(x.Company, contract.Counterparty)).Any();
      
      if (tenderAmount < totalAmount && isJustificationDocumentRequired && !isBusinessUnit)
        return lenspec.Etalon.ApprovalTasks.Resources.ErrorMessageExecuteContractualDocumentEmptyJustificationDocument;
      
      return errors;
    }
    
    /// <summary>
    /// Получить ведущий договор для вложения "Документ".
    /// </summary>
    /// <returns>Договор при наличии; Иначе – null.</returns>
    private Sungero.Contracts.IContractBase GetContractBaseFromDocumentGroup()
    {
      var document = _obj.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (Sungero.Contracts.ContractBases.Is(document))
        return Sungero.Contracts.ContractBases.As(document);
      
      if (Sungero.Contracts.SupAgreements.Is(document))
      {
        var contract = Sungero.Contracts.SupAgreements.As(document).LeadingDocument;
        return Sungero.Contracts.ContractBases.As(contract);
      }
      
      return null;
    }
    
    /// <summary>
    /// Получить доп. соглашение для вложения "Документ".
    /// </summary>
    /// <returns>Доп. соглашение при наличии; Иначе – null.</returns>
    private Sungero.Contracts.ISupAgreement GetSupAgreeementFromDocumentGroup()
    {
      var result = _obj.DocumentGroup.OfficialDocuments.SingleOrDefault();
      return Sungero.Contracts.SupAgreements.As(result);
    }
    
    /// <summary>
    /// Проверка необходимости добавления документа-обоснования в задачу.
    /// </summary>
    /// <param name="contract">Договор.</param>
    /// <param name="supAgreement">Дополнительное соглашение (если является основным вложением).</param>
    /// <returns>True, если необходимо вложение; Иначе – false.</returns>
    private bool CheckJustificationDocumentRequired()
    {
      var otherGroupDocuments = _obj.OtherGroup.All
        .Where(x => Sungero.Docflow.OfficialDocuments.Is(x))
        .Select(x => Sungero.Docflow.OfficialDocuments.As(x))
        .AsQueryable();
      
      var contract =      GetContractBaseFromDocumentGroup();
      var supAgreement =  GetSupAgreeementFromDocumentGroup();
      var isJustificationDocumentAdded = FilterJustificationDocuments(otherGroupDocuments, contract.Counterparty).Any();
      
      return !isJustificationDocumentAdded && CheckJustificationDocumentRequired(contract, supAgreement);
    }
    
    /// <summary>
    /// Проверка необходимости добавления документа-обоснования в зависимости от вложений.
    /// </summary>
    /// <param name="contract">Договор.</param>
    /// <param name="supAgreement">Дополнительное соглашение (если является основным вложением).</param>
    /// <returns>True, если необходимо вложение; Иначе – false.</returns>
    private static bool CheckJustificationDocumentRequired(Sungero.Contracts.IContractBase contract, Sungero.Contracts.ISupAgreement supAgreement)
    {
      if (contract == null)
        return false;
      
      // "Соглашение о расторжении" для доп. соглашения (если вложено в гр. "Документ").
      if (lenspec.Etalon.SupAgreements.As(supAgreement)?.IsTerminationavis == true)
        return false;
      
      // "Не требует протокола ТК" на карточке типа договора.
      if (contract.ContractTypeavis?.IsTenderProtocolNotRequired == true)
        return false;
      
      // "Не требует протокола ТК" на карточке группы категорий договора.
      if (contract.GroupContractTypeavis?.IsTenderProtocolNotRequired == true)
        return false;
      
      // "Не требует протокола ТК" на карточке категории договора.
      if (contract.ContractKindavis?.IsTenderProtocolNotRequired == true)
        return false;
      
      // "Не требует протокола ТК" на карточке вида договора.
      if (contract.ContractCategoryavis?.IsTenderProtocolNotRequiredlenspec == true)
        return false;
      
      // "Монополист" на карточке организации КА.
      if (contract.Counterparty != null && lenspec.Etalon.Companies.Is(contract.Counterparty) && lenspec.Etalon.Companies.As(contract.Counterparty).IsMonopolistlenspec == true)
        return false;
      
      return true;
    }
    
    /// <summary>
    /// Обработка Электронной доверенности
    /// </summary>
    private void ExecuteFormalizedPowerOfAttorney(Sungero.Docflow.IOfficialDocument document)
    {
      if (!Sungero.Docflow.FormalizedPowerOfAttorneys.Is(document))
        return;
      
      var fpoa = Sungero.Docflow.FormalizedPowerOfAttorneys.As(document);
      if (fpoa.FtsListState == Sungero.Docflow.FormalizedPowerOfAttorney.FtsListState.Registered)
        _obj.DocumentFtsListStatelenspec = lenspec.Etalon.ApprovalTask.DocumentFtsListStatelenspec.Registered;
      
      if (fpoa.FtsListState == Sungero.Docflow.FormalizedPowerOfAttorney.FtsListState.Rejected)
        _obj.DocumentFtsListStatelenspec = lenspec.Etalon.ApprovalTask.DocumentFtsListStatelenspec.Rejected;
    }
    
    /// <summary>
    /// Отправить уведомления о прекращении задачи на согласование по регламенту.
    /// </summary>
    public override void SendApprovalAbortNotice()
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      // Для МЧД и доверенностей отправить уведомление только Кому выдана и Подготовил.
      if (Sungero.Docflow.PowerOfAttorneyBases.Is(document))
      {
        var subject = string.Empty;
        var threadSubject = string.Empty;
        // Отправить уведомления о прекращении.
        using (Sungero.Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
        {
          threadSubject = Sungero.Docflow.ApprovalTasks.Resources.AbortNoticeSubject;
          if (document != null)
            subject = string.Format(Sungero.Exchange.Resources.TaskSubjectTemplate, threadSubject, Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(document.Name));
          else
          {
            var approvalTaskSubject = string.Format("{0}{1}", _obj.Subject.Substring(0, 1).ToLower(), _obj.Subject.Remove(0, 1));
            subject = string.Format("{0} {1}", Sungero.Docflow.ApprovalTasks.Resources.AbortApprovalTask, Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(approvalTaskSubject));
          }
        }
        
        var performers = new List<Sungero.CoreEntities.IUser>();
        var poa = Sungero.Docflow.PowerOfAttorneyBases.As(document);
        if (poa.IssuedTo != null)
          performers.Add(poa.IssuedTo);
        if (poa.PreparedBy != null)
          performers.Add(poa.PreparedBy);
        
        performers.Remove(Users.Current);
        if (performers.Any())
          Sungero.Docflow.Server.ModuleFunctions.SendNoticesAsSubtask(subject, performers, _obj, _obj.AbortingReason, _obj.Author, threadSubject);
      }
      else
        base.SendApprovalAbortNotice();
    }
    
    /// <summary>
    /// Обновить поля в теле документов из области вложений Задачи на согласование по регламенту.
    /// </summary>
    /// <param name="task">Задача на согласование по регламенту.</param>
    /// <param name="updateTemplateBeforeExecuteavis">Признак Автоматически обновлять поля в теле документа при выполнении.</param>
    /// <param name="updateTemplateNumberAndDateBeforeExecuteavis">Признак Автоматически обновлять поля Номер и Дата в теле документа.</param>
    /// <returns>Пустая строка, если обновление прошло успешно, иначе сообщение об ошибках.</returns>
    [Public]
    public static string UpdateDocumentBody(Sungero.Docflow.IApprovalTask task, bool updateTemplateBeforeExecuteavis, bool updateTemplateNumberAndDateBeforeExecuteavis)
    {
      if (!updateTemplateBeforeExecuteavis && !updateTemplateNumberAndDateBeforeExecuteavis)
        return string.Empty;
      
      var result = string.Empty;
      try
      {
        var documents = new List<Sungero.Docflow.IOfficialDocument>();
        documents.AddRange(task.DocumentGroup.OfficialDocuments.AsEnumerable());
        documents.AddRange(task.AddendaGroup.OfficialDocuments.AsEnumerable());
        
        var documentsToUpdate = new List<Sungero.Docflow.IOfficialDocument>();
        
        #region Проверка прав и блокировок
        var lockedDocs = new List<Sungero.Docflow.IOfficialDocument>();
        foreach(var document in documents)
        {
          // Если у документа нет тела или оно не docx, то дальнейшие проверки бессмысленны.
          if (!document.HasVersions || document.LastVersion.Body == null || document.LastVersion.AssociatedApplication.Extension != "docx")
          {
            continue;
          }
          // В подписанных документах не обновлять поля.
          var approvalSignature = Signatures.Get(document.LastVersion).Where(s => s.SignatureType == SignatureType.Approval);
          if (approvalSignature != null && approvalSignature.Any())
          {
            continue;
          }
          // Если нет прав на изменение, не обновлять поля.
          if (!(document.AccessRights.CanUpdate() && document.AccessRights.CanUpdateBody()))
          {
            result += lenspec.Etalon.ApprovalTasks.Resources.NoRightsToDocumentFormat(document.Name, document.Id) + "\r\n";
            continue;
          }
          // Если заблокирована карточка документа, то не обновлять поля.
          var lockInfoCard = Locks.GetLockInfo(document);
          if (lockInfoCard != null && lockInfoCard.IsLocked)
          {
            lockedDocs.Add(document);
            result += lenspec.Etalon.ApprovalTasks.Resources.DocumentIsLockedByUserFormat(document.Name, document.Id, lockInfoCard.OwnerName) + "\r\n";
            continue;
          }
          // Если заблокировано тело документа, то не обновлять поля.
          var lockInfoBody = Locks.GetLockInfo(document.LastVersion.Body);
          if (lockInfoBody != null && lockInfoBody.IsLocked)
          {
            lockedDocs.Add(document);
            result += lenspec.Etalon.ApprovalTasks.Resources.DocumentBodyIsLockedByUserFormat(document.Name, document.Id, lockInfoBody.OwnerName) + "\r\n";
            continue;
          }
          documentsToUpdate.Add(document);
        }
        #endregion
        
        if (lockedDocs.Count == 0)
        {
          var updateTemplateErrors = string.Empty;
          foreach(var document in documentsToUpdate)
          {
            try
            {
              if (updateTemplateBeforeExecuteavis)
              {
                document.UpdateTemplateParameters();
                document.Save();
              }
              else if (updateTemplateNumberAndDateBeforeExecuteavis)
              {
                var errorUpdate = UpdateTemplateParameterNumberAndDate(document);
                if (!string.IsNullOrEmpty(errorUpdate))
                {
                  updateTemplateErrors += $"{document.Name} (ИД {document.Id}) -{errorUpdate}\r\n";
                  Logger.ErrorFormat("Avis - UpdateDocumentBody - task {0}, document ID {1} - {2}", task.Id, document.Id, errorUpdate);
                }
              }
            }
            catch(Exception ex)
            {
              updateTemplateErrors += $"{document.Name} (ИД {document.Id}) - {ex.Message}\r\n";
              Logger.ErrorFormat("Avis - UpdateDocumentBody - task {0}, document ID {1} - ", ex, task.Id, document.Id);
            }
          }
          result += updateTemplateErrors;
        }
        if (lockedDocs.Count > 5)
          result = lenspec.Etalon.ApprovalTasks.Resources.FailedToUpdateFieldsInDocumentsFormat(lockedDocs.Count);
        else
          result = lenspec.Etalon.ApprovalTasks.Resources.FailedToUpdateFieldsInDocumentsPrefix + result;
      }
      catch(Exception ex)
      {
        result = "Обновление полей в теле документа завершилось ошибкой.";
        Logger.ErrorFormat("Avis - UpdateDocumentBody - task {0} - ", ex, task.Id);
      }
      return result;
    }
    
    /// <summary>
    /// Подставляем текст в тег документа.
    /// </summary>
    /// <param name="stdRuns"></param>
    /// <param name="text">Подставляемый текст.</param>
    /// <param name="tagName">Тэг в теле документа.</param>
    /// <param name="documentId">ИД документа, в котором обновляются поля.</param>
    private static string UpdateTag(IEnumerable<DocumentFormat.OpenXml.Wordprocessing.SdtRun> stdRuns, string text, string tagName, long documentId)
    {
      var errors = string.Empty;
      if (string.IsNullOrEmpty(text))
      {
        Logger.DebugFormat("Avis - UpdateTag - документ ИД {0}, тэг {1} - пустое значение в карточке документа, замены в теле документа не будет.", documentId, tagName);
        return errors;
      }
      
      foreach (SdtRun sdtRun in stdRuns)
      {
        var contentRun = sdtRun.Descendants<SdtContentRun>().FirstOrDefault();
        if (contentRun == null)
          continue;
        
        if (!contentRun.Descendants<Text>().Any())
          continue;
        
        var allContentRunText = contentRun.Descendants<Text>().ToList();
        for(int i = 0; i < allContentRunText.Count(); i++)
        {
          allContentRunText[i].Text = string.Empty;
        }
        
        var contentRunText = contentRun.Descendants<Text>().FirstOrDefault();
        if (contentRunText == null)
          continue;
        
        var contentRunTextSource = contentRunText.Text;
        try
        {
          contentRunText.Text = text;
        }
        catch(Exception ex)
        {
          if (contentRunText.Text != text)
          {
            errors += string.Format(" Не удалось заменить значение тэга {0}. Значение в карточке: '{1}', значение в теле документа: '{2}'.", tagName, text, contentRunText.Text);
            Logger.ErrorFormat("Avis - UpdateTag - Не удалось заменить значение тэга: документ ИД {0}, тэг {1} - значение из карточки документа: '{2}', значение в теле документа до изменения: '{3}', значение после изменения: '{4}'. ",
                               ex, documentId, tagName, text, contentRunTextSource, contentRunText.Text);
          }
        }
      }
      return errors;
    }
    
    /// <summary>
    /// Обновление поля Номер и Дата в документе.
    /// </summary>
    /// <param name="document"></param>
    [Public]
    public static string UpdateTemplateParameterNumberAndDate(Sungero.Docflow.IOfficialDocument document)
    {
      if (!document.HasVersions || document.LastVersion.Body == null || document.LastVersion.AssociatedApplication.Extension != "docx")
        return string.Empty;
      
      var errorUpdate = string.Empty;
      var bodyDoc = document.LastVersion.Body;
      using (var memory = new System.IO.MemoryStream())
      {
        bodyDoc.Read().CopyTo(memory);
        using (var wordprocessingDocument = WordprocessingDocument.Open(memory,true))
        {
          var tagRegNumber = string.Empty;
          var textRegNumber = string.Empty;
          var tagDocDate = string.Empty;
          var textDocDate = string.Empty;
          if (document.LeadingDocument == null)
          {
            tagRegNumber = Constants.Docflow.ApprovalTask.UpdateTemplateRegistrationNumberTag;
            textRegNumber = document.RegistrationNumber;
            tagDocDate = Constants.Docflow.ApprovalTask.UpdateTemplateRegistrationDateTag;
            textDocDate = document.DocumentDate.Value.ToString("d");
          }
          else
          {
            tagRegNumber = Constants.Docflow.ApprovalTask.UpdateHeadTemplateRegistrationNumberTag;
            textRegNumber = document.LeadingDocument.RegistrationNumber;
            tagDocDate = Constants.Docflow.ApprovalTask.UpdateHeadTemplateRegistrationDateTag;
            textDocDate = document.LeadingDocument.DocumentDate.Value.ToString("d");
          }
          
          // Обновить номер документа.
          var sdtRunsRegNumber = wordprocessingDocument.MainDocumentPart.Document.Descendants<SdtRun>()
            .Where(run => run.SdtProperties.GetFirstChild<Tag>() != null &&
                   run.SdtProperties.GetFirstChild<Tag>().Val.Value == tagRegNumber);
          errorUpdate += UpdateTag(sdtRunsRegNumber, textRegNumber, tagRegNumber, document.Id);

          // Обновить основной документ, Дату документа.
          var sdtRunsRegDate = wordprocessingDocument.MainDocumentPart.Document.Descendants<SdtRun>()
            .Where(run => run.SdtProperties.GetFirstChild<Tag>() != null &&
                   run.SdtProperties.GetFirstChild<Tag>().Val.Value == tagDocDate);
          errorUpdate += UpdateTag(sdtRunsRegDate, textDocDate, tagDocDate, document.Id);
          
          wordprocessingDocument.MainDocumentPart.Document.Save();
        }
        document.LastVersion.Body.Write(memory);
        document.Save();
      }
      return errorUpdate;
    }
    
    /// <summary>
    /// Обновление поля номера ответного письма в теле документа.
    /// </summary>
    /// <param name="document"></param>
    [Public]
    public static string UpdateTemplateParameterInResponseTo(Sungero.Docflow.IOfficialDocument document)
    {
      if (!document.HasVersions || document.LastVersion.Body == null || document.LastVersion.AssociatedApplication.Extension != "docx")
        return string.Empty;
      
      var errorUpdate = string.Empty;
      var bodyDoc = document.LastVersion.Body;
      using (var memory = new System.IO.MemoryStream())
      {
        bodyDoc.Read().CopyTo(memory);
        using (var wordprocessingDocument = WordprocessingDocument.Open(memory,true))
        {
          var tag = string.Empty;
          var textToReplace = string.Empty;
          
          // Для исх. письма заполнить тэги для поля В ответ на.
          if (Sungero.RecordManagement.OutgoingLetters.Is(document) && Sungero.RecordManagement.OutgoingLetters.As(document).InResponseTo != null)
          {
            tag = Constants.Docflow.ApprovalTask.UpdateHeadTemplateNumberInResponseToTag;
            textToReplace = Sungero.RecordManagement.OutgoingLetters.As(document).InResponseTo.InNumber ?? string.Empty;
            var sdtRunsRegNumber = wordprocessingDocument.MainDocumentPart.Document.Descendants<SdtRun>()
              .Where(run => run.SdtProperties.GetFirstChild<Tag>() != null && run.SdtProperties.GetFirstChild<Tag>().Val.Value == tag);
            errorUpdate += UpdateTag(sdtRunsRegNumber, textToReplace, tag, document.Id);
            
            tag = Constants.Docflow.ApprovalTask.UpdateHeadTemplateDateInResponseToTag;
            textToReplace = Sungero.RecordManagement.OutgoingLetters.As(document).InResponseTo.Dated.HasValue
              ? Sungero.RecordManagement.OutgoingLetters.As(document).InResponseTo.Dated.Value.ToString("d")
              : string.Empty;
            var sdtRunsRegDate = wordprocessingDocument.MainDocumentPart.Document.Descendants<SdtRun>()
              .Where(run => run.SdtProperties.GetFirstChild<Tag>() != null && run.SdtProperties.GetFirstChild<Tag>().Val.Value == tag);
            errorUpdate += UpdateTag(sdtRunsRegDate, textToReplace, tag, document.Id);
          }
          
          wordprocessingDocument.MainDocumentPart.Document.Save();
        }
        document.LastVersion.Body.Write(memory);
        document.Save();
      }
      return errorUpdate;
    }
    
    /// <summary>
    /// Получить все версии правила.
    /// </summary>
    /// <param name="rule">Правило согласования.</param>
    /// <returns>Список версий правила согласования, который включает и текущее правило (если оно уже было сохранено в БД).</returns>
    [Public, Remote]
    public static List<Sungero.Docflow.IApprovalRuleBase> GetAllRuleVersions(Sungero.Docflow.IApprovalRuleBase rule)
    {
      return Sungero.Docflow.Server.ApprovalRuleBaseFunctions.GetAllRuleVersions(rule);
    }
    
    /// <summary>
    /// Получить список всех заданий текущей задачи, параллельных указанному заданию.
    /// </summary>
    /// <param name="assignment">Задание.</param>
    /// <returns>Список заданий.</returns>
    public override List<Sungero.Workflow.IAssignment> GetParallelAssignments(Sungero.Workflow.IAssignment assignment)
    {
      return base.GetParallelAssignments(assignment);
    }
    
    /// <summary>
    /// Определить текущий этап.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="stageType">Тип этапа.</param>
    /// <returns>Текущий этап, либо null, если этапа нет (или это не тот этап).</returns>
    public static Sungero.Docflow.Structures.Module.DefinedApprovalStageLite GetStage(Sungero.Docflow.IApprovalTask task, Enumeration stageType)
    {
      var stage = task.ApprovalRule.Stages
        .Where(s => s.Stage != null)
        .Where(s => s.Stage.StageType == stageType)
        .FirstOrDefault(s => s.Number == task.StageNumber);
      
      if (stage != null)
        return Sungero.Docflow.Structures.Module.DefinedApprovalStageLite.Create(stage.Stage, stage.Number, stage.StageType);
      
      return null;
    }
    
    /// <summary>
    /// Обновить список обязательных согласующих.
    /// </summary>
    /// <param name="rule">Правило.</param>
    /// <param name="stages">Список этапов согласования.</param>
    [Remote(PackResultEntityEagerly = true)]
    public override void UpdateReglamentApprovers(Sungero.Docflow.IApprovalRuleBase rule,
                                                  List<Sungero.Docflow.Structures.Module.DefinedApprovalBaseStageLite> stages)
    {
      base.UpdateReglamentApprovers(rule, stages);
      
      try
      {
        if (stages == null || !stages.Any())
        {
          return;
        }
        
        var stagesWithComputedRole = stages
          .Where(x => x.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers && Sungero.Docflow.ApprovalStages.Is(x.StageBase))
          .Select(x => Etalon.ApprovalStages.As(x.StageBase))
          .Where(x => x != null)
          .Where(x => x.ApprovalRoles.Any(r => r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ApprovRoleKind ||
                                          r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.InitManager ||
                                          r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ManagerTaskCard ||
                                          r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.TenderMembers ||
                                          r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.AccreditationMembers ||
                                          r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ManagAttornDept ||
                                          r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.EDSOwner ||
                                          r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.Attorney));
        
        _obj.ComputedReqApproverslenspec.Clear();
        // Если в построенном регламенте нет этапа с вычисляемой ролью согласования, то коллекция ее участников останется пустой.
        if (!stagesWithComputedRole.Any())
        {
          return;
        }
        
        var recipients = new List<Sungero.CoreEntities.IRecipient>();
        foreach (var stage in stagesWithComputedRole)
        {
          var computedRoles = stage.ApprovalRoles
            .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ApprovRoleKind)
            .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole))
            .Where(x => x != null)
            .Distinct();
          foreach(var role in computedRoles)
          {
            recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.Remote.GetApprovRoleKindPerformers(role, _obj, stage));
          }
          computedRoles = stage.ApprovalRoles
            .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.TenderMembers)
            .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole))
            .Where(x => x != null)
            .Distinct();
          foreach(var role in computedRoles)
          {
            recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.Remote.GetTenderMembersPerformers(role, _obj));
          }
          computedRoles = stage.ApprovalRoles
            .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.AccreditationMembers)
            .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole))
            .Where(x => x != null)
            .Distinct();
          foreach(var role in computedRoles)
          {
            recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.Remote.GetAccreditationCommitteeMembersPerformers(role, _obj));
          }
          computedRoles = stage.ApprovalRoles
            .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ManagAttornDept)
            .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole))
            .Where(x => x != null)
            .Distinct();
          foreach(var role in computedRoles)
          {
            recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.GetManagersAttorney(role, _obj));
          }
          computedRoles = stage.ApprovalRoles
            .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.EDSOwner)
            .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole))
            .Where(x => x != null)
            .Distinct();
          foreach(var role in computedRoles)
          {
            recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.Remote.GetEDSOwnerPerformers(role, _obj));
          }
          computedRoles = stage.ApprovalRoles
            .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.Attorney)
            .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole))
            .Where(x => x != null)
            .Distinct();
          foreach(var role in computedRoles)
          {
            recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.GetAttorneys(role, _obj));
          }
        }
        
        foreach(var recipient in recipients)
        {
          if (!_obj.ReqApprovers.Where(x => x.Approver.Equals(recipient)).Any())
          {
            // Участников вычисляемой роли добавить в поле Обязательные, а также в коллекцию "Обязательные согласующие по вычисляемой роли"
            _obj.ComputedReqApproverslenspec.AddNew().Approver = recipient;
            _obj.ReqApprovers.AddNew().Approver = recipient;
          }
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - UpdateReglamentApprovers - task {0} - ", ex, _obj.Id);
      }
    }
    //конец Добавлено Avis Expert
  }
}