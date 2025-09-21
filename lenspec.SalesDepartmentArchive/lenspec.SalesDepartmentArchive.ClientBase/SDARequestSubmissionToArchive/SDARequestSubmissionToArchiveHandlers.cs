using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDARequestSubmissionToArchive;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDARequestSubmissionToArchiveClientHandlers
  {

    //Добавлено Avis Expert
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      var clientDocuments = _obj.ListOfDocument.Where(x => string.IsNullOrEmpty(x.DocumentId));
      if (_obj.ListOfDocument.Any() && !clientDocuments.Any())
      {
        foreach (var property in _obj.State.Properties)
        {
          property.IsEnabled = false;
        }
      }
    }
    //конец Добавлено Avis Expert

  }
}