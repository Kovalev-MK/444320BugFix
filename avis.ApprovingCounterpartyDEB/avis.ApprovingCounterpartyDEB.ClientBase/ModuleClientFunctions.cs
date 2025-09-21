using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.ApprovingCounterpartyDEB.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Экспресс-отчет из КонтурФокус.
    /// </summary>
    /// <param name="inn">ИНН контрагента.</param>
    [Public]
    public static void GetExpressReportFromKF(string inn)
    {
      try
      {
        var url = lenspec.Etalon.Module.Parties.PublicFunctions.Module.Remote.GetCompanyReportFromKontur(inn);
        Hyperlinks.Open(url);
      }
      catch(Exception ex)
      {
        Dialogs.ShowMessage(ex.Message, MessageType.Error);
        Logger.Error("Запрос ссылки на экспресс-отчет из КФ. ", ex);
      }
    }
  }
}