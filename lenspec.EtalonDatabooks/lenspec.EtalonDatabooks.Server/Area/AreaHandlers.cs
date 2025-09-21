using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.Area;

namespace lenspec.EtalonDatabooks
{
  partial class AreaServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (Functions.Area.GetDuplicates(_obj).Any())
        e.AddError(Sungero.Commons.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
    }
  }

}