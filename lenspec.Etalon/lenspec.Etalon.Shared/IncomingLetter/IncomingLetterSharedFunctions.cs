using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingLetter;

namespace lenspec.Etalon.Shared
{
  partial class IncomingLetterFunctions
  {

    public override void FillName()
    {
      if (_obj.Correspondent != null && Sungero.Parties.Companies.Is(_obj.Correspondent))
      {
        //Получить вид документа.
        var documentKind = _obj.DocumentKind;
        //Очистить имя.
        if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.OfficialDocuments.Resources.DocumentNameAutotext)
          _obj.Name = string.Empty;
        if (documentKind == null || !documentKind.GenerateDocumentName.Value)
          return;
        var name = string.Empty;
        // <Вид документа> от <Корреспондент> № <№> от <Дата от> <”Содержание”>.
        using (TenantInfo.Culture.SwitchTo())
        {
          // Добавить к имени от <Корреспондент>
          if (_obj.Correspondent != null)
            name += " от " + _obj.Correspondent.DisplayValue;
          // Добавить к имени № <№>
          if (!string.IsNullOrWhiteSpace(_obj.InNumber))
            name += " № " + _obj.InNumber;
          // Добавить к имени от <Дата от>
          if (_obj.Dated != null)
            name += " от " + _obj.Dated.Value.ToString("d");
          // Добавить к имени <”Содержание”>.
          if (!string.IsNullOrWhiteSpace(_obj.Subject))
            name += " \"" + _obj.Subject + "\"";
          
          if (string.IsNullOrWhiteSpace(name))
          {
            name = _obj.VerificationState == null ? OfficialDocuments.Resources.DocumentNameAutotext : _obj.DocumentKind.ShortName;
          }
          else if (_obj.DocumentKind != null)
          {
            name = _obj.DocumentKind.ShortName + name;
          }
          
          name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
          
          _obj.Name = Sungero.Docflow.PublicFunctions.OfficialDocument.AddClosingQuote(name, _obj);
        }
      }
      else
        base.FillName();
    }

  }
}