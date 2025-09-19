using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustomerRequest;

namespace avis.CustomerRequests.Server
{
  partial class CustomerRequestFunctions
  {
    
    /// <summary>
    /// Преобразовать документ в PDF с наложением отметки о поступлении в новую версию.
    /// </summary>
    /// <param name="rightIndent">Значение отступа справа.</param>
    /// <param name="bottomIndent">Значение отступа снизу.</param>
    /// <returns>Результат преобразования.</returns>
    [Remote]
    public virtual Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult AddRegistrationStamp(double rightIndent, double bottomIndent)
    {
      var versionId = _obj.LastVersion.Id;
      var result = Sungero.Docflow.Structures.OfficialDocument.ConversionToPdfResult.Create();
      result.HasErrors = true;
      
      // Проверки возможности преобразования и наложения отметки.
      var lastVersionExtension = _obj.LastVersion.AssociatedApplication.Extension.ToLower();
      if (!Sungero.Docflow.PublicFunctions.OfficialDocument.CheckPdfConvertibilityByExtension(_obj, lastVersionExtension))
        return Functions.CustomerRequest.GetExtensionValidationError(_obj, lastVersionExtension);
      
      // Выбор способа преобразования.
      var isInteractive = this.CanConvertToPdfInteractively();
      if (isInteractive)
      {
        // Способ преобразования: интерактивно.
        var registrationStamp = this.GetRegistrationStampAsHtml(versionId);
        result = this.ConvertToPdfAndAddRegistrationStamp(versionId, registrationStamp, rightIndent, bottomIndent);
        result.IsFastConvertion = true;
        result.ErrorTitle = Sungero.Docflow.Resources.AddRegistrationStampErrorTitle;
      }
      else
      {
        var asyncAddRegistrationStamp = Sungero.Docflow.AsyncHandlers.AddRegistrationStamp.Create();
        asyncAddRegistrationStamp.DocumentId = _obj.Id;
        asyncAddRegistrationStamp.VersionId = versionId;
        asyncAddRegistrationStamp.RightIndent = rightIndent;
        asyncAddRegistrationStamp.BottomIndent = bottomIndent;
        
        var startedNotificationText = Sungero.Docflow.OfficialDocuments.Resources.ConvertionInProgress;
        var completedNotificationText = Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampCompleteNotificationFormat(Hyperlinks.Get(_obj));
        var errorNotificationText = Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampErrorNotificationFormat(Hyperlinks.Get(_obj), Environment.NewLine);
        asyncAddRegistrationStamp.ExecuteAsync(startedNotificationText, completedNotificationText, errorNotificationText, Users.Current);
        result.IsOnConvertion = true;
        result.HasErrors = false;
      }
      
      Logger.DebugFormat("Registration stamp. Added {5}. Document id - {0}, kind - {6}, format - {1}, application - {2}, right indent - {3}, bottom indent - {4}.",
                         _obj.Id, _obj.AssociatedApplication.Extension, _obj.AssociatedApplication, rightIndent, bottomIndent,
                         isInteractive ? "interactively" : "async", _obj.DocumentKind.DisplayValue);
      
      return result;
    }
  }
}