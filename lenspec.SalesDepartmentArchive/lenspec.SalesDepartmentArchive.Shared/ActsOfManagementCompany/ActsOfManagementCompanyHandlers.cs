using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.ActsOfManagementCompany;

namespace lenspec.SalesDepartmentArchive
{
  partial class ActsOfManagementCompanySharedHandlers
  {

    //Добавлено Avis Expert
    public override void RoomChanged(lenspec.SalesDepartmentArchive.Shared.ActsOfManagementCompanyBaseRoomChangedEventArgs e)
    {
      base.RoomChanged(e);
      
      if (e.NewValue != null && e.NewValue.OurCF != null)
      {
        _obj.OurCF = e.NewValue.OurCF;
      }
    }

    public override void ManagementContractMKDChanged(lenspec.SalesDepartmentArchive.Shared.ActsOfManagementCompanyBaseManagementContractMKDChangedEventArgs e)
    {
      base.ManagementContractMKDChanged(e);
      
      if (e.NewValue != null)
      {
        if (e.NewValue.OurCF != null)
          _obj.OurCF = e.NewValue.OurCF;
        
        if (e.NewValue.ObjectAnSale != null)
          _obj.Room = e.NewValue.ObjectAnSale;
        
        if (!string.IsNullOrEmpty(e.NewValue.Address))
          _obj.Address = e.NewValue.Address;
      }
    }
    //конец Добавлено Avis Expert

  }
}