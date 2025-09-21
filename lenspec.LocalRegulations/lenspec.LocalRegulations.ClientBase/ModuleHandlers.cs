using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.LocalRegulations.Client
{
  partial class LocalRegulationOfCurrentUserFolderHandlers
  {

    //Добавлено Avis Expert
    public virtual void LocalRegulationOfCurrentUserValidateFilterPanel(Sungero.Domain.Client.ValidateFilterPanelEventArgs e)
    {
      // Не выполнять фильтрацию, если установлен произвольный период возврата документов и не заполнены критерии "с", "по".
      if (_filter.ManualPeriod && _filter.Info.DateRangeFrom == null && _filter.Info.DateRangeTo == null)
        e.AddError(lenspec.LocalRegulations.Resources.NeedSpecifyFilterParameters, _filter.Info.DateRangeFrom, _filter.Info.DateRangeTo);
    }
    //конец Добавлено Avis Expert
  }

  partial class LocalRegulationsOfOrganizationFolderHandlers
  {

    //Добавлено Avis Expert
    public virtual void LocalRegulationsOfOrganizationValidateFilterPanel(Sungero.Domain.Client.ValidateFilterPanelEventArgs e)
    {
      // Не выполнять фильтрацию, если установлен произвольный период возврата документов и не заполнены критерии "с", "по".
      if (_filter.ManualPeriod && _filter.Info.DateRangeFrom == null && _filter.Info.DateRangeTo == null)
        e.AddError(lenspec.LocalRegulations.Resources.NeedSpecifyFilterParameters, _filter.Info.DateRangeFrom, _filter.Info.DateRangeTo);
    }
    //конец Добавлено Avis Expert

  }
}