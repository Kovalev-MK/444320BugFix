using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.LocalRegulations.DocumentApprovedByOrder;

namespace lenspec.LocalRegulations
{
  partial class DocumentApprovedByOrderSharedHandlers
  {
    // Добавлено avis.

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      if (Equals(e.NewValue, e.OldValue))
        return;

      if (e.NewValue != null)
      {
        _obj.BusinessUnit = e.NewValue.BusinessUnit;
        _obj.OurSignatory = e.NewValue.OurSignatory;
        _obj.Assignee = e.NewValue.Assignee;
        
        Sungero.Docflow.PublicFunctions.OfficialDocument.CopyProjects(e.NewValue, _obj);
      }
      else
      {
        _obj.BusinessUnit = null;
        _obj.OurSignatory = null;
        _obj.Assignee = null;
      }
      
      FillName();
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.AddendumRelationName, e.OldValue, e.NewValue);
    }

    // Конец добавлено avis.
  }
}