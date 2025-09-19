using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using avis.EtalonParties.CreateCompanyTask;

namespace avis.EtalonParties.Server
{
  // Добавлено avis.
  
  partial class CreateCompanyTaskRouteHandlers
  {

    public virtual void StartAssignment3(avis.EtalonParties.IApproveCounterpartyAssignment assignment, avis.EtalonParties.Server.ApproveCounterpartyAssignmentArguments e)
    {
      if (_obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
        assignment.ActiveText = string.Empty;
    }

    /// <summary>
    /// Выполнение блока Согласование с отвественным за контрагентов.
    /// </summary>
    /// <param name="assignment"></param>
    /// <param name="e"></param>
    public virtual void CompleteAssignment3(avis.EtalonParties.IApproveCounterpartyAssignment assignment, avis.EtalonParties.Server.ApproveCounterpartyAssignmentArguments e)
    {
      var parallelAssignments = Functions.Module.GetParallelAssignments(assignment);
      var activeParallelAssignments = parallelAssignments
        .Where(a => a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
        .Where(a => !Equals(a, assignment))
        .Where(a => avis.EtalonParties.ApproveCounterpartyAssignments.Is(a));
      
      foreach (var parallelAssignment in activeParallelAssignments)
      {
        if (!string.IsNullOrEmpty(parallelAssignment.ActiveText))
          parallelAssignment.ActiveText += Environment.NewLine;
        
        if (assignment.CompletedBy != null && !assignment.CompletedBy.Equals(assignment.Performer))
          parallelAssignment.ActiveText += $"Выполнено сотрудником: {assignment.CompletedBy.Name} за {assignment.Performer.Name}.";
        else
          parallelAssignment.ActiveText += $"Выполнено сотрудником: {assignment.Performer.Name}.";

        parallelAssignment.Abort();
      }
    }
    
    /// <summary>
    /// Старт блока об отсутствии ответственных за контрагентов.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock9(Sungero.Workflow.Server.NoticeArguments e)
    {
      // Заполняем получателя уведомления.
      e.Block.Performers.Add(_obj.Author);
      // Заполняем тему уведомления.
      e.Block.Subject = $"Отсутствуют ответственные за создание контрагентов, обратитесь к администратору.";
    }
    
    /// <summary>
    /// Старт блока уведомление о создании контрагента.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock4(Sungero.Workflow.Server.NoticeArguments e)
    {
      // Заполняем получателя уведомления.
      e.Block.Performers.Add(_obj.Author);
      // Заполняем тему уведомления.
      if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty && _obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.NewEntry)
        e.Block.Subject = $"Контрагент {_obj.CompanyName}, {_obj.TIN} создан.";
      if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty && _obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.EditEntry)
        e.Block.Subject = $"Запись контрагента {_obj.Company.Name}, {_obj.TIN} скорректирована.";
      if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Person && _obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.NewEntry)
        e.Block.Subject = $"Персона {_obj.FIOPerson} создана.";
      if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Person && _obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.EditEntry)
        e.Block.Subject = $"Персона {_obj.Person.Name} скорректирована.";
      
      if (_obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
      {
        if (_obj.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.NewEntry)
        {
          if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty)
            e.Block.Subject = $"Уведомление об исполнении заявки «Ответственные по контрагентам» для контрагента: {_obj.Company.Name}.";
          if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Person)
            e.Block.Subject = $"Уведомление об исполнении заявки «Ответственные по контрагентам» для контрагента: {_obj.Person.Name}.";
        }
        
        if (_obj.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.EditEntry)
        {
          e.Block.Subject = $"Уведомление об исполнении заявки «Ответственные по контрагентам» для контрагента: {_obj.ResponsibleByCounterparty.Counterparty.Name}.";
        }
      }
    }
    
    /// <summary>
    /// Старт блока уведомление об отклонении контрагента.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock5(Sungero.Workflow.Server.NoticeArguments e)
    {
      // Заполняем получателя уведомления.
      e.Block.Performers.Add(_obj.Author);
      // Заполняем тему уведомления.
      if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty && _obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.NewEntry)
        e.Block.Subject = $"Контрагент {_obj.CompanyName}, {_obj.TIN} отклонен.";
      if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty && _obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.EditEntry)
        e.Block.Subject = $"Корректировка записи контрагента {_obj.Company.Name}, {_obj.TIN} отклонена.";
      if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Person && _obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.NewEntry)
        e.Block.Subject = $"Персона {_obj.FIOPerson} отклонена.";
      if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Person && _obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.EditEntry)
        e.Block.Subject = $"Корректировка персоны {_obj.Person.Name} отклонена.";
      
      if (_obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
      {
        if (_obj.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.NewEntry)
        {
          if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty)
            e.Block.Subject = $"Уведомление об отклонении заявки «Ответственные по контрагентам» для контрагента: {_obj.Company.Name}.";
          if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Person)
            e.Block.Subject = $"Уведомление об отклонении заявки «Ответственные по контрагентам» для контрагента: {_obj.Person.Name}.";
        }
        
        if (_obj.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.EditEntry)
        {
          e.Block.Subject = $"Уведомление об отклонении заявки «Ответственные по контрагентам» для контрагента: {_obj.ResponsibleByCounterparty.Counterparty.Name}.";
        }
      }
    }
    
    /// <summary>
    /// Старт блока на доработку.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock6(avis.EtalonParties.Server.ApproveRevisionArguments e)
    {
      // Определить исполнителя.
      if (_obj.Forwarding != null)
      {
        e.Block.Performers.Add(_obj.Forwarding);
        _obj.Forwarding = null;
      }
      else
        e.Block.Performers.Add(_obj.Author);

      // Устанавливаем тему.
      e.Block.Subject = $"Доработайте: {_obj.Subject}";
      // Передаём нужные параметры.
      e.Block.TIN = _obj.TIN;
      e.Block.CompanyName = _obj.CompanyName;
      e.Block.Company = _obj.Company;
      e.Block.CreateCompanyTask = _obj;
      e.Block.TypeObject = _obj.Info.Properties.TypeObject.GetLocalizedValue(_obj.TypeObject);
      e.Block.TypeRequest = _obj.Info.Properties.TypeRequest.GetLocalizedValue(_obj.TypeRequest);
      e.Block.FIOPerson = _obj.FIOPerson;
      e.Block.Person = _obj.Person;
      e.Block.DateOfBirth = _obj.DateOfBirth;
      e.Block.SpecifyNeedToBeMade = _obj.SpecifyNeedToBeMade;
      e.Block.Comment = _obj.Comment;
      
      if (_obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
      {
        e.Block.DatabookActionType = _obj.DatabookActionType;
        if (_obj.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.NewEntry)
        {
          e.Block.ResponsibleEmployee = _obj.ResponsibleEmployee;
          e.Block.BusinessUnit = _obj.BusinessUnit;
        }
        if (_obj.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.EditEntry)
        {
          e.Block.ResponsibleByCounterparty = _obj.ResponsibleByCounterparty;
        }
      }
    }
    
    /// <summary>
    /// Старт блока Согласование с отвественным за контрагентов.
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock3(avis.EtalonParties.Server.ApproveCounterpartyAssignmentArguments e)
    {
      // Для Создание или изменение ответственного по контрагентам исполнитель по Виду роли.
      if (_obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
      {
        var performers = new List<Sungero.Company.IEmployee>();
        if (_obj.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.NewEntry)
        {
          performers = lenspec.Etalon.PublicFunctions.BusinessUnit.GetRoleKindPerformer(_obj.BusinessUnit, avis.EtalonParties.Constants.ResponsibleByCounterparty.ResponsibleByCounterpartyRoleKindName);
        }
        if (_obj.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.EditEntry)
        {
          performers = lenspec.Etalon.PublicFunctions.BusinessUnit.GetRoleKindPerformer(_obj.ResponsibleByCounterparty.BusinessUnit, avis.EtalonParties.Constants.ResponsibleByCounterparty.ResponsibleByCounterpartyRoleKindName);
        }
        foreach (var performer in performers)
          e.Block.Performers.Add(performer);
      }
      else
      {
        // Устанавливаем исполнителями, всех с ролью "Канцелярия ГК".
        var officeGKRoleGuid = lenspec.EtalonDatabooks.PublicConstants.Module.OfficeGK;
        var role = Roles.GetAll(r => r.Sid == officeGKRoleGuid).FirstOrDefault();
        e.Block.Performers.Add(role);
      }
      // Передаём нужные параметры.
      e.Block.TIN = _obj.TIN;
      e.Block.CompanyName = _obj.CompanyName;
      e.Block.Company = _obj.Company;
      e.Block.CreateCompanyTask = _obj;
      e.Block.TypeObject = _obj.Info.Properties.TypeObject.GetLocalizedValue(_obj.TypeObject);
      e.Block.TypeRequest = _obj.Info.Properties.TypeRequest.GetLocalizedValue(_obj.TypeRequest);
      e.Block.FIOPerson = _obj.FIOPerson;
      e.Block.Person = _obj.Person;
      e.Block.DateOfBirth = _obj.DateOfBirth;
      e.Block.SpecifyNeedToBeMade = _obj.SpecifyNeedToBeMade;
      
      // Задаём тему задания.
      if (_obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.NewEntry)
      {
        e.Block.Comment = _obj.Comment;
        if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty)
          e.Block.Subject = $"Создание контрагента: {_obj.CompanyName}, {_obj.TIN}.";
        if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Person)
          e.Block.Subject = $"Создание персоны: {_obj.FIOPerson}.";
      }
      if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty && _obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.EditEntry)
      {
        e.Block.Subject = $"Измените контрагента: {_obj.Company.Name}, {_obj.TIN}.";
        _obj.AttachmentGroup.All.Add(_obj.Company);
      }

      if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Person && _obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.EditEntry)
      {
        e.Block.Subject = $"Измените персону: {_obj.Person.Name}.";
        _obj.AttachmentGroup.All.Add(_obj.Person);
      }
      
      if (_obj.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
      {
        e.Block.DatabookActionType = _obj.DatabookActionType;
        if (_obj.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.NewEntry)
        {
          if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty)
            e.Block.Subject = $"Необходимо внести изменения в справочник «Ответственные по контрагентам» для контрагента: {_obj.Company.Name}.";
          if (_obj.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Person)
            e.Block.Subject = $"Необходимо внести изменения в справочник «Ответственные по контрагентам» для контрагента: {_obj.Person.Name}.";
          e.Block.ResponsibleEmployee = _obj.ResponsibleEmployee;
          e.Block.BusinessUnit = _obj.BusinessUnit;
          e.Block.Comment = _obj.Comment;
        }
        
        if (_obj.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.EditEntry)
        {
          e.Block.Subject = $"Необходимо внести изменения в справочник «Ответственные по контрагентам» для контрагента: {_obj.ResponsibleByCounterparty.Counterparty.Name}.";
          e.Block.ResponsibleByCounterparty = _obj.ResponsibleByCounterparty;
        }
      }
    }
  }
  
  // Конец блока avis.
}