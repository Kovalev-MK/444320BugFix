using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForDIRECTUM.ApplicationOpeningRSBUandUU;

namespace lenspec.ApplicationsForDIRECTUM.Shared
{
  partial class ApplicationOpeningRSBUandUUFunctions
  {

    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      _obj.State.Properties.Subject.IsRequired = false;
    }
    
    public override void FillName()
    {
      //Получить вид документа.
      var documentKind = _obj.DocumentKind;
      //Очистить имя.
      if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.OfficialDocuments.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      if (documentKind == null || !documentKind.GenerateDocumentName.Value)
        return;
      var name = string.Empty;
      // Заполнить имя в формате: <Вид документа> от <Наша организация> с <Начало периода> по <Конец периода>.
      using (TenantInfo.Culture.SwitchTo())
      {
        // Добавить к имени от <Наша организация>
        if (_obj.BusinessUnit != null)
          name += " от " + _obj.BusinessUnit;
        // Добавить к имени с <Начало периода>
        if (_obj.BeginPeriod != null)
          name += " с " + _obj.BeginPeriod.Value.ToString("d");
        // Добавить к имени по <Конец периода>
        if (_obj.EndPeriod != null)
          name += " по " + _obj.EndPeriod.Value.ToString("d");
      }
      if (string.IsNullOrWhiteSpace(name))
        name = Sungero.Docflow.OfficialDocuments.Resources.DocumentNameAutotext;
      else if (_obj.DocumentKind != null)
        name = _obj.DocumentKind.ShortName + name;
      _obj.Name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
    }

  }
}