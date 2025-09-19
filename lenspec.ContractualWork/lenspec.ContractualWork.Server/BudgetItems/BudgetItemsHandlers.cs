using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ContractualWork.BudgetItems;

namespace lenspec.ContractualWork
{
  partial class BudgetItemsServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.Codifier != null)
        _obj.Name = _obj.FullName + '(' + _obj.Codifier + ')'; 
      else 
        _obj.Name = _obj.FullName;
    }
  }
}