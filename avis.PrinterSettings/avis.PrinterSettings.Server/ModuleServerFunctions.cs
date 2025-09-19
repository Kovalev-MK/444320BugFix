using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PrinterSettings.Server
{
  public class ModuleFunctions
  {
    // Добавлено avis Exprert
    /// <summary>
    /// Получить список доступных записей справочника Настройки сканирования документов.
    /// </summary>
    /// <param name="recipientTypeGuid">GUID типа сущности.</param>
    /// <returns>Список Ид настроек сканирования.</returns>
    [Public, Remote]
    public virtual List<long> GetVisibleScanSettingIds()
    {
      var scanSettingIds = new List<long>();
      
      var currentUser = Users.Current;
      if (currentUser == null)
        return scanSettingIds;
      
      var scanSettings = PrinterSettings.ScanSettings.GetAll();
      if (currentUser.IncludedIn(Roles.Administrators))
        scanSettingIds = scanSettings.Select(x => x.Id).ToList();
      else
        scanSettingIds = scanSettings.Where(x => x.User.Equals(currentUser)).Select(x => x.Id).ToList();
      
      return scanSettingIds;
    }
    
    /// <summary>
    /// Получить список активных принтеров для пользователя.
    /// </summary>
    /// <returns>Список принтеров найденных в системе сервера.</returns>
    [Public, Remote]
    public List<avis.PrinterSettings.IPrinters> GetActivePrinters()
    {
      var result = Printerses.GetAll(p=> p.Employee == Sungero.Company.Employees.Current && p.Status == avis.PrinterSettings.Printers.Status.Active);
      
      return result.ToList();
    }
    
    /// <summary>
    /// Сохранить файл в папку для прямой печати на сервере.
    /// </summary>
    /// <param name="printerName">Название принтера в системе.</param>
    /// <param name="barcodeReport">Файл с отчёток который отправить на печать.</param>
    [Public]
    public void SaveFile(string fileName, avis.PrinterSettings.IBarcodePageReportAvis barcodeReport)
    {
      var printerHelper = new AvisExpert.EtalonPrinterHelper.PrinterHelper();
      var path = lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.BarcodePathCode).FirstOrDefault();
      
      if (path == null)
        return;
      
      printerHelper.SavePdf(path.Value, barcodeReport.Export(), fileName);
    }
    // Конец добавлено avis Expert
  }
}