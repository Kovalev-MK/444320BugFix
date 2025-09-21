using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ManagementCompanyJKHArhive.DocumentForManagementCompany;

namespace avis.ManagementCompanyJKHArhive
{
  partial class DocumentForManagementCompanyServerHandlers
  { 
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
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