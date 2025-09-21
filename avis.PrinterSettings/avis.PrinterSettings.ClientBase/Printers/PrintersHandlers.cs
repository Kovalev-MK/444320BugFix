using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PrinterSettings.Printers;

namespace avis.PrinterSettings
{
  partial class PrintersClientHandlers
  {
    // Добавлено avis Exprert
    
    /// <summary>
    /// Показ формы
    /// </summary>
    /// <param name="e"></param>
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      // Сообщение о том что поле название будет заполнено позже
      if (_obj.Name == null)
      {
        _obj.Name = avis.PrinterSettings.Printerses.Resources.PrinterSettingName;
      }
      
      // Сообщение о том что после принтер заполняется по кнопке.
      if (_obj.Printer == null)
      {
        _obj.Printer = avis.PrinterSettings.Printerses.Resources.ClickButtonPrinterSetting;
      }
    }
    
    // Конец добавлено avis Expert
  }
}