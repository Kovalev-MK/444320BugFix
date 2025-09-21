using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ProviderRegister;

namespace lenspec.Tenders.Server
{
  partial class ProviderRegisterFunctions
  {
    
    /// <summary>
    /// Получить наименования ячеек Excel с данными о КА.
    /// </summary>
    /// <returns>Наименования ячеек Excel.</returns>
    [Public]
    public override List<string> GetCellsWithData()
    {
      return new List<string>(){
        Constants.ProviderRegister.XmlCells.Contact.Email,
        Constants.ProviderRegister.XmlCells.Contact.Position,
        Constants.ProviderRegister.XmlCells.Contact.FullName,
        Constants.ProviderRegister.XmlCells.Contact.PhoneWork,
        Constants.ProviderRegister.XmlCells.Contact.PhonePersonal,
        Constants.ProviderRegister.XmlCells.Counterparty.Email,
        Constants.ProviderRegister.XmlCells.Counterparty.Position,
        Constants.ProviderRegister.XmlCells.Counterparty.FullName,
        Constants.ProviderRegister.XmlCells.Counterparty.PhoneWork,
        Constants.ProviderRegister.XmlCells.Counterparty.PhonePersonal
      };
    }
    
    /// <summary>
    /// Заполнить данные о КА в карточке реестра.
    /// </summary>
    /// <param name="data">Данные о КА из ячеек таблицы.</param>
    /// <remarks>Не сохраняет сущность.</remarks>
    [Public]
    public override void FillData(System.Collections.Generic.Dictionary<string, string> data)
    {
      var contacts = new List<string>(){
        data[Constants.ProviderRegister.XmlCells.Contact.Position],
        data[Constants.ProviderRegister.XmlCells.Contact.FullName],
        data[Constants.ProviderRegister.XmlCells.Contact.PhoneWork],
        data[Constants.ProviderRegister.XmlCells.Contact.PhonePersonal],
      };
      _obj.Contact = string.Join(", ", contacts.Where(x => !string.IsNullOrWhiteSpace(x)));
      _obj.ContactEmail = data[Constants.ProviderRegister.XmlCells.Contact.Email];
      
      var counterparties = new List<string>(){
        data[Constants.ProviderRegister.XmlCells.Counterparty.Position],
        data[Constants.ProviderRegister.XmlCells.Counterparty.FullName],
        data[Constants.ProviderRegister.XmlCells.Counterparty.PhoneWork],
        data[Constants.ProviderRegister.XmlCells.Counterparty.PhonePersonal],
      };
      _obj.CounterpartyCEO = string.Join(", ", counterparties.Where(x => !string.IsNullOrWhiteSpace(x)));
      _obj.CounterpartyEmail = data[Constants.ProviderRegister.XmlCells.Counterparty.Email];
    }
    
  }
}