using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OfficialDocument;

namespace lenspec.Etalon.Shared
{
  partial class OfficialDocumentFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    ///  Получить ИД отфильтрованных журналов регистрации по документу без учета текущего пользователя.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="settingType">Тип регистрации.</param>
    /// <returns>Журналы.</returns>
    [Public]
    public static List<long> GetDocumentRegistersIdsByDocumentAvis(Sungero.Docflow.IOfficialDocument document, Enumeration? settingType)
    {
      var emptyList = new List<long>();
      var documentKind = document.DocumentKind;
      if (documentKind == null)
        return emptyList;
      
      var setting = Sungero.Docflow.PublicFunctions.Module.Remote.GetRegistrationSettings(settingType, document.BusinessUnit, documentKind, document.Department).FirstOrDefault();
      return setting != null ? new List<long> { setting.DocumentRegister.Id } : emptyList;
    }
    
    /// <summary>
    /// Получить ИД ведущего документа.
    /// </summary>
    /// <returns>ИД документа либо 0.</returns>
    [Public]
    public override long GetLeadDocumentId()
    {
      return base.GetLeadDocumentId();
    }
    
    /// <summary>
    /// Получить номер ведущего документа.
    /// </summary>
    /// <returns>Номер документа либо пустая строка.</returns>
    [Public]
    public override string GetLeadDocumentNumber()
    {
      return base.GetLeadDocumentNumber();
    }
    //конец Добавлено Avis Expert
  }
}