using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Counterparty;

namespace lenspec.Etalon.Shared
{
  partial class CounterpartyFunctions
  {
    /// <summary>
    /// Проверка введенного ОГРН по количеству символов.
    /// </summary>
    /// <param name="psrn">ОГРН.</param>
    /// <returns>Пустая строка, если длина ОГРН в порядке.
    /// Иначе текст ошибки.</returns>
    public override string CheckPsrnLength(string psrn)
    {
      return base.CheckPsrnLength(psrn);
    }
    
    /// <summary>
    /// Проверка введенного ОКПО по количеству символов.
    /// </summary>
    /// <param name="nceo">ОКПО.</param>
    /// <returns>Пустая строка, если длина ОКПО в порядке.
    /// Иначе текст ошибки.</returns>
    [Public]
    public override string CheckNceoLength(string nceo)
    {
      return base.CheckNceoLength(nceo);
    }
    
    /// <summary>
    /// Получить текст ошибки о наличии дублей контрагента.
    /// </summary>
    /// <returns>Текст ошибки.</returns>
    [Public]
    public override string GetCounterpartyDuplicatesErrorText()
    {
      return base.GetCounterpartyDuplicatesErrorText();
    }
  }
}