using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PrinterSettings.Printers;

namespace avis.PrinterSettings
{
  partial class PrintersServerHandlers
  {
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Сообщение о том что после принтер заполняется по кнопке.
      if (_obj.Printer == avis.PrinterSettings.Printerses.Resources.ClickButtonPrinterSetting)
      {
        e.AddError(avis.PrinterSettings.Printerses.Resources.FillNamePrinterErrorMessage);
        return;
      }
      
      // Заполняем название
      _obj.Name = $"{_obj.Printer} - {_obj.Employee.Name}";
      
      // Проверяем что нету настройки с таким же названием.
      if (avis.PrinterSettings.Printerses.GetAll(p => p.Name == _obj.Name && p.Id != _obj.Id).Any())
      {
        e.AddError(avis.PrinterSettings.Printerses.Resources.SettingPrinterErrorMessage);
        return;
      }
    }
  }
}