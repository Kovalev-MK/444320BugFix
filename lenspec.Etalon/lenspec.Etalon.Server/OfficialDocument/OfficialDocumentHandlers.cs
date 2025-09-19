using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OfficialDocument;

namespace lenspec.Etalon
{
  partial class OfficialDocumentServerHandlers
  {

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      
      // Назначение прав на связанные с документом задачи.
      var rules = PublicFunctions.AccessRightsRule.GetGrantingRightsOnTasks();
      foreach (var rule in rules)
        if (PublicFunctions.AccessRightsRule.GetMatchingDocuments(rule).Any(x => Equals(x, _obj)))
          lenspec.Etalon.Module.Docflow.PublicFunctions.Module.EnqueueGrantAccessRightsToTasks(_obj, rule);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.Archiveavis = false;
    }
  }
}