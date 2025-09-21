using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace avis.PrinterSettings.Server
{
  // Добавлено Avis
  
  public partial class ModuleInitializer
  {
    /// <summary>
    /// Инициализация.
    /// </summary>
    /// <param name="e"></param>
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      GetGrantFromOrubters();
    }
    
    /// <summary>
    /// Выдача прав на справочники.
    /// </summary>
    private void GetGrantFromOrubters()
    {
      InitializationLogger.DebugFormat("Init: Get grant PrinterSettings.");
      // Выдача прав на чтение справочника с настройками принтеров.
      avis.PrinterSettings.Printerses.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      ScanSettings.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      
      Printerses.AccessRights.Save();
      ScanSettings.AccessRights.Save();
    }
  }
  
  // Конец добавлено avis
}
