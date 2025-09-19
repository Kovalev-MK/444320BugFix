using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.LocalRegulations.DocumentApprovedByOrder;

namespace lenspec.LocalRegulations
{
  partial class DocumentApprovedByOrderClientHandlers
  {

    // Добавлено avis.
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Отобразить однократно нотифайку о выдаче прав на проектные документы.
      if (_obj.State.IsInserted && _obj.LeadingDocument != null && _obj.LeadingDocument.Project != null && Sungero.Projects.Projects.Is(_obj.LeadingDocument.Project))
        Sungero.Projects.PublicFunctions.Module.ShowProjectRightsNotifyOnce(e, Sungero.Projects.Projects.Resources.ProjectDocumentRightsNotifyMessage);
      
      if (_obj.AccessRights.CanUpdate())
      {
        _obj.State.Properties.Name.IsEnabled = true;
        _obj.State.Properties.Subject.IsEnabled = true;
      }
    }

    public override void LeadingDocumentValueInput(Sungero.Docflow.Client.OfficialDocumentLeadingDocumentValueInputEventArgs e)
    {
      base.LeadingDocumentValueInput(e);
      
      // Отобразить однократно нотифайку о выдаче прав на проектные документы.
      if (e.NewValue != null && !Equals(e.NewValue, e.OldValue) && e.NewValue.Project != null && Sungero.Projects.Projects.Is(e.NewValue.Project))
        Sungero.Projects.PublicFunctions.Module.ShowProjectRightsNotifyOnce(e, Sungero.Projects.Projects.Resources.ProjectDocumentRightsNotifyMessage);
    }

    // Конец добавлено avis.
  }
}