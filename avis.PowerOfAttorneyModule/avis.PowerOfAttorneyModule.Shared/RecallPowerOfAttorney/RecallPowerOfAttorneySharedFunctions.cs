using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.RecallPowerOfAttorney;

namespace avis.PowerOfAttorneyModule.Shared
{
  partial class RecallPowerOfAttorneyFunctions
  {
    public void FillName()
    {
      var count = _obj.PowerOfAttorney.PowerOfAttorneys.Count();
      
      // Карточка не сохранена, доверенностей нет.
      if (count == 0 && _obj.State.IsInserted)
        _obj.Subject = avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.DefaultSubjectTask;
      // Карточка сохранена, доверенностей нет.
      else if (count == 0)
        _obj.Subject = avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Info.LocalizedName;
      // Одна доверенность.
      else if (count == 1)
        _obj.Subject = avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources
          .SubjectTaskRecallAttorneyFormat(_obj.PowerOfAttorney.PowerOfAttorneys.First().Name);
      // 2 и более доверенности.
      else
        _obj.Subject =
          avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.SubjectTaskRecallAttorneys +
          string.Join(", ", _obj.PowerOfAttorney.PowerOfAttorneys.Select(x => x.Name));
    }

  }
}