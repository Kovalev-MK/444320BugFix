using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Counterparty;

namespace lenspec.Etalon
{
  partial class CounterpartySharedHandlers
  {

    public virtual void SalesAgentlenspecChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      // Не распространять логику на Банки.
      if (!Sungero.Parties.Banks.Is(_obj))
      {
        if (e.NewValue == true)
          _obj.ResultApprovalDEBavis = Counterparty.ResultApprovalDEBavis.DoesNotReqAppr;
        else
          _obj.ResultApprovalDEBavis = Counterparty.ResultApprovalDEBavis.NotAssessed;
      }
    }
  }


}