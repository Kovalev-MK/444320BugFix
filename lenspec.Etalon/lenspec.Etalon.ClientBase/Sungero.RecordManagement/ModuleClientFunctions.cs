using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.RecordManagement.Client
{
  partial class ModuleFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Получить отчет "Бланк ознакомления".
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Отчет.</returns>
    [Public]
    public override Sungero.Reporting.IReport GetAcquaintanceFormReport(Sungero.RecordManagement.IAcquaintanceTask task)
    {
      var report = lenspec.LocalRegulations.Reports.GetAcquaintanceFormReport();
      report.Task = task;
      return report;
    }
    
    /// <summary>
    /// Получить отчет "Контроль ознакомления".
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Отчет.</returns>
    [Public]
    public override Sungero.Reporting.IReport GetAcquaintanceReport(Sungero.RecordManagement.IAcquaintanceTask task)
    {
      var report = lenspec.LocalRegulations.Reports.GetAcquaintanceReport();
      report.Task = task;
      return report;
    }
    
    /// <summary>
    /// Получить отчет "Контроль ознакомления".
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Отчет.</returns>
    [Public]
    public override Sungero.Reporting.IReport GetAcquaintanceReport(Sungero.Docflow.IOfficialDocument document)
    {
      var report = lenspec.LocalRegulations.Reports.GetAcquaintanceReport();
      report.Document = document;
      return report;
    }
    //конец Добавлено Avis Expert
  }
}