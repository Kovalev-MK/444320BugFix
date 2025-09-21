using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Integrations;

namespace lenspec.Etalon.Client
{
  partial class IntegrationsActions
  {
    //Добавлено Avis Expert
    public override void DownloadLogs(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.DownloadLogs(e);
    }

    public override bool CanDownloadLogs(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.Code != Etalon.Module.Integration.Constants.Module.KonturFocusEGRULRecordCode && base.CanDownloadLogs(e);
    }
    //конец Добавлено Avis Expert

  }

}