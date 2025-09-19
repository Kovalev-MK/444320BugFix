using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ExecutionPowerOfAttorney;

namespace avis.PowerOfAttorneyModule
{
  partial class ExecutionPowerOfAttorneySharedHandlers
  {

    public override void SubjectChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != null && e.NewValue.Length > ExecutionPowerOfAttorneys.Info.Properties.Subject.Length)
        _obj.Subject = e.NewValue.Substring(0, ExecutionPowerOfAttorneys.Info.Properties.Subject.Length);
    }

    public virtual void ProjectPOADeleted(Sungero.Workflow.Interfaces.AttachmentDeletedEventArgs e)
    {
      _obj.AttorneyDocuments.SimpleDocuments.Clear();
      _obj.Addendums.All.Clear();
      _obj.Subject = string.Empty;
      _obj.State.Controls.Control.Refresh();
    }

    public override void AuthorChanged(Sungero.Workflow.Shared.TaskAuthorChangedEventArgs e)
    {
      Functions.ExecutionPowerOfAttorney.SetManagerInitiator(_obj, e);
    }

    public virtual void ProjectPOAAdded(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      _obj.Subject = string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.SubjectTaskPoa, lenspec.Etalon.PowerOfAttorneys.As(e.Attachment).Name);
      _obj.State.Controls.Control.Refresh();
      Functions.ExecutionPowerOfAttorney.SetAttorneyDocumentsInAttachments(_obj, e);
    }

  }
}