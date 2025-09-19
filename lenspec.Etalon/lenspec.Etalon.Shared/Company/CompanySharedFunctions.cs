using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Company;

namespace lenspec.Etalon.Shared
{
  partial class CompanyFunctions
  {

    /// <summary>
    /// Проверка статуса согласования ДБ.
    /// </summary>
    /// <returns>Пустая строка при успешной валидации; Иначе – текст ошибки.</returns>
    public string ValidateApprovalDEBStatus()
    {
      if (
        _obj.ResultApprovalDEBavis == lenspec.Etalon.Company.ResultApprovalDEBavis.DoesNotReqAppr ||
        _obj.ResultApprovalDEBavis == lenspec.Etalon.Company.ResultApprovalDEBavis.CoopPossible ||
        _obj.ResultApprovalDEBavis == lenspec.Etalon.Company.ResultApprovalDEBavis.CoopWithRisks
       )
        return string.Empty;
      
      return lenspec.Etalon.Companies.Resources.CounterpartyNotApprovedDEB;
    }
  }
}