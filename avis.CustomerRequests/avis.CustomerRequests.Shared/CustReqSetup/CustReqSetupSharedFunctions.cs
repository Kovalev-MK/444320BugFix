using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustReqSetup;

namespace avis.CustomerRequests.Shared
{
  partial class CustReqSetupFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Проверить дубли настройки.
    /// </summary>
    /// <returns>True, если дубликаты имеются, иначе - false.</returns>
    public bool HaveDuplicates()
    {
      return Functions.CustReqSetup.Remote.GetDuplicates(_obj).Any();
    }
    //конец Добавлено Avis Expert
  }
}