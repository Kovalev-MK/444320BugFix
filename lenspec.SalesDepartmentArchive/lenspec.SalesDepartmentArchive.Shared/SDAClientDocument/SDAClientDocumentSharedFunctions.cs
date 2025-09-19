using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientDocument;

namespace lenspec.SalesDepartmentArchive.Shared
{
  partial class SDAClientDocumentFunctions
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
        if (_obj.ClientContract != null)
        {
          if (!string.IsNullOrWhiteSpace(_obj.ClientContract.ClientDocumentNumber))
            name += lenspec.SalesDepartmentArchive.SDAClientDocuments.Resources.ToContractNumber + _obj.ClientContract.ClientDocumentNumber;
          
          if (_obj.ClientContract.ClientDocumentDate != null)
            name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.ClientContract.ClientDocumentDate.Value.ToString("d");
        }
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