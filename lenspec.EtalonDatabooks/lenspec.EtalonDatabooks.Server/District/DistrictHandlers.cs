using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.District;

namespace lenspec.EtalonDatabooks
{

  partial class DistrictServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (Functions.District.GetDuplicates(_obj).Any())
        e.AddError(Districts.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
    }
  }

}