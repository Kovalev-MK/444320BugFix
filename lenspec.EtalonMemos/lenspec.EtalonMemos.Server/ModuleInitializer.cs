using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.EtalonMemos.Server
{
  public partial class ModuleInitializer
  {

    /// <summary>
    /// Обработчик события инициализации.
    /// </summary>
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Вычисляемые папки.
      GrantRightsOnFolder();
    }

    /// <summary>
    /// Функция инициализации для выдачи прав на вычисляемую папку.
    /// </summary>
    public static void GrantRightsOnFolder()
    {
      var allUsers = Roles.AllUsers;
      lenspec.EtalonMemos.SpecialFolders.AssignmentsByMemoForMe.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      lenspec.EtalonMemos.SpecialFolders.AssignmentsByMemoForMe.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Выданные мне поручения'");
      
      lenspec.EtalonMemos.SpecialFolders.AssignmentsByMemoFromMe.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      lenspec.EtalonMemos.SpecialFolders.AssignmentsByMemoFromMe.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Выданные мною поручения'");
      
      lenspec.EtalonMemos.SpecialFolders.ControlIssuedAssignments.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      lenspec.EtalonMemos.SpecialFolders.ControlIssuedAssignments.AccessRights.Save();
      InitializationLogger.Debug("Выданы права на вычисляемую папку 'Контроль выданных поручений'");
    }
  }
}
