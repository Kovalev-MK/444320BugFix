using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApplicationBUDocumentBase;

namespace lenspec.EtalonDatabooks.Shared
{
  partial class ApplicationBUDocumentBaseFunctions
  {
    
    //Добавлено Avis Expert
    
    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      _obj.State.Properties.Subject.IsRequired = false;
    }
    
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
      
      //Имя в формате: <Вид документа> <Наименование организации> от <Дата> Профиль компании: <Профиль компании>.
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.BusinessUnitName != null)
          name += _obj.BusinessUnitName;
        
        if (_obj.Created != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.Created.Value.ToString("d");
        
        if (_obj.CompanyProfile != null)
          name += "." + lenspec.EtalonDatabooks.Resources.CompanyProfileName + _obj.CompanyProfile.Name;
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
        name = documentKind.ShortName + " " + name;
      }
      
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      
      _obj.Name = Sungero.Docflow.PublicFunctions.OfficialDocument.AddClosingQuote(name, _obj);
    }
    //конец Добавлено Avis Expert
  }
}