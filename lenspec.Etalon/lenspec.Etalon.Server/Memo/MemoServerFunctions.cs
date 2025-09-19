using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Memo;

namespace lenspec.Etalon.Server
{
  partial class MemoFunctions
  {
    /// <summary>
    /// Получить наши организации для фильтрации подходящих прав подписи.
    /// </summary>
    /// <returns>Наши организации.</returns>
    public override List<Sungero.Company.IBusinessUnit> GetBusinessUnits()
    {
      // Проверка вхождения в роль "Пользователи с правами на указание в документах сотрудников из любых НОР".
      var roleSid = lenspec.EtalonDatabooks.PublicConstants.Module.RightsToSelectAnyEmployees;
      var hasRightsToSelectAnyEmployees = Users.Current.IncludedIn(roleSid);
      
      // Если сотрудник входит в роль, отправляем пустой список НОР в GetSignatureSettingsQuery().
      // Доступные для выбора сотрудники по НОР не фильтруются (см. Functions.SignatureSetting.GetSignatureSettings()).
      if (hasRightsToSelectAnyEmployees)
        return new List<Sungero.Company.IBusinessUnit>() { };
      else
        return base.GetBusinessUnits();
    }
  }
}