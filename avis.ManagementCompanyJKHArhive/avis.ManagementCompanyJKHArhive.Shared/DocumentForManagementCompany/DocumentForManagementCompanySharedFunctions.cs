using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ManagementCompanyJKHArhive.DocumentForManagementCompany;

namespace avis.ManagementCompanyJKHArhive.Shared
{
  partial class DocumentForManagementCompanyFunctions
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
        if (!string.IsNullOrWhiteSpace(_obj.Number))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.Number;
        if (_obj.DateDocument != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.DateDocument.Value.ToString("d");
        
        if (_obj.MKD != null)
        {
          name += avis.ManagementCompanyJKHArhive.DocumentForManagementCompanies.Resources.ToManagementContractMKD;
          if (!string.IsNullOrEmpty(_obj.MKD.Number))
            name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.MKD.Number;
          if (_obj.MKD.DateDocument != null)
            name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.MKD.DateDocument.Value.ToString("d");
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
  }
}