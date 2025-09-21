using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ManagementCompanyJKHArhive.BasisForManagementContractMKD;

namespace avis.ManagementCompanyJKHArhive
{
  partial class BasisForManagementContractMKDServerHandlers
  {
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Проверка на наличие хотя бы одной версии документа.
      if (_obj.Versions.FirstOrDefault() == null)
      {
        e.AddError(avis.ManagementCompanyJKHArhive.BasisForManagementContractMKDs.Resources.CreateFileErrorMessage);
        return;
      }
      
      base.BeforeSave(e);
    }
  }
}