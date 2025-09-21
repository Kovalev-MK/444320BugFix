using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Bank;

namespace lenspec.Etalon.Server
{
  partial class BankFunctions
  {

    [Public]
    /// <summary>
    /// Проверить наличие обязательных полей для выгрузки записи в 1С.
    /// </summary>
    /// <returns>Сообщение об ошибке.</returns>
    public string CheckRequiredFieldsForExport()
    {
      var errors = new List<string>();
      var emptyFields = new List<string>();
      if (string.IsNullOrEmpty(_obj.LegalName) || string.IsNullOrWhiteSpace(_obj.LegalName))
        emptyFields.Add(_obj.Info.Properties.LegalName.LocalizedName);
      
      if (string.IsNullOrEmpty(_obj.TIN) || string.IsNullOrWhiteSpace(_obj.TIN))
        emptyFields.Add(_obj.Info.Properties.TIN.LocalizedName);
      
      if (string.IsNullOrEmpty(_obj.TRRC) || string.IsNullOrWhiteSpace(_obj.TRRC))
        emptyFields.Add(_obj.Info.Properties.TRRC.LocalizedName);
      
      if (string.IsNullOrEmpty(_obj.PostalAddress) || string.IsNullOrWhiteSpace(_obj.PostalAddress))
        emptyFields.Add(_obj.Info.Properties.PostalAddress.LocalizedName);
      
      if (emptyFields.Any())
        errors.Add(lenspec.Etalon.Banks.Resources.BankHasEmptyRequiredFieldsFormat(string.Join(", ", emptyFields)));
      
      return string.Join(Environment.NewLine, errors);
    }
  }
}