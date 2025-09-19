using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ConstantDatabook;

namespace lenspec.EtalonDatabooks
{
  // Добавлено avis.
  partial class ConstantDatabookSharedHandlers
  {
    /// <summary>
    /// Изменение значения свойства "Значение".
    /// </summary>
    /// <param name="e"></param>
    public virtual void ValueChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (_obj.IsNumeration == true)
      {
        // Проверяем что в случае нумерации, в значении находится именно число.
        int newValue;
        var success = int.TryParse(_obj.Value, out newValue);
        if (!success)
        {
          _obj.Value = "0";
          // TODO: Добавить тут вывод ошибки, что значение может быть только число - перенести в Изменение значение контрола.
        }
      }
    }

  }
  // Конец добавлено avis.
}