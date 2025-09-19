using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.AccreditationCommitteeProtocol;

namespace lenspec.Tenders.Shared
{
  partial class AccreditationCommitteeProtocolFunctions
  {
    
    /// <summary>
    /// Получить связанные документы, кроме приложений.
    /// </summary>
    /// <returns>Документы для группы Дополнительно.</returns>
    public IQueryable<Sungero.Content.IElectronicDocument> GetOtherDocuments()
    {
      var relatedDocuments = _obj.Relations.GetRelatedAndRelatedFromDocuments();
      // Исключить связанные документы, которые уже добавлены в группу Приложения.
      var addendaIds = _obj.Relations.GetRelatedAndRelatedFromDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName)
        .Select(x => x.Id);
      if (addendaIds.Any())
        relatedDocuments = relatedDocuments.Where(x => !addendaIds.Contains(x.Id));
      
      return relatedDocuments;
    }
    
    /// <summary>
    /// Обработать добавление документа как основного вложения в задачу.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <remarks>Только для задач, создаваемых пользователем вручную.</remarks>
    [Public]
    public override void DocumentAttachedInMainGroup(Sungero.Workflow.ITask task)
    {
      base.DocumentAttachedInMainGroup(task);
      
      if (Sungero.RecordManagement.AcquaintanceTasks.Is(task))
      {
        var acquaintanceTask = Sungero.RecordManagement.AcquaintanceTasks.As(task);
        var relatedDocuments = GetOtherDocuments();
        foreach (var document in relatedDocuments)
        {
          if (!acquaintanceTask.OtherGroup.All.Contains(document))
            acquaintanceTask.OtherGroup.All.Add(document);
        }
      }
    }
    
    /// <summary>
    /// Добавить в группу Дополнительно связанные документы.
    /// </summary>
    /// <param name="group">Группа вложений.</param>
    public override void AddRelatedDocumentsToAttachmentGroup(Sungero.Workflow.Interfaces.IWorkflowEntityAttachmentGroup group)
    {
      base.AddRelatedDocumentsToAttachmentGroup(group);
      
      var relatedDocuments = GetOtherDocuments();
      foreach (var document in relatedDocuments)
      {
        if (!group.All.Contains(document))
          group.All.Add(document);
      }
    }
    
    /// <summary>
    /// Убрать дубли адресатов.
    /// </summary>
    public virtual void DeleteDublicatesAddressees()
    {
      var distinctAdresseesList = _obj.Addressees.Select(r => r.Addressee).ToList().Distinct();
      if (distinctAdresseesList.Count() != _obj.Addressees.Count())
      {
        _obj.Addressees.Clear();
        foreach (var employee in distinctAdresseesList)
        {
          var newParticipantRow = _obj.Addressees.AddNew();
          newParticipantRow.Addressee = employee;
        }
      }
    }
    
    /// <summary>
    /// Заполнить адресатов из списка ознакомления/рассмотрения.
    /// </summary>
    /// <param name="acquaintanceList">Список ознакомления/рассмотрения.</param>
    /// <returns>Сообщение об ошибке или пустая строка.</returns>
    public virtual string TryFillFromAcquaintanceList(Sungero.RecordManagement.IAcquaintanceList acquaintanceList)
    {
      if (acquaintanceList == null)
        return "Выберите список ознакомления.";
      
      try
      {
        var participants = Sungero.RecordManagement.PublicFunctions.AcquaintanceList.GetParticipants(acquaintanceList);
        foreach (var participant in participants)
        {
          var newParticipantRow = _obj.Addressees.AddNew();
          newParticipantRow.Addressee = participant;
        }
        _obj.Save();
        return string.Empty;
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - TryFillFromAcquaintanceList - AccreditationCommitteeProtocol {0}. ", ex, _obj.Id);
        return ex.Message;
      }
    }

    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var documentKind = _obj.DocumentKind;
      
      if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      
      if (documentKind == null || !documentKind.GenerateDocumentName.Value)
        return;
      
      var name = string.Empty;
      using (TenantInfo.Culture.SwitchTo())
      {
        if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
        
        if (_obj.DocumentDate != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.DocumentDate.Value.ToString("d");
        
        if (_obj.BusinessUnit != null)
          name += " " + _obj.BusinessUnit.Name;
        
        if (_obj.Counterparty != null)
          name += lenspec.Tenders.AccreditationCommitteeProtocols.Resources.For + _obj.Counterparty.Name + ".";
        
        if (_obj.MeetingDate != null)
          name += lenspec.Tenders.AccreditationCommitteeProtocols.Resources.MeetingDateNamePart + _obj.MeetingDate.Value.ToString("d");
      }
      
      if (string.IsNullOrWhiteSpace(name))
      {
        if (_obj.VerificationState == null)
          name = Sungero.Docflow.Resources.DocumentNameAutotext;
        else
          name = _obj.DocumentKind.ShortName;
      }
      else if (documentKind != null)
      {
        name = documentKind.ShortName + name;
      }
      
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      if (name.Length > _obj.Info.Properties.Name.Length)
        _obj.Name = name.Substring(0, _obj.Info.Properties.Name.Length);
      else
        _obj.Name = name;
    }
    
    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      _obj.State.Properties.Subject.IsRequired = false;
    }
  }
}