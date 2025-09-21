using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.CourierShipments.Server
{
  public class ModuleFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Создать курьерское отправление.
    /// </summary>
    /// <returns>Курьерское отправление.</returns>
    [Remote, Public]
    public static CourierShipments.ICourierShipmentsApplication CreateCourierShipmentsApplication()
    {
      return CourierShipments.CourierShipmentsApplications.Create();
    }
    //конец Добавлено Avis Expert
  }
}