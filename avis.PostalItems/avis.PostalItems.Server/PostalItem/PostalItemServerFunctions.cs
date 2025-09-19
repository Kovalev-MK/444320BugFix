using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PostalItems.PostalItem;

namespace avis.PostalItems.Server
{
  // Добавлено avis.
  
  partial class PostalItemFunctions
  {

    /// <summary>
    /// Активация поля "Кому" в зависимости от "Адресат".
    /// </summary>      
    [Public]
    public void EnableTo()
    {
      // Активируем поле если в "Адресат" банк или организация.
      if (Sungero.Parties.Companies.Is(_obj.Counterparty) == true || Sungero.Parties.Banks.Is(_obj.Counterparty) == true)
      {
        _obj.State.Properties.To.IsEnabled = true;
        return;
      }
      
      _obj.State.Properties.To.IsEnabled = false;
    }
  }
  
  // Конец добавлено avis.
}