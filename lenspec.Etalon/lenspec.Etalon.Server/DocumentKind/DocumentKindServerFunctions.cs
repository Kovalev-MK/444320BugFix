using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentKind;

namespace lenspec.Etalon.Server
{
  partial class DocumentKindFunctions
  {
    /// <summary>
    /// Получить вид документа по коду константы.
    /// </summary>
    /// <param name="constantCode">Код константы.</param>
    /// <returns>Найденный вид документа.</returns>
    [Public, Remote]
    public static Sungero.Docflow.IDocumentKind GetDocumentKindByConstantCode(string constantCode)
    {
      var constantValue = EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(constantCode);
      
      long documentId = 0;
      if (long.TryParse(constantValue, out documentId))
        return Sungero.Docflow.DocumentKinds.GetAll(x => x.Id == documentId).FirstOrDefault();
      
      return null;
    }
  }
}