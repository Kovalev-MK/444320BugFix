using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.AcquaintanceTask;

namespace lenspec.Etalon.Server
{
  partial class AcquaintanceTaskFunctions
  {

    //Добавлено Avis Expert
    
    [Public]
    public static void CreateAcquaintanceSubTask(lenspec.Etalon.IAcquaintanceTask mainTask,
                                                 System.Collections.Generic.IEnumerable<Sungero.CoreEntities.IRecipient> performers)
    {
      var subtask = lenspec.Etalon.AcquaintanceTasks.Create();
      subtask.TaskWithAllParticipantsavis = mainTask;
      subtask.Subject = mainTask.Subject;
      subtask.Author = mainTask.Author;
      subtask.Deadline = mainTask.Deadline;
      subtask.IsElectronicAcquaintance = mainTask.IsElectronicAcquaintance;
      subtask.ReceiveOnCompletion = mainTask.ReceiveOnCompletion;
      subtask.ActiveText = mainTask.ActiveText;
      // Участники.
      foreach(var performer in performers)
      {
        var item = subtask.Performers.AddNew();
        item.Performer = performer;
      }
      // Вложения.
      var documentGroup = mainTask.DocumentGroup.All;
      foreach(var document in documentGroup)
      {
        if (!subtask.DocumentGroup.All.Contains(document))
          subtask.DocumentGroup.All.Add(document);
      }
      var addendaGroup = mainTask.AddendaGroup.All;
      foreach(var addenda in addendaGroup)
      {
        if (!subtask.AddendaGroup.All.Contains(addenda))
          subtask.AddendaGroup.All.Add(addenda);
      }
      var otherGroup = mainTask.OtherGroup.All;
      foreach(var other in otherGroup)
      {
        if (!subtask.OtherGroup.All.Contains(other))
          subtask.OtherGroup.All.Add(other);
      }
      subtask.Start();
      Logger.DebugFormat("Avis - CreateAcquaintanceSubTask - subtask {0} started.", subtask.Id);
    }
    
    /// <summary>
    /// Запомнить участников ознакомления.
    /// </summary>
    public void StoreAcquaintersAvis()
    {
      this.StoreAcquainters();
    }
    
    /// <summary>
    /// Получить сообщения валидации при старте.
    /// </summary>
    /// <returns>Сообщения валидации.</returns>
    [Remote(IsPure = true)]
    public override List<Sungero.RecordManagement.Structures.AcquaintanceTask.StartValidationMessage> GetStartValidationMessage()
    {
      var errors = new List<Sungero.RecordManagement.Structures.AcquaintanceTask.StartValidationMessage>();
      
      // Проверить наличие документа в задаче и наличие прав на него.
      if (!Functions.AcquaintanceTask.HasDocumentAndCanRead(_obj))
      {
        errors.Add(Sungero.RecordManagement.Structures.AcquaintanceTask.StartValidationMessage.Create(Sungero.Docflow.Resources.NoRightsToDocument, false, false));
        return errors;
      }
      
      var document = _obj.DocumentGroup.OfficialDocuments.First();
      var authorIsNonEmployeeMessage = Sungero.Docflow.PublicFunctions.Module.ValidateTaskAuthor(_obj);
      if (!string.IsNullOrWhiteSpace(authorIsNonEmployeeMessage))
        errors.Add(Sungero.RecordManagement.Structures.AcquaintanceTask.StartValidationMessage.Create(authorIsNonEmployeeMessage, false, true));
      
      // Проверить существование тела документа.
      if (_obj.IsElectronicAcquaintance.Value && !document.HasVersions)
        errors.Add(Sungero.RecordManagement.Structures.AcquaintanceTask.StartValidationMessage.Create(Sungero.RecordManagement.AcquaintanceTasks.Resources.AcquaintanceTaskDocumentWithoutBodyMessage, false, false));
      
      // Валидация подписи документа.
      var validationMessages = document.HasVersions
        ? Sungero.RecordManagement.PublicFunctions.Module.GetDocumentSignatureValidationErrors(document.LastVersion, true)
        : new List<string>();
      if (validationMessages.Any())
      {
        validationMessages.Insert(0, Sungero.RecordManagement.Resources.SignatureValidationErrorMessage);
        errors.Add(Sungero.RecordManagement.Structures.AcquaintanceTask.StartValidationMessage.Create(string.Join(Environment.NewLine, validationMessages), false, false));
      }
      
      // Проверить корректность срока.
      if (!Sungero.Docflow.PublicFunctions.Module.CheckDeadline(_obj.Deadline, Calendar.Now))
        errors.Add(Sungero.RecordManagement.Structures.AcquaintanceTask.StartValidationMessage.Create(Sungero.RecordManagement.Resources.ImpossibleSpecifyDeadlineLessThanToday, false, false));
      
      // Проверить наличие участников ознакомления.
      var employees = Sungero.RecordManagement.PublicFunctions.AcquaintanceTask.Remote.GetParticipants(_obj);
      if (employees.Count == 0)
        errors.Add(Sungero.RecordManagement.Structures.AcquaintanceTask.StartValidationMessage.Create(Sungero.RecordManagement.AcquaintanceTasks.Resources.PerformersCantBeEmpty, false, false));
      
      //Добавлено Avis Expert
      // Для Приказа/Распоряжения и Протокола комитета аккредитации игнорировать проверку на превышение количества участников и наличие неавтоматизированных сотрудников.
      if (!Sungero.RecordManagement.OrderBases.Is(document) && !Tenders.AccreditationCommitteeProtocols.Is(document))
      {
        // Техническое ограничение платформы на запуск задачи для большого числа участников.
        var performersLimit = Sungero.Docflow.PublicFunctions.Module.Remote.GetDocflowParamsNumbericValue(Sungero.RecordManagement.Constants.AcquaintanceTask.PerformersLimitParamName);
        if (employees.Count > performersLimit)
          errors.Add(Sungero.RecordManagement.Structures.AcquaintanceTask.StartValidationMessage.Create(Sungero.RecordManagement.AcquaintanceTasks.Resources.TooManyPerformersFormat(performersLimit), false, false));
        
        // Запрещено отправлять ознакомления неавтоматизированным сотрудникам без замещения.
        var notAutomatedEmployees = Sungero.Company.PublicFunctions.Module.Remote.GetNotAutomatedEmployees(employees);
        if (notAutomatedEmployees.Any())
          errors.Add(Sungero.RecordManagement.Structures.AcquaintanceTask.StartValidationMessage.Create(AcquaintanceTasks.Resources.NotAutomatedUserWithoutSubstitutionError, true, false));
      }
      //конец Добавлено Avis Expert
      return errors;
    }
    //конец Добавлено Avis Expert
  }
}