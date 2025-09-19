using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.CourierShipments.Client
{
  public class ModuleFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Создать курьерское отправления.
    /// </summary>
    [LocalizeFunction("CreateCourierShipmentsApplication_ResourceKey", "CreateCourierShipmentsApplication_DescriptionResourceKey")]
    public virtual void CreateCourierShipmentsApplication()
    {
      CourierShipments.PublicFunctions.Module.Remote.CreateCourierShipmentsApplication().Show();
    }
    //конец Добавлено Avis Expert

  }
}