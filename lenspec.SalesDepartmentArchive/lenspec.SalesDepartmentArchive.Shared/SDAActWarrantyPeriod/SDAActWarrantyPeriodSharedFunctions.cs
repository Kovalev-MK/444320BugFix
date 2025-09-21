using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAActWarrantyPeriod;

namespace lenspec.SalesDepartmentArchive.Shared
{
  partial class SDAActWarrantyPeriodFunctions
  {
    /// <summary>
    /// Заполнить имя документа, базовый.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.CommissionInspectionReportPublicKind);
      if (_obj.DocumentKind == null || documentKind != _obj.DocumentKind)
      {
        base.FillName();
        return;
      }
      
      // Формируем имя документа, есди это акт осмотра мест общего пользования.
      //_obj.Name = $"{_obj?.DocumentKind?.Name}";
      _obj.Name = $"Акт комиссионного осмотра МОП";
      
      if (_obj.ActDate.HasValue)
        _obj.Name += $" от {_obj?.ActDate.Value.ToString("dd.MM.yyyy")}";
      
      if (_obj.ClientContract != null)
        _obj.Name += $" к {_obj?.ClientContract?.Name}";
      
      if (_obj.Client != null)
        _obj.Name += $" от {_obj?.Client?.Name}";
      
      if (_obj.Room != null)
        _obj.Name += $" {_obj?.Room?.Name}";
    }
  }
}