using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.ActsOfManagementCompanyBase;

namespace lenspec.SalesDepartmentArchive.Server
{
  partial class ActsOfManagementCompanyBaseFunctions
  {
    
    #region Простановка штампа
    /// <summary>
    /// Проставляем штамп в документ.
    /// </summary>
    /// <returns></returns>
    [Public]
    public virtual string StarmpAvis(double rightIndent, double bottomIndent, string stampHtml)
    {
      var versionId = _obj.LastVersion.Id;
      
      // Проверки возможности преобразования и наложения отметки.
      var lastVersionExtension = _obj.LastVersion.AssociatedApplication.Extension.ToLower();
      if (!Sungero.Docflow.PublicFunctions.OfficialDocument.CheckPdfConvertibilityByExtension(_obj, lastVersionExtension))
        return lenspec.SalesDepartmentArchive.ActsOfManagementCompanyBases.Resources.ErrorMessageCheckPdfConvertibilityByExtension;

      // Способ преобразования: интерактивно.
      this.ConvertToPdfAndAddRegistrationStamp(versionId, stampHtml, rightIndent, bottomIndent);
      
      return string.Empty;
    }
    
    #endregion
  }
}