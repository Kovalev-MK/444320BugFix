using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustomerRequest;

namespace avis.CustomerRequests.Shared
{
  partial class CustomerRequestFunctions
  {
    
    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var documentKind = _obj.DocumentKind;
      
      if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      
      if (documentKind == null || !documentKind.GenerateDocumentName.Value)
        return;
      
      var name = string.Empty;
      using (TenantInfo.Culture.SwitchTo())
      {
        if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
        
        if (_obj.RegistrationDate != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.RegistrationDate.Value.ToString("d");
        
        if (_obj.Client != null)
        {
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.Client.ShortName;
        }
        
        if (_obj.ReqCategory != null)
        {
          name += ", (" + _obj.ReqCategory.CategoryCode + ") " + _obj.ReqCategory.Name;
        }
      }
      
      if (string.IsNullOrWhiteSpace(name))
      {
        if (_obj.VerificationState == null)
          name = Sungero.Docflow.Resources.DocumentNameAutotext;
        else
          name = _obj.DocumentKind.ShortName;
      }
      else if (documentKind != null)
      {
        name = documentKind.ShortName + name;
      }
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      if (name.Length > _obj.Info.Properties.Name.Length)
        _obj.Name = name.Substring(0, _obj.Info.Properties.Name.Length);
      else
        _obj.Name = name;
    }
    
    /// <summary>
    /// Получить сообщение об ошибке для неподдерживаемых форматов.
    /// </summary>
    /// <param name="extension">Расширение.</param>
    /// <returns>Результат преобразования.</returns>
    public override Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult GetExtensionValidationError(string extension)
    {
      return base.GetExtensionValidationError(extension);
    }
  }
}