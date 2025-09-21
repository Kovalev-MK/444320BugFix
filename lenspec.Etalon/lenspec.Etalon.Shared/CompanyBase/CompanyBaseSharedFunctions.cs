using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.CompanyBase;

namespace lenspec.Etalon.Shared
{
  partial class CompanyBaseFunctions
  {
    /// <summary>
    /// Получить текст ошибки о наличии дублей контрагента.
    /// </summary>
    /// <returns>Текст ошибки.</returns>
    [Public]
    public override string GetCounterpartyDuplicatesErrorText()
    {
      if (Users.Current.IsSystem == true)
        return base.GetCounterpartyDuplicatesErrorText();
      
      // Не проверять для закрытых записей.
      if (_obj.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
        return string.Empty;
      
      lenspec.Etalon.ICompanyBase duplicate = null;
      var company = lenspec.Etalon.CompanyBases.As(_obj);
      
      if (!string.IsNullOrEmpty(_obj.TIN) && !string.IsNullOrEmpty(_obj.TRRC))
        duplicate = CompanyBases.GetAll(c => c.TIN == company.TIN && c.TRRC == company.TRRC && c.Id != company.Id).FirstOrDefault();
      else if (!string.IsNullOrEmpty(_obj.TIN))
        duplicate = CompanyBases.GetAll(c => c.TIN == company.TIN && c.Id != company.Id).FirstOrDefault();
      
      // Выводим ошибку с дубликатом.
      if (duplicate == null)
        return string.Empty;
      
      return $"Найден дубль с таким же ИНН/КПП {duplicate.Name}";
    }
  }
}