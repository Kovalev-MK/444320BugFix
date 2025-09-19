using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ContractorRegister;

namespace lenspec.Tenders.Server
{
  partial class ContractorRegisterFunctions
  {
    
    /// <summary>
    /// Получить наименования ячеек Excel с данными о КА.
    /// </summary>
    /// <returns>Наименования ячеек Excel.</returns>
    [Public]
    public override List<string> GetCellsWithData()
    {
      return new List<string>(){
        Constants.ContractorRegister.XmlCells.Contact.Email,
        Constants.ContractorRegister.XmlCells.Contact.Position,
        Constants.ContractorRegister.XmlCells.Contact.FullName,
        Constants.ContractorRegister.XmlCells.Contact.PhoneWork,
        Constants.ContractorRegister.XmlCells.Contact.PhonePersonal,
        Constants.ContractorRegister.XmlCells.Counterparty.Email,
        Constants.ContractorRegister.XmlCells.Counterparty.Position,
        Constants.ContractorRegister.XmlCells.Counterparty.FullName,
        Constants.ContractorRegister.XmlCells.Counterparty.PhoneWork,
        Constants.ContractorRegister.XmlCells.Counterparty.PhonePersonal
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
        data[Constants.ContractorRegister.XmlCells.Contact.Position],
        data[Constants.ContractorRegister.XmlCells.Contact.FullName],
        data[Constants.ContractorRegister.XmlCells.Contact.PhoneWork],
        data[Constants.ContractorRegister.XmlCells.Contact.PhonePersonal],
      };
      _obj.Contact = string.Join(", ", contacts.Where(x => !string.IsNullOrWhiteSpace(x)));
      _obj.ContactEmail = data[Constants.ContractorRegister.XmlCells.Contact.Email];
      
      var counterparties = new List<string>(){
        data[Constants.ContractorRegister.XmlCells.Counterparty.Position],
        data[Constants.ContractorRegister.XmlCells.Counterparty.FullName],
        data[Constants.ContractorRegister.XmlCells.Counterparty.PhoneWork],
        data[Constants.ContractorRegister.XmlCells.Counterparty.PhonePersonal],
      };
      _obj.CounterpartyCEO = string.Join(", ", counterparties.Where(x => !string.IsNullOrWhiteSpace(x)));
      _obj.CounterpartyEmail = data[Constants.ContractorRegister.XmlCells.Counterparty.Email];
    }
    
  }
}