using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.OutgoingLetters.MassMailingApplication;

namespace lenspec.OutgoingLetters.Shared
{
  partial class MassMailingApplicationFunctions
  {
    
    //Добавлено Avis Expert
    
    #region Мас. рассылки: исх. письма - приложения Заявки
    /*
    /// <summary>
    /// Получить документы, связанные типом связи "Приложение".
    /// </summary>
    /// <returns>Документы, связанные типом связи "Приложение".</returns>
    [Public]
    public override List<Sungero.Docflow.IOfficialDocument> GetAddenda()
    {
      var addenda = new List<Sungero.Docflow.IOfficialDocument>();
      return addenda;
    }
    */
    #endregion
    
    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      _obj.State.Properties.Subject.IsRequired = false;
    }
    
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
        <Вид документа> по Объекту <Наименование объекта>, <Тип рассылки>.
       */
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.ObjectAnProject != null && !string.IsNullOrEmpty(_obj.ObjectAnProject.Name))
        {
          name += " по Объекту " + _obj.ObjectAnProject.Name;
        }
        
        if (_obj.MailingType != null)
          name += ", " + _obj.MailingType.Name;
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
      
      _obj.Name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
    }
    //конец Добавлено Avis Expert
  }
}