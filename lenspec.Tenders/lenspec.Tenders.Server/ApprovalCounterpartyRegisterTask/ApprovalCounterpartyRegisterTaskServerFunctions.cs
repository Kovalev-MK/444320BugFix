using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ApprovalCounterpartyRegisterTask;

namespace lenspec.Tenders.Server
{
  partial class ApprovalCounterpartyRegisterTaskFunctions
  {
    
    #region [Валидация групп вложений]

    /// <summary>
    /// Валидация вложений задачи.
    /// </summary>
    /// <returns>Пустая строка при успешной валидации; Иначе – текст ошибки.</returns>
    [Public, Remote(IsPure = true)]
    public string ValidateAttachments()
    {
      var messages = new List<string> {
        ValidateQualificationForms(),
        ValidateJustificationDocuments()
      };
      
      var missingDocumentKinds = messages.Where(x => !string.IsNullOrEmpty(x)).Select(x => $"\"{x}\"");
      var count = missingDocumentKinds.Count();
      if (count == 0)
        return string.Empty;
      
      var documentName = count == 1 ?
        lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Resources.Document :
        lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Resources.DocumentPlural;
      
      return lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Resources.AddMissingDocumentFormat(documentName, string.Join(", ", missingDocumentKinds));
    }
    
    /// <summary>
    /// Проверка наличия анкеты квалификации во вложениях.
    /// </summary>
    /// <returns>Пустая строка при успешной валидации; Иначе – название недостающего вида документа.</returns>
    private string ValidateQualificationForms()
    {
      if (_obj.Procedure == Tenders.ApprovalCounterpartyRegisterTask.Procedure.Exclusion)
        // Группа вложений не обязательна к заполнению.
        return string.Empty;
      
      // Проверка наличия анкеты квалификации.
      var tenderAccreditationFormKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.TenderAccreditationFormKind);
      if (_obj.QualificationFormGroup.TenderAccreditationForms.Any(x => Equals(x.DocumentKind, tenderAccreditationFormKind)))
        return string.Empty;
      
      return tenderAccreditationFormKind.Name;
    }
    
    /// <summary>
    /// Проверка наличия СЗ на включение/исключение из реестра во вложениях.
    /// </summary>
    /// <returns>Пустая строка при успешной валидации; Иначе – название недостающего вида документа.</returns>
    private string ValidateJustificationDocuments()
    {
      // Тип СЗ – включение/исключение?
      var decisionKindGuid = _obj.Procedure == Tenders.ApprovalCounterpartyRegisterTask.Procedure.Inclusion ?
        Constants.Module.MemoOnInclusionOfCounterpartyKind :
        Constants.Module.MemoOnExclusionOfCounterpartyKind;
      
      // Проверка наличия СЗ нужного вида.
      var decisionKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(decisionKindGuid);
      if (_obj.JustificationDocumentGroup.TenderDocuments.Any(x => Equals(x.DocumentKind, decisionKind)))
        return string.Empty;
      
      return decisionKind.Name;
    }
    
    #endregion [Валидация групп вложений]
    
    /// <summary>
    /// Получить вложения подзадач, созданных из карточки задания.
    /// </summary>
    /// <param name="assigment">Задание.</param>
    /// <returns>Список документов.</returns>
    public List<Sungero.Docflow.IOfficialDocument> GetDocumentsFromSubtasks(Sungero.Workflow.IAssignmentBase assigment)
    {
      var documents = new List<Sungero.Docflow.IOfficialDocument>();
      var subtasks = Sungero.Workflow.SimpleTasks.GetAll(x => Equals(x.ParentAssignment, assigment));
      foreach (var subtask in subtasks)
      {
        var attachments = subtask.AllAttachments.Where(x => Sungero.Docflow.OfficialDocuments.Is(x))
          .Select(x => Sungero.Docflow.OfficialDocuments.As(x));
        documents.AddRange(attachments.ToList());
      }
      
      return documents;
    }

    /// <summary>
    /// Создать реестр контрагентов.
    /// </summary>
    /// <param name="counterpartyRegistryDecision">Решение КК.</param>
    /// <returns>Реестр контрагентов.</returns>
    [Public, Remote(IsPure = false)]
    public ICounterpartyRegisterBase CreateCounterpartyRegister(ITenderDocument counterpartyRegistryDecision)
    {
      var counterpartyRegister = CounterpartyRegisterBases.Null;
      
      // Реестр подрядчиков.
      if (_obj.RegisterKind == Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Contractor)
        counterpartyRegister = ContractorRegisters.Create();
      // Реестр поставщиков.
      else if (_obj.RegisterKind == Tenders.ApprovalCounterpartyRegisterTask.RegisterKind.Provider)
        counterpartyRegister = ProviderRegisters.Create();
      else
        throw new ArgumentException(Tenders.Resources.UnacceptableRegisterKindValueError);
      
      counterpartyRegister.Counterparty = lenspec.Etalon.Companies.As(_obj.Counterparty);
      counterpartyRegister.QCDecision = counterpartyRegistryDecision;
      counterpartyRegister.ReasonForInclusionInRegister = Tenders.Resources.Memo;

      return counterpartyRegister;
    }
    
    /// <summary>
    /// Добавить новое решение КК во вложения задачи.
    /// </summary>
    /// <returns>Решение КК.</returns>
    [Remote(IsPure = false)]
    public ITenderDocument AttachCounterpartyRegistryDecision()
    {
      var decisionOnExclusion = Functions.TenderDocument.GetCounterpartyRegistryDecision(_obj);
      // Заполним решение КК заново.
      _obj.TenderDocumentGroup.TenderDocuments.Clear();
      _obj.TenderDocumentGroup.TenderDocuments.Add(decisionOnExclusion);
      // Сохраняем карточку задачи, чтобы найти ее по вложенному решению КК для формирования содержимого из шаблона.
      _obj.Save();
      decisionOnExclusion.UpdateTemplateParameters();
      decisionOnExclusion.Save();
      
      return decisionOnExclusion;
    }
    
    /// <summary>
    /// Получить актуальные задания на согласование КК.
    /// </summary>
    /// <param name="approvalCounterpartyRegisterTask">Задача на согласование включения/исключения и реестра.</param>
    /// <returns>Задания на согласование КК из последнего круга согласования.</returns>
    private static IEnumerable<ICommitteeApprovalAssignment> GetActualCommitteeApprovalAssignments(IApprovalCounterpartyRegisterTask approvalCounterpartyRegisterTask)
    {
      var approvalAssignments = CommitteeApprovalAssignments.GetAll(x => Equals(x.Task, approvalCounterpartyRegisterTask) &&
                                                                    x.TaskStartId == approvalCounterpartyRegisterTask.StartId &&
                                                                    x.Result != Tenders.CommitteeApprovalAssignment.Result.Forward &&
                                                                    x.Performer != null);
      if (approvalCounterpartyRegisterTask.DateOfLastProcessingResults != null)
      {
        approvalAssignments = approvalAssignments.Where(x => x.Created.Value >= approvalCounterpartyRegisterTask.DateOfLastProcessingResults.Value);
      }
      
      var lastIteration = approvalAssignments.Max(x => x.IterationId);
      approvalAssignments = approvalAssignments.Where(x => x.IterationId == lastIteration);
      
      return approvalAssignments.ToList();
    }
    
    /// <summary>
    /// Получить актуальное задание для исполнителя с учетом переадресации.
    /// </summary>
    /// <param name="performer">Исходный исполнитель.</param>
    /// <param name="taskAssignments">Выборка заданий по задаче.</param>
    /// <returns>Задание.</returns>
    private static ICommitteeApprovalAssignment GetLastAssignmentForPerformer(IRecipient performer, IEnumerable<ICommitteeApprovalAssignment> taskAssignments)
    {
      var performerAssignments = taskAssignments.Where(x => Equals(x.Performer, performer)).Where(x => x.Created.HasValue);
      var lastCreated = performerAssignments.Max(x => x.Created);
      var assignment = performerAssignments.Where(x => Equals(x.Created, lastCreated)).FirstOrDefault();
      
      if (assignment == null)
        return assignment;
      
      if (assignment.Result != Tenders.CommitteeApprovalAssignment.Result.Forward)
        return assignment;
      
      var forwardedTo = assignment.ForwardedTo.FirstOrDefault();
      if (forwardedTo == null)
        return assignment;
      
      return GetLastAssignmentForPerformer(forwardedTo, taskAssignments);
    }
    
    /// <summary>
    /// Подсчитать результаты согласования.
    /// </summary>
    [Remote(IsPure = true)]
    public Structures.ApprovalCounterpartyRegisterTask.IApprovalResults GetApprovalResultsCount()
    {
      var actualAssignments = GetActualCommitteeApprovalAssignments(_obj);
      var approvalExecutionResults = actualAssignments.Select(x => x.Result);
      var approvedCount = approvalExecutionResults.Where(x => x == Tenders.CommitteeApprovalAssignment.Result.Approved).Count();
      var rejectedCount = approvalExecutionResults.Where(x => x == Tenders.CommitteeApprovalAssignment.Result.Rejected).Count();
      
      var approvalResults = Structures.ApprovalCounterpartyRegisterTask.ApprovalResults.Create();
      approvalResults.Approved = approvedCount;
      approvalResults.Rejected = rejectedCount;
      
      return approvalResults;
    }
    
    /// <summary>
    /// Получить список результатов согласования.
    /// </summary>
    /// <param name="approvalCounterpartyRegisterTask">Задача на согласование включения в реестр квалифицированных контрагентов/список дисквалифицированных контрагентов.</param>
    /// <returns>Результаты согласования в формате "ФИО – результат".</returns>
    [Public, Remote(IsPure = true)]
    public static string GetApprovalResults(IApprovalCounterpartyRegisterTask approvalCounterpartyRegisterTask)
    {
      try
      {
        var committeeApprovalAssignments = GetActualCommitteeApprovalAssignments(approvalCounterpartyRegisterTask);
        // Согласовано.
        var approvedAssignments = committeeApprovalAssignments
          .Where(x => x.Result == Tenders.CommitteeApprovalAssignment.Result.Approved);
        var approvedResults = approvedAssignments
          .Select(x => lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Resources.ApprovedCounterpartyRegisterFormat(x.Performer.Name))
          .ToList();
        
        // Отказано.
        var rejectedAssignments = committeeApprovalAssignments
          .Where(x => x.Result == Tenders.CommitteeApprovalAssignment.Result.Rejected);
        var rejectedResults = rejectedAssignments
          .Select(x => lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Resources.RejectedCounterpartyRegisterFormat(x.Performer.Name))
          .ToList();
        
        var result = new List<string>();
        var i = 1;
        foreach (var approvalResult in approvedResults)
          result.Add(string.Format("{0}. {1}", i++, approvalResult));
        foreach (var approvalResult in rejectedResults)
          result.Add(string.Format("{0}. {1}", i++, approvalResult));
        
        return string.Join(Environment.NewLine, result);
      }
      catch(Exception ex)
      {
        Logger.Error("GetApprovalResults error function", ex);
      }
      return string.Empty;
    }
    
    /// <summary>
    /// Получить минимальное время выполнения задачи (обход направленного графа).
    /// </summary>
    /// <param name="baseDateTime">Базовое (опорное) время.</param>
    /// <returns>Срок выполнения задачи, вычисленный относительно базового времени.</returns>
    [Public]
    public DateTime GetMinExpectedDeadline(DateTime baseDateTime)
    {
      var startSchemeBlock = StartSchemeBlocks.Get(_obj.Scheme);

      // Инициализация.
      var schemeBlocks = SchemeBlocks.GetAll(_obj.Scheme).ToList();
      var distances = schemeBlocks.Select(x => Equals(x, startSchemeBlock) ? TimeSpan.Zero : TimeSpan.MaxValue).ToList();
      var isVisitedBlocks = schemeBlocks.Select(x => false).ToList();
      
      // Разметка графа.
      var visitedBlocksCount = 0;
      while(visitedBlocksCount < schemeBlocks.Count)
      {
        // Находим непосещенный блок с наименьшим дедлайном.
        var minSpanIndex = -1;
        for (var i = 0; i < schemeBlocks.Count; i++)
        {
          if (isVisitedBlocks[i])
            continue;
          
          if (minSpanIndex < 0 || TimeSpan.Compare(distances[i], distances[minSpanIndex]) < 0)
            minSpanIndex = i;
        }
        
        // Обновляем дедлайны соседних непосещенных блоков.
        var targetBlocks = _obj.Scheme.GetOutputEdges(schemeBlocks[minSpanIndex]).Select(x => x.Target);
        for(var j = 0; j < schemeBlocks.Count; j++)
        {
          // Смотрим соседние блоки, куда можно перейти.
          if (!targetBlocks.Contains(schemeBlocks[j]))
            continue;
          
          // В уже просмотренные блоки не возвращаемся.
          if (isVisitedBlocks[j])
            continue;
          
          var deadline = AddBlockDeadline(distances[minSpanIndex], schemeBlocks[j]);
          // Текущий интервал длиннее нового.
          if (TimeSpan.Compare(distances[j], deadline) > 0)
            distances[j] = deadline;
        }
        // Отмечаем вершину как посещенную.
        isVisitedBlocks[minSpanIndex] = true;
        visitedBlocksCount++;
      }
      
      for (var i = 0; i < schemeBlocks.Count; i++)
      {
        if (FinishSchemeBlocks.Is(schemeBlocks[i]))
        {
          var result = baseDateTime;
          result = Calendar.AddWorkingDays(result, distances[i].Days);
          result = Calendar.AddWorkingHours(result, distances[i].Hours);
          return result;
        }
      }
      
      throw new Exception("Не найден блок \"Конец\".");
    }
    
    /// <summary>
    /// Рассчитать время выполнения блока.
    /// </summary>
    /// <param name="deadline">Исходное время выполнения.</param>
    /// <param name="schemeBlock">Блок.</param>
    /// <returns>Суммарное время выполнения: исходное + время выполнения блока.</returns>
    private TimeSpan AddBlockDeadline(TimeSpan deadline, ISchemeBlock schemeBlock)
    {
      if (AssignmentSchemeBlocks.Is(schemeBlock))
      {
        var timeSpan = AssignmentSchemeBlocks.As(schemeBlock).Deadline.Relative;
        return (timeSpan != null && timeSpan.HasValue) ?
          deadline.Add(timeSpan.Value) :
          deadline;
      }
      
      if (TaskSchemeBlocks.Is(schemeBlock))
      {
        var timeSpan = TaskSchemeBlocks.As(schemeBlock).MaxDeadline.Relative;
        return (timeSpan != null && timeSpan.HasValue) ?
          deadline.Add(timeSpan.Value) :
          deadline;
      }
      
      // TODO process other block types.
      
      return deadline;
    }
  }
}