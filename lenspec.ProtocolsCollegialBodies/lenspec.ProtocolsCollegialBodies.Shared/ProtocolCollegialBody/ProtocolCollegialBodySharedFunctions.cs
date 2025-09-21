using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.ProtocolCollegialBody;

namespace lenspec.ProtocolsCollegialBodies.Shared
{
  partial class ProtocolCollegialBodyFunctions
  {
    /// <summary>
    /// Заполнение имя документа.
    /// </summary>
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
      // Заполнить имя в формате: <Вид документа> №<Рег. номер> от <Дата проведения собрания>. Тема:<Тема> .
      using (TenantInfo.Culture.SwitchTo())
      {
        // Добавить к имени №<Номер>.
        if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
        // Добавить к имени от <Дата проведения собрания>.
        if (_obj.DateMeeting != null)
          name += " от " + _obj.DateMeeting.Value.ToString("d");
        // Добавить к имени Тема:<Тема>.
        if (_obj.Subject != null)
          name += ". Тема: " + _obj.Subject;
      }
      
      if (string.IsNullOrWhiteSpace(name))
        name = Sungero.Docflow.OfficialDocuments.Resources.DocumentNameAutotext;
      else if (_obj.DocumentKind != null)
        name = _obj.DocumentKind.ShortName + name;
      
      _obj.Name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
    }
  }
}