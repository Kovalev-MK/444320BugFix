using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.EDSApplication;

namespace lenspec.ElectronicDigitalSignatures
{
  partial class EDSApplicationClientHandlers
  {

    public virtual void IsSelfCancellationValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      _obj.State.Controls.Control.Refresh();
    }

    public override void PreparedByValueInput(Sungero.Docflow.Client.OfficialDocumentPreparedByValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.PreparedByValueInput(e);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      if (_obj.ApplicationCategory == EDSApplication.ApplicationCategory.Renewal)
        e.AddInformation(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.RenewalInstructionMessage, _obj.Info.Actions.ShowRenewalInstruction);
      
      if (_obj.ApplicationCategory == EDSApplication.ApplicationCategory.Cancellation)
        e.AddInformation(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.CancellationInstructionMessage, _obj.Info.Actions.ShowCancellationInstruction);
    }

  }
}