using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Docflow.Server
{
  partial class ModuleJobs
  {

    /// <summary>
    /// Документооборот. Массовое назначение прав на задачи.
    /// </summary>
    public virtual void GrantAccessRightsToTaskslenspec()
    {
      var prefix = "avis - GrantAccessRightsToTasksJob -";
      var rules = lenspec.Etalon.PublicFunctions.AccessRightsRule.GetGrantingRightsOnTasks();
      foreach (var rule in rules)
      {
        Logger.DebugFormat("{0} Start granting rights by rule {1}.", prefix, rule.Name);
        Functions.Module.GrantAccessRightsToTasks(rule, false, prefix);
      }
    }

    /// <summary>
    /// Документооборот. Массовое назначение прав на документы.
    /// </summary>
    public override void GrantAccessRightsToDocuments()
    {
      base.GrantAccessRightsToDocuments();
    }

  }
}