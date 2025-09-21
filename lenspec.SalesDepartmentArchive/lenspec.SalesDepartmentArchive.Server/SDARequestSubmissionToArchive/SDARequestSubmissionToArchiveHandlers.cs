using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDARequestSubmissionToArchive;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDARequestSubmissionToArchiveServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.UrgentRequest = SDARequestSubmissionToArchive.UrgentRequest.No;
    }
    //конец Добавлено Avis Expert
  }

}