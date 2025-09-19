using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.BankDetail;

namespace avis.EtalonParties
{
  partial class BankDetailServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      // Заполнение контрагента при создании из карточки организации.
      if (CallContext.CalledFrom(Sungero.Parties.Companies.Info))
      {
        var companyId = CallContext.GetCallerEntityId(Sungero.Parties.Companies.Info);
        var company = Sungero.Parties.Counterparties.Get(companyId);
        _obj.Counterparty = company;
        return;
      }
      
      // Заполнение контрагента при создании из карточки банка.
      if (CallContext.CalledFrom(Sungero.Parties.Banks.Info))
      {
        var bankId = CallContext.GetCallerEntityId(Sungero.Parties.Banks.Info);
        var bank = Sungero.Parties.Banks.Get(bankId);
        _obj.Counterparty = bank;
        _obj.BIC = bank.BIC;
        return;
      }
      
      // Заполнение контрагента при создании из карточки персоны.
      if (CallContext.CalledFrom(Sungero.Parties.People.Info))
      {
        var personId = CallContext.GetCallerEntityId(Sungero.Parties.People.Info);
        var person = Sungero.Parties.People.Get(personId);
        _obj.Counterparty = person;
      }
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (Functions.BankDetail.GetDuplicates(_obj).Any())
      {
        e.AddError(BankDetails.Resources.DuplicatesFound, _obj.Info.Actions.ShowDuplicates);
        return;
      }
      
      // Проверка заполненности поля "Корр. счет".
      if (string.IsNullOrEmpty(_obj.CorrespondentAccount) || string.IsNullOrWhiteSpace(_obj.CorrespondentAccount))
      {
        e.AddError(avis.EtalonParties.BankDetails.Resources.NoCorrAccountErrorMessage);
        return;
      }
    }
  }

}