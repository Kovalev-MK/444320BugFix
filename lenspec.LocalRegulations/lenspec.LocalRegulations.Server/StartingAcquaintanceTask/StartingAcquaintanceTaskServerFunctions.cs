using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.LocalRegulations.StartingAcquaintanceTask;

namespace lenspec.LocalRegulations.Server
{
  partial class StartingAcquaintanceTaskFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = base.Execute(approvalTask);
      Logger.DebugFormat("Avis - StartingAcquaintanceTask - старт сценария.");
      
      try
      {
        var document = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
        if (document == null)
        {
          return this.GetErrorResult("Не найден документ.");
        }
        
        var employees = GetDocumentAddresses(document);
        if (!employees.Any())
        {
          return this.GetErrorResult("Не найдены исполнители.");
        }
        
        var task = Sungero.RecordManagement.PublicFunctions.Module.Remote.CreateAcquaintanceTask(document);
        
        #region Список ознакомления
        
        if (lenspec.Tenders.TenderCommitteeProtocols.Is(document) ||
            lenspec.Tenders.AccreditationCommitteeProtocols.Is(document))
        {
          task.Performers.AddNew().Performer = approvalTask.Author;
          var addApprovers = approvalTask.AddApprovers.Select(x => x.Approver).AsEnumerable();
          foreach (var approver in addApprovers)
          {
            if (!task.Performers.Any(x => x.Performer == approver))
              task.Performers.AddNew().Performer = approver;
          }
        }
        foreach (var employee in employees)
        {
          if (!task.Performers.Any(x => x.Performer == employee))
            task.Performers.AddNew().Performer = employee;
        }
        Logger.DebugFormat("Avis - StartingAcquaintanceTask: задача ИД {0}, {1} исполнителей.", task.Id, task.Performers.Count);
        
        #endregion
        
        // Инициатор задачи.
        task.Author = GetTaskAuthor(document);
        
        // Срок Задачи на ознакомление.
        task.Deadline = Calendar.Now.AddWorkingDays(_obj.AcquaintanceTaskDeadline ?? 1);
        
        // Ознакомление в электронном виде.
        task.IsElectronicAcquaintance = _obj.IsElectronicAcquaintance ?? false;
        
        // Вложения группы Дополнительно.
        var others = approvalTask.OtherGroup.All;
        foreach(var attachment in others)
        {
          if (!task.OtherGroup.All.Contains(attachment))
            task.OtherGroup.All.Add(attachment);
        }
        
        if (Tenders.TenderCommitteeProtocols.Is(document))
        {
          // Вложения группы Приложения.
          var addendas = approvalTask.AddendaGroup.All;
          foreach (var attachment in addendas)
          {
            if (!task.AddendaGroup.All.Contains(attachment))
              task.AddendaGroup.All.Add(attachment);
          }
        }



        
        #region Выдача прав на задачу
        
        var approvalTaskAccessRights = approvalTask.AccessRights.Current;
        foreach(var accessRight in approvalTaskAccessRights)
        {
          if (!task.AccessRights.IsGranted(accessRight.AccessRightsType, accessRight.Recipient))
          {
            task.AccessRights.Grant(accessRight.Recipient, accessRight.AccessRightsType);
          }
        }
        
        #endregion
        
        task.Start();
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Avis - StartingAcquaintanceTask: ", ex);
        return this.GetErrorResult(ex.Message);
      }
      Logger.DebugFormat("Avis - StartingAcquaintanceTask - окончание сценария.");
      return result;
    }
    
    /// <summary>
    /// Получить сотрудников из поля документа Адресаты.
    /// </summary>
    /// <returns>Список сотрудников-адресатов.</returns>
    private List<Sungero.Company.IEmployee> GetDocumentAddresses(Sungero.Docflow.IOfficialDocument document)
    {
      var employees = new List<Sungero.Company.IEmployee>();
      
      if (Etalon.IncomingLetters.Is(document))
      {
        var addresses = Etalon.IncomingLetters.As(document).Addressees.Select(x => x.Addressee).ToList();
        if (addresses != null && addresses.Any())
        {
          employees.AddRange(addresses);
        }
      }
      if (Etalon.Memos.Is(document))
      {
        var addresses = Etalon.Memos.As(document).Addressees.Select(x => x.Addressee).ToList();
        if (addresses != null && addresses.Any())
        {
          employees.AddRange(addresses);
        }
      }
      if (Etalon.OrderBases.Is(document))
      {
        var addresses = Etalon.OrderBases.As(document).Addresseeslenspec.Select(x => x.Addressee).ToList();
        if (addresses != null && addresses.Any())
        {
          employees.AddRange(addresses);
        }
      }
      if (lenspec.Tenders.TenderCommitteeProtocols.Is(document))
      {
        var addresses = lenspec.Tenders.TenderCommitteeProtocols.As(document).Addressees.Select(x => x.Addressee).ToList();
        if (addresses != null && addresses.Any())
        {
          employees.AddRange(addresses);
        }
      }
      if (lenspec.Tenders.AccreditationCommitteeProtocols.Is(document))
      {
        var committee = lenspec.Tenders.AccreditationCommitteeProtocols.As(document).AccreditationCommittee;
        if (committee != null)
        {
          employees.Add(committee.Chairman);
          var members = committee.Members.Select(x => x.Member).ToList();
          if (members != null && members.Any())
          {
            employees.AddRange(members);
          }
        }
        var addresses = lenspec.Tenders.AccreditationCommitteeProtocols.As(document).Addressees.Select(x => x.Addressee).ToList();
        if (addresses != null && addresses.Any())
        {
          employees.AddRange(addresses);
        }
      }
      
      return employees;
    }
    
    /// <summary>
    /// Получить Инициатора задачи на ознакомление.
    /// </summary>
    /// <returns>Инициатор задачи.</returns>
    private Sungero.CoreEntities.IUser GetTaskAuthor(Sungero.Docflow.IOfficialDocument document)
    {
      var author = Sungero.CoreEntities.Users.Null;
      
      if (lenspec.Tenders.TenderCommitteeProtocols.Is(document))
      {
        var protocol = lenspec.Tenders.TenderCommitteeProtocols.As(document);
        if (protocol.TenderCommittee != null)
          author = protocol.TenderCommittee.ProtocolRegistrar;
        
        return author;
      }
      
      if (lenspec.Tenders.AccreditationCommitteeProtocols.Is(document))
      {
        var protocol = lenspec.Tenders.AccreditationCommitteeProtocols.As(document);
        if (protocol.AccreditationCommittee != null)
          author = protocol.AccreditationCommittee.Registrator;
        
        return author;
      }
      
      if (document.BusinessUnit != null)
      {
        var roleKindEmployeeBU = Etalon.BusinessUnits.GetAll(x => x.Equals(document.BusinessUnit)).Single().RoleKindEmployeelenspec;
        if (roleKindEmployeeBU.Any())
        {
          var performerBU = roleKindEmployeeBU.Where(x => x.RoleKind.Name.Equals(lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk)).FirstOrDefault();
          if (performerBU != null)
          {
            return performerBU.Employee;
          }
        }
      }
      
      if (document.DocumentRegister != null)
      {
        var registerGroup = document.DocumentRegister.RegistrationGroup;
        if (registerGroup != null)
        {
          author = Sungero.CoreEntities.Users.As(registerGroup.ResponsibleEmployee);
        }
      }
      if (author == null && document.PreparedBy != null && document.PreparedBy.Login != null)
      {
        author = Sungero.CoreEntities.Users.As(document.PreparedBy);
      }
      return author ?? document.Author;
    }
    //конец Добавлено Avis Expert
  }
}