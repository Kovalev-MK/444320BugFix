using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectObjectPermitDocument;

namespace lenspec.EtalonDatabooks.Shared
{
  partial class ProjectObjectPermitDocumentFunctions
  {
    
    //Добавлено Avis Expert
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
      var fullName = string.Empty;
      using (TenantInfo.Culture.SwitchTo())
      {
        if (!string.IsNullOrWhiteSpace(_obj.NumberRNV))
        {
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.NumberRNV;
          if (_obj.SpecDeveloper != null)
            name += ",";
          
          fullName += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.NumberRNV;
        }
        
        if (_obj.SpecDeveloper != null)
          name += " " + _obj.SpecDeveloper.Name;
        
        if (_obj.DateRNV != null)
          fullName += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.DateRNV.Value.ToString("d") + ".";
        
        if (_obj.OurCF != null)
          fullName += " ИСП: " + _obj.OurCF.CommercialName + ".";
        
        if (_obj.CollectionAccountingProject.Any(x => x.AccountingProject != null))
          fullName += " Объекты: " + string.Join(", ", _obj.CollectionAccountingProject.Where(x => x.AccountingProject != null).Select(x => x.AccountingProject.Name).ToList());
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
        fullName = documentKind.ShortName + fullName;
      }
      
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      if (name.Length > _obj.Info.Properties.Name.Length)
        _obj.Name = name.Substring(0, _obj.Info.Properties.Name.Length);
      else
        _obj.Name = name;
      
      fullName = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(fullName);
      if (fullName.Length > _obj.Info.Properties.FullName.Length)
        _obj.FullName = fullName.Substring(0, _obj.Info.Properties.FullName.Length);
      else
        _obj.FullName = fullName;
    }
 
    //конец Добавлено Avis Expert
  }
}