using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.ResponsibleByCounterparty;

namespace avis.EtalonParties.Shared
{
  partial class ResponsibleByCounterpartyFunctions
  {

    public void FillName()
    {
      _obj.Name = string.Empty;
      
      if (_obj.Counterparty != null)
      {
        _obj.Name = "Ответственные по контрагенту " + _obj.Counterparty.Name;
        if (_obj.BusinessUnit != null)
        {
          _obj.Name += " для НОР " + _obj.BusinessUnit.Name;
        }
      }
      else
        _obj.Name = Resources.DatabookNameAutotext;
    }
  }
}