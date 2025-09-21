using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.EDSDocument;

namespace lenspec.ElectronicDigitalSignatures.Shared
{
  partial class EDSDocumentFunctions
  {
    
    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var documentKind = _obj.DocumentKind;
      
      if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      
      if (documentKind == null || !documentKind.GenerateDocumentName.Value)
        return;
      
      var name = string.Empty;
      
      /* Имя в формате:
        <Вид документа>. Персона: <Персона> От <Дата создания>
       */
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.Person != null)
        {
          name += lenspec.ElectronicDigitalSignatures.EDSDocuments.Resources.ForPersonFormat(_obj.Person.Name);
        }
        
        if (_obj.Created != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.Created.Value.ToString("d");
      }
      
      if (string.IsNullOrWhiteSpace(name))
      {
        if (_obj.VerificationState == null)
          name = Sungero.Docflow.Resources.DocumentNameAutotext;
        else
          name = _obj.DocumentKind.ShortName;
      }
      else if (documentKind != null)
      {
        name = documentKind.ShortName + name;
      }
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      if (name.Length > _obj.Info.Properties.Name.Length)
        _obj.Name = name.Substring(0, _obj.Info.Properties.Name.Length);
      else
        _obj.Name = name;
    }

    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      _obj.State.Properties.Subject.IsRequired = false;
    }
    
    /// <summary>
    /// Изменить отображение панели регистрации.
    /// </summary>
    /// <param name="needShow">Признак отображения.</param>
    /// <param name="repeatRegister">Признак повторной регистрации\изменения реквизитов.</param>
    public override void ChangeRegistrationPaneVisibility(bool needShow, bool repeatRegister)
    {
      base.ChangeRegistrationPaneVisibility(needShow, repeatRegister);
      
      _obj.State.Properties.Author.IsVisible = true;
      _obj.State.Properties.Created.IsVisible = true;
    }
    
    /// <summary>
    /// Сменить доступность реквизитов документа.
    /// </summary>
    /// <param name="isEnabled">True, если свойства должны быть доступны.</param>
    /// <param name="repeatRegister">Перерегистрация.</param>
    public override void ChangeDocumentPropertiesAccess(bool isEnabled, bool repeatRegister)
    {
      base.ChangeDocumentPropertiesAccess(isEnabled, repeatRegister);
      
      _obj.State.Properties.DocumentKind.IsEnabled = false;
    }
  }
}