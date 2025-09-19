using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OfficialDocument;

namespace lenspec.Etalon
{
  partial class OfficialDocumentClientHandlers
  {

    //Добавлено Avis Expert
    public virtual void ArchiveavisValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue.Value == true)
      {
        foreach(var property in _obj.State.Properties)
        {
          property.IsRequired = false;
        }
      }
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Блокировать поля в Жизненном цикле.
      Functions.OfficialDocument.BlockLifecycle(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      // Блокировать поля в Жизненном цикле.
      Functions.OfficialDocument.BlockLifecycle(_obj);
    }
    //конец Добавлено Avis Expert
  }
}