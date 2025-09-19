using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientContract;

namespace lenspec.SalesDepartmentArchive.Shared
{
  partial class SDAClientContractFunctions
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
        <Вид документа недвижимости> №<№ договора> от <Дата договора>
        или <Вид документа> №<№ договора> от <Дата договора>, если поле Вид документа недвижимости пустое.
       */
      using (TenantInfo.Culture.SwitchTo())
      {
        if (!string.IsNullOrWhiteSpace(_obj.ClientDocumentNumber))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.ClientDocumentNumber;
        
        if (_obj.ClientDocumentDate != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.ClientDocumentDate.Value.ToString("d");
      }
      
      if (_obj.RealEstateDocumentKind != null)
      {
        name = _obj.RealEstateDocumentKind.Name + name;
      }
      else
      {
        name = _obj.DocumentKind.ShortName + name;
      }
      
      _obj.Name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
    }
 
    //конец Добавлено Avis Expert
  }
}