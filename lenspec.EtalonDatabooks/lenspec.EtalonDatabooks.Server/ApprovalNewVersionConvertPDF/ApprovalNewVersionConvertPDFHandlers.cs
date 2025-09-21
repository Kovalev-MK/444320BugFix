using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApprovalNewVersionConvertPDF;

namespace lenspec.EtalonDatabooks
{
  partial class ApprovalNewVersionConvertPDFServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.RewriteOriginal = false;
    }
  }

}