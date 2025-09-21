using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Contact;

namespace lenspec.Etalon.Shared
{
  partial class ContactFunctions
  {

    //Добавлено Avis Expert
    
    /// <summary>
    /// Проверить дубли контактов.
    /// </summary>
    /// <returns>True, если дубликаты имеются, иначе - false.</returns>      
    public bool HaveContactDuplicates()
    {
      return this.HaveDuplicates();
    }
    
    //конец Добавлено Avis Expert

  }
}