using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using lenspec.Etalon.AcquaintanceTask;
using Sungero.Domain.Shared;

namespace lenspec.Etalon.Server
{
  partial class AcquaintanceTaskRouteHandlers
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Условие "Есть главная задача?"
    /// </summary>
    /// <returns>True, если задача создана из другой задачи на ознакомление с количеством участников больше максимально допустимого, иначе false.</returns>
    public virtual bool Decision11EtalonResult()
    {
      if (_obj.GetStartedSchemeVersion() == LayerSchemeVersions.V1)
      {
        var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
        if (document == null || (!Sungero.RecordManagement.OrderBases.Is(document) && !Tenders.AccreditationCommitteeProtocols.Is(document)))
          return false;
        
        return _obj.TaskWithAllParticipantsavis != null;
      }
      return false;
    }

    public virtual void Script9EtalonExecute()
    {
      try
      {
        // Запомнить номер версии и хеш для отчета.
        var document = _obj.DocumentGroup.OfficialDocuments.First();
        if (document != null)
        {
          _obj.AcquaintanceVersions.Clear();
          Functions.AcquaintanceTask.StoreAcquaintanceVersionAvis(_obj, document, true);
          var addenda = _obj.AddendaGroup.OfficialDocuments;
          foreach (var addendum in addenda)
            Functions.AcquaintanceTask.StoreAcquaintanceVersionAvis(_obj, addendum, false);
        }
        
        #region Выдать права участникам до создания подчиненных задач
        
        Logger.DebugFormat("Avis - AcquaintanceTask {0}, custom grant rights.", _obj.Id);
        // Выдать права на просмотр исполнителям и наблюдателям.
        var documents = _obj.DocumentGroup.OfficialDocuments
          .Concat(_obj.AddendaGroup.OfficialDocuments)
          .Concat(_obj.OtherGroup.All)
          .AsQueryable<IEntity>();
        var recipients = _obj.Observers.Select(x => x.Observer).ToList();
        recipients.AddRange(_obj.Performers.Select(p => p.Performer));
        Sungero.Docflow.PublicFunctions.Module.GrantOptimalReadAccessRights(documents, recipients.AsQueryable<IRecipient>());
        
        #endregion
        
        var maxNumberOfPerformers = Convert.ToInt32(lenspec.EtalonDatabooks.ConstantDatabooks
                                                    .GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.MaxNumberOfAcquaintanceTaskPerformers)
                                                    .FirstOrDefault().Value);
        var performers = Sungero.RecordManagement.PublicFunctions.AcquaintanceTask.Remote.GetParticipants(_obj);
        var performersCount = performers.Count();
        int performersPartsCount = performersCount % maxNumberOfPerformers == 0
          ? performersCount / maxNumberOfPerformers
          : performersCount / maxNumberOfPerformers + 1;
        Logger.DebugFormat("Avis - AcquaintanceTask {0}, {1} performers, {2} parts count", _obj.Id, performers.Count(), performersPartsCount);
        for (int i = 0; i < performersPartsCount; i++)
        {
          try
          {
            var performersPart = performers.Skip(i * maxNumberOfPerformers).Take(maxNumberOfPerformers);
            lenspec.Etalon.PublicFunctions.AcquaintanceTask.CreateAcquaintanceSubTask(_obj, performersPart);
          }
          catch(Exception ex)
          {
            Logger.ErrorFormat("Avis - AcquaintanceTask {0}, error creating subtask.", ex, _obj.Id);
          }
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - AcquaintanceTask {0}, error set params.", ex, _obj.Id);
      }
    }

    /// <summary>
    /// Мониторинг завершения отправленных задач.
    /// </summary>
    /// <returns>True, если все подчиненные задачи завершены, иначе false.</returns>
    public virtual bool Monitoring10EtalonResult()
    {
      var existsNotCompleted = lenspec.Etalon.AcquaintanceTasks.GetAll(x => _obj.Equals(x.TaskWithAllParticipantsavis))
        .Any(x => x.Status != Sungero.RecordManagement.AcquaintanceTask.Status.Completed);
      
      return !existsNotCompleted;
    }

    /// <summary>
    /// Условие "Участников > макс. допустимого кол-ва?"
    /// </summary>
    /// <returns>True, если количество участников задачи превышает максимально допустимое, иначе false.</returns>
    public virtual bool Decision8EtalonResult()
    {
      if (_obj.GetStartedSchemeVersion() == LayerSchemeVersions.V1)
      {
        var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
        if (document == null || (!Sungero.RecordManagement.OrderBases.Is(document) && !Tenders.AccreditationCommitteeProtocols.Is(document)))
          return false;
        var maxNumberOfPerformers = Convert.ToInt32(lenspec.EtalonDatabooks.ConstantDatabooks
                                                    .GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.MaxNumberOfAcquaintanceTaskPerformers)
                                                    .FirstOrDefault().Value);
        var performersCount = Sungero.RecordManagement.PublicFunctions.AcquaintanceTask.Remote.GetParticipants(_obj).Count();
        
        return _obj.Performers.Any() && performersCount > maxNumberOfPerformers;
      }
      return false;
    }
    
    #region 3. Ознакомление
    
    public override void StartBlock3(Sungero.RecordManagement.Server.AcquaintanceAssignmentArguments e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.First();
      if (document == null || (!Sungero.RecordManagement.OrderBases.Is(document) && !Tenders.AccreditationCommitteeProtocols.Is(document)))
      {
        base.StartBlock3(e);
        return;
      }
      
      if (_obj.Deadline.HasValue)
        e.Block.AbsoluteDeadline = _obj.Deadline.Value;
      
      e.Block.IsParallel = true;
      e.Block.Subject = Sungero.RecordManagement.AcquaintanceTasks.Resources.AcquaintanceAssignmentSubjectFormat(_obj.DocumentGroup.OfficialDocuments.First().DisplayValue);
      var participants = Sungero.RecordManagement.PublicFunctions.AcquaintanceTask.Remote.GetParticipants(_obj);
      // Исключить неавтоматизированных пользователей из исполнителей.
      var notAutomated = lenspec.EtalonDatabooks.PublicFunctions.Module.GetNotAutomatedEmployeesAvis(participants).ToList();
      if (notAutomated.Any())
        participants = participants.Except(notAutomated).ToList();
      
      foreach (var participant in participants)
        e.Block.Performers.Add(participant);
      
      // Запомнить участников ознакомления.
      Functions.AcquaintanceTask.StoreAcquaintersAvis(_obj);
      
      if (_obj.GetStartedSchemeVersion() != LayerSchemeVersions.V1 || _obj.TaskWithAllParticipantsavis == null)
      {
        Logger.DebugFormat("Avis - AcquaintanceTask {0}, base grant rights.", _obj.Id);
        // Выдать права на просмотр исполнителям и наблюдателям.
        var documents = _obj.DocumentGroup.OfficialDocuments
          .Concat(_obj.AddendaGroup.OfficialDocuments)
          .Concat(_obj.OtherGroup.All)
          .AsQueryable<IEntity>();
        var recipients = _obj.Observers.Select(x => x.Observer).ToList();
        recipients.AddRange(_obj.Performers.Select(p => p.Performer));
        Sungero.Docflow.PublicFunctions.Module.GrantOptimalReadAccessRights(documents, recipients.AsQueryable<IRecipient>());
      }
      
      // Отправить запрос на подготовку предпросмотра для документов.
      Sungero.Docflow.PublicFunctions.Module.PrepareAllAttachmentsPreviews(_obj);
      
      // Запомнить номер версии и хеш для отчета.
      if (document != null)
      {
        _obj.AcquaintanceVersions.Clear();
        Functions.AcquaintanceTask.StoreAcquaintanceVersionAvis(_obj, document, true);
        var addenda = _obj.AddendaGroup.OfficialDocuments;
        foreach (var addendum in addenda)
          Functions.AcquaintanceTask.StoreAcquaintanceVersionAvis(_obj, addendum, false);
      }
    }
    
    #endregion
    //конец Добавлено Avis Expert

  }
}