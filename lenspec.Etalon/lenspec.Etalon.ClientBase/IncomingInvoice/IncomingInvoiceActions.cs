using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingInvoice;

namespace lenspec.Etalon.Client
{
  partial class IncomingInvoiceActions
  {
    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

    public virtual void CreateApplicationForPaymentlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanCreateApplicationForPaymentlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}