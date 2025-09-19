using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.AcquaintanceTask;

namespace lenspec.Etalon.Shared
{
  partial class AcquaintanceTaskFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Проверить наличие документа в задаче и наличие прав на него.
    /// </summary>
    /// <returns>True, если с документом можно работать.</returns>
    public override bool HasDocumentAndCanRead()
    {
      return base.HasDocumentAndCanRead();
    }
    
    /// <summary>
    /// Сохранить номер версии и хеш документа в задаче.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="isMainDocument">Признак главного документа.</param>
    public void StoreAcquaintanceVersionAvis(Sungero.Content.IElectronicDocument document, bool isMainDocument)
    {
      this.StoreAcquaintanceVersion(document, isMainDocument);
    }
    //конец Добавлено Avis Expert
  }
}