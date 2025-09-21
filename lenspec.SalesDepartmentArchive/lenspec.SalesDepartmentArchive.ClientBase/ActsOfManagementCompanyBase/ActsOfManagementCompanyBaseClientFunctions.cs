using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.ActsOfManagementCompanyBase;

namespace lenspec.SalesDepartmentArchive.Client
{
  partial class ActsOfManagementCompanyBaseFunctions
  {
    [Public]
    public void ManagementContractMKDIsEnable()
    {
      var managementContractMKD = avis.ManagementCompanyJKHArhive.ManagementContractMKDs.Null;
      var hasOwner = _obj.Owner != null;
      
      _obj.State.Properties.ManagementContractMKD.IsEnabled = hasOwner;
      
      if (hasOwner)
      {
        var mkds = avis.ManagementCompanyJKHArhive.ManagementContractMKDs.GetAll(m => m.Client == _obj.Owner);
        if (mkds.Count() > 1)
          return;
        
        managementContractMKD = mkds.FirstOrDefault();
      }
      
      if (!Equals(managementContractMKD, _obj.ManagementContractMKD))
        _obj.ManagementContractMKD = managementContractMKD;
    }
    
    /// <summary>
    /// Показать диалог для выбора расположения отметки о поступлении.
    /// </summary>
    /// <returns>Отступы для простановки отметки.</returns>
    public virtual Structures.ActsOfManagementCompanyBase.RegistrationStampPosition ShowAddRegistrationStampDialog()
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
      dialog.HelpCode = Constants.ActsOfManagementCompanyBase.AddRegistrationStampHelpCode;
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
        return Structures.ActsOfManagementCompanyBase.RegistrationStampPosition.Create(rightIndent.Value.Value, bottomIndent.Value.Value);
      }
      
      return null;
    }
  }
}