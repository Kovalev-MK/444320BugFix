using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectDocumentBase;

namespace lenspec.EtalonDatabooks
{
  // Добавлено avis.
  partial class ProjectDocumentBaseClientHandlers
  {
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      _obj.State.Properties.Name.IsEnabled = false;
      
      if (_obj.Name == null)
      {
        _obj.Name = "<Наименование будет заполнено автоматически при сохранении.>";
        _obj.Subject = "<Наименование будет заполнено автоматически при сохранении.>";
      }
      
      base.Showing(e);
    }
  }
  // Конец добавлено avis.
}