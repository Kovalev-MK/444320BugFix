using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingInvoice;

namespace lenspec.Etalon.Server
{
  partial class IncomingInvoiceFunctions
  {

    /// <summary>
    /// Посчитать остаточную сумму договора
    /// </summary>
    public void CalculateRemainingAmountLeadingContract()
    {
      if(_obj.Contract == null)
        return;
      
      var handler = avis.EtalonContracts.AsyncHandlers.CalculateRemainingAmount.Create();
      handler.ContractId = _obj.Contract.Id;
      handler.ExecuteAsync();
    }

  }
}