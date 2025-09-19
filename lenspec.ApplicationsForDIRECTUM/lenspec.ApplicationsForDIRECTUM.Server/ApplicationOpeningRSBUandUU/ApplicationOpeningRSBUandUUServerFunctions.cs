using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForDIRECTUM.ApplicationOpeningRSBUandUU;

namespace lenspec.ApplicationsForDIRECTUM.Server
{
  partial class ApplicationOpeningRSBUandUUFunctions
  {

    /// <summary>
    /// Построить сводку по документу.
    /// </summary>
    /// <returns>Сводка по документу.</returns>
    public override StateView GetDocumentSummary()
    {
      var documentSummary = StateView.Create();
      var block = documentSummary.AddBlock();
      var documentName = string.Empty;
      const string none = "-";

      // Наша. орг
      // Добавить к имени от <Наша организация>
      if (_obj.BusinessUnit != null)
        documentName += " от " + _obj.BusinessUnit.Name;
      // Добавить к имени с <Начало периода>
      if (_obj.BeginPeriod != null)
        documentName += " с " + _obj.BeginPeriod.Value.ToString("d");
      // Добавить к имени по <Конец периода>
      if (_obj.EndPeriod != null)
        documentName += " по " + _obj.EndPeriod.Value.ToString("d");
      // Добавить к имени <Вид учета>
      if (_obj.Type != null)
        documentName += ", " + _obj.Info.Properties.Type.GetLocalizedValue(_obj.Type);
      
      if (string.IsNullOrEmpty(documentName))
      {
        documentName = none;
      }
      
      block.AddLabel(documentName);
      return documentSummary;
    }
  }
}