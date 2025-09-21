using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PrinterSettings.ScanSetting;

namespace avis.PrinterSettings.Server
{
  partial class ScanSettingFunctions
  {
    //Добавлено Avis Expert
    
    /// <summary>
    /// Показать доступные сканеры.
    /// </summary>
    public void ShowAvailableScanners()
    {
      
    }
    
    /// <summary>
    /// Получить дубли настроек сканирования документов.
    /// </summary>
    /// <returns>Настройка сканирования документов, дублирующая текущее.</returns>
    [Remote(IsPure = true)]
    public IQueryable<PrinterSettings.IScanSetting> GetDublicates()
    {
      var dublicate = ScanSettings.GetAll().Where(x => x.User.Equals(_obj.User) && x.Scanner.Equals(_obj.Scanner));      
      return dublicate;
    }
    //конец Добавлено Avis Expert
  }
}