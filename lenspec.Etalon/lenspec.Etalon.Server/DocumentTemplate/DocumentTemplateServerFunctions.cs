using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentTemplate;

namespace lenspec.Etalon.Server
{
  partial class DocumentTemplateFunctions
  {

    /// <summary>
    /// Получить дубли записей.
    /// </summary>
    /// <param name="name">Наименование.</param>
    /// <param name="excludedId">ИД для исключения.</param>
    /// <returns>Список дублей.</returns>
    [Remote(IsPure = true)]
    public static IQueryable<IDocumentTemplate> GetDuplicateDocumentTemplates(string name, long? excludedId)
    {
      var duplicates = DocumentTemplates.GetAll(x => x.Name.Trim() == name.Trim());
      if (excludedId.HasValue)
        duplicates = duplicates.Where(x => x.Id != excludedId.Value);
      
      return duplicates;
    }
    
    /// <summary>
    /// Проверка необходимости показа сообщения об ошибке.
    /// </summary>
    /// <returns>true, если необходимо вывести сообщение об ошибке; иначе – false.</returns>
    [Public, Remote(IsPure = true)]
    public static bool IsDocumentTypeRestrictionError(string documentTypeGuid, bool isDocumentTypeRestrictionEnabled)
    {
      if (!isDocumentTypeRestrictionEnabled)
        return false;
      
      // Доступные типы документов при создании шаблона.
      var allowedTypeGuids = new string[]
      {
        // Договор.
        Sungero.Contracts.PublicConstants.Module.ContractGuid,
        // Доп. соглашение.
        Sungero.Contracts.PublicConstants.Module.SupAgreementGuid
      };
      
      var isAllowedType = allowedTypeGuids.Contains(documentTypeGuid);
      return !isAllowedType;      
    }
  }
}