using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAActBase;

namespace lenspec.SalesDepartmentArchive.Shared
{
  partial class SDAActBaseFunctions
  {
    
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
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.ActDate != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.ActDate.Value.ToString("d");
        
        if (_obj.ClientContract != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.NamePartForLeadDocument + _obj.ClientContract.Name;
        
        if (_obj.Client != null)
          name += lenspec.SalesDepartmentArchive.SDAActBases.Resources.NamePartClient + _obj.Client.Name;
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
      
      _obj.Name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
    }
  }
}