using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustomerRequest;

namespace avis.CustomerRequests.Client
{
  partial class CustomerRequestFunctions
  {
    /// <summary>
    /// Проверить возможность преобразования в PDF.
    /// </summary>
    /// <returns>Результат проверки.</returns>
    public virtual Sungero.Docflow.Structures.OfficialDocument.IConversionToPdfResult ValidatePdfConvertibilityByExtension()
    {
      var lastVersionExtension = _obj.LastVersion.AssociatedApplication.Extension.ToLower();
      if (!Sungero.Docflow.IsolatedFunctions.PdfConverter.CheckIfExtensionIsSupported(lastVersionExtension))
        return Functions.CustomerRequest.GetExtensionValidationError(_obj, lastVersionExtension);
      
      var result = Sungero.Docflow.Structures.OfficialDocument.ConversionToPdfResult.Create();
      result.HasErrors = false;
      return result;
    }
    
    /// <summary>
    /// Показать диалог для выбора расположения отметки о поступлении.
    /// </summary>
    /// <returns>Отступы для простановки отметки.</returns>
    public virtual Sungero.Docflow.Structures.IncomingDocumentBase.RegistrationStampPosition ShowAddRegistrationStampDialog()
    {
      string positionValue = Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogBottomRightPosition;
      double rightIndentValue = Sungero.Docflow.PublicConstants.Module.RegistrationStampDefaultRightIndent;
      double bottomIndentValue = Sungero.Docflow.PublicConstants.Module.RegistrationStampDefaultBottomIndent;
      var personalSettings = Sungero.Docflow.PublicFunctions.PersonalSetting.GetPersonalSettings(null);
      if (personalSettings != null)
      {
        positionValue = personalSettings.RegistrationStampPosition.HasValue ?
          Sungero.Docflow.PersonalSettings.Info.Properties.RegistrationStampPosition.GetLocalizedValue(personalSettings.RegistrationStampPosition.Value) :
          Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogBottomRightPosition;
        rightIndentValue = personalSettings.RightIndent ?? Sungero.Docflow.PublicConstants.Module.RegistrationStampDefaultRightIndent;
        bottomIndentValue = personalSettings.BottomIndent ?? Sungero.Docflow.PublicConstants.Module.RegistrationStampDefaultBottomIndent;
      }
      
      var dialog = Dialogs.CreateInputDialog(Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogTitle);
      dialog.HelpCode = Sungero.Docflow.Constants.IncomingDocumentBase.AddRegistrationStampHelpCode;
      var position = dialog.AddSelect(Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogPosition,
                                      true,
                                      positionValue)
        .From(Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogBottomRightPosition,
              Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogBottomCenterPosition,
              Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogCustomPosition);
      var rightIndent = dialog.AddDouble(Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogRightIndent, false, rightIndentValue);
      rightIndent.IsVisible = false;
      var bottomIndent = dialog.AddDouble(Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogBottomIndent, false, bottomIndentValue);
      bottomIndent.IsVisible = false;
      
      var addButton = dialog.Buttons.AddCustom(Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogCreateButton);
      dialog.Buttons.AddCancel();
      
      dialog.SetOnRefresh(
        args =>
        {
          if (position.Value == Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogCustomPosition)
          {
            rightIndent.IsVisible = true;
            bottomIndent.IsVisible = true;
            rightIndent.IsRequired = true;
            bottomIndent.IsRequired = true;
            if (rightIndent.Value.HasValue && rightIndent.Value < 0 ||
                (bottomIndent.Value.HasValue && bottomIndent.Value < 0))
            {
              args.AddError(Sungero.Docflow.Resources.MarkCoordinatesMustBePositive);
            }
          }
          else
          {
            rightIndent.IsVisible = false;
            bottomIndent.IsVisible = false;
            rightIndent.IsRequired = false;
            bottomIndent.IsRequired = false;
          }
        });
      
      dialog.SetOnButtonClick(
        args =>
        {
          if (!Equals(args.Button, addButton))
            return;
          
          if (position.Value == Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogBottomRightPosition)
          {
            rightIndent.Value = Sungero.Docflow.PublicConstants.Module.RegistrationStampDefaultRightIndent;
            bottomIndent.Value = Sungero.Docflow.PublicConstants.Module.RegistrationStampDefaultBottomIndent;
          }
          
          if (position.Value == Sungero.Docflow.IncomingDocumentBases.Resources.AddRegistrationStampDialogBottomCenterPosition)
          {
            rightIndent.Value = Sungero.Docflow.PublicConstants.Module.RegistrationStampDefaultPageCenterIndent;
            bottomIndent.Value = Sungero.Docflow.PublicConstants.Module.RegistrationStampDefaultBottomIndent;
          }
          
          if (rightIndent.Value.HasValue && rightIndent.Value < 0 ||
              (bottomIndent.Value.HasValue && bottomIndent.Value < 0))
          {
            args.AddError(Sungero.Docflow.Resources.MarkCoordinatesMustBePositive);
          }
        });
      
      if (dialog.Show() == addButton)
      {
        return Sungero.Docflow.Structures.IncomingDocumentBase.RegistrationStampPosition.Create(rightIndent.Value.Value, bottomIndent.Value.Value);
      }
      
      return null;
    }
  }
}