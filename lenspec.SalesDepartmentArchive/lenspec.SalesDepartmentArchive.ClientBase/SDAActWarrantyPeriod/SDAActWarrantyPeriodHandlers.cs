using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAActWarrantyPeriod;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAActWarrantyPeriodClientHandlers
  {
    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Если это "Акт комиссионного осмотра мест общего пользования".
      var documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.CommissionInspectionReportPublicKind);
      if (_obj.DocumentKind != null && documentKind == _obj.DocumentKind)
      {
        _obj.State.Properties.Client.IsRequired = false;
        _obj.State.Properties.ClientContract.IsRequired = false;
        _obj.State.Properties.BusinessUnit.IsRequired = true;
        _obj.State.Properties.BusinessUnit.IsEnabled = true;
        _obj.State.Properties.Name.IsEnabled = true;
      }
      else
      {
        _obj.State.Properties.Client.IsRequired = true;
        _obj.State.Properties.ClientContract.IsRequired = true;
        _obj.State.Properties.BusinessUnit.IsRequired = false;
        _obj.State.Properties.BusinessUnit.IsEnabled = false;
        _obj.State.Properties.Name.IsEnabled = false;
      }
    }
  }
}