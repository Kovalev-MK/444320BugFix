using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForDIRECTUM.ApplicationOpeningRSBUandUU;

namespace lenspec.ApplicationsForDIRECTUM.Client
{
  partial class ApplicationOpeningRSBUandUUFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Показывать сводку по документу в заданиях на согласование и подписание.
    /// </summary>
    /// <returns>True, если в заданиях нужно показывать сводку по документу.</returns>
    [Public]
    public override bool NeedViewDocumentSummary()
    {
      return true;
    }
    //конец Добавлено Avis Expert
  }
}