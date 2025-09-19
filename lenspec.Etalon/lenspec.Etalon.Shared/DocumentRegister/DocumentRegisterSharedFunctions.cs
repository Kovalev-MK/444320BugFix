using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentRegister;

namespace lenspec.Etalon.Shared
{
  partial class DocumentRegisterFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Проверить дубли журналов регистрации.
    /// </summary>
    /// <returns>True, если дубликаты имеются, иначе - false.</returns>
    public bool HaveDuplicates()
    {
      return
        !string.IsNullOrWhiteSpace(_obj.Name) &&
        _obj.Status != Sungero.Docflow.DocumentRegister.Status.Closed &&
        Functions.DocumentRegister.Remote.GetDuplicates(_obj).Any();
    }
    
    /// <summary>
    /// Проверить регистрационный номер на валидность.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Сообщение об ошибке. Пустая строка, если номер соответствует журналу.</returns>
    [Public]
    public override string CheckRegistrationNumberFormat(Sungero.Docflow.IOfficialDocument document)
    {
      return base.CheckRegistrationNumberFormat(document);
    }
    //конец Добавлено Avis Expert
  }
}