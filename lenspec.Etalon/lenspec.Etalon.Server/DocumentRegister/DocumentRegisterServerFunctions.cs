using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentRegister;

namespace lenspec.Etalon.Server
{
  partial class DocumentRegisterFunctions
  {
    
    //Добавлено Avis Expert
    
    /// <summary>
    /// Получить дубли журналов региcтрации.
    /// </summary>
    /// <returns>Журналы регистрации, дублирующие по группе рагистрации, индексу, документопотоку.</returns>
    [Remote(IsPure = true)]
    public IQueryable<Sungero.Docflow.IDocumentRegister> GetDuplicates()
    {
      return Sungero.Docflow.DocumentRegisters.GetAll()
        .Where(x => x.Status != Sungero.Docflow.DocumentRegister.Status.Closed &&
               !Equals(_obj, x) &&
               Equals(x.RegistrationGroup, _obj.RegistrationGroup) && Equals(x.Index, _obj.Index) && Equals(x.DocumentFlow, _obj.DocumentFlow));
    }
    //конец Добавлено Avis Expert
  }
}