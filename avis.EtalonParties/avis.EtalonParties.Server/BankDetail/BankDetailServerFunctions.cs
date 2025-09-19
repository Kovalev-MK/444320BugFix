using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.BankDetail;

namespace avis.EtalonParties.Server
{
  partial class BankDetailFunctions
  {
    /// <summary>
    /// Получить банковские реквизиты по контрагенту.
    /// </summary>
    /// <param name="counterparty">Контрагент.</param>
    /// <returns>Выборка реквизитов для указанного контрагента.</returns>
    [Public, Remote(IsPure = true)]
    public static IQueryable<IBankDetail> GetBankDetails(Sungero.Parties.ICounterparty counterparty)
    {
      return EtalonParties.BankDetails.GetAll(d => Equals(d.Counterparty, counterparty));
    }
    
    /// <summary>
    /// Поиск банка по БИК.
    /// </summary>
    /// <param name="bic">БИК.</param>
    /// <returns>Банк.</returns>
    [Remote(IsPure = true)]
    public static Sungero.Parties.IBank GetBank(string bic)
    {
      return Sungero.Parties.Banks.GetAll(b => b.BIC == bic).FirstOrDefault();
    }
    
    /// <summary>
    /// Получение дублей банковских реквизитов.
    /// </summary>
    /// <returns>Сведения, дублирующие текущие.</returns>
    [Remote(IsPure = true)]
    public IQueryable<IBankDetail> GetDuplicates()
    {
      return BankDetails.GetAll(d =>
                                !Equals(d, _obj) &&
                                Equals(d.Counterparty, _obj.Counterparty) &&
                                Equals(d.BankAccount, _obj.BankAccount));
    }
  }
}