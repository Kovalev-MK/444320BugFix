using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnSale;

namespace lenspec.EtalonDatabooks.Server
{
  partial class ObjectAnSaleFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Получить для суммы значение прописью с указанной валютой.
    /// </summary>
    /// <param name="amount">Сумма.</param>
    /// <param name="currency">Валюта.</param>
    /// <returns>Сумма прописью с валютой.</returns>
    [Converter("AmountInCurrencyToWordsObjectAnSale")]
    public static string AmountInCurrencyToWordsObjectAnSale(IObjectAnSale objectAnSale)
    {
      var currency = Sungero.Commons.Currencies.GetAll(x => x.AlphaCode.Contains("RUB")).FirstOrDefault();
      var amount = objectAnSale.SurchargeAmount ?? objectAnSale.RefundAmount;
      if (amount == null || currency == null)
        return null;
      
      return EtalonDatabooks.PublicFunctions.Module.AmountInCurrencyToWords(amount, currency);
    }
    
    [Public]
    public void FillAddress()
    {
      var newAddress = "";
      
      if (_obj.ObjectAnProject != null && !string.IsNullOrEmpty(_obj.ObjectAnProject.AddressMail))
        newAddress += $"{_obj.ObjectAnProject.AddressMail}, ";
      
      if (_obj.PurposeOfPremises != null && !string.IsNullOrEmpty(_obj.PurposeOfPremises.ShortName))
        newAddress += $"{_obj.PurposeOfPremises.ShortName} ";
      
      if (!string.IsNullOrEmpty(_obj.NumberRoomMail))
        newAddress += $"{_obj.NumberRoomMail}";
      
      _obj.Address = newAddress;
    }
    //конец Добавлено Avis Expert
  }
}