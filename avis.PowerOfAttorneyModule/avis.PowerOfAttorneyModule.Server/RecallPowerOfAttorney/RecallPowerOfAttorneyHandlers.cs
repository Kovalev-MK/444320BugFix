using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.RecallPowerOfAttorney;

namespace avis.PowerOfAttorneyModule
{
  partial class RecallPowerOfAttorneyServerHandlers
  {

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      if (!_obj.MainAttachment.ApplicationRelinquishmentAuthorities.Any() || !_obj.MainAttachment.ApplicationRelinquishmentAuthorities.SingleOrDefault().HasVersions)
      {
        e.AddError(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.ErrorMessageEmptyMainAttachment);
        return;
      }
      if (!_obj.PowerOfAttorney.PowerOfAttorneys.Any())
      {
        e.AddError(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.ErrorMessageEmptyPowerOfAttorney);
        return;
      }
      // Проверка обновления полей шаблона на отзыв доверенностей.
      var documentTemplateFieldsUpdated = true;
      e.Params.TryGetValue(Constants.RecallPowerOfAttorney.Params.DocumentTemplateFieldsUpdated, out documentTemplateFieldsUpdated);
      if (!documentTemplateFieldsUpdated)
        e.AddError(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.UpdateDocumentTemplateFields, _obj.Info.Actions.UpdateApplicationNumberAndDate);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      Functions.RecallPowerOfAttorney.FillName(_obj);
    }
  }

}