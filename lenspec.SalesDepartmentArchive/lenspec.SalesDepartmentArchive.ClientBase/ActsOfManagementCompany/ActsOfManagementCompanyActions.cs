using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.ActsOfManagementCompany;

namespace lenspec.SalesDepartmentArchive.Client
{
  partial class ActsOfManagementCompanyActions
  {
    
    //Добавлено Avis Expert
    public override void SaveAndClose(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.HasVersions)
      {
        e.AddError(lenspec.SalesDepartmentArchive.ActsOfManagementCompanies.Resources.NeedToCreateDocumentVersion);
        return;
      }
      base.SaveAndClose(e);
    }

    public override bool CanSaveAndClose(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSaveAndClose(e);
    }

    public override void Save(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.HasVersions)
      {
        e.AddError(lenspec.SalesDepartmentArchive.ActsOfManagementCompanies.Resources.NeedToCreateDocumentVersion);
        return;
      }
      base.Save(e);
    }

    public override bool CanSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSave(e);
    }
    //конец Добавлено Avis Expert

  }

}