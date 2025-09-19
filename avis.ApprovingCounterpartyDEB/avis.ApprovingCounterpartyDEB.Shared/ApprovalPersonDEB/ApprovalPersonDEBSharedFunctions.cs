using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalPersonDEB;

namespace avis.ApprovingCounterpartyDEB.Shared
{
  partial class ApprovalPersonDEBFunctions
  {

    /// <summary>
    /// Заполнить имя
    /// </summary>
    public override void FillName()
    {
      if (_obj.Counterparty != null)
      {
        var date = _obj.Created.Value.ToShortDateString();
        _obj.Name = string.Format("Согласование персоны {0} c ДБ от {1}", _obj.Counterparty, date);
      }
      else
        _obj.Name = string.Empty;
    }
    
    public void FillDateOfBirth()
    {
      _obj.DateOfBirth = null;
      if (_obj.Counterparty != null)
      {
        var person = lenspec.Etalon.People.As(_obj.Counterparty);
        if (person != null && person.DateOfBirth != null)
          _obj.DateOfBirth = person.DateOfBirth;
      }
    }

  }
}