using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAActWarrantyPeriod;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAActWarrantyPeriodSharedHandlers
  {
    /// <summary>
    /// Изменение свойства "Дата акта".
    /// </summary>
    /// <param name="e"></param>
    public override void ActDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.ActDateChanged(e);
      FillName();
    }
    
    /// <summary>
    /// Изменение свойства "Клиентский договор".
    /// </summary>
    /// <param name="e"></param>
    public override void ClientContractChanged(lenspec.SalesDepartmentArchive.Shared.SDAActBaseClientContractChangedEventArgs e)
    {
      base.ClientContractChanged(e);
      FillName();
    }
    
    /// <summary>
    /// Изменение свойства "Клиент".
    /// </summary>
    /// <param name="e"></param>
    public override void ClientChanged(lenspec.SalesDepartmentArchive.Shared.SDAActBaseClientChangedEventArgs e)
    {
      base.ClientChanged(e);
      FillName();
    }
    
    /// <summary>
    /// Изменение свойства "Помещение".
    /// </summary>
    /// <param name="e"></param>
    public override void RoomChanged(lenspec.SalesDepartmentArchive.Shared.SDAActBaseRoomChangedEventArgs e)
    {
      base.RoomChanged(e);
      FillName();
    }
    
    /// <summary>
    /// Изменение свойства "Вид документа".
    /// </summary>
    /// <param name="e"></param>
    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      
      var documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.CommissionInspectionReportPublicKind);
      if (_obj.DocumentKind != null && documentKind == _obj.DocumentKind)
      {
        _obj.BusinessUnit = lenspec.Etalon.Employees.As(_obj.Author)?.BusinessUnitlenspec;
      }
      FillName();
    }
  }
}