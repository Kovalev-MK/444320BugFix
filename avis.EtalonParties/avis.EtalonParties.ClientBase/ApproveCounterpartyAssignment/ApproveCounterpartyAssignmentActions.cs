using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.ApproveCounterpartyAssignment;

namespace avis.EtalonParties.Client
{
  // Добавлено avis
  
  partial class ApproveCounterpartyAssignmentActions
  {
    /// <summary>
    /// Кнопка "создать банк".
    /// </summary>
    /// <param name="e"></param>
    public virtual void CreateBank(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Отображаем окно создания нового банка.
      var bank = Sungero.Parties.Banks.Create();
      bank.ShowModal();
      
      if (bank.State.IsInserted == false)
        _obj.AttachmentGroup.All.Add(bank);
    }

    /// <summary>
    /// Отображение кнопки "Создать банк".
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual bool CanCreateBank(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Отображаем кнопку если создание нового контрагента.
      if (_obj.CreateCompanyTask.TypeObject == CreateCompanyTask.TypeObject.Counterparty && _obj.CreateCompanyTask.TypeRequest == CreateCompanyTask.TypeRequest.NewEntry)
        return true;
      
      return false;
    }

    /// <summary>
    /// Кнопка "создать компанию".
    /// </summary>
    /// <param name="e"></param>
    public virtual void CreateCompany(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Отображаем окно создания новой организации.
      var company =  Sungero.Parties.Companies.Create();
      company.ShowModal();
      
      if (company.State.IsInserted == false)
        _obj.AttachmentGroup.All.Add(company);
    }

    /// <summary>
    /// Отображение кнопки "Создать компанию".
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual bool CanCreateCompany(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Отображаем кнопку если создание нового контрагента.
      if (_obj.CreateCompanyTask.TypeObject == CreateCompanyTask.TypeObject.Counterparty && _obj.CreateCompanyTask.TypeRequest == CreateCompanyTask.TypeRequest.NewEntry)
        return true;
      
      return false;
    }

    /// <summary>
    /// Кнопка "создать персону".
    /// </summary>
    /// <param name="e"></param>
    public virtual void CreatePerson(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Отображаем окно создания новой персоны.
      var person = Sungero.Parties.People.Create();
      person.ShowModal();
      
      if (person.State.IsInserted == false)
        _obj.AttachmentGroup.All.Add(person);
    }

    /// <summary>
    /// Отображение кнопки "Создать персону".
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual bool CanCreatePerson(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Отображаем кнопку если создание новой персону.
      if (_obj.CreateCompanyTask.TypeObject == CreateCompanyTask.TypeObject.Person && _obj.CreateCompanyTask.TypeRequest == CreateCompanyTask.TypeRequest.NewEntry)
        return true;
      
      return false;
    }

    /// <summary>
    /// Кнопка на доработку.
    /// </summary>
    /// <param name="e"></param>
    public virtual void Revision(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      // Валидация заполненности активного текста.
      if (!Functions.CreateCompanyTask.ValidateBeforeReject(_obj, "Оставьте комментарий в тексте задания, прежде чем отклонить.", e))
      {
        e.Cancel();
      }
    }

    public virtual bool CanRevision(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    /// <summary>
    /// Кнопка отклонить.
    /// </summary>
    /// <param name="e"></param>
    public virtual void Reject(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      // Валидация заполненности активного текста.
      if (!Functions.CreateCompanyTask.ValidateBeforeReject(_obj, "Оставьте комментарий в тексте задания, прежде чем отклонить.", e))
      {
        e.Cancel();
      }
    }

    public virtual bool CanReject(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    /// <summary>
    /// Кнопка "Выполнить".
    /// </summary>
    /// <param name="e"></param>
    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }
  }

  // Конец добавлено avis
}