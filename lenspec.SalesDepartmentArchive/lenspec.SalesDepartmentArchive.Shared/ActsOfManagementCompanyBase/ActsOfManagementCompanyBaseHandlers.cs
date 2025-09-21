using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.ActsOfManagementCompanyBase;

namespace lenspec.SalesDepartmentArchive
{
  partial class ActsOfManagementCompanyBaseSharedHandlers
  {

    public virtual void ActDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      FillName();
    }

    public virtual void OwnerChanged(lenspec.SalesDepartmentArchive.Shared.ActsOfManagementCompanyBaseOwnerChangedEventArgs e)
    {
      PublicFunctions.ActsOfManagementCompanyBase.ManagementContractMKDIsEnable(_obj);
      FillName();
    }

    public virtual void RoomChanged(lenspec.SalesDepartmentArchive.Shared.ActsOfManagementCompanyBaseRoomChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.Address = e.NewValue?.Address;
    }

    public virtual void ManagementContractMKDChanged(lenspec.SalesDepartmentArchive.Shared.ActsOfManagementCompanyBaseManagementContractMKDChangedEventArgs e)
    {
      FillName();
    }

  }
}