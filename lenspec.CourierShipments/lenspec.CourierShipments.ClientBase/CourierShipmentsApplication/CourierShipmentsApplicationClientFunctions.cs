using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.CourierShipments.CourierShipmentsApplication;

namespace lenspec.CourierShipments.Client
{
  partial class CourierShipmentsApplicationFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Показывать сводку по документу в заданиях на согласование и подписание.
    /// </summary>
    /// <returns>True, если в заданиях нужно показывать сводку по документу.</returns>
    [Public]
    public bool NeedViewInstruction()
    {
      return true;
    }
    //конец Добавлено Avis Expert
  }
}