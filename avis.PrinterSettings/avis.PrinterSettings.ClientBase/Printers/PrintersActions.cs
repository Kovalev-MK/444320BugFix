using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PrinterSettings.Printers;
using AvisExpert.EtalonPrinterHelper;

namespace avis.PrinterSettings.Client
{
  partial class PrintersActions
  {
    /// <summary>
    /// Кнопка выбора принтера
    /// </summary>
    /// <param name="e"></param>
    public virtual void SelectPrinter(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var printerHelper = new PrinterHelper();
      var printers = printerHelper.GetPrinterList();
      
      var dialog = Dialogs.CreateInputDialog("Выбор принтера");
      
      var printer = dialog.AddSelect("Принтер", true).From(printers.ToArray());
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        _obj.Printer = printer.Value;
      }
    }

    public virtual bool CanSelectPrinter(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
  }
}