using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.RegistrationGroup;

namespace lenspec.Etalon.Server
{
  partial class RegistrationGroupFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Проверить журналы группы регистрации на возможность их регистрации.
    /// </summary>
    /// <returns>Текст хинта, с указанием необходимых документопотоков.</returns>
    public string ValidateDocumentFlow()
    {
      return base.ValidateDocumentFlow();
    }
    //конец Добавлено Avis Expert
  }
}