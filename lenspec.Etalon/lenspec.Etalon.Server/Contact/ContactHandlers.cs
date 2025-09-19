using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Contact;

namespace lenspec.Etalon
{
  partial class ContactServerHandlers
  {

    //Добавлено Avis Expert
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (Functions.Contact.HaveContactDuplicates(_obj))
      {
        e.AddError(Sungero.Commons.Resources.DuplicateDetected, _obj.Info.Actions.ShowDuplicates);
        return;
      }
      
      base.BeforeSave(e); 
    }
    //конец Добавлено Avis Expert
  }

}