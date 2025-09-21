using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.ActsOfManagementCompanyBase;

namespace lenspec.SalesDepartmentArchive
{
  partial class ActsOfManagementCompanyBaseClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Functions.ActsOfManagementCompanyBase.ManagementContractMKDIsEnable(_obj);
    }

  }
}