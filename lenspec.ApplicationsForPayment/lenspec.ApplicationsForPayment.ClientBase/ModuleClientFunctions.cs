using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ApplicationsForPayment.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Создать Заявку на оплату.
    /// </summary>
    [LocalizeFunction("CreateApplicationForPayment_ResourceKey", "CreateApplicationForPayment_DescriptionResourceKey")]
    public virtual void CreateApplicationForPayment()
    {
      lenspec.ApplicationsForPayment.PublicFunctions.Module.Remote.CreateApplicationForPayment().Show();
    }
  }
}