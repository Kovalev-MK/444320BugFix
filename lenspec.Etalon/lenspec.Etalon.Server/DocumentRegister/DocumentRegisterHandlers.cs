using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentRegister;

namespace lenspec.Etalon
{
  partial class DocumentRegisterServerHandlers
  {

    //Добавлено Avis Expert
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (Functions.DocumentRegister.HaveDuplicates(_obj))
      {
        e.AddError(Sungero.Commons.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicatesavis);
        return;
      }
      
      base.BeforeSave(e);
    }
    //конец Добавлено Avis Expert
  }

}