using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDARequestSubmissionToArchive;

namespace lenspec.SalesDepartmentArchive.Shared
{
  partial class SDARequestSubmissionToArchiveFunctions
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
      
      /* Имя в формате:
        <Вид документа> №<Рег. номер> от <Подразделение> от <Дата создания>. Место хранения <Место хранения>.
       */
      using (TenantInfo.Culture.SwitchTo())
      {
        if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
        
        if (_obj.Department != null)
        {
          if (!string.IsNullOrEmpty(_obj.Department.ShortName))
          {
            name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.Department.ShortName;
          }
          else
          {
            name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.Department.Name;
          }
        }
        
        if (_obj.RegistrationDate != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.RegistrationDate.Value.ToString("d");
        
        if (_obj.StorageAddress != null)
          name += lenspec.SalesDepartmentArchive.SDARequestSubmissionToArchives.Resources.StoragePlaceForName + _obj.StorageAddress.Name;
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
    //конец Добавлено Avis Expert
  }
}