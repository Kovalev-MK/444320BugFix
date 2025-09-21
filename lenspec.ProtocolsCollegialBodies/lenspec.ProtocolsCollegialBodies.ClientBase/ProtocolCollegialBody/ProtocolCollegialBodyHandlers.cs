using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.ProtocolCollegialBody;

namespace lenspec.ProtocolsCollegialBodies
{

  partial class ProtocolCollegialBodyClientHandlers
  {
    public virtual void SecretaryValueInput(lenspec.ProtocolsCollegialBodies.Client.ProtocolCollegialBodySecretaryValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.RegistrationNumber.IsVisible = true;         
      _obj.State.Properties.LifeCycleState.IsVisible = true;
      _obj.State.Properties.Name.IsEnabled = false;
      _obj.State.Properties.Created.IsEnabled = false;
      _obj.State.Properties.RegistrationNumber.IsEnabled = false;   
      _obj.State.Properties.Department.IsRequired = false;
      _obj.State.Properties.DateMeeting.IsRequired = true;
    }
  }
}