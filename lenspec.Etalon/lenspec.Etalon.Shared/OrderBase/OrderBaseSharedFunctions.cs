using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OrderBase;
using lenspec.Etalon.Constants.RecordManagement;

namespace lenspec.Etalon.Shared
{
  partial class OrderBaseFunctions
  {
    /// <summary>
    /// Очистить адресатов и заполнить первого адресата из карточки.
    /// </summary>
    public virtual void ClearAndFillFirstAddressee()
    {
      _obj.Addresseeslenspec.Clear();
      if (_obj.Addresseelenspec != null)
      {
        var newAddressee = _obj.Addresseeslenspec.AddNew();
        newAddressee.Addressee = _obj.Addresseelenspec;
        newAddressee.Number = 1;
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
        return string.Empty;
      
      var participants = Sungero.RecordManagement.PublicFunctions.AcquaintanceList.GetParticipants(acquaintanceList);
      /*
      var addresseesLimit = this.GetAddresseesLimit();
      var notAutomatedEmployees = Sungero.Company.PublicFunctions.Module.Remote.GetNotAutomatedEmployees(participants);
      participants = participants.Except(notAutomatedEmployees).ToList();
      if (participants.Count > addresseesLimit)
        return Memos.Resources.TooManyParticipantsFormat(addresseesLimit);
       */
      foreach (var participant in participants)
      {
        var newParticipantRow = _obj.Addresseeslenspec.AddNew();
        newParticipantRow.Addressee = participant;
      }
      _obj.Save();
      return string.Empty;
    }
    
    /// <summary>
    /// Заполнить адресата из коллекции адресатов.
    /// </summary>
    public virtual void FillAddresseeFromAddressees()
    {
      var addressee = _obj.Addresseeslenspec.OrderBy(a => a.Number).FirstOrDefault(a => a.Addressee != null);
      if (addressee != null)
        _obj.Addresseelenspec = addressee.Addressee;
      else
        _obj.Addresseelenspec = null;
    }
    
    /// <summary>
    /// Установить метку "Несколько адресатов".
    /// </summary>
    public virtual void SetManyAddresseesPlaceholder()
    {
      // Заполнить метку в локали тенанта.
      using (TenantInfo.Culture.SwitchTo())
        _obj.ManyAddresseesPlaceholderlenspec = Sungero.Docflow.OfficialDocuments.Resources.ManyAddresseesPlaceholder;
    }
    
    /// <summary>
    /// Получить максимальное количество адресатов.
    /// </summary>
    /// <returns>Максимальное количество адресатов.</returns>
    public virtual int GetAddresseesLimit()
    {
      return Constants.RecordManagement.OrderBase.AddresseesLimit;
    }
    /// <summary>
    /// Убрать дубли адресатов.
    /// </summary>
    public virtual void DeleteDublicatesAddressees()
    {
      var distinctAdresseesList = _obj.Addresseeslenspec.Select(r => r.Addressee).ToList().Distinct();
      if (distinctAdresseesList.Count() != _obj.Addresseeslenspec.Count())
      {
        _obj.Addresseeslenspec.Clear();
        foreach (var employee in distinctAdresseesList)
        {
          var newParticipantRow = _obj.Addresseeslenspec.AddNew();
          newParticipantRow.Addressee = employee;
        }
      }
    }
  }
}