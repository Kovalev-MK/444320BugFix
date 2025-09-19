using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.RegistrationPowerOfAttorneyAssignment;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class RegistrationPowerOfAttorneyAssignmentActions
  {
    public virtual void RegisterPowerOfAttorneys(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var powerOfAttorneys = _obj.POAs.PowerOfAttorneys.Cast<Sungero.Docflow.IOfficialDocument>().ToList();
      if(powerOfAttorneys == null || !powerOfAttorneys.Any())
      {
        e.AddError(avis.PowerOfAttorneyModule.RegistrationPowerOfAttorneyAssignments.Resources.ErrorMessageRegistrationAction);
        return;
      }
      var error = lenspec.OutgoingLetters.PublicFunctions.Module.RegisterListOfDocuments(powerOfAttorneys);
      if (!string.IsNullOrEmpty(error))
      {
        e.AddError(error);
        return;
      }
      else
      {
        Dialogs.ShowMessage(avis.PowerOfAttorneyModule.RegistrationPowerOfAttorneyAssignments.Resources.SuccessMessageRegistrationAction, MessageType.Information);
        _obj.Save();
      }
    }

    public virtual bool CanRegisterPowerOfAttorneys(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var poas = _obj.POAs.PowerOfAttorneys;
      if (poas.Any())
      {
        var unregisteredPoas = poas.Where(x => x.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered);
        if (unregisteredPoas.Any())
        {
          e.AddError(avis.PowerOfAttorneyModule.RegistrationPowerOfAttorneyAssignments.Resources.ErrorMessageUnregistreredPoas);
          return;
        }
      }
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}