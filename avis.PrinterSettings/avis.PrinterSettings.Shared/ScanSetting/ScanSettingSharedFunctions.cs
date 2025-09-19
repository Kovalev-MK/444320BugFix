using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PrinterSettings.ScanSetting;

namespace avis.PrinterSettings.Shared
{
  partial class ScanSettingFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Заполнить имя записи.
    /// </summary>
    [Public]
    public void FillName()
    {
      _obj.Name = string.Empty;
      
      //Имя в формате: <Имя пользователя> - <Наименование сканера>.
      if (_obj.User != null)
      {
        _obj.Name += _obj.User.Name;
        if (!string.IsNullOrEmpty(_obj.Scanner))
          _obj.Name += " - " + _obj.Scanner;
      }
      else
      {
        _obj.Name += _obj.Scanner;
      }
    }
    //конец Добавлено Avis Expert
  }
}