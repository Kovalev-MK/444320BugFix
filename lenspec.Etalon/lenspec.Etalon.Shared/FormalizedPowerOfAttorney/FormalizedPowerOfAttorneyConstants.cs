using System;
using Sungero.Core;

namespace lenspec.Etalon.Constants.Docflow
{
  public static class FormalizedPowerOfAttorney
  {

    /// <summary>
    /// Наименовании группы контрагента - 'ИП' для фильтрации в эл. доверенности
    /// </summary>
    [Public]
    public const string IECompanyGroupName = "ИП";
    
    /// <summary>
    /// Значение поля-подсказки
    /// </summary>
    public const string FlagOurSignatoryValue = "В поле Подписант укажите первое лицо нашей организации";
    
    /// <summary>
    /// Максимальная длина поля Полномочия.
    /// </summary>
    public const int PowersLength = 10000;
    
    /// <summary>
    /// Имя параметра "Длина строки Полномочий была изменена".
    /// </summary>
    public const string PowersLengthChangedParamName = "Powers length changed";
  }
}