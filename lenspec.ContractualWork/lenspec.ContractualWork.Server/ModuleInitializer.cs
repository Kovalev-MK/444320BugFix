using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.ContractualWork.Server
{
  public partial class ModuleInitializer
  {
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Справочники.
      ReadRightsOnDatabooks();
    }
    
    /// <summary>
    /// Выдать права на справочники (на чтение).
    /// </summary>
    /// <param name="allUsers">Роль "Все пользователи"</param>
    public static void ReadRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Read rights on databooks to all users.");
      
      var allUsers = Sungero.CoreEntities.Roles.AllUsers;
      
      BudgetItemses.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      DecodingBudgetItemses.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      GroupsBudgetItemses.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      BudgetItemses.AccessRights.Save();
      DecodingBudgetItemses.AccessRights.Save();
      GroupsBudgetItemses.AccessRights.Save();
    }
  }
}
