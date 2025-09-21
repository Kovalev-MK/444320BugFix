using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ApplicationsForPayment.Server
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Создать Заявку на оплату.
    /// </summary>
    /// <returns>Заявка на оплату.</returns>
    [Remote, Public]
    public static lenspec.ApplicationsForPayment.IApplicationForPayment CreateApplicationForPayment()
    {
      return lenspec.ApplicationsForPayment.ApplicationForPayments.Create();
    }
  }
}